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

        public string ID {
            get {
                string id = this.colonistHistoryDef.defName;
                if (this.def != null) {
                    id += "/" + this.def.defName;
                }
                return id;
            }
        }

        public string Label {
            get {
                if (def == null) {
                    return colonistHistoryDef.LabelCap;
                }
                return "ColonistHistory.ColonistHistoryRecordLabel".Translate(colonistHistoryDef.LabelCap, def.LabelCap);
            }
        }

        public string Description {
            get {
                string description = this.colonistHistoryDef.LabelCap + ": " + this.colonistHistoryDef.description;
                if (this.def != null) {
                    description += "\n\n" + this.def.LabelCap + ": " + this.def.description;
                }
                return description;
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

        public override string ToString() {
            return "(" + colonistHistoryDef.ToStringSafe() + "/" + def.ToStringSafe() + ")";
        }
    }
}
