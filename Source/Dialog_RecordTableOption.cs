using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    public class Dialog_RecordTableOption : Window {
        private MainTabWindow_ColonistHistory parent;

        public override Vector2 InitialSize {
            get {
                return new Vector2(500f, 175f);
            }
        }

        public Dialog_RecordTableOption(MainTabWindow_ColonistHistory parent) {
            this.optionalTitle = "ColonistHistory.Dialog_RecordTableOptionTitle".Translate();
            this.doCloseX = true;
            this.parent = parent;
        }

        public override void DoWindowContents(Rect inRect) {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = inRect.width;
            listing_Standard.Begin(inRect);

            bool previousFlag = ColonistHistoryMod.Settings.hideNullOrEmpty;
            listing_Standard.CheckboxLabeled("ColonistHistory.HideNullOrEmpty".Translate(),ref ColonistHistoryMod.Settings.hideNullOrEmpty);
            if (previousFlag != ColonistHistoryMod.Settings.hideNullOrEmpty) {
                this.parent.RefreshDrawEntries();
            }

            previousFlag = ColonistHistoryMod.Settings.hideUnrecorded;
            listing_Standard.CheckboxLabeled("ColonistHistory.HideUnrecorded".Translate(), ref ColonistHistoryMod.Settings.hideUnrecorded);
            if (previousFlag != ColonistHistoryMod.Settings.hideUnrecorded) {
                this.parent.RefreshDrawEntries();
            }
            listing_Standard.End();
        }
    }
}
