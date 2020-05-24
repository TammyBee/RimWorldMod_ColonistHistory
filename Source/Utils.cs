using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    internal static class Utils {
        public static string HoursToString(this int hours) {
            int tick = hours * GenDate.TicksPerHour;
            return tick.ToStringTicksToPeriod();
        }

        public static void ScribeObjectValue(ref object value, string label, Type type) {
            if (TryScribeObjectValueInternal<int>(ref value, label, type)) {
                return;
            }
            if (TryScribeObjectValueInternal<float>(ref value, label, type)) {
                return;
            }
            if (TryScribeObjectValueInternal<string>(ref value, label, type)) {
                return;
            }
            Log.Warning("[ScribeObjectValue] cannot scribe value.\n" + string.Join("/ ", label, value.ToStringSafe(), type, Scribe.mode));
        }

        private static bool TryScribeObjectValueInternal<T>(ref object value, string label, Type type) {
            if (type == typeof(T) && (value == null || value.GetType() == typeof(T))) {
                T x = default(T);
                if (Scribe.mode == LoadSaveMode.Saving) {
                    if (value != null) {
                        x = (T)value;
                    }
                }
                Scribe_Values.Look<T>(ref x, label, default(T), true);
                if (Scribe.mode == LoadSaveMode.LoadingVars) {
                    value = x;
                }
                return true;
            }
            return false;
        }

        public static void ScribeDefValue(ref Def value, string label, bool onlyDefNameSave) {
            if (value != null) {
                ScribeDefValue(ref value, label, value.GetType(), onlyDefNameSave);
            }
        }

        public static void ScribeDefValue(ref Def value, string label, Type type, bool onlyDefNameSave) {
            string labelDefType = null;
            if (!onlyDefNameSave) {
                labelDefType = label + "_defType";
            }
            ScribeDefValue(ref value, label + "_defName", labelDefType, type);
        }

        public static void ScribeDefValue(ref Def value, string labelDefName, string labelDefType, Type type) {
            if (TryScribeDefValueInternal<SkillDef>(ref value, labelDefName, labelDefType, type)) {
                return;
            }
            if (TryScribeDefValueInternal<RecordDef>(ref value, labelDefName, labelDefType, type)) {
                return;
            }
            Log.Warning("[ScribeDefValue] cannot scribe def.\n" + string.Join("/ ", labelDefName, labelDefType, value.ToStringSafe(), type, value.ToStringSafe(), Scribe.mode));
        }

        private static bool TryScribeDefValueInternal<T>(ref Def value, string labelDefName, string labelDefType, Type type) where T : Def {
            if (type != null && type == typeof(T)) {
                string defName = "";
                if (Scribe.mode == LoadSaveMode.Saving) {
                    defName = value.defName;
                }
                Scribe_Values.Look<string>(ref defName, labelDefName);
                if (labelDefType != null) {
                    Scribe_Values.Look<Type>(ref type, labelDefType);
                }
                if (Scribe.mode == LoadSaveMode.LoadingVars) {
                    value = DefDatabase<T>.GetNamed(defName);
                }
                return true;
            }
            return false;
        }

        public static void ScribeObjectsValue(ref List<object> values, string label, Type type) {
            if (TryScribeObjectsValueInternal<int>(ref values, label, type)) {
                return;
            }
            if (TryScribeObjectsValueInternal<float>(ref values, label, type)) {
                return;
            }
            if (TryScribeObjectsValueInternal<string>(ref values, label, type)) {
                return;
            }
            Log.Warning("[ScribeObjectsValue] cannot scribe values.\n" + string.Join("/ ", label, values.ToStringSafeEnumerable(), type, Scribe.mode));
        }

        private static bool TryScribeObjectsValueInternal<T>(ref List<object> values, string label, Type type) {
            if (type == typeof(T) && (values.NullOrEmpty() || values.First().GetType() == typeof(T))) {
                List<T> list = default(List<T>);
                if (Scribe.mode == LoadSaveMode.Saving) {
                    if (values != null) {
                        list = values.ConvertAll<T>(v => (T)v);
                    } else {
                        list = null;
                    }
                }
                Scribe_Collections.Look<T>(ref list, label, LookMode.Value);
                if (Scribe.mode == LoadSaveMode.LoadingVars) {
                    values = new List<object>();
                    if (list != null) {
                        foreach (T x in list) {
                            values.Add(x);
                        }
                    } else {
                        values = null;
                    }
                }
                return true;
            }
            return false;
        }

        public static string ConvertToDateTimeString(int tick, int tile) {
            Vector2 vector = Find.WorldGrid.LongLatOf(tile);
            string hourString = GenDate.HourInteger((long)tick, vector.x) + "LetterHour".Translate();
            return "ColonistHistory.DateString".Translate(GenDate.DateReadoutStringAt((long)tick, vector), hourString);
        }

        public static bool ExistExtraNoPlayerFactions(this Pawn p) {
            List<Quest> questsListForReading = Find.QuestManager.QuestsListForReading;
            for (int i = 0; i < questsListForReading.Count; i++) {
                List<QuestPart> partsListForReading = questsListForReading[i].PartsListForReading;
                for (int j = 0; j < partsListForReading.Count; j++) {
                    QuestPart_ExtraFaction questPart_ExtraFaction = partsListForReading[j] as QuestPart_ExtraFaction;
                    if (questPart_ExtraFaction?.extraFaction?.faction != null && questPart_ExtraFaction.affectedPawns.Contains(p) && !questPart_ExtraFaction.extraFaction.faction.IsPlayer ) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
