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

    [HarmonyPatch(typeof(Widgets))]
    [HarmonyPatch("DrawLine")]
    public class Widgets_DrawLine_Patch {
        public static float fixWidth = 0f;

        static void Prefix(ref float width) {
            if (!Mathf.Approximately(fixWidth, 0f)) {
                width = fixWidth;
            }
        }
    }

    [HarmonyPatch(typeof(SimpleCurveDrawer))]
    [HarmonyPatch("DrawCurveLines")]
    public class SimpleCurveDrawer_DrawCurveLines_Patch {
        public static SimpleCurveDrawInfo highLightCurve = null;

        public static bool IsHighlightedCurve(SimpleCurveDrawInfo curve) {
            return highLightCurve != null && highLightCurve.label == curve.label && highLightCurve.color == curve.color;
        }

        static void Prefix(SimpleCurveDrawInfo curve) {
            if (IsHighlightedCurve(curve)) {
                Widgets_DrawLine_Patch.fixWidth = ColonistHistoryMod.Settings.highlightedCurveWidth;
            }
        }

        static void Postfix() {
            Widgets_DrawLine_Patch.fixWidth = 0f;
        }
    }
}
