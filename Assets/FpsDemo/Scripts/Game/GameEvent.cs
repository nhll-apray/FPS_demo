using FpsDemo.Combat;

namespace FpsDemo.Game
{
    public class GameEvent {}

    public class PlayerLandEvent : GameEvent
    {
        public float Velocity;
    }

    public class DamageDealtEvent : GameEvent
    {
        public DamageResult DamageResult;
    }
}