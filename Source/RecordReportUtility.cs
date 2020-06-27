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
    public static class RecordReportUtility {
		private static RecordDrawEntry selectedEntry;

		private static RecordDrawEntry mousedOverEntry;

		private static Vector2 scrollPosition;

		private static float listHeight;

		private static List<RecordDrawEntry> cachedDrawEntries = new List<RecordDrawEntry>();

		private static GameComponent_ColonistHistoryRecorder CompRecorder {
			get {
				return Current.Game.GetComponent<GameComponent_ColonistHistoryRecorder>();
			}
		}

		public static void Reset() {
			RecordReportUtility.scrollPosition = default(Vector2);
			RecordReportUtility.selectedEntry = null;
			RecordReportUtility.mousedOverEntry = null;
			RecordReportUtility.cachedDrawEntries.Clear();
		}

		public static void ResolveDrawEntries(ColonistHistoryData data) {
			RecordReportUtility.cachedDrawEntries.Clear();
			if (ColonistHistoryMod.Settings.hideUnrecorded) {
				foreach (ColonistHistoryDef def in ColonistHistoryMod.Settings.ColonistHistorysOrder) {
					foreach (ColonistHistoryRecord record in data.records.records) {
						if (def == record.Parent) {
							RecordReportUtility.cachedDrawEntries.Add(new RecordDrawEntry(record));
						}
					}
				}
			} else {
				CompRecorder.ResolveAvailableRecords();
				foreach (ColonistHistoryDef def in ColonistHistoryMod.Settings.ColonistHistorysOrder) {
					foreach (RecordIdentifier recordID in CompRecorder.AvailableRecords) {
						if (recordID.colonistHistoryDef == def) {
							ColonistHistoryRecord record = data.GetRecord(recordID, true);
							RecordReportUtility.cachedDrawEntries.Add(new RecordDrawEntry(record));
						}
					}
				}
			}
		}

		public static void Draw(Rect rect) {
			Rect rect2 = new Rect(rect);
			Text.Font = GameFont.Small;
			Rect viewRect = new Rect(0f, 0f, rect2.width - 16f, RecordReportUtility.listHeight);
			Widgets.BeginScrollView(rect2, ref RecordReportUtility.scrollPosition, viewRect, true);
			float num = 0f;
			string b = null;
			RecordReportUtility.mousedOverEntry = null;
			for (int i = 0; i < RecordReportUtility.cachedDrawEntries.Count; i++) {
				RecordDrawEntry ent = RecordReportUtility.cachedDrawEntries[i];
				if (ent.CanRender) {
					if (ent.data.Parent.LabelCap != b) {
						Widgets.ListSeparator(ref num, viewRect.width, ent.data.Parent.LabelCap);
						b = ent.data.Parent.LabelCap;
					}
					num += ent.Draw(8f, num, viewRect.width - 8f, RecordReportUtility.selectedEntry == ent, delegate {
						RecordReportUtility.SelectEntry(ent, true);
					}, delegate {
						RecordReportUtility.mousedOverEntry = ent;
					}, RecordReportUtility.scrollPosition, rect2);
				}
			}
			RecordReportUtility.listHeight = num + 100f;
			Widgets.EndScrollView();
		}

		private static void SelectEntry(RecordDrawEntry rec, bool playSound = true) {
			RecordReportUtility.selectedEntry = rec;
			if (playSound) {
				SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
			}
		}

	}
}
