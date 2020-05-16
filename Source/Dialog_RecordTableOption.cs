using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    public class Dialog_RecordTableOption : Window {
        public override Vector2 InitialSize {
            get {
                return new Vector2(500f, 175f);
            }
        }

        public Dialog_RecordTableOption() {
            this.optionalTitle = "ColonistHistory.Dialog_RecordTableOptionTitle".Translate();
            this.doCloseX = true;
        }

        public override void DoWindowContents(Rect inRect) {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = inRect.width;
            listing_Standard.Begin(inRect);
            listing_Standard.CheckboxLabeled("ColonistHistory.HideNullOrEmpty".Translate(),ref ColonistHistoryMod.Settings.hideNullOrEmpty);
        }
    }
}
