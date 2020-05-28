using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ColonistHistory {
    public class ColonistHistoryMod : Mod {
        public static ColonistHistorySettings Settings {
            get {
                return LoadedModManager.GetMod<ColonistHistoryMod>().settings;
            }
        }

        public static int XmlFormatVersion {
            get {
                return 1;
            }
        }

        private Vector2 scrollPosition = Vector2.zero;

        private int indexRecordingIntervalHours = 0;

        private static readonly List<int> RecordingIntervalHoursItems = new List<int> {
            1,2,3,4,6,8,12,24,48,72,96,120,180,240,360,720,1440
        };

        public ColonistHistorySettings settings;

        private string highlightedCurveWidthBuf;

        public ColonistHistoryMod(ModContentPack content) : base(content) {
            this.settings = GetSettings<ColonistHistorySettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect) {

            Text.Font = GameFont.Small;

            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = inRect.width;
            listing_Standard.Begin(inRect);

            listing_Standard.Label("ColonistHistory.SettingsTitleGeneral".Translate());

            indexRecordingIntervalHours = RecordingIntervalHoursItems.IndexOf(Settings.recordingIntervalHours);
            if (indexRecordingIntervalHours == -1) {
                indexRecordingIntervalHours = 0;
            }
            listing_Standard.Label("ColonistHistory.SettingsRecordingIntervalHours".Translate(RecordingIntervalHoursItems[indexRecordingIntervalHours].HoursToString()));
            indexRecordingIntervalHours = (int)listing_Standard.Slider(indexRecordingIntervalHours, 0, RecordingIntervalHoursItems.Count - 1);
            Settings.recordingIntervalHours = RecordingIntervalHoursItems[indexRecordingIntervalHours];

            listing_Standard.CheckboxLabeled("ColonistHistory.SettingsSaveNullOrEmpty".Translate(), ref settings.saveNullOrEmpty);

            listing_Standard.Label("ColonistHistory.SettingsSaveFolderPath".Translate());
            Settings.saveFolderPath = listing_Standard.TextEntry(Settings.saveFolderPath);

            listing_Standard.CheckboxLabeled("ColonistHistory.SettingsRecordOtherFactionPawn".Translate(), ref settings.recordOtherFactionPawn);

            bool previousShowOtherFactionPawn = settings.showOtherFactionPawn;
            listing_Standard.CheckboxLabeled("ColonistHistory.SettingsShowOtherFactionPawn".Translate(), ref settings.showOtherFactionPawn);
            if (previousShowOtherFactionPawn != settings.showOtherFactionPawn) {
                RecordGroup.ForceRedraw();
            }

            listing_Standard.CheckboxLabeled("ColonistHistory.SettingsLightWeightSaveMode".Translate(), ref settings.lightWeightSaveMode, "ColonistHistory.SettingsLightWeightSaveModeDesc".Translate());

            listing_Standard.GapLine();

            listing_Standard.Label("ColonistHistory.SettingsTitleGraph".Translate());

            listing_Standard.TextFieldNumericLabeled<float>("ColonistHistory.SettingsHighlightedCurveWidth".Translate(), ref settings.highlightedCurveWidth, ref this.highlightedCurveWidthBuf);
            bool previousTreatingUnrecordedAsZero = settings.treatingUnrecordedAsZero;
            listing_Standard.CheckboxLabeled("ColonistHistory.SettingsTreatingUnrecordedAsZero".Translate(), ref settings.treatingUnrecordedAsZero);
            if (previousTreatingUnrecordedAsZero != settings.treatingUnrecordedAsZero) {
                RecordGroup.ForceRedraw();
            }
            bool previousAddZeroBeforeFirst = settings.addZeroBeforeFirst;
            listing_Standard.CheckboxLabeled("ColonistHistory.SettingsAddZeroBeforeFirst".Translate(), ref settings.addZeroBeforeFirst);
            if (previousAddZeroBeforeFirst != settings.addZeroBeforeFirst) {
                RecordGroup.ForceRedraw();
            }

            listing_Standard.GapLine();

            listing_Standard.Label("ColonistHistory.SettingsTitleOutputRecords".Translate());

            listing_Standard.End();

            float rowHeight = 28f;
            Rect outRect = new Rect(inRect.x, inRect.y + listing_Standard.CurHeight, listing_Standard.ColumnWidth, inRect.height - listing_Standard.CurHeight);
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, rowHeight * settings.ColonistHistorysOrder.Count);
            //Log.Message("rect:" + rect);
            Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect);
            float num = 0f;
            int indexColonistHistoryDef = 0;
            int indexReorderDown = -1;
            foreach (ColonistHistoryDef def in settings.ColonistHistorysOrder) {
                bool value = Settings.CanOutput(def);
                Rect rectRow = new Rect(0f, num, viewRect.width - 20f, rowHeight);
                if (indexColonistHistoryDef > 0 && Widgets.ButtonImage(new Rect(0f, num + (rowHeight - 24f) / 2f, 24f, 24f), MyTex.ReorderUp, Color.white, true)) {
                    indexReorderDown = indexColonistHistoryDef - 1;
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                }
                if (indexColonistHistoryDef < settings.ColonistHistorysOrder.Count - 1 && Widgets.ButtonImage(new Rect(28f, num + (rowHeight - 24f) / 2f, 24f, 24f), MyTex.ReorderDown, Color.white, true)) {
                    indexReorderDown = indexColonistHistoryDef;
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                }

                Rect rectCheckbox = new Rect(56f, num, rectRow.width - 90f, rowHeight);
                //Log.Message("rectCheckbox:" + rectCheckbox);
                Widgets.CheckboxLabeled(rectCheckbox, def.LabelCap, ref value);
                if (Mouse.IsOver(rectRow)) {
                    Widgets.DrawHighlight(rectRow);
                }
                TooltipHandler.TipRegion(rectRow, def.description);
                Settings.ColonistHistoryOutput[def.defName] = value;

                if (def.RecordIDs.Count() >= 2 && Widgets.ButtonText(new Rect(rectCheckbox.xMax + 4f, num, 30f, rowHeight),"...")) {
                    Find.WindowStack.Add(new Dialog_ColonistHistoryOutputDetailed(def));
                }

                num += rectCheckbox.height;
                indexColonistHistoryDef++;
            }
            if (indexReorderDown != -1) {
                ColonistHistoryDef def = this.settings.ColonistHistorysOrder[indexReorderDown];
                this.settings.ColonistHistorysOrder.Remove(def);
                this.settings.ColonistHistorysOrder.Insert(indexReorderDown + 1, def);
            }
            Widgets.EndScrollView();

            Text.Font = GameFont.Medium;
        }

        public override string SettingsCategory() {
            return "Colonist History";
        }
    }
}
