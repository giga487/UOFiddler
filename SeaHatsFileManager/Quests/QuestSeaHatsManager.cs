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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SeaHatsExternal.Crypto;
using SeaHatsExternal.Quests;

namespace SeaHatsExternal.Quests
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

    public class QuestData
    {
        public Dictionary<ushort, QuestDataInfo> Quests { get; set; } = new Dictionary<ushort, QuestDataInfo>();
        public string SecretKey { get; set; } = string.Empty;
        public QuestData()
        {

        }
        public QuestData(QuestData dataToCopy)
        {
            SecretKey = dataToCopy.SecretKey;
            Quests = new Dictionary<ushort, QuestDataInfo>(dataToCopy.Quests);
        }
    }

    public enum QuestParameters
    {
        From,    //Who call
        ExtName, //With name
        MobType, //With type
        Amount,  //With amount  
    }

    public class QuestSeaHatsManager
    {

        public event EventHandler<QuestListEventArgs> QuestListChangeRequest;
        public event EventHandler<StepListEventArgs> StepListChangeRequest;
        public string FileQuests { get; set; } = "Quests.Json";
        public int MaxStep { get; private set; } = 5;
        public QuestData Data { get; set; } = new QuestData();
        public QuestSeaHatsManager()
        {
            if (!File.Exists(FileQuests))
            {
                SaveJsonQuest(FileQuests);
            }
            else
            {
                LoadQuestJson(FileQuests);
            }
        }

        public void ChangeQuestPriority(string title, QuestPriority_T prio)
        {
            var result = Data.Quests.Where(t => t.Value.QuestName == title).FirstOrDefault();
            result.Value.Priority = prio;

            OnQuestListChangeRequest(new QuestListEventArgs());
        }

        public void ChangeMaxStep(int value)
        {
            MaxStep = value;
        }

        public QuestDataInfo GetQuest(string title)
        {
            var result = Data.Quests.Where(t => t.Value.QuestName == title).FirstOrDefault();

            return result.Value;
        }

        public bool DeleteQuest(ushort questId)
        {
            if (Data.Quests.ContainsKey(questId))
            {
                if (Data.Quests.Remove(questId))
                {
                    OnQuestListChangeRequest(new QuestListEventArgs());
                    return true;
                }
            }

            return false;
        }

        private string _lastPath { get; set; } = string.Empty;

        public string ExtractDataJson()
        {
            JsonSerializerSettings settings = new();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;

            return JsonConvert.SerializeObject(Data, settings);
        }


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
                    serializer.Serialize(writer, Data);
                    // {"ExpiryDate":new Date(1230375600000),"Price":0}
                }

                return true;
            }
            catch
            { 
            }

            return false;
        }

        public QuestData DeserializeQuest(string toDeserialize)
        {
            QuestData deseriazed = JsonConvert.DeserializeObject<QuestData>(toDeserialize);

            try
            {
                foreach (var quest in deseriazed.Quests)
                {
                    quest.Value.SetStepParent(quest.Value);
                }

                Data = deseriazed;
            }
            catch
            {
                return null;
            }            //Allineamento quest

            return deseriazed;
        }
        public bool LoadQuestJsonCrypted(string file, string secret)
        {
            try
            {
                if (File.Exists(file))
                {
                    string decrypted = SeaHatsFileManager.DecryptFile(file, secret);

                    Data = DeserializeQuest(decrypted);

                    OnQuestListChangeRequest(new QuestListEventArgs());
                    return true;
                }
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

                    Data = DeserializeQuest(toDeserialize);

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

        public bool DeleteStep(ushort questId, short stepId)
        {
            if (Data.Quests.TryGetValue(questId, out var questData) && questData.Steps.ContainsKey(stepId))
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
                result = (ushort)(Data.Quests.Keys.Max() + 1);
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
            if (Data.Quests.TryGetValue(questId, out var questData))
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

            if (Data.Quests.TryGetValue(questId, out var questData))
            {
                QuestDataInfo questDataInfo = new QuestDataInfo(questData)
                {
                    ID = newId,
                    QuestName = $"CLONED_{questData.QuestName}"
                };

                questDataInfo.SetStepParent(questDataInfo);

                if (Data.Quests.TryAdd(newId, questDataInfo))
                {
                    OnQuestListChangeRequest(new QuestListEventArgs() { QuestData = questDataInfo });
                    return true;
                }
            }

            return false;

        }
        public void ChangeTitleName(ushort questId, string title)
        {
            if (Data.Quests.TryGetValue(questId, out var questData))
            {
                questData.QuestName = title;

                OnQuestListChangeRequest(new QuestListEventArgs() { QuestData = questData });
            }
        }

        public string AddParameter(QuestParameters par, string param)
        {
            string paramToAdd = $"@[{par}:{param}]";

            return paramToAdd;
        }

        public bool AddStep(ushort questIndex)
        {
            if(Data.Quests.TryGetValue(questIndex, out var questData))
            {
                int nextStep = questData.GetFreeStep();
                if (nextStep > MaxStep)
                {
                    //MessageBox.Show("can't add the step, the maxium step quest is reached");
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
                    //MessageBox.Show("can't add the step");
                }
            }

            return false;
        }
 
        public Dictionary<int, string> FixedSteps { get; private set; } = new Dictionary<int, string>()
        {
            { 0, "Init"},
            {short.MaxValue, "Finish" }
        };


        public QuestListEventArgs AddQuest()
        {
            ushort newId = GetFreeId();

            QuestDataInfo questDataInfo = new QuestDataInfo()
            {
                ID = newId
            };

            short stepId = 0;
            questDataInfo.AddStep(new QuestDataStep(questDataInfo) { Step = stepId, StepName = FixedSteps[stepId] });

            stepId = short.MaxValue;
            questDataInfo.AddStep(new QuestDataStep(questDataInfo) { Step = stepId, StepName = FixedSteps[stepId] });

            if (Data.Quests.TryAdd(newId, questDataInfo))
            {
                return new QuestListEventArgs()
                {
                    Success = true,
                    QuestData = questDataInfo
                };
            }

            return null;
        }


        public Dictionary<QuestParameters, string> GetParameters(string text)
        {

            string pattern = @"@\[(\w+):(\w+)\]"; // Pattern per trovare uno o più numeri
            Regex regex = new Regex(pattern);

            MatchCollection matches = regex.Matches(text);

            Dictionary<QuestParameters, string> results = new Dictionary<QuestParameters, string>();

            foreach (var match in matches)
            {

            }

            return results;
        }
    }
}
