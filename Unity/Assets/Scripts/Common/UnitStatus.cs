namespace MyGame
{
    namespace Common
    {
        public class UnitStatus
        {
            public UnitStatus(
                float _maxHp = 2f,
                float _maxMp = 2f,
                float _moveExpendedTime = 0.5f,
                int _moveGridNum = 1,
                float _attackPower = 1f,
                int _attackRange = 1,
                float _damageCutRatio = 0f,
                float _idleWaitTime = 0.5f
                )
            {
                maxHp = hp = _maxHp;                
                maxMp = hp = _maxMp;
                moveExpendedTime = _moveExpendedTime;
                moveGridNum = _moveGridNum;
                attackPower = _attackPower;
                attackRange = _attackRange;
                damageCutRatio = _damageCutRatio;
                idleWaitTime = _idleWaitTime;
            }

            // ヒットポイント
            float m_hp;
            public float hp
            {
                get { return m_hp; }
                set { m_hp = value; }
            }

            // 最大ヒットポイント
            float m_maxHp;
            public float maxHp
            {
                get { return m_maxHp; }
                set { m_maxHp = value; }
            }

            // マジックポイント
            float m_mp;
            public float mp
            {
                get { return m_mp; }
                set { m_mp = value; }
            }

            // 最大マジックポイント
            float m_maxMp;
            public float maxMp
            {
                get { return m_maxMp; }
                set { m_maxMp = value; }
            }

            // スピード。1マス移動するのにかかる時間
            float m_moveExpandingTime;
            public float moveExpendedTime
            {
                get { return m_moveExpandingTime; }
                set { m_moveExpandingTime = value; }
            }

            // 移動距離。一回の移動で最大何マス動くか
            int m_moveGridNum;
            public int moveGridNum
            {
                get { return m_moveGridNum; }
                set { m_moveGridNum = value; }
            }

            // アタックパワー
            float m_attackPower;
            public float attackPower
            {
                get { return m_attackPower; }
                set { m_attackPower = value; }
            }

            // 攻撃可能距離。直線距離想定
            int m_attackRange;
            public int attackRange
            {
                get { return m_attackRange; }
                set { m_attackRange = value; }
            }

            // 攻撃に費やす時間
            // float m_attackExpandingTime;
            public float attackExpandingTime
            {
                //get { return m_attackExpandingTime; }
                get { return m_moveExpandingTime; }
                //set { m_attackExpandingTime = value; }
            }

            // ダメージカット率。防御力の代わり
            float m_damageCutRatio;
            public float damageCutRatio
            {
                get { return m_damageCutRatio; }
                set { m_damageCutRatio = value; }
            }

            // 行動開始前のウェイト時間
            float m_idleWaitTIme;
            public float idleWaitTime
            {
                get { return m_idleWaitTIme; }
                set { m_idleWaitTIme = value; }
            }

            // TODO 装備ID
            // equipment

            // TODO バフID
            // Buff
        }
    }
}
