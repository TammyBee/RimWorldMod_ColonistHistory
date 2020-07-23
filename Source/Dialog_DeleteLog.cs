using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonistHistory {
	class Dialog_DeleteLog : Window {
		private enum TargetPawn {
			Single,
			All
		}
		private enum TargetDatetime {
			Only,
			Before,
			After
		}

		private MainTabWindow_ColonistHistory parent;

		private TargetPawn targetPawn;
		private TargetDatetime targetDatetime;

		public override Vector2 InitialSize {
			get {
				return new Vector2(500f, 400f);
			}
		}

		public Dialog_DeleteLog(MainTabWindow_ColonistHistory parent) {
			this.parent = parent;

			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.closeOnClickedOutside = true;
			this.doCloseX = true;
			this.closeOnAccept = false;
		}

		public override void DoWindowContents(Rect inRect) {
			bool flag = false;
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
				flag = true;
				Event.current.Use();
			}
			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(15f, 15f, 500f, 30f), "ColonistHistory.Dialog_DeleteLogDesc".Translate());
			Widgets.Label(new Rect(15f, 60f, 500f, 30f), "ColonistHistory.Dialog_DeleteLogTargetPawn".Translate());
			if (Widgets.RadioButtonLabeled(new Rect(45f, 90f, 300f, 30f), this.parent.CurPawn.Name.ToStringShort, this.targetPawn == TargetPawn.Single)) {
				this.targetPawn = TargetPawn.Single;
			}
			if (Widgets.RadioButtonLabeled(new Rect(45f, 120f, 300f, 30f), "ColonistHistory.Dialog_DeleteLogTargetPawnAll".Translate(), this.targetPawn == TargetPawn.All)) {
				this.targetPawn = TargetPawn.All;
			}
			Widgets.Label(new Rect(15f, 165f, 500f, 30f), "ColonistHistory.Dialog_DeleteLogTargetDatetime".Translate());
			if (Widgets.RadioButtonLabeled(new Rect(45f, 195f, 300f, 30f), "ColonistHistory.Dialog_DeleteLogTargetDatetimeOnly".Translate(), this.targetDatetime == TargetDatetime.Only)) {
				this.targetDatetime = TargetDatetime.Only;
			}
			if (Widgets.RadioButtonLabeled(new Rect(45f, 225f, 300f, 30f), "ColonistHistory.Dialog_DeleteLogTargetDatetimeBefore".Translate(), this.targetDatetime == TargetDatetime.Before)) {
				this.targetDatetime = TargetDatetime.Before;
			}
			if (Widgets.RadioButtonLabeled(new Rect(45f, 255f, 300f, 30f), "ColonistHistory.Dialog_DeleteLogTargetDatetimeAfter".Translate(), this.targetDatetime == TargetDatetime.After)) {
				this.targetDatetime = TargetDatetime.After;
			}
			if (Widgets.ButtonText(new Rect(inRect.width / 2f + 20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), "OK", true, true, true) || flag) {
				Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ColonistHistory.ConfirmDeleteLog".Translate(), delegate {
					DeleteLog(this.targetPawn, this.targetDatetime);
				}, true, null));
				Find.WindowStack.TryRemove(this, true);
			}
		}

		private void DeleteLog(TargetPawn targetPawn, TargetDatetime targetDatetime) {
			if (targetPawn == TargetPawn.Single) {
				DeleteLog(this.parent.CurRecords, targetDatetime, this.parent.CurData.recordTick);
			} else {
				foreach (ColonistHistoryDataList records in this.parent.CompRecorder.ColonistHistories.Values) {
					DeleteLog(records, targetDatetime, this.parent.CurData.recordTick);
				}
			}
			this.parent.RefreshDrawEntries();
			RecordGraphUtility.Reset(this.parent.CompRecorder);
			RecordGraphUtility.CurRecordGroup.ResolveGraph();
		}

		private void DeleteLog(ColonistHistoryDataList dataList, TargetDatetime targetDatetime, int targetTick) {
			if (targetDatetime == TargetDatetime.Only) {
				dataList.DeleteLog(targetTick);
			} else if (targetDatetime == TargetDatetime.Before) {
				dataList.DeleteLogRange(-1, targetTick);
			} else if(targetDatetime == TargetDatetime.After) {
				dataList.DeleteLogRange(targetTick, -1);
			}
		}
	}
}
