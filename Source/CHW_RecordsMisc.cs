using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class CHW_RecordsMisc : ColonistHistoryWorker {
        public override IEnumerable<ColonistHistoryRecord> GetRecords(Pawn p) {
            if (p.records == null) {
                yield break;
            }
            foreach (RecordDef recordDef in DefDatabase<RecordDef>.AllDefsListForReading) {
                if (recordDef.type == RecordType.Int || recordDef.type == RecordType.Float) {
                    string label = GenerateColonistHistoryRecordLabel(recordDef.LabelCap);
                    float value = p.records.GetValue(recordDef);
                    yield return new ColonistHistoryRecord(recordDef, label, value, this.def);
                }
            }
        }

        public override string GetValueAsString(ColonistHistoryRecord record) {
            if (record.Value is float) {
                return ((float)record.Value).ToString("0.##");
            }
            return base.GetValueAsString(record);
        }

        public override IEnumerable<RecordIdentifier> GetRecordIDs() {
            foreach (RecordDef recordDef in DefDatabase<RecordDef>.AllDefsListForReading) {
                if (recordDef.type == RecordType.Int || recordDef.type == RecordType.Float) {
                    yield return new RecordIdentifier(def, recordDef);
                }
            }
        }
    }
}
