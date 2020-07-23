using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class GameComponent_ColonistHistoryRecorder : GameComponent {
        private int lastAutoRecordTick = -1;
        private int lastManualRecordTick = -1;
        private bool lastRecordIsManual = false;
        private int firstTile = -1; 
        private Dictionary<Pawn, ColonistHistoryDataList> colonistHistories = new Dictionary<Pawn, ColonistHistoryDataList>();

        private HashSet<RecordIdentifier> cacheAvailableRecords = null;
        private int cachedFirstRecordTick = -1;

        private bool isLightWeightSave = false;
        private Dictionary<Pawn, ColonistHistoryDataList> lightWeightColonistHistories = new Dictionary<Pawn, ColonistHistoryDataList>();

        private List<Pawn> tmpPawns = new List<Pawn>();
        private List<ColonistHistoryDataList> tmpColonistHistories = new List<ColonistHistoryDataList>();

        public static GameComponent_ColonistHistoryRecorder Instance {
            get {
                return Current.Game.GetComponent<GameComponent_ColonistHistoryRecorder>();
            }
        }

        public Dictionary<Pawn, ColonistHistoryDataList> ColonistHistories {
            get {
                return this.colonistHistories;
            }
        }

        public int NextRecordTick {
            get {
                if (this.LastAutoRecordTick != -1) {
                    return this.LastAutoRecordTick + ColonistHistoryMod.Settings.recordingIntervalHours * GenDate.TicksPerHour;
                }
                return -1;
            }
        }

        public int LastAutoRecordTick {
            get {
                return this.lastAutoRecordTick;
            }
        }

        public int LastManualRecordTick {
            get {
                return this.lastManualRecordTick;
            }
        }

        public int LastRecordTick {
            get {
                if (LastRecordIsManual) {
                    return LastManualRecordTick;
                } else {
                    return LastAutoRecordTick;
                }
            }
        }

        public string LastRecordDateTime {
            get {
                return Utils.ConvertToDateTimeString(this.LastRecordTick, this.firstTile);
            }
        }

        public bool LastRecordIsManual {
            get {
                return lastRecordIsManual;
            }
        }

        public int FirstRecordTick {
            get {
                if (this.cachedFirstRecordTick == -1) {
                    this.cachedFirstRecordTick = colonistHistories.Values.Select(v => v.log[0].recordTick).Min();
                }
                return this.cachedFirstRecordTick;
            }
        }

        public static string SaveFileName {
            get {
                string fileName = "";
                if (Faction.OfPlayer.HasName) {
                    fileName = Faction.OfPlayer.Name;
                } else {
                    fileName = SaveGameFilesUtility.UnusedDefaultFileName(Faction.OfPlayer.def.LabelCap);
                }
                fileName += "_" + Current.Game.World.info.seedString;
                return fileName;
            }
        }

        public static string SaveFilePath {
            get {
                return string.Join("/", ColonistHistoryMod.Settings.saveFolderPath, SaveFileName + ".xml");
            }
        }

        public IEnumerable<Pawn> Colonists {
            get {
                foreach (Pawn p in this.colonistHistories.Keys.OrderBy(x => x.thingIDNumber)) {
                    if (!this.colonistHistories[p].log.NullOrEmpty()) {
                        yield return p;
                    }
                }
            }
        }

        public HashSet<RecordIdentifier> AvailableRecords {
            get {
                if (this.cacheAvailableRecords == null) {
                    ResolveAvailableRecords();
                }
                return this.cacheAvailableRecords;
            }
        }

        public IEnumerable<RecordIdentifier> NumericRecords {
            get {
                foreach (RecordIdentifier recordID in this.AvailableRecords) {
                    if (recordID.IsNumeric) {
                        yield return recordID;
                    }
                }
            }
        }

        public GameComponent_ColonistHistoryRecorder(Game game) {
            this.lastAutoRecordTick = -1;
            this.lastManualRecordTick = -1;
            this.lastRecordIsManual = false;
            this.colonistHistories = new Dictionary<Pawn, ColonistHistoryDataList>();
        }

        public override void GameComponentTick() {
            if (Current.Game.tickManager.TicksAbs >= NextRecordTick) {
                Record();
            }
        }

        public ColonistHistoryDataList GetRecords(Pawn p) {
            return this.colonistHistories[p];
        }

        public bool Record(bool manualRecord = false) {
            int currentTick = Current.Game.tickManager.TicksAbs;
            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder().Where(p => ColonistHistoryMod.Settings.recordOtherFactionPawn || !p.ExistExtraNoPlayerFactions()).ToList();
            if (colonists.NullOrEmpty()) {
                return false;
            }

            if (this.firstTile == -1) {
                this.firstTile = colonists.First().MapHeld.Tile;
            }

            foreach (Pawn colonist in colonists) {
                if (!this.colonistHistories.ContainsKey(colonist)) {
                    this.colonistHistories[colonist] = new ColonistHistoryDataList(colonist);
                }
                ColonistHistoryData colonistHistoryData = new ColonistHistoryData(currentTick, this.firstTile, manualRecord, colonist);
                this.colonistHistories[colonist].Add(colonistHistoryData, colonist);
            }

            this.lastRecordIsManual = manualRecord;
            if (manualRecord) {
                this.lastManualRecordTick = currentTick;
            } else {
                this.lastAutoRecordTick = currentTick;
            }

            ResolveAvailableRecords();
            RecordGraphUtility.CurRecordGroup.ResolveGraph();

            return true;
        }

        public void Save(string fileName = null) {
            try {
                string filePath = SaveFilePath;
                if (fileName != null) {
                    filePath = string.Join("/", ColonistHistoryMod.Settings.saveFolderPath, fileName);
                }
                SafeSaver.Save(filePath, "root", delegate {
                    int xmlFormatVersion = ColonistHistoryMod.XmlFormatVersion;
                    Scribe_Values.Look(ref xmlFormatVersion, "xmlFormatVersion");
                    List<ColonistHistoryDataList> list = this.colonistHistories.Values.ToList();
                    Scribe_Collections.Look(ref list, "colonistHistories", LookMode.Deep);
                });
                Messages.Message("ColonistHistory.SaveColonistHistoriesFileAs".Translate(filePath), MessageTypeDefOf.NeutralEvent,false);
            } catch (Exception ex) {
                Log.Error("Exception while saving world: " + ex.ToString());
            }
        }

        public override void ExposeData() {
            Scribe_Values.Look(ref this.lastAutoRecordTick, "lastRecordTick", -1);
            Scribe_Values.Look(ref this.lastManualRecordTick, "lastManualRecordTick", -1);
            Scribe_Values.Look(ref this.lastRecordIsManual, "lastRecordIsManual", false);
            Scribe_Values.Look(ref this.firstTile, "firstTile", -1);

            if (Scribe.mode == LoadSaveMode.Saving) {
                isLightWeightSave = ColonistHistoryMod.Settings.lightWeightSaveMode;
            }
            Scribe_Values.Look(ref isLightWeightSave, "isLightWeightSave", false);

            if (isLightWeightSave) {
                //Log.Message("[Load]isLightWeightSave");
                if (Scribe.mode == LoadSaveMode.Saving) {
                    lightWeightColonistHistories = new Dictionary<Pawn, ColonistHistoryDataList>();
                    foreach (Pawn p in this.colonistHistories.Keys) {
                        lightWeightColonistHistories[p] = LightWeightSaver.GetLightWeight(this.colonistHistories[p]);
                    }
                }
                Scribe_Collections.Look(ref lightWeightColonistHistories, "colonistHistories", LookMode.Reference, LookMode.Deep, ref this.tmpPawns, ref this.tmpColonistHistories);
                //Log.Message("dict:" + Scribe.mode + "/" + (lightWeightColonistHistories.EnumerableNullOrEmpty() ? "null or 0" : (lightWeightColonistHistories?.Keys?.Count).ToStringSafe()));
                if (lightWeightColonistHistories?.Keys != null && lightWeightColonistHistories.Keys.Count > 0 && Scribe.mode != LoadSaveMode.Saving) {
                    //Log.Message("ConvertFromLightWeight:" + Scribe.mode);
                    this.colonistHistories = new Dictionary<Pawn, ColonistHistoryDataList>();
                    foreach (Pawn p in lightWeightColonistHistories.Keys) {
                        this.colonistHistories[p] = LightWeightSaver.ConvertFromLightWeight(lightWeightColonistHistories[p]);
                    }
                }
            } else {
                Scribe_Collections.Look(ref this.colonistHistories, "colonistHistories", LookMode.Reference, LookMode.Deep, ref this.tmpPawns, ref this.tmpColonistHistories);
            }
        }

        public void ResolveAvailableRecords() {
            this.cacheAvailableRecords = new HashSet<RecordIdentifier>();
            foreach (ColonistHistoryDataList dataList in this.colonistHistories.Values) {
                this.cacheAvailableRecords.AddRange(dataList.AvailableRecords);
            }
        }
    }
}
