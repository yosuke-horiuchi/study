using UnityEngine;
using System.Collections;

namespace MyGame
{
    using Manager;

    namespace Unit
    {
        /// <summary>
        /// ヒーロー（防衛目標）のコントローラークラス
        /// </summary>
        public class HeroController : UnitBaseController
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
                m_status.idleWaitTime = 2f;

                m_isDontDestroyUnit = true;
            }

            /// <summary>
            /// 
            /// </summary>                                    
            void Update()
            {
            }
        }
    }
}
