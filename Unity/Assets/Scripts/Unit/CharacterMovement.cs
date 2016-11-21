using UnityEngine;
using System.Collections;
using MyGame.Common;

namespace MyGame
{
    using Manager;
    using Common;

    namespace Unit
    {
        public class CharacterMovement : MonoBehaviour
        {
            // 移動中か
            [System.NonSerialized]
            public bool isMoving;

            // 現在のグリッド座標
            public GridPosition currentGridPosition
            {
                get { return GridManager.Instance.GetUnitGrid(m_unitBaseController.unitIndex); }
            }

            // UnitBaseController。ステータス等を取るために保持
            UnitBaseController m_unitBaseController;

            // ステータスのリファレンス
            UnitStatus m_status;

            /// <summary>
            ///
            /// </summary>
            void Awake()
            {
                isMoving = false;
                m_unitBaseController = GetComponent<UnitBaseController>();
            }

            /// <summary>
            /// 初期位置設定用のグリッド
            /// </summary>
            /// <param name="grid"></param>
            public void SetInitialGrid(GridPosition grid, uint unitIndex)
            {
                GridManager.Instance.AddUnitGrid(grid, unitIndex);                
                SetPositionByCurrentGrid();
            }

            /// <summary>
            /// 移動リクエスト。値指定
            /// </summary>
            /// <param name="dir"></param>
            /// <param name="time"></param>
            /// <param name="moveGridNum"></param>
            public bool MoveRequest(MoveDirection dir, float time, int moveGridNum)
            {
                if (isMoving || m_unitBaseController.unitIndex == 0)
                {
                    return false;
                }

                bool isSuccess;

                switch (dir)
                {
                    case MoveDirection.Front:
                        isSuccess = MoveTo(time, 0, 0, moveGridNum);
                        break;

                    case MoveDirection.Back:
                        isSuccess = MoveTo(time, 0, 0, -moveGridNum);
                        break;

                    case MoveDirection.Left:
                        isSuccess = MoveTo(time, -moveGridNum, 0, 0);
                        break;

                    case MoveDirection.Right:
                        isSuccess = MoveTo(time, moveGridNum, 0, 0);
                        break;

                    default:
                        isSuccess = false;
                        break;
                }

                return isSuccess;
            }

            /// <summary>
            /// 通常移動。引数が空ならステータスから取ってくる
            /// </summary>
            /// <param name="dir"></param>
            public bool MoveRequest(MoveDirection dir)
            {
                if (isMoving || m_unitBaseController.unitIndex == 0)
                {
                    return false;
                }

                if (m_status == null)
                {
                    m_status = m_unitBaseController.GetStatus();
                }

                return MoveRequest(dir, m_status.moveExpendedTime, m_status.moveGridNum);
            }

            /// <summary>
            /// 移動メソッド
            /// </summary>
            /// <param name="time"></param>
            /// <param name="deltaX"></param>
            /// <param name="deltaY"></param>
            /// <param name="deltaZ"></param>
            bool MoveTo(float time, int deltaX, int deltaY, int deltaZ)
            {
                var moveToGrid = currentGridPosition;
                moveToGrid.x += deltaX;
                moveToGrid.y += deltaY;
                moveToGrid.z += deltaZ;

                // 移動不可の場合は中断
                if (!CheckCanMove(moveToGrid))
                {
                    return false;
                }

                // 移動中フラグ立てとく
                isMoving = true;

                // Gridを更新する
                GridManager.Instance.UpdateUnitGrid(m_unitBaseController.unitIndex, moveToGrid);

                // 移動後のposition
                var goalPosition = GridManager.Instance.GetPositionByGrid(moveToGrid);

                // 移動
                StartCoroutine(
                    MoveToCoroutine(
                        time,
                        deltaX * DefineVariables.CellSize,
                        deltaY * DefineVariables.CellSize,
                        deltaZ * DefineVariables.CellSize,
                        goalPosition
                        )
                    );

                return true;
            }

            /// <summary>
            /// /// 移動先について、移動が可能かチェックする
            /// </summary>
            /// <param name="grid"></param>
            /// <returns></returns>
            bool CheckCanMove(GridPosition grid)
            {
                // 移動先にUnitが存在する場合は移動不可
                if (GridManager.Instance.CheckAnyUnitExistByGrid(grid))
                {
#if DEBUG_BUILD
                    Debug.Log("Unitが存在するため、移動不可");
#endif
                    return false;
                }

                if (!GridManager.Instance.CheckWalkableCellByGrid(grid))
                {
#if DEBUG_BUILD
                    Debug.Log("地形が進入不可のため移動不可");
#endif
                    return false;
                }

                return true;
            }

            /// <summary>
            /// timeの時間をかけて、X,Zを移動するコルーチン
            /// </summary>
            /// <param name="time"></param>
            /// <param name="x"></param>
            /// <param name="z"></param>
            /// <param name="goalPosition"></param>
            /// <returns></returns>
            IEnumerator MoveToCoroutine(float time, float x, float y, float z, Vector3 goalPosition)
            {
                float bufferTime = 0;

                // 速度正規化
                x = x / time;
                y = y / time;
                z = z / time;

                while (bufferTime < time)
                {
                    yield return new WaitForEndOfFrame();

                    transform.position += new Vector3(
                        x * Time.deltaTime,
                        y * Time.deltaTime,
                        z * Time.deltaTime);

                    bufferTime += Time.deltaTime;
                }

                // 半端な値が入るので補正
                transform.position = goalPosition;
                isMoving = false;
            }

            /// <summary>
            /// 現在のグリッド座標を変換し、Transform.Positionにセットする
            /// </summary>
            void SetPositionByCurrentGrid()
            {
                transform.position = GridManager.Instance.GetPositionByGrid(currentGridPosition);
            }
        }
    }
}
