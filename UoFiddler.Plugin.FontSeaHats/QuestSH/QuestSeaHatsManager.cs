// /***************************************************************************
//  *
//  * $Author: Turley
//  * 
//  * "THE BEER-WARE LICENSE"
//  * As long as you retain this notice you can do whatever you want with 
//  * this stuff. If we meet some day, and you think this stuff is worth it,
//  * you can buy me a beer in return.
//  *
//  ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace UoFiddler.Plugin.FontSeaHats.QuestSH
{
    public class StepListEventArgs
    {
        public bool Success { get; set; } = false;
        public QuestDataStep StepData { get; set; } = null;
    }

    public class QuestListEventArgs
    {
        public bool Success { get; set; } = false;
        public QuestDataInfo QuestData { get; set; } = null;
    }

    public class QuestSeaHatsManager
    {
        public event EventHandler<QuestListEventArgs> QuestListChangeRequest;
        public event EventHandler<StepListEventArgs> StepListChangeRequest;
        public Dictionary<ushort, QuestDataInfo> Quests { get; set; } = new Dictionary<ushort, QuestDataInfo>();
        public string FileQuests { get; set; } = "Quests.JSon";
        public int MaxStep { get; private set; } = 5;

        public QuestSeaHatsManager()
        {
            if (!File.Exists(FileQuests))
            {
                SaveJsonQuest(FileQuests);
                MessageBox.Show($"File is created: {FileQuests}");
            }
            else
            {
                LoadQuestJson(FileQuests);
            }
        }

        public void ChangeQuestPriority(string title, QuestPriority_T prio)
        {
            var result = Quests.Where(t => t.Value.QuestName == title).FirstOrDefault();
            result.Value.Priority = prio;

            OnQuestListChangeRequest(new QuestListEventArgs());
        }

        public void ChangeMaxStep(int value)
        {
            MaxStep = value;
        }

        public QuestDataInfo GetQuest(string title)
        {
            var result = Quests.Where(t => t.Value.QuestName == title).FirstOrDefault();

            return result.Value;
        }

        public bool DeleteQuest(ushort questId)
        {
            if (Quests.ContainsKey(questId))
            {
                if (Quests.Remove(questId))
                {
                    OnQuestListChangeRequest(new QuestListEventArgs());
                    return true;
                }
            }

            return false;
        }

        private string _lastPath { get; set; } = string.Empty;

        public bool SaveJsonQuest(string file)
        {

            JsonSerializer serializer = new JsonSerializer();
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.NullValueHandling = NullValueHandling.Ignore;

            try
            {
                using (StreamWriter sw = new StreamWriter(file))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    FileQuests = file;
                    writer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, Quests);
                    // {"ExpiryDate":new Date(1230375600000),"Price":0}
                }

                return true;
            }
            catch
            { 
            }

            return false;
        }

        public bool LoadQuestJson(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    var toDeserialize = File.ReadAllText(file);
                    Dictionary<ushort, QuestDataInfo> deseriazed = JsonConvert.DeserializeObject<Dictionary<ushort, QuestDataInfo>>(toDeserialize);

                    //Allineamento quest
                    foreach (var quest in deseriazed.Values)
                    {
                        quest.SetStepParent(quest);
                    }

                    Quests = deseriazed.OrderBy(t => t.Key).ToDictionary<ushort, QuestDataInfo>();

                    OnQuestListChangeRequest(new QuestListEventArgs());

                    return true;
                }
            }
            catch
            {

            }

            return false;
        }
        public void OnQuestListChangeRequest(QuestListEventArgs data)
        {
            if (QuestListChangeRequest is not null)
            {
                QuestListChangeRequest.Invoke(this, data);
            }
        }

        public void OnStepListChangeRequest(StepListEventArgs data)
        {
            if (StepListChangeRequest is not null)
            {
                StepListChangeRequest.Invoke(this, data);
            }
        }

        public bool DeleteStep(ushort questId, int stepId)
        {
            if (Quests.TryGetValue(questId, out var questData) && questData.Steps.ContainsKey(stepId))
            {
                if (questData.Steps.Remove(stepId))
                {
                    OnStepListChangeRequest(new StepListEventArgs());
                    return true;
                }
            }

            return false;
        }

        public ushort GetFreeId()
        {
            ushort result = 0;
            try
            {
                result = (ushort)(Quests.Keys.Max() + 1);
            }
            catch(Exception ex)
            {
                result = 1;
            }

            return result;
        }

        //public void ChangeStepName(ushort questId, int stepId, string title)
        //{
        //    if (_questDatas.TryGetValue(questId, out var questData) && questData.Steps.TryGetValue(stepId, out var step))
        //    {
        //        step.StepName = title;
        //        ChangeDataRequest.Invoke(this, new AddResult() { QuestData = questData });
        //    }
        //}

        public void UpdateStep(ushort questId, QuestDataStep step)
        {
            if (Quests.TryGetValue(questId, out var questData))
            {
                if (step is not null)
                {
                    foreach (var valueDict in FixedSteps)
                    {
                        if (step.Step == valueDict.Key)
                        {
                            step.StepName = valueDict.Value;
                        }
                    }

                    questData.Steps[step.Step] = step;
                    OnStepListChangeRequest(new StepListEventArgs() { StepData = step});
                }
            }
        }


        public bool CloneQuest(string questTitle)
        {
            return CloneQuest(GetQuest(questTitle).ID);
        }

        public bool CloneQuest(ushort questId)
        {
            ushort newId = GetFreeId();

            if (Quests.TryGetValue(questId, out var questData))
            {
                QuestDataInfo questDataInfo = new QuestDataInfo(questData)
                {
                    ID = newId,
                    QuestName = $"CLONED_{questData.QuestName}"
                };

                questDataInfo.SetStepParent(questDataInfo);

                if (Quests.TryAdd(newId, questDataInfo))
                {
                    OnQuestListChangeRequest(new QuestListEventArgs() { QuestData = questDataInfo });
                    return true;
                }
            }

            return false;

        }
        public void ChangeTitleName(ushort questId, string title)
        {
            if (Quests.TryGetValue(questId, out var questData))
            {
                questData.QuestName = title;

                OnQuestListChangeRequest(new QuestListEventArgs() { QuestData = questData });
            }
        }

        public bool AddStep(ushort questIndex)
        {
            if(Quests.TryGetValue(questIndex, out var questData))
            {
                int nextStep = questData.GetFreeStep();
                if (nextStep > MaxStep)
                {
                    MessageBox.Show("can't add the step, the maxium step quest is reached");
                    return false;
                }

                if(questData.AddStep(new QuestDataStep(questData)))
                {
                    questData.Steps = questData.Steps.OrderBy(entry => entry.Key).ToDictionary(entry => entry.Key, entry => entry.Value);
                    OnStepListChangeRequest(new StepListEventArgs());
                    return true;
                }
                else
                {
                    MessageBox.Show("can't add the step");
                }
            }

            return false;
        }
 
        public Dictionary<int, string> FixedSteps { get; private set; } = new Dictionary<int, string>()
        {
            { 0, "Init"},
            {Int32.MaxValue, "Finish" }
        };


        public QuestListEventArgs AddQuest()
        {
            ushort newId = GetFreeId();

            QuestDataInfo questDataInfo = new QuestDataInfo()
            {
                ID = newId
            };

            int stepId = 0;
            questDataInfo.AddStep(new QuestDataStep(questDataInfo) { Step = stepId, StepName = FixedSteps[stepId] });

            stepId = Int32.MaxValue;
            questDataInfo.AddStep(new QuestDataStep(questDataInfo) { Step = stepId, StepName = FixedSteps[stepId] });

            if (Quests.TryAdd(newId, questDataInfo))
            {
                return new QuestListEventArgs()
                {
                    Success = true,
                    QuestData = questDataInfo
                };
            }

            return null;
        }
    }
}
