using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    class Dialog_NameSaveFile : Window {
		private string curName;

		public override Vector2 InitialSize {
			get {
				return new Vector2(500f, 175f);
			}
		}

		public Dialog_NameSaveFile(string defaultFileName) {
			this.curName = defaultFileName;

			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.closeOnClickedOutside = true;
			this.closeOnAccept = false;
		}

		public override void DoWindowContents(Rect inRect) {
			bool flag = false;
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
				flag = true;
				Event.current.Use();
			}
			Text.Font = GameFont.Small;
			Widgets.Label(new Rect(15f, 15f, 500f, 50f), "ColonistHistory.Dialog_NameSaveFileDesc".Translate());
			Rect rectTextField = new Rect(15f, 50f, inRect.width - 60f, 35f);
			this.curName = Widgets.TextField(rectTextField, this.curName);
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(new Rect(rectTextField.xMax + 4f, rectTextField.y, 30f, rectTextField.height), ".xml");
			Text.Anchor = TextAnchor.UpperLeft;
			if (Widgets.ButtonText(new Rect(inRect.width / 2f + 20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), "OK", true, true, true) || flag) {
				GameComponent_ColonistHistoryRecorder.Instance.Save(this.curName + ".xml");
				Find.WindowStack.TryRemove(this, true);
			}
		}
	}
}
