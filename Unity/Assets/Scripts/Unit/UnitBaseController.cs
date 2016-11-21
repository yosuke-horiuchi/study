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
        /// ユニットのコントローラークラス
        /// </summary>
        [RequireComponent(typeof(CharacterMovement))]
        public abstract class UnitBaseController : MonoBehaviour
        {
            // チームわり
            public Team myTeam;

            // 初期位置
            public GridPosition initialGrid;

            // パス検索のバッファ
            [System.NonSerialized]
            public List<Cell> findPathBuffer = new List<Cell>();

            // パス検索の成功フラグ
            public bool isSuccessPathFinding;

            // 攻撃中か
            public bool isAttacking;

            // Unitマネージャーより発行されるID
            public uint unitIndex;

            // 移動の操作用コンポーネント
            protected CharacterMovement m_characterMovement;

            // 共通ステータス用
            protected UnitStatus m_status;

            // 仮。壊されては困るユニットのフラグ（基本はFalse。プレイヤーとかヒーローはTrueに
            protected bool m_isDontDestroyUnit;

            /// <summary>
            /// 
            /// </summary>
            protected virtual void Start()
            {
                m_characterMovement = GetComponent<CharacterMovement>();
                m_status = new UnitStatus();
                unitIndex = UnitManager.AddUnitBaseController(this);
                isAttacking = isSuccessPathFinding = false;
            }

            /// <summary>
            /// 移動コンポーネント取得用
            /// </summary>
            /// <returns></returns>
            public CharacterMovement GetCharacterMovement()
            {
                return m_characterMovement;
            }

            /// <summary>
            /// ステータス取得用
            /// </summary>
            /// <returns></returns>
            public UnitStatus GetStatus()
            {
                return m_status;
            }

            /// <summary>
            /// 座標を対象にパス検索を行う
            /// </summary>
            public void DoRequestPathFinding(GridPosition grid)
            {
                isSuccessPathFinding = false;

                PathRequestManager.RequestPath(
                    m_characterMovement.currentGridPosition,
                    grid,
                    OnPathFound
                    );
            }

            /// <summary>
            /// 特定セルを対象にパス検索を行う
            /// </summary>
            public void DoRequestPathFinding(MapMetaPattern pattern)
            {
                var map = MapManager.Instance.GetMapGeneratorByGrid(m_characterMovement.currentGridPosition);

                if (map == null)
                {
                    return;
                }

                if (map.metaCells.Count > 0)
                {
                    for (int i = 0; i < map.metaCells.Count; i++)
                    {
                        // TODO 複数同じGridなど存在する可能性があるので、Nearとか条件が別途必要
                        if (pattern == map.metaCells[i].mapMeta)
                        {
                            DoRequestPathFinding(map.metaCells[i].gridPosition);
                            return;
                        }
                    }
                }
            }

            /// <summary>
            /// ユニットを対象にパス検索を行う
            /// </summary>
            public void DoRequestPathFinding(TargetUnitDetectionPattern pattern)
            {
                UnitBaseController target;

                // 仮
                target = UnitManager.Instance.heroController;

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
                        target = UnitManager.Instance.playerController;
                        break;

                    case TargetUnitDetectionPattern.Hero:
                        target = UnitManager.Instance.heroController;
                        break;

                    case TargetUnitDetectionPattern.OtherUnit:
                        break;
                    default:
                        break;
                }

                DoRequestPathFinding(target.GetCharacterMovement().currentGridPosition);
            }

            /// <summary>
            /// パス発見時のコールバック
            /// </summary>
            /// <param name="findPath"></param>
            /// <param name="pathSuccessful"></param>
            public void OnPathFound(List<Cell> findPath, bool pathSuccessful)
            {
                if (pathSuccessful)
                {
                    isSuccessPathFinding = true;
                    findPathBuffer = findPath;
                }
            }

            /// <summary>
            /// 現在のセルから、移動すべきパスの方向を返す
            /// </summary>
            public MoveDirection GetMoveDirectionFromPath()
            {
                if (findPathBuffer.Count == 0)
                {
                    return MoveDirection.KeepPosition;
                }


                for (int i = 0; i < findPathBuffer.Count; i++)
                {
                    if (m_characterMovement.currentGridPosition.Equals(findPathBuffer[i].parent.gridPosition))
                    {
                        return GridManager.Instance.GetMoveDirection(
                            m_characterMovement.currentGridPosition, findPathBuffer[i].gridPosition);
                    }
                }

                return MoveDirection.KeepPosition;
            }

#if DEBUG_BUILD
            /// <summary>
            /// デバッグ用。パスの線を表示する         
            /// </summary>
            void OnDrawGizmos()
            {
                for (int i = 0; i < findPathBuffer.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(
                        GridManager.Instance.GetPositionByGrid(findPathBuffer[i].gridPosition),
                        GridManager.Instance.GetPositionByGrid(findPathBuffer[i].parent.gridPosition)
                    );
                }
            }
