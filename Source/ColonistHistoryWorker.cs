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
            return "ColonistHistoryWorker.ColonistHistoryRecordLabel".Translate(def.LabelCap,label);
        }

        public virtual string GetValueAsString(ColonistHistoryRecord record) {
            return record.Value.ToStringSafe();
        }
    }
}
