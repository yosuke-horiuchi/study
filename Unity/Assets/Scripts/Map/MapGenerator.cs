using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyGame
{
    using Common;
    using Manager;

    namespace Map
    {
        /// <summary>
        /// マップのジェネレータ
        /// マップを生成と、そのマップのセルの管理をする
        /// </summary>
        public class MapGenerator : MonoBehaviour
        {
            // セルの情報格納。intは階層
            public Dictionary<int, Cell[,]> cellInfomation = new Dictionary<int, Cell[,]>();

            // 特殊セルのリスト
            public List<Cell> metaCells = new List<Cell>();

            // このマップとグリッド中央とのオフセット
            public GridPosition offsetGrid;

            /// <summary>
            /// イニシャライズ
            /// </summary>
            /// <param name="offsetGrid"></param>
            public void InitializeMap(MapPosition mapPosition)
            {
                // マップを配置するオフセット座標
                var offsetGrid = new GridPosition(
                    mapPosition.x * DefineVariables.GridNum,
                    0,
                    mapPosition.z * DefineVariables.GridNum
                    );

                transform.position = GridManager.Instance.GetPositionByGrid(offsetGrid);
                this.offsetGrid = offsetGrid;


                // Todo 仮。初期セルの作成
                int floor = 0;
                cellInfomation.Add(floor, new Cell[DefineVariables.GridNum, DefineVariables.GridNum]);

                // 仮板
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.position = new Vector3(this.offsetGrid.x + DefineVariables.GridNum / 2, 0, this.offsetGrid.z + DefineVariables.GridNum / 2);
                plane.transform.localScale = new Vector3(3f, 3f, 3f);
                plane.transform.parent = transform;
                plane.isStatic = true;

                for (int x = 0; x < DefineVariables.GridNum; x++)
                {
                    for (int z = 0; z < DefineVariables.GridNum; z++)
                    {
                        var grid = new GridPosition(
                                this.offsetGrid.x + x,
                                this.offsetGrid.y + floor,
                                this.offsetGrid.z + z
                            );

                        cellInfomation[floor][x, z] = new Cell(true, grid, x, floor, z);

                        // 仮ゴール設定
                        if (x == DefineVariables.GridNum / 2f && z == DefineVariables.GridNum - 1)
                        {
                            cellInfomation[floor][x, z].mapMeta = MapMetaPattern.Goal;
                            metaCells.Add(cellInfomation[floor][x, z]);
                            continue;
                        }

                        // todo 仮障害物
                        if (Random.Range(0f, 1f) < 0.05f && !(x == 0 && z == 0))
                        {
                            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            cube.transform.position = new Vector3(this.offsetGrid.x + x, 0.5F, this.offsetGrid.z + z);
                            cube.transform.parent = transform;
                            cube.isStatic = true;
                            cellInfomation[floor][x, z].walkable = false;
                        }
                    }
                }
            }
        }
    }
}
