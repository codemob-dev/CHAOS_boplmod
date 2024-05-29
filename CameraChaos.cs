using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace CHAOS
{
    public class UpsideDownCamera : ChaosEvent
    {
        static Harmony harmony = new Harmony(CHAOS.harmony.Id + ".UpsideDownCamera");
        public override void Disable()
        {
            harmony.UnpatchSelf();
        }

        public override string Enable()
        {
            harmony.PatchAll(GetType());

            var func = typeof(PlayerAverageCamera).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
            var patch = GetType().GetMethod(nameof(PlayerAverageCamera_Start));

            harmony.Patch(func, postfix: new HarmonyMethod(patch));
            return "Upside Down Camera";
        }

        public static void PlayerAverageCamera_Start(PlayerAverageCamera __instance)
        {
            Camera camera = __instance.GetComponent<Camera>();
            camera.projectionMatrix *= Matrix4x4.Scale(new Vector3(1, -1, 1));
        }

        public override void LevelStart()
        {

        }
    }
}
