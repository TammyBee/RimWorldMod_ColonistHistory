using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class ColonistHistoryRecord : IExposable {
        public Def Def { get; set; }
        public string Label { get; set; }
        public object Value { get; set; }
        public List<object> Values { get; set; }
        public ColonistHistoryDef Parent { get; set; }

        private Type ValueType { get; set; }
        private Type DefType { get; set; }

        private bool isNull = false;

        public bool HasDef {
            get {
                return Def != null;
            }
        }

        public bool IsList {
            get {
                return Values != null;
            }
        }

        public bool IsNull {
            get {
                return !this.IsList && this.isNull;
            }
        }

        public bool IsNullOrEmpty {
            get {
                return this.IsList && this.Values.NullOrEmpty();
            }
        }

        public bool IsNumeric {
            get {
                return !IsList && (ValueType == typeof(int) || ValueType == typeof(float) || ValueType == typeof(double));
            }
        }

        public string ValueString {
            get {
                if (this.IsList) {
                    return this.Parent.Worker.GetValuesAsString(this);
                }
                return this.Parent.Worker.GetValueAsString(this);
            }
        }

        public float ValueForGraph {
            get {
                if (this.IsNumeric && !this.IsList) {
                    return this.Parent.Worker.GetValueForGraph(this.Value);
                }
                return 0f;
            }
        }

        public RecordIdentifier RecordID {
            get {
                return new RecordIdentifier(this.Parent, this.Def);
            }
        }

        public ColonistHistoryRecord() {

        }

        public ColonistHistoryRecord(Def def, string label, List<object> values, ColonistHistoryDef parent) {
            Def = def;
            Label = label;
            Value = null;
            Values = values;
            Parent = parent;
            ValueType = Parent.valueType;
            DefType = Parent.defType;
        }

        public ColonistHistoryRecord(string label, List<object> values, ColonistHistoryDef parent) {
            Def = null;
            Label = label;
            Value = null;
            Values = values;
            Parent = parent;
            ValueType = Parent.valueType;
            DefType = null;
        }

        public ColonistHistoryRecord(Def def, string label, object value, ColonistHistoryDef parent) {
            Def = def;
            Label = label;
            Value = value;
            Values = null;
            Parent = parent;
            ValueType = Parent.valueType;
            DefType = Parent.defType;
            if (Value == null) {
                Value = "ColonistHistory.NullValue".Translate();
                this.isNull = true;
            }
        }

        public ColonistHistoryRecord(string label, object value, ColonistHistoryDef parent) {
            Def = null;
            Label = label;
            Value = value;
            Values = null;
            Parent = parent;
            ValueType = Parent.valueType;
            DefType = null;
            if (Value == null) {
                Value = "ColonistHistory.NullValue".Translate();
                this.isNull = true;
            }
        }

        public override string ToString() {
            return string.Join(" - ", Def.ToStringSafe(), Label, Value.ToString());
        }

        public void ExposeData() {
            string label = this.Label;
            object value = this.Value;
            Def def = this.Def;
            List<object> values = this.Values;
            ColonistHistoryDef parent = this.Parent;
            Type valueType = this.ValueType;
            Type defType = this.DefType;

            Scribe_Values.Look<Type>(ref valueType, "valueType");
            Scribe_Values.Look<Type>(ref defType, "defType");
            Scribe_Values.Look<string>(ref label, "label");
            Scribe_Defs.Look<ColonistHistoryDef>(ref parent, "parent");
            Utils.ScribeObjectValue(ref value, "value", valueType);
            Utils.ScribeObjectsValue(ref values, "values", valueType);

            if (defType != null) {
                Utils.ScribeDefValue(ref def, "def", defType, true);
            }

            this.Label = label;
            this.Value = value;
            this.Values = values;
            this.Parent = parent;
            this.ValueType = valueType;
            this.Def = def;
            this.DefType = defType;
        }
    }
}
