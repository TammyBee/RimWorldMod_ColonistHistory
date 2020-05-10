using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class CHW_AgeBiologicalYears : ColonistHistoryWorker {
        public override IEnumerable<ColonistHistoryRecord> GetRecords(Pawn p) {
            if (p.ageTracker == null) {
                yield break;
            }
            yield return new ColonistHistoryRecord(def.LabelCap, p.ageTracker.AgeBiologicalYears, this.def);
        }
    }
}
