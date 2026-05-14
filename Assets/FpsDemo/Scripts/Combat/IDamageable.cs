namespace FpsDemo.Combat
{
    public interface IDamageable
    {
        public DamageResult TakeDamage(DamageInfo damageInfo);
    }
}