#endif

            /// <summary>
            /// 攻撃リクエストをする
            /// </summary>
            /// <param name="enemyUnit"></param>
            /// <param name="attackDir"></param>
            /// <returns></returns>
            public bool RequestAttack(UnitBaseController enemyUnit, MoveDirection attackDir = MoveDirection.KeepPosition)
            {
                if (!isAttacking && BattleManager.CanApplyDamage(unitIndex, enemyUnit.unitIndex))
                {
                    if (attackDir != MoveDirection.KeepPosition)
                    {
                        DoAttackMotion(attackDir);
                    }
                    else if (m_characterMovement.currentGridPosition.x - enemyUnit.m_characterMovement.currentGridPosition.x > 0)
                    {
                        DoAttackMotion(MoveDirection.Left);
                    }
                    else if (m_characterMovement.currentGridPosition.x - enemyUnit.m_characterMovement.currentGridPosition.x < 0)
                    {
                        DoAttackMotion(MoveDirection.Right);
                    }
                    else if (m_characterMovement.currentGridPosition.z - enemyUnit.m_characterMovement.currentGridPosition.z > 0)
                    {
                        DoAttackMotion(MoveDirection.Back);
                    }
                    else if (m_characterMovement.currentGridPosition.z - enemyUnit.m_characterMovement.currentGridPosition.z < 0)
                    {
                        DoAttackMotion(MoveDirection.Front);
                    }

                    BattleManager.ApplyDamage(unitIndex, enemyUnit.unitIndex);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// 敵のリストから、攻撃すべき敵を返す
            /// </summary>
            /// <param name="enemyUnits"></param>
            /// <param name="pattern"></param>
            /// <returns></returns>
            public UnitBaseController SelectAttackEnemy(List<UnitBaseController> enemyUnits, TargetUnitDetectionPattern pattern)
            {
                if (enemyUnits.Count <= 0)
                {
                    return null;
                }

                for (int i = 0; i < enemyUnits.Count; i++)
                {
                    // Todo ひとまずPlayerかHeroのみ
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
                            if (enemyUnits[i] == UnitManager.Instance.playerController)
                            {
                                return enemyUnits[i];
                            }
                            break;
                        case TargetUnitDetectionPattern.Hero:
                            if (enemyUnits[i] == UnitManager.Instance.heroController)
                            {
                                return enemyUnits[i];
                            }
                            break;
                        case TargetUnitDetectionPattern.OtherUnit:
                            break;
                        default:
                            break;
                    }
                }

                return enemyUnits[0];
            }

            /// <summary>
            /// ダメージを受けた際の処理
            /// </summary>
            /// <param name="fromUnit"></param>
            public void OnAnyDamage(UnitBaseController fromUnit)
            {
                DoDamageMotion();

                // 死亡処理を呼ぶ。オブジェクトの削除はマネージャで行う
                if (m_status.hp <= 0f)
                {
                    // todo 同処理するか保留
                    if (m_isDontDestroyUnit)
                    {
                        return;
                    }

                    UnitManager.OnKilledUnit(unitIndex);
                    //TODO 死亡アニメ等コルーチン後削除。とりあえず仮                    
                    Destroy(gameObject);
                }
            }

            /// <summary>
            /// 攻撃モーションを行う
            /// </summary>
            /// <param name="dir"></param>
            protected void DoAttackMotion(MoveDirection dir)
            {
                if (isAttacking)
                {
                    return;
                }

                isAttacking = true;
                StartCoroutine(AttackToCoroutine(m_status.attackExpandingTime, dir));
            }

            /// <summary>
            /// 攻撃時のウェイト
            /// </summary>
            /// <param name="time"></param>
            /// <param name="dir"></param>
            /// <returns></returns>
            IEnumerator AttackToCoroutine(float time, MoveDirection dir)
            {
                float bufferTime = 0;

                // Todo 仮アクション。これはアニメーション側で吸収する予定
                float x, y, z;
                x = y = z = 0f;
                switch (dir)
                {
                    case MoveDirection.KeepPosition:
                        break;
                    case MoveDirection.Front:
                        z = 0.5f;
                        break;
                    case MoveDirection.Back:
                        z = -0.5f;
                        break;
                    case MoveDirection.Left:
                        x = -0.5f;
                        break;
                    case MoveDirection.Right:
                        x = 0.5f;
                        break;
                    default:
                        break;
                }
                y = 1;

                // 速度正規化
                x = x / time;
                y = y / time;
                z = z / time;

                while (bufferTime < time)
                {
                    yield return new WaitForEndOfFrame();

                    float _x = 0, _y = 0, _z = 0;
                    if (bufferTime <= time * 1 / 4)
                    {
                        _x = x;
                        _y = y;
                        _z = z;
                    }
                    else if (bufferTime <= time * 2 / 4)
                    {
                        _x = x;
                        _y = -y;
                        _z = z;
                    }
                    else if (bufferTime <= time * 3 / 4)
                    {
                        _x = -x;
                        _y = y;
                        _z = -z;
                    }
                    else
                    {
                        _x = -x;
                        _y = -y;
                        _z = -z;
                    }

                    transform.position += new Vector3(
                        _x * Time.deltaTime,
                        _y * Time.deltaTime,
                        _z * Time.deltaTime);

                    bufferTime += Time.deltaTime;
                }

                // 半端な値が入るので補正
                transform.position = GridManager.Instance.GetPositionByGrid(m_characterMovement.currentGridPosition);
                isAttacking = false;
            }

            /// <summary>
            /// TODO 仮ダメージモーションを行う
            /// </summary>
            /// <param name="dir"></param>
            protected void DoDamageMotion()
            {
                StartCoroutine(DamageMotionCoroutine());
            }

            /// <summary>
            /// TODO 仮ダメージモーションのコルーチン
            /// </summary>
            /// <param name="time"></param>
            /// <param name="dir"></param>
            /// <returns></returns>
            IEnumerator DamageMotionCoroutine()
            {
                // Todo 仮
                var mat = transform.GetComponentInChildren<Renderer>().material;
                var defCol = mat.color;

                int fream = 0;

                while (fream < 8)
                {
                    yield return new WaitForEndOfFrame();

                    if (fream % 2 == 0)
                    {
                        mat.color = Color.red;
                    }
                    else
                    {
                        mat.color = Color.white;
                    }

                    fream++;
                }

                mat.color = defCol;
            }
        }
    }
}
