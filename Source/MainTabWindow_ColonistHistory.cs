using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ColonistHistory {
    class MainTabWindow_ColonistHistory : MainTabWindow {

		private List<TabRecord> tabs = new List<TabRecord>();

		private static MainTabWindow_ColonistHistory.TabType curTab = MainTabWindow_ColonistHistory.TabType.Home;

		public override Vector2 RequestedTabSize {
			get {
				return new Vector2(1010f, 640f);
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
			}, () => MainTabWindow_ColonistHistory.curTab == MainTabWindow_ColonistHistory.TabType.Table));
			this.tabs.Add(new TabRecord("ColonistHistory.TabGraph".Translate(), delegate () {
				MainTabWindow_ColonistHistory.curTab = MainTabWindow_ColonistHistory.TabType.Graph;
			}, () => MainTabWindow_ColonistHistory.curTab == MainTabWindow_ColonistHistory.TabType.Graph));
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

		}

		private void DoTablePage(Rect rect) {

		}

		private void DoGraphPage(Rect rect) {

		}

		private enum TabType : byte {
			Home,
			Table,
			Graph
		}
	}
}
