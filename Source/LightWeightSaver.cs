using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
    public static class LightWeightSaver {
        private static Dictionary<RecordIdentifier, object> tableValues = new Dictionary<RecordIdentifier, object>();
        private static Dictionary<RecordIdentifier, bool> tableIsUnrecorded = new Dictionary<RecordIdentifier, bool>();
        private static Dictionary<RecordIdentifier, ColonistHistoryRecord> tableRecords = new Dictionary<RecordIdentifier, ColonistHistoryRecord>();

        public static ColonistHistoryDataList GetLightWeight(ColonistHistoryDataList dataListSrc) {
            LightWeightSaver.tableValues.Clear();
            LightWeightSaver.tableIsUnrecorded.Clear();

            ColonistHistoryDataList dataList = new ColonistHistoryDataList(dataListSrc.pawnName);
            foreach (ColonistHistoryData dataSrc in dataListSrc.log) {
                ColonistHistoryData data = new ColonistHistoryData(dataSrc, false);
                foreach (ColonistHistoryRecord recordSrc in dataSrc.records.records) {
                    if (RegisterTable(recordSrc)) {
                        data.records.records.Add(recordSrc);
                    }
                }
                dataList.Add(data);
            }
            return dataList;
        }

        private static bool RegisterTable(ColonistHistoryRecord record) {
            if (!LightWeightSaver.tableIsUnrecorded.ContainsKey(record.RecordID)) {
                LightWeightSaver.tableIsUnrecorded[record.RecordID] = record.IsUnrecorded;
            } else if (LightWeightSaver.tableIsUnrecorded[record.RecordID] != record.IsUnrecorded) {
                LightWeightSaver.tableIsUnrecorded[record.RecordID] = record.IsUnrecorded;
                RegisterValue(record);
                return true;
            }

            if (!LightWeightSaver.tableValues.ContainsKey(record.RecordID)) {
                RegisterValue(record);
                return true;
            }
            bool isChanged = false;
            if (!record.IsList) {
                if (record.IsNull) {
                    isChanged = LightWeightSaver.tableValues[record.RecordID] != null;
                } else {
                    isChanged = !record.Value.Equals(LightWeightSaver.tableValues[record.RecordID]);
                }
            } else {
                if (record.IsNullOrEmpty) {
                    isChanged = LightWeightSaver.tableValues[record.RecordID] != null;
                } else {
                    isChanged = !record.Values.SequenceEqual((List<object>)LightWeightSaver.tableValues[record.RecordID]);
                }
            }
            if (isChanged) {
                RegisterValue(record);
            }
            return isChanged;
        }

        private static void RegisterValue(ColonistHistoryRecord record) {
            if (!record.IsList) {
                LightWeightSaver.tableValues[record.RecordID] = record.Value;
            } else {
                LightWeightSaver.tableValues[record.RecordID] = record.Values;
            }
        }

        public static ColonistHistoryDataList ConvertFromLightWeight(ColonistHistoryDataList lightWeightList) {
            ColonistHistoryDataList dataList = new ColonistHistoryDataList(lightWeightList.pawnName);

            LightWeightSaver.tableRecords.Clear();
            foreach (ColonistHistoryData dataSrc in lightWeightList.log) {
                ColonistHistoryData data = new ColonistHistoryData(dataSrc, false);
                foreach (ColonistHistoryRecord recordNew in dataSrc.records.records) {
                    if (LightWeightSaver.tableRecords.ContainsKey(recordNew.RecordID)) {
                        if (!LightWeightSaver.tableRecords[recordNew.RecordID].IsEqualValue(recordNew)) {
                            LightWeightSaver.tableRecords[recordNew.RecordID] = recordNew;
                        }
                    } else {
                        LightWeightSaver.tableRecords[recordNew.RecordID] = recordNew;
                    }
                }
                foreach (ColonistHistoryDef def in DefDatabase<ColonistHistoryDef>.AllDefsListForReading) {
                    foreach (RecordIdentifier recordID in def.RecordIDs) {
                        if (LightWeightSaver.tableRecords.ContainsKey(recordID)) {
                            data.records.records.Add(LightWeightSaver.tableRecords[recordID]);
                        }
                    }
                }
                dataList.Add(data);
            }
            /*
            dataList.log.Add(new ColonistHistoryData(lightWeightList.log[0], true));
            for (int i = 1; i < lightWeightList.log.Count; i++) {
                ColonistHistoryData current = lightWeightList.log[i];
                ColonistHistoryData previous = new ColonistHistoryData(dataList.log[i-1],true);

                ColonistHistoryData data = new ColonistHistoryData(current, false);
                data.records.records = new List<ColonistHistoryRecord>(previous.records.records);
                foreach (ColonistHistoryRecord recordNew in current.records.records) {
                    int index = data.records.records.FindIndex(r => r.RecordID.IsSame(recordNew.RecordID));
                    if (index != -1) {
                        data.records.records[index].SetValue(recordNew);
                    } else {
                        data.records.records.Add(recordNew);
                    }
                }
                dataList.log.Add(data);
            }
            */
            return dataList;
        }
    }
}
