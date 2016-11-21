using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MyGame
{
    using Manager;
    using Common;

    namespace Handler
    {
        /// <summary>
        /// LeanTouchからタップやスワイプをハンドルするためのクラス
        /// </summary>
        public class InputHandler : SingletonMonoBehaviour<InputHandler>
        {
#if DEBUG_BUILD
            // デバッグ用表示
            public Text debugText;
#endif

            /// <summary>
            /// 
            /// </summary>
            protected virtual void OnEnable()
            {
                // フック
                Lean.LeanTouch.OnFingerSwipe += OnFingerSwipe;
                Lean.LeanTouch.OnFingerTap += OnFingerTap;
            }

            /// <summary>
            /// 
            /// </summary>
            protected virtual void OnDisable()
            {
                Lean.LeanTouch.OnFingerSwipe -= OnFingerSwipe;
                Lean.LeanTouch.OnFingerTap -= OnFingerTap;
            }

            /// <summary>
            /// タップのハンドラー
            /// </summary>
            /// <param name="finger"></param>
            public void OnFingerTap(Lean.LeanFinger finger)
            {
                UnitManager.RequestTap();
#if DEBUG_BUILD
                string s = "　○　タップ";
                Debug.Log(s);
                debugText.text = s;
#endif
            }

            /// <summary>
            /// スワイプのハンドラー
            /// </summary>
            /// <param name="finger"></param>
            public void OnFingerSwipe(Lean.LeanFinger finger)
            {
#if DEBUG_BUILD
                var s = default(string);
#endif
                if (UnitManager.Instance == null) return;

                var swipe = finger.SwipeDelta;

                if (swipe.y > Mathf.Abs(swipe.x))
                {
                    UnitManager.RequestMovePlayerCharacter(MoveDirection.Front);
#if DEBUG_BUILD
                    s = "　↑　上スワイプ";
#endif
                }

                if (swipe.y < -Mathf.Abs(swipe.x))
                {
                    UnitManager.RequestMovePlayerCharacter(MoveDirection.Back);
#if DEBUG_BUILD
                    s = "　↓　下スワイプ";
#endif
                }

                if (swipe.x < -Mathf.Abs(swipe.y))
                {
                    UnitManager.RequestMovePlayerCharacter(MoveDirection.Left);
#if DEBUG_BUILD
                    s = "　←　左スワイプ";
#endif
                }

                if (swipe.x > Mathf.Abs(swipe.y))
                {
                    UnitManager.RequestMovePlayerCharacter(MoveDirection.Right);
#if DEBUG_BUILD
                    s = "　→　右スワイプ";
#endif
                }

#if DEBUG_BUILD
                Debug.Log(s);
                debugText.text = s;
#endif
            }
        }
    }
}
