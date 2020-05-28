using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    public class ColonistHistorySettings : ModSettings {
        private Dictionary<string, bool> colonistHistoryOutput = null;
        public int recordingIntervalHours = 24;
        public string saveFolderPath = DefaultSaveFolderPath;
        public bool saveNullOrEmpty = true;
        public bool lightWeightSaveMode = false;

        public bool recordOtherFactionPawn = true;
        public bool showOtherFactionPawn = true;

        public float highlightedCurveWidth = 2f;
        public bool treatingUnrecordedAsZero = false;
        public bool addZeroBeforeFirst = true;

        public bool hideNullOrEmpty = true;
        public bool hideUnrecorded = true;

        private List<ColonistHistoryDef> colonistHistorysOrder = null;
        private List<string> colonistHistorysOrderDefNames = new List<string>();

        private Dictionary<string, bool> colonistHistoryOutputDetailed = null;

        public Dictionary<string, bool> ColonistHistoryOutput {
            get {
                if (this.colonistHistoryOutput == null) {
                    InitColonistHistoryOutput();
                }
                return this.colonistHistoryOutput;
            }
        }

        public Dictionary<string, bool> ColonistHistoryOutputDetailed {
            get {
                if (this.colonistHistoryOutputDetailed == null) {
                    InitColonistHistoryOutputDetailed();
                }
                return this.colonistHistoryOutputDetailed;
            }
        }

        public IEnumerable<ColonistHistoryDef> OutputColonistHistories {
            get {
                foreach (ColonistHistoryDef def in ColonistHistorysOrder) {
                    if (CanOutput(def)) {
                        yield return def;
                    }
                }
            }
        }

        public List<ColonistHistoryDef> ColonistHistorysOrder {
            get {
                if (this.colonistHistorysOrder == null) {
                    this.colonistHistorysOrder = this.colonistHistorysOrderDefNames.ConvertAll(defName => DefDatabase<ColonistHistoryDef>.GetNamed(defName));
                    foreach (ColonistHistoryDef def in DefDatabase<ColonistHistoryDef>.AllDefsListForReading) {
                        if (!this.colonistHistorysOrder.Contains(def)) {
                            this.colonistHistorysOrder.Add(def);
                        }
                    }
                }
                return this.colonistHistorysOrder;
            }
        }

        public static string DefaultSaveFolderPath {
            get {
                MethodInfo FolderUnderSaveData = AccessTools.Method(typeof(GenFilePaths), "FolderUnderSaveData");
                return (string)FolderUnderSaveData.Invoke(null, new object[] { "ColonistHistory" });
            }
        }

        public void InitColonistHistoryOutput() {
            this.colonistHistoryOutput = new Dictionary<string, bool>();
            foreach (ColonistHistoryDef def in ColonistHistorysOrder) {
                this.colonistHistoryOutput[def.defName] = def.defaultOutput;
            }
        }

        public void InitColonistHistoryOutputDetailed() {
            this.colonistHistoryOutputDetailed = new Dictionary<string, bool>();
            foreach (ColonistHistoryDef def in ColonistHistorysOrder) {
                foreach (RecordIdentifier recordID in def.RecordIDs) {
                    this.colonistHistoryOutputDetailed[recordID.ID] = CanOutput(def);
                }
            }
        }

        public bool CanOutput(ColonistHistoryDef def) {
            bool result = false;
            if (!this.ColonistHistoryOutput.TryGetValue(def.defName, out result)) {
                this.ColonistHistoryOutput[def.defName] = def.defaultOutput;
            }
            return result;
        }

        public bool CanOutput(RecordIdentifier recordID) {
            bool result = false;
            if (!this.ColonistHistoryOutputDetailed.TryGetValue(recordID.ID, out result)) {
                this.ColonistHistoryOutputDetailed[recordID.ID] = recordID.colonistHistoryDef.defaultOutput;
            }
            return result;
        }

        public override void ExposeData() {
            Scribe_Collections.Look(ref this.colonistHistoryOutput, "colonistHistoryOutput");
            Scribe_Collections.Look(ref this.colonistHistoryOutputDetailed, "colonistHistoryOutputDetailed");
            Scribe_Values.Look(ref this.recordingIntervalHours, "recordingIntervalHours");
            Scribe_Values.Look(ref this.saveNullOrEmpty, "saveNullOrEmpty", true);
            Scribe_Values.Look(ref this.lightWeightSaveMode, "lightWeightSaveMode", false);

            Scribe_Values.Look(ref this.recordOtherFactionPawn, "recordOtherFactionPawn", true);
            Scribe_Values.Look(ref this.showOtherFactionPawn, "showOtherFactionPawn", true);

            Scribe_Values.Look(ref this.highlightedCurveWidth, "highlightedCurveWidth", 2f);
            Scribe_Values.Look(ref this.treatingUnrecordedAsZero, "treatingUnrecordedAsZero", false);
            Scribe_Values.Look(ref this.addZeroBeforeFirst, "addZeroBeforeFirst", true);

            Scribe_Values.Look(ref this.hideNullOrEmpty, "hideNullOrEmpty", true);
            Scribe_Values.Look(ref this.hideUnrecorded, "hideUnrecorded", true);

            if (Scribe.mode == LoadSaveMode.Saving) {
                this.colonistHistorysOrderDefNames = this.colonistHistorysOrder.ConvertAll(def => def.defName);
            }
            Scribe_Collections.Look(ref this.colonistHistorysOrderDefNames, "colonistHistorysOrder", LookMode.Value);
        }
    }
}
