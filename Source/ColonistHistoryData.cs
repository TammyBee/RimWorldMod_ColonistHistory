using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    public struct ColonistHistoryData : IExposable {
        public int recordTick;
        public string dateString;
        public ColonistHistoryDataRecords records;
        public bool forceRecord;

        public ColonistHistoryData(int currentTick, int tile, bool forceRecord, Pawn pawn) {
            this.recordTick = currentTick;
            this.forceRecord = forceRecord;
            this.records = new ColonistHistoryDataRecords(pawn);
            this.dateString = Utils.ConvertToDateTimeString(Find.TickManager.TicksAbs, tile);
        }

        public ColonistHistoryRecord GetRecord(RecordIdentifier recordID) {
            return this.records.records.Find(r => r.RecordID.Equals(recordID));
        }

        public void ExposeData() {
            Scribe_Values.Look(ref this.recordTick, "recordTick");
            Scribe_Values.Look(ref this.dateString, "dateString");
            Scribe_Values.Look(ref this.forceRecord, "forceRecord");
            Scribe_Deep.Look(ref records, "records");
        }
    }
}
