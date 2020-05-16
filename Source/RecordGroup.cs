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

		private static Vector2 scrollPosition = Vector2.zero;

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
					ColonistHistoryRecord record = data.GetRecord(recordID,false);
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
						//simpleCurveDrawInfo.curve.Add(new CurvePoint(1.66666669E-05f, vectors[0].y), true);
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
			curveDrawerStyle.DrawLegend = false;
			SimpleCurveDrawer.DrawCurves(graphRect, this.curves, curveDrawerStyle, marks, legendRect);
			DrawCurvesLegend(legendRect,this.curves);
			Text.Anchor = TextAnchor.UpperLeft;
		}

		private void DrawCurvesLegend(Rect rect, List<SimpleCurveDrawInfo> curves) {
			float newWidth = rect.width - GUI.skin.verticalScrollbar.fixedWidth - 2f;
			int columnCount = (int)(newWidth / 140f);
			int rowCount = curves.Count / columnCount + 1;
			float newHeight = rowCount * 20f;
			Rect newRect = new Rect(rect.x, rect.y, newWidth, newHeight);
			Widgets.BeginScrollView(rect, ref scrollPosition, newRect);
			SimpleCurveDrawer.DrawCurvesLegend(newRect, curves);
			Widgets.EndScrollView();
		}
	}
}
