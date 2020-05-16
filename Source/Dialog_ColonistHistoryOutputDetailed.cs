using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    class Dialog_ColonistHistoryOutputDetailed : Window {
		private ColonistHistoryDef def;

		private Vector2 scrollPosition = new Vector2(0f, 0f);

		public override Vector2 InitialSize {
			get {
				return new Vector2(500f, 500f);
			}
		}

		public Dialog_ColonistHistoryOutputDetailed(ColonistHistoryDef def) {
			this.def = def;

			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.closeOnClickedOutside = true;
			this.closeOnAccept = false;
			this.optionalTitle = def.LabelCap;
			this.doCloseX = true;
		}

		public override void DoWindowContents(Rect inRect) {
			float lineHeight = 28f;
			Rect viewRect = new Rect(0f, 0f, inRect.width - 20f, lineHeight * this.def.RecordIDs.EnumerableCount());
			Widgets.BeginScrollView(inRect, ref this.scrollPosition, viewRect);
			float num = 0f;
			foreach (RecordIdentifier recordID in this.def.RecordIDs) {
				bool flag = ColonistHistoryMod.Settings.ColonistHistoryOutputDetailed[recordID.ID];
				Rect rect = new Rect(0f, num, viewRect.width, lineHeight);
				Widgets.CheckboxLabeled(rect, recordID.def.LabelCap, ref flag);
				if (Mouse.IsOver(rect)) {
					Widgets.DrawHighlight(rect);
				}
				TooltipHandler.TipRegion(rect, recordID.def.description);
				ColonistHistoryMod.Settings.ColonistHistoryOutputDetailed[recordID.ID] = flag;
				num += lineHeight;
			}
			Widgets.EndScrollView();
		}
	}
}
