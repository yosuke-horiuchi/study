using UnityEngine;
using System.Collections;

namespace MyGame
{
    using Unit;
    using Common;

    namespace Manager
    {
        /// <summary>
        /// ユニットの管理をするマネージャー
        /// </summary>
        public class MyGameManager : SingletonMonoBehaviour<UnitManager>
        {
            /// <summary>
            /// 
            /// </summary>
            new void Awake()
            {
                base.Awake();
            }
        }
    }
}

