using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            Log.Error("[ScribeObjectValue] cannot scribe value.\n" + string.Join("/ ", label, type, Scribe.mode));
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
            Log.Error("[ScribeDefValue] cannot scribe def.\n" + string.Join("/ ", labelDefName, labelDefType, type, value.ToStringSafe(), Scribe.mode));
        }

        private static bool TryScribeDefValueInternal<T>(ref Def value, string labelDefName, string labelDefType, Type type) where T : Def {
            if (type != null) {
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
            Log.Error("[ScribeObjectsValue] cannot scribe values.\n" + string.Join("/ ", label, type, Scribe.mode));
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

        public static string ConvertToDateTimeString(int tick,int tile) {
            Vector2 vector = Find.WorldGrid.LongLatOf(tile);
            string hourString = GenDate.HourInteger((long)tick, vector.x) + "LetterHour".Translate();
            return "ColonistHistoryWorker.DateString".Translate(GenDate.DateReadoutStringAt((long)tick, vector), hourString);
        }
    }
}
