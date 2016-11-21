using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyGame
{
    using Manager;

    namespace Common
    {
        /// <summary>
        /// A*の経路検索クラス
        /// </summary>
        public class PathFinding : MonoBehaviour
        {
            /// <summary>
            /// パス検索の入り口
            /// </summary>
            /// <param name="startGrid"></param>
            /// <param name="endGrid"></param>
            public void StartFindPath(GridPosition startGrid, GridPosition endGrid)
            {
                StartCoroutine(FindPath(startGrid, endGrid));
            }

            /// <summary>
            /// 実処理
            /// </summary>
            /// <param name="startGrid"></param>
            /// <param name="endGrid"></param>
            /// <returns></returns>
            IEnumerator FindPath(GridPosition startGrid, GridPosition endGrid)
            {
                var waypoints = new List<Cell>();
                bool pathSuccess = false;

                Cell startCell = MapManager.Instance.GetCellByGrid(startGrid);
                Cell targetCell = MapManager.Instance.GetCellByGrid(endGrid);

                // このIFは取るかも
                if (startCell.walkable && targetCell.walkable)
                {
                    List<Cell> openSet = new List<Cell>();
                    HashSet<Cell> closedSet = new HashSet<Cell>();
                    openSet.Add(startCell);

                    while (openSet.Count > 0)
                    {
                        Cell currentCell = openSet[0];

                        // 参照セルの変更
                        for (int i = 1; i < openSet.Count; i++)
                        {
                            if (openSet[i].fCost < currentCell.fCost || openSet[i].fCost == currentCell.fCost)
                            {
                                if (openSet[i].hCost < currentCell.hCost)
                                {
                                    currentCell = openSet[i];
                                }
                            }
                        }

                        // 探索リストの整理
                        openSet.Remove(currentCell);
                        closedSet.Add(currentCell);

                        // 目的のセルなら探索終了
                        if (currentCell == targetCell)
                        {
                            pathSuccess = true;
                            break;
                        }

                        // 近接セルを取得
                        foreach (Cell neighbour in GridManager.Instance.GetNeighbourCell(currentCell))
                        {
                            if (!neighbour.walkable || closedSet.Contains(neighbour))
                            {
                                continue;
                            }

                            // コスト計算
                            int costToNeighbour = currentCell.gCost + GetDistance(currentCell, neighbour);
                            if (costToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                            {
                                neighbour.gCost = costToNeighbour;
                                neighbour.hCost = GetDistance(neighbour, targetCell);
                                neighbour.parent = currentCell;

                                if (!openSet.Contains(neighbour))
                                {
                                    openSet.Add(neighbour);
                                }                                    
                            }
                        }
                    }
                }
                yield return null;

                if (pathSuccess)
                {                    
                    waypoints = RetracePath(startCell, targetCell);
                }
                PathRequestManager.Instance.FinishedProcessingPath(waypoints, pathSuccess);
            }

            /// <summary>
            /// パスを返す
            /// </summary>
            /// <param name="startCell"></param>
            /// <param name="endCell"></param>
            /// <returns></returns>
            List<Cell> RetracePath(Cell startCell, Cell endCell)
            {
                List<Cell> path = new List<Cell>();
                Cell currentCell = endCell;

                while (currentCell != startCell)
                {
                    path.Add(currentCell);
                    currentCell = currentCell.parent;
                }
                path.Reverse();

                return path;
            }
            
            /// <summary>
            /// 距離計算。14は10の√2の近似
            /// </summary>
            /// <param name="cellA"></param>
            /// <param name="cellB"></param>
            /// <returns></returns>
            int GetDistance(Cell cellA, Cell cellB)
            {
                int dstX = Mathf.Abs(cellA.gridPosition.x - cellB.gridPosition.x);
                int dstZ = Mathf.Abs(cellA.gridPosition.z - cellB.gridPosition.z);

                if (dstX > dstZ)
                    return 14 * dstZ + 10 * (dstX - dstZ);

                return 14 * dstX + 10 * (dstZ - dstX);
            }
        }
    }
}
