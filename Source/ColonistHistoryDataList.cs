using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public class ColonistHistoryDataList : IExposable {
        public string pawnName;
        public List<ColonistHistoryData> log;

        public ColonistHistoryDataList() {

        }

        public ColonistHistoryDataList(Pawn pawn) {
            this.pawnName = pawn.Name.ToStringShort;
            this.log = new List<ColonistHistoryData>();
        }

        public void Add(ColonistHistoryData data, Pawn pawn = null) {
            this.log.Add(data);
            if (pawn != null) {
                this.pawnName = pawn.Name.ToStringShort;
            }
        }

        public void ExposeData() {
            Scribe_Values.Look(ref this.pawnName, "pawnName");
            Scribe_Collections.Look(ref this.log, "log", LookMode.Deep);
        }
    }
}
