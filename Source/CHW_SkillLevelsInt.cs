using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class CHW_SkillLevelsInt : ColonistHistoryWorker {
        public override IEnumerable<ColonistHistoryRecord> GetRecords(Pawn p) {
            if (p.skills?.skills == null) {
                yield break;
            }
            foreach (SkillRecord skill in p.skills.skills.OrderByDescending(s => s.def.listOrder)) {
                string label = GenerateColonistHistoryRecordLabel(skill.def.LabelCap);
                yield return new ColonistHistoryRecord(skill.def, label, skill.Level, this.def);
            }
        }

        public override IEnumerable<RecordIdentifier> GetRecordIDs() {
            foreach (SkillDef skillDef in DefDatabase<SkillDef>.AllDefsListForReading.OrderByDescending(s => s.listOrder)) {
                yield return new RecordIdentifier(def, skillDef);
            }
        }
    }
}
