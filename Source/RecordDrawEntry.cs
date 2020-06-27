using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    public class RecordDrawEntry {
        public ColonistHistoryRecord data;

        public RecordDrawEntry(ColonistHistoryRecord data) {
            this.data = data;
        }

		public string LabelCap {
			get {
				if (this.data.HasDef) {
					return this.data.Def.LabelCap;
				}
				return this.data.Label.CapitalizeFirst();
			}
		}

		public string ValueString {
			get {
				return this.data.ValueString;
			}
		}

		public bool CanRender {
			get {
				if (ColonistHistoryMod.Settings.hideNullOrEmpty) {
					return !this.data.IsNull && !this.data.IsNullOrEmpty;
				}
				if (ColonistHistoryMod.Settings.hideUnrecorded) {
					return !this.data.IsUnrecorded || !this.data.IsNull || !this.data.IsNullOrEmpty;
				}
				return true;
			}
		}

		public float Draw(float x, float y, float width, bool selected, Action clickedCallback, Action mousedOverCallback, Vector2 scrollPosition, Rect scrollOutRect) {
			float num = width * 0.45f;
			Rect rect = new Rect(8f, y, width, Text.CalcHeight(this.ValueString, num));
			if (y - scrollPosition.y + rect.height >= 0f && y - scrollPosition.y <= scrollOutRect.height) {
				if (selected) {
					Widgets.DrawHighlightSelected(rect);
				} else if (Mouse.IsOver(rect)) {
					Widgets.DrawHighlight(rect);
				}
				Rect rect2 = rect;
				rect2.width -= num;
				Widgets.Label(rect2, this.LabelCap);
				Rect rect3 = rect;
				rect3.x = rect2.xMax;
				rect3.width = num;
				Widgets.Label(rect3, this.ValueString);
				if (this.data.Parent != null && Mouse.IsOver(rect)) {
					TooltipHandler.TipRegion(rect, new TipSignal(this.data.RecordID.Description, this.data.GetHashCode()));
				}
				if (Widgets.ButtonInvisible(rect, true)) {
					clickedCallback();
				}
				if (Mouse.IsOver(rect)) {
					mousedOverCallback();
				}
			}
			return rect.height;
		}
	}
}
