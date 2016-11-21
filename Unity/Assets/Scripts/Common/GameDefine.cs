using UnityEngine;
using System.Collections;

namespace MyGame
{
    namespace Common
    {
        /// <summary>
        /// 定数
        /// </summary>
        public static class DefineVariables
        {
            // ゲームの１マスのサイズを定義。（TransformPositionの値そのまま）
            public const float CellSize = 1f;

            // 守護対象から離れてはいけないエリア
            public const int SafeArea = 5;

            // 1枚のマップのグリッドに含まれるセルのサイズ。中央の1マスを含めるため、GridSize+1のマスになる
            public const int GridNum = 30;

            // AIの行動を事前計算するための時間
            public const float AiActionDecisionTimeBuffer = 0.2f;
        }

        /// <summary>
        /// チームわり
        /// </summary>
        public enum Team
        {
            Alone,
            Ally,
            Enemy,
            OtherGroup
        }
        
        /// <summary>
        /// 移動方向のEnum
        /// </summary>
        public enum MoveDirection
        {
            KeepPosition,
            Front,
            Back,            
            Left,
            Right
        }

        /// <summary>
        /// 移動先や攻撃先の検出優先度を決めるもの
        /// </summary>
        public enum TargetUnitDetectionPattern
        {
            Near,
            Far,
            LowestHp,
            HighestHp,
            Player,
            Hero,
            OtherUnit
        }

        /// <summary>
        /// マップ上の特定セルのメタデータ識別用
        /// </summary>
        public enum MapMetaPattern
        {
            Normal,
            Goal,
            Camp,
            Treasure
        }

        /// <summary>
        /// Unityの座標空間ではなく、原点を(0, 0, 0)とした時のマスのインデックス
        /// 例えばインデックスが(1, 0, 0)ならば、Unityの座標では(1*Gridのサイズ, 0, 0)となる
        /// </summary>
        [System.Serializable]
        public struct GridPosition
        {
            public GridPosition(int _x, int _y, int _z)
            {
                x = _x;
                y = _y;
                z = _z;
            }

            public int x, y, z;
        }
        /// <summary>
        /// マップ毎の並びを管理するための座標系
        /// </summary>
        [System.Serializable]
        public struct MapPosition
        {
            public MapPosition(int _x, int _z)
            {
                x = _x;
                z = _z;
            }

            public int x, z;
        }
    }
}
