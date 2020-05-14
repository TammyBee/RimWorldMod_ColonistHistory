using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ColonistHistory {
    public class RecordGroup {
        private GameComponent_ColonistHistoryRecorder comp;
        private RecordIdentifier recordID;

		private List<SimpleCurveDrawInfo> curves = new List<SimpleCurveDrawInfo>();

		private int cachedGraphTickCount = -1;

		private Dictionary<Pawn, List<Vector2>> cachedGraph;

		public RecordGroup(GameComponent_ColonistHistoryRecorder comp, RecordIdentifier recordID) {
            this.comp = comp;
            this.recordID = recordID;
			ResolveGraph();
		}

		public void ResolveGraph() {
			this.cachedGraph = new Dictionary<Pawn, List<Vector2>>();
			foreach (Pawn pawn in this.comp.Colonists) {
				this.cachedGraph[pawn] = new List<Vector2>();
				ColonistHistoryDataList dataList = this.comp.GetRecords(pawn);
				foreach (ColonistHistoryData data in dataList.log) {
					ColonistHistoryRecord record = data.GetRecord(recordID);
					Log.Message(record.Parent.ToStringSafe() + "/" + record.Value + "/" + record.Parent.valueType);
					float x = GenDate.TickAbsToGame(data.recordTick);
					float y = record.ValueForGraph;
					this.cachedGraph[pawn].Add(new Vector2(x,y));
				}
			}
		}

		public void DrawGraph(Rect graphRect, Rect legendRect, FloatRange section, List<CurveMark> marks) {
			int ticksGame = Find.TickManager.TicksGame;
			if (ticksGame != this.cachedGraphTickCount) {
				this.cachedGraphTickCount = ticksGame;
				this.curves.Clear();
				int i = 0;
				int numOfColonist = this.comp.Colonists.Count();
				foreach (Pawn pawn in this.comp.Colonists) {
					List<Vector2> vectors = this.cachedGraph[pawn];

					SimpleCurveDrawInfo simpleCurveDrawInfo = new SimpleCurveDrawInfo();
					simpleCurveDrawInfo.color = Color.HSVToRGB((float)i / numOfColonist, 1f, 1f);
					simpleCurveDrawInfo.label = pawn.Name.ToStringShort;
					simpleCurveDrawInfo.labelY = recordID.colonistHistoryDef.GraphLabelY;
					simpleCurveDrawInfo.curve = new SimpleCurve();
					for (int j = 0; j < vectors.Count; j++) {
						float x = vectors[j].x / 60000f;
						float y = vectors[j].y;
						simpleCurveDrawInfo.curve.Add(new CurvePoint(x, y), false);
					}
					simpleCurveDrawInfo.curve.SortPoints();
					if (vectors.Count == 1) {
						simpleCurveDrawInfo.curve.Add(new CurvePoint(1.66666669E-05f, vectors[0].y), true);
					}
					if (ticksGame % 100 == 0) {
						Log.Message(pawn.Label + "\n" + string.Join("\n",simpleCurveDrawInfo.curve.Points.ConvertAll(p => p.ToString())));
					}
					this.curves.Add(simpleCurveDrawInfo);
					i++;
				}
			}
			if (Mathf.Approximately(section.min, section.max)) {
				section.max += 1.66666669E-05f;
			}
			SimpleCurveDrawerStyle curveDrawerStyle = Find.History.curveDrawerStyle;
			curveDrawerStyle.FixedSection = section;
			curveDrawerStyle.UseFixedScale = this.recordID.colonistHistoryDef.useFixedScale;
			curveDrawerStyle.FixedScale = this.recordID.colonistHistoryDef.fixedScale;
			curveDrawerStyle.YIntegersOnly = this.recordID.colonistHistoryDef.integersOnly;
			curveDrawerStyle.OnlyPositiveValues = this.recordID.colonistHistoryDef.onlyPositiveValues;
			SimpleCurveDrawer.DrawCurves(graphRect, this.curves, curveDrawerStyle, marks, legendRect);
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
