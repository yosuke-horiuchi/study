using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyGame
{
    using Common;
    using Manager;

    namespace Unit
    {
        /// <summary>
        /// NPCのコントローラークラス
        /// </summary>
        public class NpcController : UnitBaseController
        {
            /// <summary>
            /// 
            /// </summary>
            protected new void Start()
            {
                base.Start();

                // TODO 初期位置（仮）
                m_characterMovement.SetInitialGrid(initialGrid, unitIndex);

                // TODO
                m_status.idleWaitTime = 1f;
            }
            
            /// <summary>
            /// 攻撃目標を取得する
            /// </summary>
            /// <param name="pattern"></param>
            /// <returns></returns>
            public UnitBaseController GetTargetUnitForMove(TargetUnitDetectionPattern pattern)
            {
                // 条件判定
                switch (pattern)
                {
                    case TargetUnitDetectionPattern.Near:
                        break;

                    case TargetUnitDetectionPattern.Far:
                        break;

                    case TargetUnitDetectionPattern.LowestHp:
                        break;

                    case TargetUnitDetectionPattern.HighestHp:
                        break;

                    case TargetUnitDetectionPattern.Player:
                        return UnitManager.Instance.playerController;

                    case TargetUnitDetectionPattern.Hero:
                        return UnitManager.Instance.heroController;

                    case TargetUnitDetectionPattern.OtherUnit:
                        break;

                    default:
                        break;
                }

                return null;
            }
            /*
            /// <summary>
            /// 攻撃可能な敵がいれば、そのUnitを返す
            /// </summary>
            /// <returns></returns>
            public UnitBaseController GetTargetUnitForAttack(TargetUnitDetectionPattern pattern)
            {
                //var unitList = new List<UnitBaseController>();

                switch (myTeam)
                {
                    case Team.Alone:
                        break;
                    case Team.Ally:
                        break;
                    case Team.Enemy:
                        // UnitManager.Instance.PlayerController
                        // UnitManager.Instance.HeroController
                        break;
                    case Team.OtherGroup:
                        break;
                    default:
                        break;
                }                

                switch (pattern)
                {
                    case TargetUnitDetectionPattern.Near:
                        break;
                    case TargetUnitDetectionPattern.Far:
                        break;
                    case TargetUnitDetectionPattern.LowestHp:
                        break;
                    case TargetUnitDetectionPattern.HighestHp:
                        break;
                    case TargetUnitDetectionPattern.Player:
                        break;
                    case TargetUnitDetectionPattern.Hero:
                        break;
                    case TargetUnitDetectionPattern.OtherUnit:
                        break;
                    default:
                        break;
                }
                return null;
            }
            */
        }
    }
}
