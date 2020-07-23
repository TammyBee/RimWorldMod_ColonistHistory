using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ColonistHistory {
    public static class RecordGraphUtility {
		private static GameComponent_ColonistHistoryRecorder comp;

		private static FloatRange graphSection;

		private static RecordIdentifier curRecordID = RecordIdentifier.Invalid;

		private static RecordGroup curRecordGroup;

		private static List<CurveMark> marks = new List<CurveMark>();

		public static RecordIdentifier CurRecordID {
			get {
				if (!RecordGraphUtility.curRecordID.IsVaild && !RecordGraphUtility.comp.NumericRecords.EnumerableNullOrEmpty()) {
					RecordGraphUtility.curRecordID = RecordGraphUtility.comp.NumericRecords.First();
				}
				return RecordGraphUtility.curRecordID;
			}
			set {
				RecordGraphUtility.curRecordID = value;
				RecordGraphUtility.CurRecordGroup = new RecordGroup(RecordGraphUtility.comp, RecordGraphUtility.CurRecordID);
			}
		}

		public static RecordGroup CurRecordGroup {
			get {
				if (RecordGraphUtility.comp == null) {
					RecordGraphUtility.Reset(Current.Game.GetComponent<GameComponent_ColonistHistoryRecorder>());
				}
				if (RecordGraphUtility.curRecordGroup == null) {
					RecordGraphUtility.CurRecordGroup = new RecordGroup(RecordGraphUtility.comp, RecordGraphUtility.CurRecordID);
				}
				return RecordGraphUtility.curRecordGroup;
			}

			set {
				RecordGraphUtility.curRecordGroup = value;
			}
		}

		public static void Reset(GameComponent_ColonistHistoryRecorder comp) {
			RecordGraphUtility.comp = comp;
			if (!RecordGraphUtility.CurRecordID.IsVaild) {
				RecordGraphUtility.CurRecordID = RecordGraphUtility.comp.NumericRecords.First();
			}

			RecordGraphUtility.graphSection = new FloatRange(0f, (float)Find.TickManager.TicksGame / 60000f);
			RecordGraphUtility.CurRecordGroup.ResolveGraph();
		}

		public static void Draw(Rect rect) {
			rect.yMin += 17f;
			GUI.BeginGroup(rect);
			Rect graphRect = new Rect(0f, 0f, rect.width, rect.height - 100f);
			Rect legendRect = new Rect(468f, graphRect.yMax, rect.width - 468f, 80f);
			Rect rect2 = new Rect(0f, legendRect.yMin + 44f, rect.width, 40f);
			if (RecordGraphUtility.CurRecordGroup != null) {
				RecordGraphUtility.marks.Clear();
				List<Tale> allTalesListForReading = Find.TaleManager.AllTalesListForReading;
				for (int i = 0; i < allTalesListForReading.Count; i++) {
					Tale tale = allTalesListForReading[i];
					if (tale.def.type == TaleType.PermanentHistorical) {
						float x = (float)GenDate.TickAbsToGame(tale.date) / 60000f;
						RecordGraphUtility.marks.Add(new CurveMark(x, tale.ShortSummary, tale.def.historyGraphColor));
					}
				}
				RecordGraphUtility.CurRecordGroup.DrawGraph(graphRect, legendRect, RecordGraphUtility.graphSection, RecordGraphUtility.marks);
			}
			Text.Font = GameFont.Small;
			float num = (float)Find.TickManager.TicksGame / 60000f;
			if (Widgets.ButtonText(new Rect(0f, legendRect.yMin, 110f, 40f), "Last30Days".Translate(), true, true, true)) {
				RecordGraphUtility.graphSection = new FloatRange(Mathf.Max(0f, num - 30f), num);
				SoundDefOf.Click.PlayOneShotOnCamera(null);
			}
			if (Widgets.ButtonText(new Rect(114f, legendRect.yMin, 110f, 40f), "Last100Days".Translate(), true, true, true)) {
				RecordGraphUtility.graphSection = new FloatRange(Mathf.Max(0f, num - 100f), num);
				SoundDefOf.Click.PlayOneShotOnCamera(null);
			}
			if (Widgets.ButtonText(new Rect(228f, legendRect.yMin, 110f, 40f), "Last300Days".Translate(), true, true, true)) {
				RecordGraphUtility.graphSection = new FloatRange(Mathf.Max(0f, num - 300f), num);
				SoundDefOf.Click.PlayOneShotOnCamera(null);
			}
			if (Widgets.ButtonText(new Rect(342f, legendRect.yMin, 110f, 40f), "AllDays".Translate(), true, true, true)) {
				RecordGraphUtility.graphSection = new FloatRange(0f, num);
				SoundDefOf.Click.PlayOneShotOnCamera(null);
			}
			if (Widgets.ButtonText(new Rect(rect2.x, rect2.y, 110f, 40f), "SelectGraph".Translate(), true, true, true)) {
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				foreach (RecordIdentifier recordID in RecordGraphUtility.comp.NumericRecords) {
					RecordIdentifier localRecordID = recordID;
					list.Add(new FloatMenuOption(localRecordID.Label, delegate () {
						RecordGraphUtility.CurRecordID = localRecordID;
					}, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				FloatMenu window = new FloatMenu(list, "SelectGraph".Translate(), false);
				Find.WindowStack.Add(window);
			}
			if (RecordGraphUtility.CurRecordID.IsVaild) {
				Text.Anchor = TextAnchor.MiddleLeft;
				Widgets.Label(new Rect(rect2.x + 118f, rect2.y, 300f, 40f), RecordGraphUtility.CurRecordID.Label);
				Text.Anchor = TextAnchor.UpperLeft;
			}
			GUI.EndGroup();
		}
    }
}
