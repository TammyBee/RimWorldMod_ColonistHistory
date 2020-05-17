using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    [StaticConstructorOnStartup]
    public class HarmonyPatches {
        static HarmonyPatches() {
            var harmony = new Harmony("com.tammybee.colonisthistory");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(SimpleCurveDrawer))]
    [HarmonyPatch("DrawCurveMousePoint")]
    public class SimpleCurveDrawer_DrawCurveMousePoint_Patch {
        public static Rect screenRect;
        public static Rect viewRect;

        static void Prefix(Rect screenRect, Rect viewRect) {
            SimpleCurveDrawer_DrawCurveMousePoint_Patch.screenRect = screenRect;
            SimpleCurveDrawer_DrawCurveMousePoint_Patch.viewRect = viewRect;
        }
    }
}
