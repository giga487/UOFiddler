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
using System.Linq;


namespace SeaHatsExternal.Quests
{
    public class QuestRewardInfo()
    {
        public int Amount { get; set; } = 1;
        public Type Type { get; set; } = null;
        public int InternalAmount { get; set; } = 1;
    }
    public class QuestStepUpdated : EventArgs
    {
        public int QuestID { get; private set; } = -1;
        public int StepId { get; private set; } = -1;
        public QuestStepUpdated(int questId, int stepId)
        {
            QuestID = questId;
            StepId = stepId;
        }
    }
    public interface IStepDataContainer
    {
        public QuestType_T Type { get; }
        public string Name { get; }
    }

    public class LocationContainer : IStepDataContainer
    {
        public string X { get; set; }  = "0";
        public string Y { get; set; }  = "0";
        public string Z { get; set; }  = "0";
        public string MapID { get; set; }  = "0";
        public QuestType_T Type => QuestType_T.ReachLocation;
        public string Name { get; set; } = string.Empty;
    }
    public class MobTypeContainer : IStepDataContainer
    {
        public string MobName { get; set; } = string.Empty;
        public string Max { get; set; } = "0";
        public QuestType_T Type => QuestType_T.KillMob;
        public string Name { get; set; } = string.Empty;
    }
    public class GatherContainer : IStepDataContainer
    {
        public string ObjectToGather { get; set; } = string.Empty;
        public string Max { get; set; } = "0";
        public QuestType_T Type => QuestType_T.GatherObject;
        public string Name { get; set; } = string.Empty;
    }
    public class TalkContainer : IStepDataContainer
    {
        public string NameToTalk { get; set; } = string.Empty;
        public QuestType_T Type => QuestType_T.TalkWithNPC;
        public string Name { get; set; } = string.Empty;
    }
    public class QuestDataStep
    {
        //public Dictionary<QuestParameters, string> NPCTextParameters { get; set; } = new Dictionary<QuestParameters, string>();
        //public Dictionary<QuestParameters, string> QuestTextParameters { get; set; } = new Dictionary<QuestParameters, string>();
        public string? StepName { get; set; } = string.Empty;
        public short Step { get; set; } = -1;
        public QuestDataInfo? Own { get; set; } = null;
        public string? Text { get; set; } = string.Empty;
        public List<string> StepObjective { get; set; } = new List<string>();
        public string Notes { get; set; } = string.Empty;
        public string NpcGumpText { get; set; } = string.Empty;

        public List<IStepDataContainer> StepDataContainers { get; set; } = new List<IStepDataContainer>();
        public QuestDataStep()
        {

        }

        public QuestDataStep(QuestDataStep? toCopy)
        {
            if (toCopy is null)
            {
                return;
            }

            StepName = toCopy.StepName;
            StepObjective = toCopy.StepObjective;
            Step = toCopy.Step;
            //Type = toCopy.Type;
            NpcGumpText = toCopy.NpcGumpText;
            Text = toCopy.Text;
            Own = toCopy.Own;
            Notes = toCopy.Notes;
            StepDataContainers = new List<IStepDataContainer>(toCopy.StepDataContainers);
        }
        public QuestDataStep(QuestDataInfo? parent)
        {
            Own = parent;
        }
    }

    public enum QuestType_T
    {
        Default,
        ReachLocation,
        KillMob,
        GatherObject,
        TalkWithNPC
    }
    public enum QuestPriority_T
    {
        Primary,
        SecondaryQuest,
        Daily,
        Worker
    }
    public class QuestDataInfo
    {
        public Dictionary<string, QuestRewardInfo> Reward = new Dictionary<string, QuestRewardInfo>();
        public bool CanRepeat { get; set; } = false;
        public string QuestName { get; set; } = "DEFAULT NAME";
        public ushort ID { get; set; } = 0;
        public QuestPriority_T Priority { get; set; }
        public Dictionary<short, QuestDataStep> Steps { get; set; } = new Dictionary<short, QuestDataStep>();
        public string Group { get; set; } = string.Empty;

        public QuestDataInfo()
        { }

        public QuestDataInfo(QuestDataInfo toCopy)
        {
            CanRepeat = toCopy.CanRepeat;
            Reward = toCopy.Reward;
            QuestName = toCopy.QuestName;
            ID = toCopy.ID;
            Priority = toCopy.Priority;
            Group = toCopy.Group;

            foreach (var stepKV in toCopy.Steps)
            {
                Steps.Add(stepKV.Key, new QuestDataStep(stepKV.Value));
            }
        }

        public void SetStepParent(QuestDataInfo owner)
        {
            foreach (var stepKV in Steps)
            {
                stepKV.Value.Own = owner;
            }
        }

        public short GetFreeStep()
        {
            if (Steps.Keys.Count > 0)
            {
                short max = Steps.Keys
                    .OrderByDescending(n => n)
                    .Distinct()
                    .Skip(1)
                    .FirstOrDefault();

                return Convert.ToInt16(max + 1);
            }
            else
                return (short)1;
        }

        public bool AddStep(QuestDataStep step)
        {
            short index = step.Step;

            if (step.Step == -1)
            {
                index = GetFreeStep();
            }


            if (!Steps.TryAdd(index, step))
            {
                return false;
            }
            else
            {
                step.Step = index;

                if (string.IsNullOrEmpty(step.StepName))
                {
                    step.StepName = $"STEP {index}";
                }
            }

            return true;
        }
    }
}
