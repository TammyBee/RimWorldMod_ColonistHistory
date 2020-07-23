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

        private static List<int> hidePawnIndexes = new List<int>();

        private static bool forceRedraw = false;

        public RecordGroup(GameComponent_ColonistHistoryRecorder comp, RecordIdentifier recordID) {
            this.comp = comp;
            this.recordID = recordID;
            ResolveGraph();
        }

        public void ResolveGraph() {
            this.cachedGraph = new Dictionary<Pawn, List<Vector2>>();
            if (this.comp != null) {
                foreach (Pawn pawn in this.comp.Colonists) {
                    ColonistHistoryDataList dataList = this.comp.GetRecords(pawn);
                    if (!dataList.log.NullOrEmpty()) {
                        this.cachedGraph[pawn] = new List<Vector2>();
                        if (dataList != null) {
                            foreach (ColonistHistoryData data in dataList.log) {
                                ColonistHistoryRecord record = data.GetRecord(recordID, false);
                                if ((record != null && !record.IsUnrecorded) || ColonistHistoryMod.Settings.treatingUnrecordedAsZero) {
                                    float x = GenDate.TickAbsToGame(data.recordTick);
                                    float y = 0f;
                                    if (record != null) {
                                        y = record.ValueForGraph;
                                    }
                                    if (this.cachedGraph[pawn].Count == 0 && ColonistHistoryMod.Settings.addZeroBeforeFirst) {
                                        this.cachedGraph[pawn].Add(new Vector2(x - 0.001f, 0));
                                    }
                                    this.cachedGraph[pawn].Add(new Vector2(x, y));
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawGraph(Rect graphRect, Rect legendRect, FloatRange section, List<CurveMark> marks) {
            int ticksGame = Find.TickManager.TicksGame;
            if (ticksGame != this.cachedGraphTickCount || RecordGroup.forceRedraw) {
                this.cachedGraphTickCount = ticksGame;
                this.curves.Clear();
                int i = 0;
                Func<Pawn, bool> pred = delegate (Pawn p) {
                    return (ColonistHistoryMod.Settings.showOtherFactionPawn || !p.ExistExtraNoPlayerFactions());
                };
                int numOfColonist = this.comp.Colonists.Where(pred).Count();
                foreach (Pawn pawn in this.comp.Colonists.Where(pred)) {
                    List<Vector2> vectors = this.cachedGraph[pawn];

                    SimpleCurveDrawInfo simpleCurveDrawInfo = new SimpleCurveDrawInfo();
                    simpleCurveDrawInfo.color = Color.HSVToRGB((float)i / numOfColonist, 1f, 1f);
                    simpleCurveDrawInfo.label = pawn.Name.ToStringShort;
                    simpleCurveDrawInfo.valueFormat = recordID.colonistHistoryDef.GraphLabelY;
                    simpleCurveDrawInfo.curve = new SimpleCurve();
                    for (int j = 0; j < vectors.Count; j++) {
                        float x = vectors[j].x / 60000f;
                        float y = vectors[j].y;
                        simpleCurveDrawInfo.curve.Add(new CurvePoint(x, y), false);
                    }
                    simpleCurveDrawInfo.curve.SortPoints();
                    /*
					if (ticksGame % 100 == 0) {
						Log.Message(pawn.Label + "\n" + string.Join("\n",simpleCurveDrawInfo.curve.Points.ConvertAll(p => p.ToString())));
					}
					*/
                    this.curves.Add(simpleCurveDrawInfo);
                    i++;
                }

                RecordGroup.forceRedraw = false;
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
            List<SimpleCurveDrawInfo> renderableCurves = new List<SimpleCurveDrawInfo>();
            for (int i = 0; i < this.curves.Count; i++) {
                if (!hidePawnIndexes.Contains(i)) {
                    renderableCurves.Add(this.curves[i]);
                }
            }
            if (SimpleCurveDrawer_DrawCurveLines_Patch.highLightCurve != null) {
                renderableCurves.SortBy(c => SimpleCurveDrawer_DrawCurveLines_Patch.IsHighlightedCurve(c));
            }
            SimpleCurveDrawer.DrawCurves(graphRect, renderableCurves, curveDrawerStyle, marks, legendRect);
            DrawCurvesLegend(legendRect, this.curves);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static List<SimpleCurveDrawInfo> GetFocusedCurves(List<SimpleCurveDrawInfo> curves, Rect screenRect, Rect viewRect) {
            if (curves.Count == 0) {
                return null;
            }
            if (!Mouse.IsOver(screenRect)) {
                return null;
            }
            GUI.BeginGroup(screenRect);
            Vector2 mousePosition = Event.current.mousePosition;
            Vector2 vector = default(Vector2);
            Vector2 vector2 = default(Vector2);
            bool flag = false;
            List<SimpleCurveDrawInfo> simpleCurveDrawInfos = new List<SimpleCurveDrawInfo>();
            foreach (SimpleCurveDrawInfo simpleCurveDrawInfo2 in curves) {
                if (simpleCurveDrawInfo2.curve.PointsCount != 0) {
                    Vector2 vector3 = SimpleCurveDrawer.ScreenToCurveCoords(screenRect, viewRect, mousePosition);
                    vector3.y = simpleCurveDrawInfo2.curve.Evaluate(vector3.x);
                    Vector2 vector4 = SimpleCurveDrawer.CurveToScreenCoordsInsideScreenRect(screenRect, viewRect, vector3);
                    if (!flag || Mathf.Approximately(Vector2.Distance(vector4, mousePosition), Vector2.Distance(vector2, mousePosition))) {
                        flag = true;
                        vector = vector3;
                        vector2 = vector4;
                        simpleCurveDrawInfos.Add(simpleCurveDrawInfo2);
                    } else if (!flag || Vector2.Distance(vector4, mousePosition) < Vector2.Distance(vector2, mousePosition)) {
                        flag = true;
                        vector = vector3;
                        vector2 = vector4;
                        simpleCurveDrawInfos.Clear();
                        simpleCurveDrawInfos.Add(simpleCurveDrawInfo2);
                    }
                }
            }
            GUI.EndGroup();
            return simpleCurveDrawInfos;
        }

        private void DrawCurvesLegend(Rect rect, List<SimpleCurveDrawInfo> curves) {
            List<SimpleCurveDrawInfo> focusedCurves = GetFocusedCurves(curves, SimpleCurveDrawer_DrawCurveMousePoint_Patch.screenRect, SimpleCurveDrawer_DrawCurveMousePoint_Patch.viewRect);
            float newWidth = rect.width - GUI.skin.verticalScrollbar.fixedWidth - 2f;
            int columnCount = (int)(newWidth / 140f);
            int rowCount = curves.Count / columnCount + 1;
            float newHeight = rowCount * 20f;
            Rect newRect = new Rect(rect.x, rect.y, newWidth, newHeight);
            Widgets.BeginScrollView(rect, ref scrollPosition, newRect);
            RecordGroup.DrawCurvesLegendInternal(newRect, curves, focusedCurves);
            Widgets.EndScrollView();
        }

        private static void DrawCurvesLegendInternal(Rect rect, List<SimpleCurveDrawInfo> curves, List<SimpleCurveDrawInfo> focusedCurves) {
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
            Text.WordWrap = false;
            GUI.BeginGroup(rect);
            float num = 0f;
            float num2 = 0f;
            int num3 = (int)(rect.width / 140f);
            int num4 = 0;
            int i = 0;

            SimpleCurveDrawer_DrawCurveLines_Patch.highLightCurve = null;
            foreach (SimpleCurveDrawInfo simpleCurveDrawInfo in curves) {
                bool isHidden = hidePawnIndexes.Contains(i);
                GUI.color = simpleCurveDrawInfo.color;
                if (isHidden) {
                    GUI.color *= Color.grey;
                }
                Rect rectColor = new Rect(num, num2 + 2f, 15f, 15f);
                GUI.DrawTexture(rectColor, BaseContent.WhiteTex);
                GUI.color = Color.white;
                num += 20f;
                if (simpleCurveDrawInfo.label != null) {
                    Rect rectLabel = new Rect(num, num2, 140f, 20f);
                    if (isHidden) {
                        GUI.color = Color.grey;
                    }
                    Widgets.Label(rectLabel, simpleCurveDrawInfo.label);
                    if (isHidden) {
                        GUI.color = Color.white;
                    }
                    if (!isHidden && !focusedCurves.NullOrEmpty() && focusedCurves.Exists(c => simpleCurveDrawInfo.label == c.label && simpleCurveDrawInfo.color == c.color)) {
                        Widgets.DrawHighlight(rectLabel);
                    }
                    if (Mouse.IsOver(rectLabel) || Mouse.IsOver(rectColor)) {
                        SimpleCurveDrawer_DrawCurveLines_Patch.highLightCurve = simpleCurveDrawInfo;
                    }

                    if (Widgets.ButtonInvisible(rectLabel) || Widgets.ButtonInvisible(rectColor)) {
                        if (isHidden) {
                            hidePawnIndexes.Remove(i);
                        } else {
                            hidePawnIndexes.Add(i);
                        }
                    }
                }
                num4++;
                if (num4 == num3) {
                    num4 = 0;
                    num = 0f;
                    num2 += 20f;
                } else {
                    num += 140f;
                }
                i++;
            }
            GUI.EndGroup();
            GUI.color = Color.white;
            Text.WordWrap = true;
        }

        public static void ForceRedraw() {
            RecordGroup.forceRedraw = true;
        }
    }
}
