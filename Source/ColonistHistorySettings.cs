using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    public class ColonistHistorySettings : ModSettings {
        private Dictionary<string, bool> colonistHistoryOutput = null;
        public int recordingIntervalHours = 6;
        public string saveFolderPath = DefaultSaveFolderPath;
        public bool saveNullOrEmpty = true;
        
        public Dictionary<string, bool> ColonistHistoryOutput {
            get {
                if (this.colonistHistoryOutput == null) {
                    InitColonistHistoryOutput();
                }
                return this.colonistHistoryOutput;
            }
        }

        public IEnumerable<ColonistHistoryDef> OutputColonistHistorys {
            get {
                foreach (ColonistHistoryDef def in DefDatabase<ColonistHistoryDef>.AllDefsListForReading) {
                    if (CanOutput(def)) {
                        yield return def;
                    }
                }
            }
        }

        public static string DefaultSaveFolderPath {
            get {
                MethodInfo FolderUnderSaveData = typeof(GenFilePaths).GetMethod("FolderUnderSaveData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
                return (string)FolderUnderSaveData.Invoke(null, new object[] { "ColonistHistory" });
            }
        }

        public void InitColonistHistoryOutput() {
            this.colonistHistoryOutput = new Dictionary<string, bool>();
            foreach (ColonistHistoryDef def in DefDatabase<ColonistHistoryDef>.AllDefsListForReading) {
                this.colonistHistoryOutput[def.defName] = def.defaultOutput;
            }
        }

        public bool CanOutput(ColonistHistoryDef def) {
            bool result = false;
            if (!this.ColonistHistoryOutput.TryGetValue(def.defName, out result)) {
                this.ColonistHistoryOutput[def.defName] = def.defaultOutput;
            }
            return result;
        }

        public override void ExposeData() {
            Scribe_Collections.Look(ref this.colonistHistoryOutput, "colonistHistoryOutput");
            Scribe_Values.Look(ref this.recordingIntervalHours, "recordingIntervalHours");
            Scribe_Values.Look(ref this.saveNullOrEmpty, "saveNullOrEmpty", true);
            Scribe_Values.Look(ref this.saveFolderPath, "saveFolderPath", DefaultSaveFolderPath);
        }
    }
}
