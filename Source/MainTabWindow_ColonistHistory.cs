using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ColonistHistory {
    public class MainTabWindow_ColonistHistory : MainTabWindow {

		private List<TabRecord> tabs = new List<TabRecord>();

		private static MainTabWindow_ColonistHistory.TabType curTab = MainTabWindow_ColonistHistory.TabType.Home;

		private static Pawn curPawn = null;

		private static int curDateIndex = 0;

		private GameComponent_ColonistHistoryRecorder CompRecorder {
			get {
				return Current.Game.GetComponent<GameComponent_ColonistHistoryRecorder>();
			}
		}

		private ColonistHistoryDataList CurRecords {
			get {
				return CompRecorder.GetRecords(MainTabWindow_ColonistHistory.curPawn);
			}
		}

		private ColonistHistoryData CurData {
			get {
				return CurRecords.log[MainTabWindow_ColonistHistory.curDateIndex];
			}
		}

		public override Vector2 RequestedTabSize {
			get {
				return new Vector2(1010f, 800f);
			}
		}

		public override void PreOpen() {
			base.PreOpen();
			this.tabs.Clear();
			this.tabs.Add(new TabRecord("ColonistHistory.TabHome".Translate(), delegate () {
				MainTabWindow_ColonistHistory.curTab = MainTabWindow_ColonistHistory.TabType.Home;
			}, () => MainTabWindow_ColonistHistory.curTab == MainTabWindow_ColonistHistory.TabType.Home));
			this.tabs.Add(new TabRecord("ColonistHistory.TabTable".Translate(), delegate () {
				MainTabWindow_ColonistHistory.curTab = MainTabWindow_ColonistHistory.TabType.Table;
				RefreshDrawEntries();
			}, () => MainTabWindow_ColonistHistory.curTab == MainTabWindow_ColonistHistory.TabType.Table));
			this.tabs.Add(new TabRecord("ColonistHistory.TabGraph".Translate(), delegate () {
				MainTabWindow_ColonistHistory.curTab = MainTabWindow_ColonistHistory.TabType.Graph;
				RefreshGraph();
			}, () => MainTabWindow_ColonistHistory.curTab == MainTabWindow_ColonistHistory.TabType.Graph));

			MainTabWindow_ColonistHistory.curPawn = CompRecorder.Colonists.First();
			RecordReportUtility.Reset();
			RefreshDrawEntries();
			RefreshGraph();

			Pawn pawn = MainTabWindow_ColonistHistory.curPawn;
			int lastIndex = CompRecorder.GetRecords(pawn).log.Count - 1;
			MainTabWindow_ColonistHistory.curDateIndex = Mathf.Clamp(MainTabWindow_ColonistHistory.curDateIndex, 0, lastIndex);
		}

		public override void DoWindowContents(Rect rect) {
			base.DoWindowContents(rect);
			Rect rect2 = rect;
			rect2.yMin += 45f;
			TabDrawer.DrawTabs(rect2, this.tabs, 200f);
			switch (MainTabWindow_ColonistHistory.curTab) {
			case MainTabWindow_ColonistHistory.TabType.Home:
				this.DoHomePage(rect2);
				return;
			case MainTabWindow_ColonistHistory.TabType.Table:
				this.DoTablePage(rect2);
				return;
			case MainTabWindow_ColonistHistory.TabType.Graph:
				this.DoGraphPage(rect2);
				return;
			default:
				return;
			}
		}

		private void DoHomePage(Rect rect) {
			rect.yMin += 17f;
			Text.Font = GameFont.Small;
			GUI.BeginGroup(rect);


			List<ListableOption> list = new List<ListableOption>();
			list.Add(new ListableOption("ColonistHistory.ForceRecordButton".Translate(), delegate{
				CompRecorder.Record(true);
			}, null));
			list.Add(new ListableOption("ColonistHistory.OutputRecordButton".Translate(), delegate {
				Find.WindowStack.Add(new Dialog_NameSaveFile(GameComponent_ColonistHistoryRecorder.SaveFileName));
			}, null));

			Rect rectButtonList = new Rect(0f, 0f, 170f, rect.height);
			OptionListingUtility.DrawOptionListing(rectButtonList, list);


			StringBuilder stringBuilder = new StringBuilder();
			{
				string lastRecordDateTimeString = "ColonistHistory.NoRecord".Translate();
				if (CompRecorder.LastRecordTick != -1) {
					lastRecordDateTimeString = CompRecorder.LastRecordDateTime;
				}
				string lastRecordIsManual = "";
				if (CompRecorder.LastRecordIsManual) {
					lastRecordIsManual = "ColonistHistory.LastRecordIsManual".Translate();
				}
				stringBuilder.AppendLine("ColonistHistory.LastRecordDateTime".Translate(lastRecordDateTimeString, lastRecordIsManual));
			}

			{
				int leftTick = CompRecorder.NextRecordTick - Current.Game.tickManager.TicksAbs;
				stringBuilder.AppendLine("ColonistHistory.NextRecordTimeLeft".Translate(leftTick.ToStringTicksToPeriod()));
			}

			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.Label(new Rect(rectButtonList.xMax + 16f , 0f, rect.width - rectButtonList.xMax - 16f, rect.height), stringBuilder.ToString());


			GUI.EndGroup();
		}

		private void DoTablePage(Rect rect) {
			rect.yMin += 16f;
			DrawHeaderOnTable(new Rect(rect.x, rect.y, rect.width, 32f));

			rect.yMin += 40f;
			DrawRecordInfoOnTable(rect);
		}

		private Vector2 DrawHeaderOnTable(Rect rect) {
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.BeginGroup(rect);
			float num = 0f;
			float height = rect.height;

			{
				Rect rectButton = new Rect(num, 0f, 140f, height);
				if (Widgets.ButtonText(rectButton, MainTabWindow_ColonistHistory.curPawn.Name.ToStringShort)) {
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					foreach (Pawn colonist in CompRecorder.Colonists) {
						Pawn p = colonist;
						list.Add(new FloatMenuOption(p.Name.ToStringShort, delegate () {
							int previousSelectedRecordsTick = this.CurData.recordTick;
							MainTabWindow_ColonistHistory.curPawn = p;
							int nextIndex = CurRecords.log.FindIndex(data => data.recordTick == previousSelectedRecordsTick);
							if (nextIndex == -1) {
								nextIndex = 0;
							}
							MainTabWindow_ColonistHistory.curDateIndex = nextIndex;
							RefreshDrawEntries();
						}, MenuOptionPriority.Default, null, null, 0f, null, null));
					}
					Find.WindowStack.Add(new FloatMenu(list));
				}
				num += rectButton.width;
			}
			num += 12f;

			Pawn pawn = MainTabWindow_ColonistHistory.curPawn;
			int lastIndex = CompRecorder.GetRecords(pawn).log.Count - 1;
			int currentIndex = curDateIndex;
			{
				Rect rectButton = new Rect(num, 0f, height, height);
				if (Widgets.ButtonText(rectButton, "<") && curDateIndex > 0) {
					curDateIndex--;
				}
				num += rectButton.width;
			}
			num += 4f;
			{
				string labelSlider = CompRecorder.GetRecords(pawn).log[curDateIndex].dateString;

				Rect rectSlider = new Rect(num, 0f, 640f, height);
				curDateIndex = Mathf.RoundToInt(Widgets.HorizontalSlider(rectSlider, curDateIndex, 0f, lastIndex, true, labelSlider));
				num += rectSlider.width;
			}
			num += 4f;
			{
				Rect rectButton = new Rect(num, 0f, height, height);
				if (Widgets.ButtonText(rectButton, ">") && curDateIndex < lastIndex) {
					curDateIndex++;
				}
				num += rectButton.width;
			}
			if (curDateIndex != currentIndex) {
				RefreshDrawEntries();
			}
			num += 12f;
			{
				Rect rectButton = new Rect(num, 0f, 40f, height);
				if (Widgets.ButtonText(rectButton, "...")) {
					Find.WindowStack.Add(new Dialog_RecordTableOption(this));
				}
				num += rectButton.width;
			}

			Text.Anchor = TextAnchor.UpperLeft;
			GUI.EndGroup();

			return new Vector2(num, height);
		}

		private void DrawRecordInfoOnTable(Rect rect) {
			RecordReportUtility.Draw(rect);
		}

		public void RefreshDrawEntries() {
			RecordReportUtility.ResolveDrawEntries(this.CurData);
		}

		private void DoGraphPage(Rect rect) {
			RecordGraphUtility.Draw(rect);
		}

		private void RefreshGraph() {
			RecordGraphUtility.Reset(CompRecorder);
		}

		private enum TabType : byte {
			Home,
			Table,
			Graph
		}
	}
}
