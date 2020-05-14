using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public struct RecordIdentifier {
        public ColonistHistoryDef colonistHistoryDef;
        public Def def;

        public string Label {
            get {
                if (def == null) {
                    return colonistHistoryDef.LabelCap;
                }
                return "ColonistHistoryWorker.ColonistHistoryRecordLabel".Translate(colonistHistoryDef.LabelCap, def.LabelCap);
            }
        }

        public bool IsNumeric {
            get {
                Type type = colonistHistoryDef.valueType;
                return type == typeof(int) || type == typeof(float);
            }
        }

        public bool IsVaild {
            get {
                return this.colonistHistoryDef != null;
            }
        }

        public static RecordIdentifier Invalid {
            get {
                return new RecordIdentifier(null, null);
            }
        }

        public RecordIdentifier(ColonistHistoryDef colonistHistoryDef, Def def) {
            this.colonistHistoryDef = colonistHistoryDef;
            this.def = def;
        }

        public bool IsSame(RecordIdentifier other) {
            return this.colonistHistoryDef == other.colonistHistoryDef && this.def == other.def;
        }
    }
}
