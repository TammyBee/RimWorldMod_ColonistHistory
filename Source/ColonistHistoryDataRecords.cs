using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class ColonistHistoryDataRecords : IExposable {
        public List<ColonistHistoryRecord> records;

        public ColonistHistoryDataRecords() {
            this.records = new List<ColonistHistoryRecord>();
        }

        public ColonistHistoryDataRecords(ColonistHistoryDataRecords records) {
            this.records = new List<ColonistHistoryRecord>(records.records);
        }

        public ColonistHistoryDataRecords(Pawn pawn) {
            this.records = new List<ColonistHistoryRecord>();
            foreach (ColonistHistoryDef colonistHistoryDef in ColonistHistoryMod.Settings.OutputColonistHistories) {
                foreach(ColonistHistoryRecord record in colonistHistoryDef.Worker.GetRecords(pawn)){
                    if (ColonistHistoryMod.Settings.CanOutput(record.RecordID)) {
                        this.records.Add(record);
                    }
                }
            }
        }

        public void ExposeData() {
            Scribe_Collections.Look(ref records, "records", LookMode.Deep);
        }
    }
}
