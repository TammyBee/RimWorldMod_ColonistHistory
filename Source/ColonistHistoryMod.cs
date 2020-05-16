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

        private int indexRecordingIntervalHours = 0;

        private static readonly List<int> RecordingIntervalHoursItems = new List<int> {
            1,2,3,4,6,8,12,24,48,72,96,120,180,240,360,720,1440
        };

        public ColonistHistorySettings settings;

        public ColonistHistoryMod(ModContentPack content) : base(content) {
            this.settings = GetSettings<ColonistHistorySettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect) {

            Text.Font = GameFont.Small;

            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = inRect.width;
            listing_Standard.Begin(inRect);

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

            listing_Standard.Gap();

            listing_Standard.Label("ColonistHistory.SettingsOutputRecords".Translate());

            listing_Standard.End();

            Rect rect = new Rect(inRect.x, inRect.y + listing_Standard.CurHeight, listing_Standard.ColumnWidth, inRect.height - listing_Standard.CurHeight);
            //Log.Message("rect:" + rect);
            GUI.BeginGroup(rect);
            float num = 0f;
            float heightRow = 28f;
            int indexColonistHistoryDef = 0;
            int indexReorderDown = -1;
            foreach (ColonistHistoryDef def in settings.ColonistHistorysOrder) {
                bool value = Settings.CanOutput(def);
                Rect rectRow = new Rect(0f, num, rect.width, heightRow);
                if (indexColonistHistoryDef > 0 && Widgets.ButtonImage(new Rect(0f, num + (heightRow - 24f) / 2f, 24f, 24f), MyTex.ReorderUp, Color.white, true)) {
                    indexReorderDown = indexColonistHistoryDef - 1;
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                }
                if (indexColonistHistoryDef < settings.ColonistHistorysOrder.Count - 1 && Widgets.ButtonImage(new Rect(28f, num + (heightRow - 24f) / 2f, 24f, 24f), MyTex.ReorderDown, Color.white, true)) {
                    indexReorderDown = indexColonistHistoryDef;
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                }

                Rect rectCheckbox = new Rect(56f, num, rectRow.width - 56f, heightRow);
                //Log.Message("rectCheckbox:" + rectCheckbox);
                Widgets.CheckboxLabeled(rectCheckbox,def.LabelCap, ref value);
                if (Mouse.IsOver(rectRow)) {
                    Widgets.DrawHighlight(rectRow);
                }
                TooltipHandler.TipRegion(rectRow, def.description);
                Settings.ColonistHistoryOutput[def.defName] = value;

                num += rectCheckbox.height;
                indexColonistHistoryDef++;
            }
            if (indexReorderDown != -1) {
                ColonistHistoryDef def = this.settings.ColonistHistorysOrder[indexReorderDown];
                this.settings.ColonistHistorysOrder.Remove(def);
                this.settings.ColonistHistorysOrder.Insert(indexReorderDown + 1, def);
            }
            GUI.EndGroup();

            Text.Font = GameFont.Medium;
        }

        public override string SettingsCategory() {
            return "Colonist History";
        }
    }
}
