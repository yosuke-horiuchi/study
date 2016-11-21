using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyGame
{
    using Unit;
    using Common;

    namespace Manager
    {
        /// <summary>
        /// ユニットの管理をするマネージャー
        /// </summary>
        public class UnitManager : SingletonMonoBehaviour<UnitManager>
        {
            // プレイヤーキャラクタのコントローラ。インスペクターでセットしとく
            public PlayerController playerController;

            // ヒーローキャラクタのコントローラ。インスペクターでセットしとく
            public HeroController heroController;

            // NPCリスト
            public Dictionary<uint, UnitBaseController> unitBaseControllers;

            // Unitの管理用インデックス
            uint m_unitIndex = 0;

            /// <summary>
            /// 
            /// </summary>
            new void Awake()
            {
                base.Awake();

                unitBaseControllers = new Dictionary<uint, UnitBaseController>();
            }

            /// <summary>
            /// 
            /// </summary>
            void Start()
            {
                // 念のため
                if (playerController == null)
                {
                    playerController = FindObjectOfType<PlayerController>();
                    if (playerController == null)
                        Debug.LogError("プレイヤーキャラクターの取得に失敗");
                }
                if (heroController == null)
                {
                    heroController = FindObjectOfType<HeroController>();
                    if (heroController == null)
                        Debug.LogError("ヒーローキャラクターの取得に失敗");
                }
            }

            /// <summary>
            /// NPCリストに追加する
            /// </summary>
            /// <param name="npc"></param>
            public static uint AddUnitBaseController(UnitBaseController unit)
            {
                if (Instance.unitBaseControllers.ContainsValue(unit))
                {
                    return 0;
                }

                Instance.m_unitIndex++;
                Instance.unitBaseControllers.Add(Instance.m_unitIndex, unit);
                return Instance.m_unitIndex;
            }

            /// <summary>
            /// Unitが死んだ時の処理
            /// </summary>
            /// <param name="index"></param>
            public static void OnKilledUnit(uint index)
            {
                if (Instance.unitBaseControllers[index] == Instance.playerController)
                {

                }
                else if (Instance.unitBaseControllers[index] == Instance.heroController)
                {

                }
                else
                {
                    GridManager.Instance.RemoveUnit(index);
                    Instance.unitBaseControllers.Remove(index);
                }
            }

            /// <summary>
            /// タップ
            /// </summary>
            /// <param name="dir"></param>
            public static void RequestTap()
            {
                Instance.playerController.Tap();
            }

            /// <summary>
            /// プレイヤーキャラクターへ移動リクエストをする
            /// </summary>
            /// <param name="dir"></param>
            public static void RequestMovePlayerCharacter(MoveDirection dir)
            {
                Instance.playerController.MoveRequest(dir);
            }

            /// <summary>
            /// 四近傍のUnitを取得する
            /// </summary>
            /// <param name="fromUnitBaseController"></param>
            /// <returns></returns>
            public List<UnitBaseController> GetNeighbourUnit(UnitBaseController fromUnitBaseController, bool isEnemyOnly)
            {
                List<UnitBaseController> neighbourUnits = new List<UnitBaseController>();
                int range = 1; // todo 2マス移動する奴とか想定中
                var currentGrid = fromUnitBaseController.GetCharacterMovement().currentGridPosition;

                // 斜めは除外
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
                            var buffer = GridManager.Instance.FindUnitByGrid(new GridPosition(
                                    currentGrid.x + x,
                                    currentGrid.y,
                                    currentGrid.z + z
                                    ));

                            if (buffer != null)
                            {
                                if (!isEnemyOnly)
                                {
                                    neighbourUnits.Add(buffer);
                                    continue;
                                }

                                if (fromUnitBaseController.myTeam != buffer.myTeam)
                                {
                                    neighbourUnits.Add(buffer);
                                }                                
                            }
                        }
                    }
                }
                return neighbourUnits;
            }
        }
    }
}

