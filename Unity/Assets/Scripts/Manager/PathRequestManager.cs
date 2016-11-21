using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MyGame
{
    using Common;
    namespace Manager
    {
        /// <summary>
        /// 経路検索のリクエストを管理するクラス
        /// 同時に2つ以上処理はしない
        /// </summary>
        [RequireComponent(typeof(PathFinding))]
        public class PathRequestManager : SingletonMonoBehaviour<PathRequestManager>
        {
            // キュー
            Queue<PathRequest> m_pathRequestQueue = new Queue<PathRequest>();

            // 今のリクエスト
            PathRequest m_currentPathRequest;

            // パスクラス
            PathFinding m_pathfinding;

            // 処理中フラグ
            bool m_isProcessing;

            /// <summary>
            /// 
            /// </summary>
            new void Awake()
            {
                base.Awake();

                m_pathfinding = GetComponent<PathFinding>();
            }

            /// <summary>
            /// パスのリクエスト。コールバックに返す
            /// </summary>
            /// <param name="pathStart"></param>
            /// <param name="pathEnd"></param>
            /// <param name="callback"></param>                                
            public static void RequestPath(
                GridPosition pathStart,
                GridPosition pathEnd,
                Action<List<Cell>, bool> callback)
            {
                PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
                Instance.m_pathRequestQueue.Enqueue(newRequest);
                Instance.TryProcessNext();
            }

            /// <summary>
            /// パス検索処理
            /// </summary>
            void TryProcessNext()
            {
                if (!m_isProcessing && m_pathRequestQueue.Count > 0)
                {
                    m_currentPathRequest = m_pathRequestQueue.Dequeue();
                    m_isProcessing = true;
                    m_pathfinding.StartFindPath(m_currentPathRequest.pathStart, m_currentPathRequest.pathEnd);
                }
            }

            /// <summary>
            /// パス検索終了時のコールバック
            /// </summary>
            /// <param name="path"></param>
            /// <param name="success"></param>
            public void FinishedProcessingPath(List<Cell> path, bool success)
            {
                m_currentPathRequest.callback(path, success);
                m_isProcessing = false;
                TryProcessNext();
            }

            /// <summary>
            /// パスリクエスト構造体
            /// </summary>
            struct PathRequest
            {
                public GridPosition pathStart;
                public GridPosition pathEnd;
                public Action<List<Cell>, bool> callback;

                public PathRequest(GridPosition _start, GridPosition _end, Action<List<Cell>, bool> _callback)
                {
                    pathStart = _start;
                    pathEnd = _end;
                    callback = _callback;
                }
            }
        }
    }
}
