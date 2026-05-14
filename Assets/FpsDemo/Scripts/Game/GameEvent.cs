using FpsDemo.Combat;

namespace FpsDemo.Game
{
    public class GameEvent {}

    public class PlayerLandEvent : GameEvent
    {
        public float velocity;
    }

    public class DamageDealtEvent : GameEvent
    {
        public DamageResult damageResult;
    }
}