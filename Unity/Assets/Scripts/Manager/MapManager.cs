using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyGame
{
    using Map;
    using Common;

    namespace Manager
    {
        /// <summary>
        /// マップのマネージャー。自動ジェネレートされたマップを管理する
        /// </summary>
        public class MapManager : SingletonMonoBehaviour<MapManager>
        {
            // マップリスト
            List<MapGenerator> m_maps = new List<MapGenerator>();

            // マップの親オブジェクト
            GameObject m_mapParent;

            // オブジェクト名
            const string m_parentName = "Maps";
            const string m_childName = "map";

            /// <summary>
            /// 
            /// </summary>
            new void Awake()
            {
                base.Awake();

                // インスペクター整理用
                var obj = new GameObject();
                obj.name = m_parentName;
                obj.transform.parent = transform;
                m_mapParent = obj;
            }

            /// <summary>
            /// マップのジェネレートをコールする
            /// </summary>
            /// <param name="grid"></param>
            public void RequestGenerateMap(MapPosition mapPosition)
            {
                // 空オブジェクトを作る
                var obj = new GameObject();
                obj.name = m_childName;
                // obj.transform.parent = m_mapParent.transform;

                // マップコンポーネントを付与する
                var map = obj.AddComponent<MapGenerator>();
                map.InitializeMap(mapPosition);
                m_maps.Add(map);
            }

            /// <summary>
            /// グリッド情報から対象となるCellを返す
            /// </summary>
            /// <param name="grid"></param>
            /// <returns></returns>
            public Cell GetCellByGrid(GridPosition grid)
            {
                // マップに含まれるか確認し、Cellを返す
                for (int i = 0; i < m_maps.Count; i++)
                {
                    if (m_maps[i].offsetGrid.x <= grid.x && grid.x < m_maps[i].offsetGrid.x + DefineVariables.GridNum
                        && m_maps[i].offsetGrid.z <= grid.z && grid.z < m_maps[i].offsetGrid.z + DefineVariables.GridNum)
                    {
                        if (m_maps[i].cellInfomation.ContainsKey(grid.y))
                        {
                            return m_maps[i].cellInfomation[grid.y][grid.x - m_maps[i].offsetGrid.x, grid.z - m_maps[i].offsetGrid.z];
                        }
                        break;
                    }
                }

                return null;
            }

            /// <summary>
            /// そのグリッドの座標が含まれるMapGeneratorを返す
            /// </summary>
            /// <param name="cell"></param>
            /// <returns></returns>
            public MapGenerator GetMapGeneratorByGrid(GridPosition grid)
            {
                for (int i = 0; i < m_maps.Count; i++)
                {
                    if (m_maps[i].offsetGrid.x <= grid.x && grid.x < m_maps[i].offsetGrid.x + DefineVariables.GridNum
                        && m_maps[i].offsetGrid.z <= grid.z && grid.z < m_maps[i].offsetGrid.z + DefineVariables.GridNum)
                    {
                        return m_maps[i];
                    }
                }

                return null;
            }
        }
    }
}
