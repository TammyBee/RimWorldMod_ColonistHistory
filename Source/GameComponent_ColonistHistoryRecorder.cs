using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class GameComponent_ColonistHistoryRecorder : GameComponent {
        private int lastRecordTick = -1;
        private int firstTile = -1; 
        private Dictionary<Pawn, ColonistHistoryDataList> colonistHistories = new Dictionary<Pawn, ColonistHistoryDataList>();

        private List<Pawn> tmpPawns = new List<Pawn>();
        private List<ColonistHistoryDataList> tmpColonistHistories = new List<ColonistHistoryDataList>();


        private int NextRecordTick {
            get {
                if (this.lastRecordTick != -1) {
                    return this.lastRecordTick + ColonistHistoryMod.Settings.recordingIntervalHours * GenDate.TicksPerHour;
                }
                return -1;
            }
        }

        public static string SaveFilePath {
            get {
                string fileName = "";
                if (Faction.OfPlayer.HasName) {
                    fileName = Faction.OfPlayer.Name;
                } else {
                    fileName = SaveGameFilesUtility.UnusedDefaultFileName(Faction.OfPlayer.def.LabelCap);
                }
                fileName += "_" + Current.Game.World.info.seedString;
                fileName += ".xml";
                return string.Join("/", ColonistHistoryMod.Settings.saveFolderPath, fileName);
            }
        }

        public GameComponent_ColonistHistoryRecorder(Game game) {
            this.lastRecordTick = -1;
            this.colonistHistories = new Dictionary<Pawn, ColonistHistoryDataList>();
        }

        public override void GameComponentTick() {
            if (Current.Game.tickManager.TicksAbs >= NextRecordTick) {
                Record();
            }
        }

        public bool Record(bool forceRecord = false) {
            int currentTick = Current.Game.tickManager.TicksAbs;
            List<Pawn> colonists = Find.ColonistBar.GetColonistsInOrder();
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
                ColonistHistoryData colonistHistoryData = new ColonistHistoryData(currentTick, this.firstTile, forceRecord, colonist);
                this.colonistHistories[colonist].Add(colonistHistoryData, colonist);
            }
            if (!forceRecord) {
                this.lastRecordTick = currentTick;
            }
            return true;
        }

        public void Save() {
            try {
                SafeSaver.Save(SaveFilePath, "root", delegate {
                    int xmlFormatVersion = ColonistHistoryMod.XmlFormatVersion;
                    Scribe_Values.Look(ref xmlFormatVersion, "xmlFormatVersion");
                    List<ColonistHistoryDataList> list = this.colonistHistories.Values.ToList();
                    Scribe_Collections.Look(ref list, "colonistHistories", LookMode.Deep);
                });
                Messages.Message("ColonistHistoryWorker.SaveColonistHistoriesFileAs".Translate(SaveFilePath), MessageTypeDefOf.NeutralEvent,false);
            } catch (Exception ex) {
                Log.Error("Exception while saving world: " + ex.ToString());
            }
        }

        public override void ExposeData() {
            Scribe_Values.Look(ref this.lastRecordTick, "lastRecordTick", -1);
            Scribe_Values.Look(ref this.firstTile, "firstTile", -1);
            Scribe_Collections.Look(ref this.colonistHistories, "colonistHistories", LookMode.Reference ,LookMode.Deep, ref this.tmpPawns, ref this.tmpColonistHistories);
        }
    }
}
