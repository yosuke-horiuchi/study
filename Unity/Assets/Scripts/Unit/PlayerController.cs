using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace MyGame
{
    using Manager;
    using Common;

    namespace Unit
    {
        /// <summary>
        /// プレイヤーのコントローラークラス
        /// </summary>
        public class PlayerController : UnitBaseController
        {
            MoveDirection m_lastCommand;

            /// <summary>
            /// 
            /// </summary>
            protected new void Start()
            {
                base.Start();

                // TODO 初期位置（仮）
                m_characterMovement.SetInitialGrid(initialGrid, unitIndex);
                m_lastCommand = MoveDirection.KeepPosition;

                m_isDontDestroyUnit = true;
            }

            /// <summary>
            /// タップ時の操作
            /// </summary>
            public void Tap()
            {
                MoveRequest(m_lastCommand);
            }

            /// <summary>
            /// 移動時の操作
            /// </summary>
            public void MoveRequest(MoveDirection dir)
            {
                var moveToGrid = m_characterMovement.currentGridPosition;
                m_lastCommand = dir;

                switch (dir)
                {
                    case MoveDirection.KeepPosition:
                        return;

                    case MoveDirection.Front:
                        moveToGrid.z++;
                        break;

                    case MoveDirection.Back:
                        moveToGrid.z--;
                        break;

                    case MoveDirection.Left:
                        moveToGrid.x--;
                        break;

                    case MoveDirection.Right:
                        moveToGrid.x++;
                        break;

                    default:
                        return;
                }

                // 移動先にUnitがいた場合、攻撃する
                if (GridManager.Instance.CheckAnyUnitExistByGrid(moveToGrid))
                {
                    if (isAttacking)
                    {
                        return;
                    }

                    var unit = GridManager.Instance.FindUnitByGrid(moveToGrid);
                    if (unit == null)
                    {
                        return;
                    }

                    RequestAttack(unit, dir);
                }
                else
                {
                    // 移動可能ならば移動する
                    m_characterMovement.MoveRequest(dir);
                }
            }
        }
    }
}
