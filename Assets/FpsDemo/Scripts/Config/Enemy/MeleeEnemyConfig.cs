using UnityEngine;

namespace FpsDemo.Config.Enemy
{
    [CreateAssetMenu(fileName = "MeleeEnemyConfig", menuName = "Config/Enemy/Melee", order = 0)]
    public class MeleeEnemyConfig : EnemyConfigBase
    {
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float attackHitRange = 2.2f;
        [SerializeField] private float attackHitAngle = 100f;

        public int AttackDamage => attackDamage;
        public float AttackHitRange => attackHitRange;
        public float AttackHitAngle => attackHitAngle;
    }
}
