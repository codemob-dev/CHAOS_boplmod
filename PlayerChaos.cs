using BoplFixedMath;
using HarmonyLib;

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

    public class SpeedyBopls : ChaosEvent
    {
        static Harmony harmony = new Harmony(CHAOS.harmony.Id + ".SpeedyBopls");
        public override string Enable()
        {
            harmony.PatchAll(GetType());
            return "Speedy Bopls";
        }
        public override void Disable()
        {
            harmony.UnpatchSelf();
            return;
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.UpdateSim))]
        [HarmonyPrefix]
        public static void PlayerPhysics_UpdateSim(PlayerPhysics __instance)
        {
            __instance.Speed = (Fix)40;
        }
        public override void LevelStart()
        {
        }
    }
}
