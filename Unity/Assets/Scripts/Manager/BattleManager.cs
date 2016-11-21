using UnityEngine;
using System.Collections;

namespace MyGame
{
    using Unit;
    using Common;

    namespace Manager
    {
        /// <summary>
        /// 戦闘関係のマネージャ
        /// </summary>
        public static class BattleManager// : SingletonMonoBehaviour<UnitManager>
        {
            /// <summary>
            /// 攻撃計算をして、HPに適応する
            /// </summary>
            /// <param name="fromUnitIndex"></param>
            /// <param name="toUnitIndex"></param>
            public static void ApplyDamage(uint fromUnitIndex, uint toUnitIndex)
            {
                if (!UnitManager.Instance.unitBaseControllers.ContainsKey(fromUnitIndex)
                    || !UnitManager.Instance.unitBaseControllers.ContainsKey(toUnitIndex))
                {
                    return;
                }

                var fromStatus = UnitManager.Instance.unitBaseControllers[fromUnitIndex].GetStatus();
                var toStatus = UnitManager.Instance.unitBaseControllers[toUnitIndex].GetStatus();

                // 攻撃者側の攻撃力を、防御側のカット率分軽減した値をHPから引く（変えるかも）
                toStatus.hp = toStatus.hp - (1f - toStatus.damageCutRatio) * fromStatus.attackPower;

#if DEBUG_BUILD
                Debug.Log(UnitManager.Instance.unitBaseControllers[fromUnitIndex].name + " Attack to "
                    + UnitManager.Instance.unitBaseControllers[toUnitIndex].name + ". ダメージ :"
                    + (1f - toStatus.damageCutRatio) * fromStatus.attackPower + " 残りHP "
                    + toStatus.hp);
#endif

                UnitManager.Instance.unitBaseControllers[toUnitIndex].OnAnyDamage(
                    UnitManager.Instance.unitBaseControllers[fromUnitIndex]
                    );
            }

            /// <summary>
            /// 攻撃できるかチェックする
            /// </summary>
            /// <param name="fromUnitIndex"></param>
            /// <param name="toUnitIndex"></param>
            /// <returns></returns>
            public static bool CanApplyDamage(uint fromUnitIndex, uint toUnitIndex)
            {
                var fromUnit = UnitManager.Instance.unitBaseControllers[fromUnitIndex];
                var toUnit = UnitManager.Instance.unitBaseControllers[toUnitIndex];

                return fromUnit.myTeam != toUnit.myTeam;
            }
        }
    }
}

