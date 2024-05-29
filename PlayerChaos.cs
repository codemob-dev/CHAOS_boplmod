using BoplFixedMath;

namespace CHAOS
{
    public class BigBopls : ChaosEvent
    {
        public override string Enable()
        {
            return "Big Bopls";
        }
        public override void Disable()
        {
            return;
        }
        public override void LevelStart()
        {
            foreach (var player in PlayerHandler.Get().PlayerList())
            {
                player.Scale = (Fix)3;
            }
        }
    }
}
