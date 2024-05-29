using BepInEx;
using HarmonyLib;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Mathematics;

namespace CHAOS
{
    [BepInPlugin("com.codemob.chaos", "CHAOS", "1.0.0")]
    public class CHAOS : BaseUnityPlugin
    {
        internal static Harmony harmony;
        internal static GameObject overlayObject = null;
        internal static Canvas overlayCanvas;
        internal static List<ChaosText> chaosTexts = new List<ChaosText>();
        internal static List<int> activeChaos = new List<int>();
        internal static List<ChaosEvent> availableChaos = new List<ChaosEvent>();

        public static IEnumerable<ChaosEvent> InactiveChaos => availableChaos.Where(chaos => !chaos.Active);
        public static IEnumerable<ChaosEvent> ActiveChaos => activeChaos.Select(id => availableChaos[id]);

        public static int maxChaosActive = 3;
        private void Awake()
        {
            harmony = new Harmony(Info.Metadata.GUID);
            harmony.PatchAll(typeof(CHAOS));

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;

            RegisterChaosEvent<BigBopls>();
            RegisterChaosEvent<UpsideDownCamera>();
            RegisterChaosEvent<SpeedyBopls>();
            RegisterChaosEvent<AbilityScramble>();
        }

        public static void RegisterChaosEvent<T>() where T : ChaosEvent, new()
        {
            ChaosEvent ev = new T
            {
                Id = availableChaos.Count
            };
            availableChaos.Add(ev);
        }

        public static void ActivateChaos(int id)
        {
            if (id >= availableChaos.Count)
            {
                throw new IndexOutOfRangeException(
                    $"Id must be less than the number of available chaos! " +
                    $"({id} >= {availableChaos.Count})");
            }

            var chaos = availableChaos[id];
            if (chaos.Active)
            {
                throw new InvalidOperationException(
                    $"Cannot activate an already activated chaos! " +
                    $"({chaos.GetType().Name} is already active)");
            };

            activeChaos.Add(id);
            AddChaosText(chaos.Enable());
            chaos.Active = true;
        }

        public static void DeactivateFirstChaos()
        {
            if (!activeChaos.Any())
            {
                throw new IndexOutOfRangeException("No chaos to deactivate!");
            }
            var chaosId = activeChaos.First();
            activeChaos.RemoveAt(0);

            var chaos = availableChaos[chaosId];
            chaos.Disable();
            RemoveChaosText();
            chaos.Active = false;
        }

        public static void DeactivateSpecificChaos(int id)
        {
            if (!activeChaos.Contains(id))
            {
                throw new IndexOutOfRangeException("Chaos is not activated or does not exist!");
            }

            activeChaos.Remove(id);
            var chaos = availableChaos[id];
            chaos.Disable();

        }

        private static ChaosText AddChaosText(string text)
        {
            Vector2? previousPosition;
            if (chaosTexts.Any())
            {
                previousPosition = chaosTexts.Last().animateTo;
            }
            else
            {
                previousPosition = null;
            }

            chaosTexts.Add(ChaosText.CreateNew(text, previousPosition));
            return chaosTexts.Last();
        }

        private static void RemoveChaosText(int id = 0)
        {
            if (!chaosTexts.Any()) return;
            chaosTexts[id].Remove();
            chaosTexts.RemoveAt(0);
            for (int i = id; i < chaosTexts.Count; i++)
            {
                chaosTexts[i].Move(-1);
            }
        }

        private static void DeactivateAllChaos()
        {
            foreach (var text in chaosTexts)
            {
                text.Remove();
            }
            chaosTexts.Clear();

            foreach (var chaosId in activeChaos)
            {
                availableChaos[chaosId].Disable();
            }
            activeChaos.Clear();
        }

        internal static Unity.Mathematics.Random RandomFromState
        {
            get
            {
                uint num = 0;
                num += GameSession.CurrentLevel();
                num *= (uint)PlayerHandler.Get().NumberOfPlayers();
                num *= (uint)PlayerHandler.Get().NumberOfTeams();
                foreach (var player in PlayerHandler.Get().PlayerList())
                {
                    num += (uint)((player.Deaths + 1) * (player.GamesWon + 1) + (player.Team + 1) * (player.Kills + 1));
                }
                return new Unity.Mathematics.Random(num);
            }
        }

        [HarmonyPatch(typeof(GameSessionHandler), nameof(GameSessionHandler.LoadNextLevelScene))]
        [HarmonyPrefix]
        private static void GameSessionHandler_LoadNextLevelScene()
        {
            var inactiveChaos = InactiveChaos.ToList();
            if (inactiveChaos.Count() == 0) return;
            ActivateChaos(inactiveChaos[RandomFromState.NextInt(inactiveChaos.Count())].Id);
            if (chaosTexts.Count > maxChaosActive) DeactivateFirstChaos();
        }

        [HarmonyPatch(typeof(GameSession), nameof(GameSession.Init))]
        [HarmonyPrefix]
        private static void GameSession_Init()
        {
            DeactivateAllChaos();
        }

        [HarmonyPatch(typeof(Updater), nameof(Updater.PostLevelLoad))]
        [HarmonyPrefix]
        private static void Updater_PostLevelLoad()
        {
            foreach (var chaos in ActiveChaos)
            {
                chaos.LevelStart();
            }
        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode arg1)
        {
            if (overlayObject != null)
            {
                return;
            }

            // Create a new canvas
            overlayObject = new GameObject("chaos object :3");
            DontDestroyOnLoad(overlayObject);
            overlayCanvas = overlayObject.AddComponent<Canvas>();
            overlayCanvas.name = "chaos canvas :3";
            CanvasScaler scaler = overlayObject.AddComponent<CanvasScaler>();

            overlayCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            overlayCanvas.worldCamera = Camera.current;

            overlayCanvas.sortingLayerName = "behind Walls Infront of everything else";
            overlayCanvas.sortingOrder = 1;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        }
    }
}