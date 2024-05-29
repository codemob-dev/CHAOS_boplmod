using BoplFixedMath;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CHAOS
{
    public abstract class ChaosEvent
    {
        public bool Active { get; internal set; } = false;
        public int Id { get; internal set; }
        public ChaosEvent() { }
        public abstract string Enable();
        public abstract void Disable();
        public abstract void LevelStart();
    }

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
