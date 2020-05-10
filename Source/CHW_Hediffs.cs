using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class CHW_Hediffs : ColonistHistoryWorker {
        public override IEnumerable<ColonistHistoryRecord> GetRecords(Pawn p) {
            if (p.health?.hediffSet == null) {
                yield break;
            }

            List<object> values = new List<object>();

            MethodInfo VisibleHediffGroupsInOrder = typeof(HealthCardUtility).GetMethod("VisibleHediffGroupsInOrder", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var grouping in (IEnumerable<IGrouping<BodyPartRecord, Hediff>>)VisibleHediffGroupsInOrder.Invoke(null, new object[] {p, true})) {
                foreach (Hediff hediff in grouping) {
                    string label = hediff.LabelCap;
                    if (grouping.Key != null) {
                        label = "ColonistHistoryWorker.CHW_Hediffs_Label".Translate(hediff.LabelCap, grouping.Key.LabelCap);
                    }
                    values.Add(label);
                }
            }
            yield return new ColonistHistoryRecord(this.def.LabelCap, values, this.def);
        }
    }
}
