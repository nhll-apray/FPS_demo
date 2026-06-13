namespace FpsDemo.Config
{
    public static class GameResourcePaths
    {
        public static class Config
        {
            public static class Player
            {
                public const string DefaultCameraEffect = "Player/DefaultPlayerCameraEffectProfile";
            }

            public static class Weapon
            {
                public const string GrenadeAltFire = "Weapon/GrenadeAltFireData";
                public const string HitscanWeaponAk47 = "Weapon/HitscanWeaponData_AK47";
            }

            public static class Enemy
            {
                public const string GoblinMelee = "Enemy/GoblinMeleeData";
                public const string ElfRanged = "Enemy/ElfRangedData";
            }
        }

        public static class Prefabs
        {
            public static class Enemy
            {
                public const string EnemyDum = "Enemy/EnemyDum";
                public const string EnemyGoblin = "Enemy/Enemy_Goblin";
                public const string EnemyRangedElf = "Enemy/Enemy_Ranged_Elf";
            }

            public static class Projectiles
            {
                public const string HandGrenade = "Projectiles/Hand_Grenade_Prefab";
                public const string EnemyEnergyBall = "Projectiles/Enemy_Energy_Ball";
            }

            public static class VFX
            {
                public const string Explosion = "VFX/Explosion Prefab";
            }
        }

        public static class Audio
        {
            public static class Sfx
            {
                public const string Damage = "damage";
                public const string Headshot = "headshot";
                public const string Kill = "kill";
                public const string Soldier76 = "soldier-76-0000000436CE";

                public static class Weapon
                {
                    public static class AK47
                    {
                        public const string Shoot = "Weapon/AK47/shoot";
                        public const string Reload = "Weapon/AK47/reload";
                    }
                }
            }
        }
    }
}
