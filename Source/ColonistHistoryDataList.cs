using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class ColonistHistoryDataList : IExposable {
        public string pawnName;
        public List<ColonistHistoryData> log;

        public HashSet<RecordIdentifier> AvailableRecords {
            get {
                HashSet<RecordIdentifier> records = new HashSet<RecordIdentifier>();
                foreach (ColonistHistoryData data in log) {
                    records.AddRange(data.records.records.ConvertAll(r => r.RecordID));
                }
                return records;
            }
        }

        public ColonistHistoryDataList() {
            this.pawnName = "";
            this.log = new List<ColonistHistoryData>();
        }

        public ColonistHistoryDataList(string pawnName) {
            this.pawnName = pawnName;
            this.log = new List<ColonistHistoryData>();
        }

        public ColonistHistoryDataList(Pawn pawn) {
            this.pawnName = pawn.Name.ToStringShort;
            this.log = new List<ColonistHistoryData>();
        }

        public void Add(ColonistHistoryData data, Pawn pawn = null) {
            this.log.Add(data);
            if (pawn != null) {
                this.pawnName = pawn.Name.ToStringShort;
            }
        }

        public void ExposeData() {
            Scribe_Values.Look(ref this.pawnName, "pawnName");
            Scribe_Collections.Look(ref this.log, "log", LookMode.Deep);
        }

        public void DeleteLog(int recordTick) {
            this.log.RemoveAll(log => log.recordTick == recordTick);
        }

        public void DeleteLogRange(int beginTick,int endTick) {
            Predicate<ColonistHistoryData> match = delegate (ColonistHistoryData data) {
                if (beginTick == -1) {
                    return data.recordTick <= endTick;
                } else if (endTick == -1) {
                    return data.recordTick >= beginTick;
                }
                return data.recordTick >= beginTick && data.recordTick <= endTick;
            };
            this.log.RemoveAll(match);
        }
    }
}
