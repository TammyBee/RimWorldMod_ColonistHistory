using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public static class DebugActionsColonistHistory {
		[DebugAction("ColonistHistory", "record colonist histories.", allowedGameStates = AllowedGameStates.Playing)]
		private static void RecordColonistHistories() {
			GameComponent_ColonistHistoryRecorder comp = Current.Game.GetComponent<GameComponent_ColonistHistoryRecorder>();
			if (comp != null) {
				comp.Record(true);
			}
		}

		[DebugAction("ColonistHistory", "save colonist histories.", allowedGameStates = AllowedGameStates.Playing)]
		private static void SaveColonistHistories() {
			GameComponent_ColonistHistoryRecorder comp = Current.Game.GetComponent<GameComponent_ColonistHistoryRecorder>();
			if (comp != null) {
				comp.Save();
			}
		}
	}
}
