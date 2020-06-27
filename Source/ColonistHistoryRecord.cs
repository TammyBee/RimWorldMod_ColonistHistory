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
        private bool isUnrecorded = false;

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

        public bool IsUnrecorded {
            get {
                return this.isUnrecorded;
            }
        }

        public ColonistHistoryRecord() {

        }

        public ColonistHistoryRecord(ColonistHistoryRecord src) {
            this.Def = src.Def;
            this.Label = src.Label;
            this.Parent = src.Parent;
            if (src.IsList) {
                this.Values = src.Values;
            } else {
                this.Value = src.Value;
            }
            this.ValueType = Parent.valueType;
            this.DefType = Parent.defType;
            this.isUnrecorded = src.IsUnrecorded;
            if (Value == null) {
                Value = "ColonistHistory.NullValue".Translate();
                this.isNull = true;
            }
            if (this.isUnrecorded) {
                Value = "ColonistHistory.UnrecordedValue".Translate();
            }
        }

        public ColonistHistoryRecord(RecordIdentifier recordID) {
            Def = recordID.def;
            Label = recordID.Label;
            Parent = recordID.colonistHistoryDef;
            Value = "ColonistHistory.UnrecordedValue".Translate();
            ValueType = Parent.valueType;
            DefType = Parent.defType;
            this.isUnrecorded = true;
            this.isNull = true;
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

        public bool IsEqualValue(ColonistHistoryRecord other) {
            if (!this.RecordID.IsSame(other.RecordID)) {
                return false;
            }
            if (!this.IsList) {
                if ((this.Value != null) != (other.Value != null)) {
                    return false;
                }else if ((this.Value == null) && (other.Value == null)) {
                    return true;
                }
                return this.Value.Equals(other.Value);
            } else {
                if (this.Values.NullOrEmpty() != other.Values.NullOrEmpty()) {
                    return false;
                }else if (this.Values.NullOrEmpty() && other.Values.NullOrEmpty()) {
                    return true;
                }
                return this.Values.SequenceEqual(other.Values);
            }
        }

        public T GetValue<T>(T defaultValue = default(T)) {
            if (this.IsUnrecorded) {
                return defaultValue;
            }
            return (T)this.Value;
        }

        public void SetValue(ColonistHistoryRecord other) {
            if (this.IsList) {
                this.Values = new List<object>(other.Values);
            } else {
                if (this.ValueType == typeof(int)) {
                    this.Value = (int)other.Value;
                    return;
                } else if (this.ValueType == typeof(float)) {
                    this.Value = (float)other.Value;
                    return;
                } else if (this.ValueType == typeof(string)) {
                    string s = other.Value as string;
                    if (s != null) {
                        this.Value = String.Copy(s);
                    } else {
                        this.Value = null;
                    }
                    return;
                }
                Log.Warning("ValueType is " + this.ValueType);
                this.Value = other.Value;
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
            Scribe_Values.Look<bool>(ref isNull, "isNull", false);
            Scribe_Values.Look<bool>(ref isUnrecorded, "isUnrecorded", false);

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
