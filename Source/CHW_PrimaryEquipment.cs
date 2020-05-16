using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class CHW_PrimaryEquipment : ColonistHistoryWorker {
        public override IEnumerable<ColonistHistoryRecord> GetRecords(Pawn p) {
            if (p.equipment?.Primary == null) {
                if (ColonistHistoryMod.Settings.saveNullOrEmpty) {
                    object obj = null;
                    yield return new ColonistHistoryRecord(def.LabelCap, obj, this.def);
                }
                yield break;
            }
            yield return new ColonistHistoryRecord(def.LabelCap, p.equipment.Primary.Label, this.def);
        }
    }
}
