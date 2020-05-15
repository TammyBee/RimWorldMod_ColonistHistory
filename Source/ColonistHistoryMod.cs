using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

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
            float num = inRect.y;

            Text.Font = GameFont.Small;

            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = inRect.width;
            listing_Standard.Begin(inRect);

            indexRecordingIntervalHours = RecordingIntervalHoursItems.IndexOf(Settings.recordingIntervalHours);
            if (indexRecordingIntervalHours == -1) {
                indexRecordingIntervalHours = 0;
            }
            listing_Standard.Label("ColonistHistoryWorker.SettingsRecordingIntervalHours".Translate(RecordingIntervalHoursItems[indexRecordingIntervalHours].HoursToString()));
            indexRecordingIntervalHours = (int)listing_Standard.Slider(indexRecordingIntervalHours, 0, RecordingIntervalHoursItems.Count - 1);
            Settings.recordingIntervalHours = RecordingIntervalHoursItems[indexRecordingIntervalHours];

            listing_Standard.CheckboxLabeled("ColonistHistoryWorker.SettingsSaveNullOrEmpty".Translate(), ref settings.saveNullOrEmpty);

            listing_Standard.Label("ColonistHistoryWorker.SettingsSaveFolderPath".Translate());
            Settings.saveFolderPath = listing_Standard.TextEntry(Settings.saveFolderPath);

            listing_Standard.Gap();

            listing_Standard.Label("ColonistHistoryWorker.SettingsOutputRecords".Translate());
            foreach(ColonistHistoryDef def in DefDatabase<ColonistHistoryDef>.AllDefsListForReading) {
                bool value = Settings.CanOutput(def);
                listing_Standard.CheckboxLabeled(def.LabelCap, ref value, def.description);
                Settings.ColonistHistoryOutput[def.defName] = value;
            }

            listing_Standard.End();

            Text.Font = GameFont.Medium;
        }

        public override string SettingsCategory() {
            return "Colonist History";
        }
    }
}
