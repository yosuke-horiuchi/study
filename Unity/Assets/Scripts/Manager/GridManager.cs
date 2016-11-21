using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyGame
{
    using Common;
    using Unit;

    namespace Manager
    {
        /// <summary>
        /// 移動マスを管理するマネージャー
        /// </summary>
        public class GridManager : SingletonMonoBehaviour<GridManager>
        {
            // uintはUnitの識別用
            Dictionary<uint, GridPosition> m_unitPosition;

            /// <summary>
            /// 
            /// </summary>
            new void Awake()
            {
                base.Awake();
                m_unitPosition = new Dictionary<uint, GridPosition>();
            }
            /// <summary>
            /// 
            /// </summary>
            void Start()
            {
                // TODO　仮
                MapManager.Instance.RequestGenerateMap(new MapPosition(0, 0));
                MapManager.Instance.RequestGenerateMap(new MapPosition(0, 1));
            }

            /// <summary>
            /// Unitの初期位置を登録する
            /// エラー時は0を出力する
            /// </summary>
            /// <param name="grid"></param>
            /// <returns></returns>
            public void AddUnitGrid(GridPosition grid, uint unitIndex)
            {
                if (m_unitPosition.ContainsKey(unitIndex))
                {
                    return;
                }

                m_unitPosition.Add(unitIndex, grid);
            }

            /// <summary>
            /// Unitの位置を更新する
            /// </summary>
            /// <param name="index"></param>
            /// <param name="g"></param>
            public bool UpdateUnitGrid(uint index, GridPosition g)
            {
                if (m_unitPosition.ContainsKey(index))
                {
                    m_unitPosition[index] = g;
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Unitの情報を削除する
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public bool RemoveUnit(uint index)
            {
                return m_unitPosition.Remove(index);
            }

            /// <summary>
            /// UnitのGridを取得する
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public GridPosition GetUnitGrid(uint index)
            {
                GridPosition grid = new GridPosition();
                m_unitPosition.TryGetValue(index, out grid);
                return grid;
            }

            /// <summary>
            /// 対象グリッドになにかユニットがいるか
            /// </summary>
            /// <param name="grid"></param>
            /// <returns></returns>
            public bool CheckAnyUnitExistByGrid(GridPosition grid)
            {
                return m_unitPosition.ContainsValue(grid);
            }

            /// <summary>
            /// そのグリッドに存在するUnitを返す
            /// </summary>
            /// <param name="grid"></param>
            /// <returns></returns>
            public UnitBaseController FindUnitByGrid(GridPosition grid)
            {
                if (!CheckAnyUnitExistByGrid(grid))
                {
                    return null;
                }

                foreach (var item in m_unitPosition)
                {
                    if (grid.Equals(item.Value))
                    {
                        return UnitManager.Instance.unitBaseControllers[item.Key];
                    }
                }
                return null;
            }

            /// <summary>
            /// 対象グリッドの地形は進行可能か
            /// </summary>
            /// <param name="grid"></param>
            /// <returns></returns>
            public bool CheckWalkableCellByGrid(GridPosition grid)
            {
                var cell = MapManager.Instance.GetCellByGrid(grid);
                if (cell == null)
                {
                    return false;
                }

                return cell.walkable;
            }

            /// <summary>
            /// 目標グリッドへ到達するための移動方向を返す
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public MoveDirection GetMoveDirectionToTargetGrid(GridPosition from, GridPosition to)
            {
                // TODO とりあえず近づく

                // 同じ階ではない場合は返さない
                if (from.y != to.y)
                {
                    return MoveDirection.KeepPosition;
                }

                // 一旦XとZを比較し、遠い距離を縮めるように動かす単純な感じに
                if (Mathf.Abs(from.x - to.x) >= Mathf.Abs(from.z - to.z))
                {
                    var value = from.x - to.x;
                    if (value < 0)
                    {
                        return MoveDirection.Right;
                    }
                    else
                    {
                        return MoveDirection.Left;
                    }
                }
                else
                {
                    var value = from.z - to.z;
                    if (value < 0)
                    {
                        return MoveDirection.Front;
                    }
                    else
                    {
                        return MoveDirection.Back;
                    }
                }
            }

            /// <summary>
            /// ユニット指定の受け口
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public MoveDirection GetMoveDirectionToTargetGrid(UnitBaseController from, UnitBaseController to)
            {
                return GetMoveDirectionToTargetGrid(
                    from.GetCharacterMovement().currentGridPosition,
                    to.GetCharacterMovement().currentGridPosition
                    );
            }

            /// <summary>
            /// 近接するセルを返す
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            public List<Cell> GetNeighbourCell(Cell cell)
            {
                List<Cell> neighbourCells = new List<Cell>();
                int range = 1; // todo 2マス移動する奴とか想定中

                // 斜め移動はしない想定
                for (int x = -range; x <= range; x++)
                {
                    for (int z = -range; z <= range; z++)
                    {
                        if (x == 0 && z == 0)
                        {
                            continue;
                        }

                        // 4方向に
                        if (x == 0 || z == 0)
                        {
                            var buffer = MapManager.Instance.GetCellByGrid(
                                new GridPosition(
                                    cell.gridPosition.x + x,
                                    cell.gridPosition.y,
                                    cell.gridPosition.z + z
                                    )
                                );

                            if (buffer != null)
                            {
                                neighbourCells.Add(buffer);
                            }
                        }
                    }
                }
                return neighbourCells;
            }

            /// <summary>
            /// グリッドをUnity空間のVector3に変換する
            /// </summary>
            /// <param name="grid"></param>
            /// <returns></returns>
            public Vector3 GetPositionByGrid(GridPosition grid)
            {
                return new Vector3(
                    grid.x * DefineVariables.CellSize,
                    grid.y * DefineVariables.CellSize,
                    grid.z * DefineVariables.CellSize
                    );
            }

            /// <summary>
            /// 2つのGridから4方向を返す
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public MoveDirection GetMoveDirection(GridPosition from, GridPosition to)
            {
                if(from.x - to.x != 0 && from.z == to.z)
                {
                    if(from.x - to.x > 0)
                    {
                        return MoveDirection.Left;
                    }
                    return MoveDirection.Right;
                }

                if (from.z - to.z != 0 && from.x == to.x)
                {
                    if (from.z - to.z > 0)
                    {
                        return MoveDirection.Back;
                    }
                    return MoveDirection.Front;
                }

                return MoveDirection.KeepPosition;
            }
        }
    }
}
