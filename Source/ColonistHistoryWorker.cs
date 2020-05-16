using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public abstract class ColonistHistoryWorker {
        public ColonistHistoryDef def;

        public abstract IEnumerable<ColonistHistoryRecord> GetRecords(Pawn p);

        protected string GenerateColonistHistoryRecordLabel(string label) {
            return "ColonistHistory.ColonistHistoryRecordLabel".Translate(def.LabelCap,label);
        }

        public virtual string GetValuesAsString(ColonistHistoryRecord record) {
            return string.Join(", ",record.Values.Select(value => this.GetValuesAsStringInternal(value)));
        }

        protected virtual string GetValuesAsStringInternal(object value) {
            return value.ToString();
        }

        public virtual string GetValueAsString(ColonistHistoryRecord record) {
            return record.Value.ToStringSafe();
        }

        public virtual float GetValueForGraph(object value) {
            return System.Convert.ToSingle(value);
        }

        public virtual IEnumerable<RecordIdentifier> GetRecordIDs() {
            yield return new RecordIdentifier(def, null);
        }
    }
}
