namespace FpsDemo.Game
{
    public static class GameResourcePaths
    {
        public static class Data
        {
            public static class Player
            {
                public const string DefaultCameraEffectProfile = "Player/DefaultPlayerCameraEffectProfile";
            }

            public static class Weapon
            {
                public const string GrenadeAltFireData = "Weapon/GrenadeAltFireData";
                public const string HitscanWeaponDataAK47 = "Weapon/HitscanWeaponData_AK47";
            }
        }

        public static class Prefabs
        {
            public static class Enemy
            {
                public const string EnemyDum = "Enemy/EnemyDum";
            }

            public static class Projectiles
            {
                public const string HandGrenade = "Projectiles/Hand_Grenade_Prefab";
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
