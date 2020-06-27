using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class CHW_RecordsTime : ColonistHistoryWorker {
        public override IEnumerable<ColonistHistoryRecord> GetRecords(Pawn p) {
            if (p.records == null) {
                yield break;
            }
            foreach (RecordDef recordDef in DefDatabase<RecordDef>.AllDefsListForReading) {
                if (recordDef.type == RecordType.Time) {
                    string label = GenerateColonistHistoryRecordLabel(recordDef.LabelCap);
                    int value = p.records.GetAsInt(recordDef);
                    yield return new ColonistHistoryRecord(recordDef, label, value, this.def);
                }
            }
        }

        public override string GetValueAsString(ColonistHistoryRecord record) {
            if (record.Value is int) {
                return ((int)record.Value).ToStringTicksToPeriod(true, false, true, true);
            }
            return base.GetValueAsString(record);
        }

        public override float GetValueForGraph(object value) {
            return base.GetValueForGraph(value) / 2500f;
        }

        public override IEnumerable<RecordIdentifier> GetRecordIDs() {
            foreach (RecordDef recordDef in DefDatabase<RecordDef>.AllDefsListForReading) {
                if (recordDef.type == RecordType.Time) {
                    yield return new RecordIdentifier(def, recordDef);
                }
            }
        }
    }
}
