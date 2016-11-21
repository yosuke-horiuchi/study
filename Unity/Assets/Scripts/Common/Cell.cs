using UnityEngine;
using System.Collections;

namespace MyGame
{
    using Manager;

    namespace Common
    {
        /// <summary>
        /// Gridを構成するCellのクラス
        /// 主に経路検索用につかう
        /// </summary>
        public class Cell
        {
            public bool walkable;
            public MapMetaPattern mapMeta;
            public GridPosition gridPosition;

            public int localCellIndexX;
            public int localCellIndexY;
            public int localCellIndexZ;

            // A*のコスト管理用
            public int gCost; // スタート地点からの距離
            public int hCost; // ゴール地点からの距離
            public int fCost
            {
                get { return gCost + hCost; }
            }

            public Cell parent;

            public Cell(
                bool _walkable,
                GridPosition _gridPosition, 
                int _localCellIndexX, 
                int _localCellIndexY, 
                int _localCellIndexZ,
                MapMetaPattern _mapMeta = MapMetaPattern.Normal)
            {
                walkable = _walkable;
                gridPosition = _gridPosition;
                localCellIndexX = _localCellIndexX;
                localCellIndexY = _localCellIndexY;
                localCellIndexZ = _localCellIndexZ;
                mapMeta = _mapMeta;
            }
        }
    }
}
