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

    public class QuestDataStep
    {
        public string StepName { get; set; } = string.Empty;
        public int Step { get; set; } = -1;
        public QuestDataInfo Own { get; set; } = null;
        public string Text { get; set; }
        public List<string> StepObjective { get; set; } = new List<string>();
        public QuestType_T Type { get; set; } = QuestType_T.Default;
        public string Notes { get; set; } = string.Empty;
        public string NpcGumpText { get; set; } = string.Empty;

        public QuestDataStep()
        {

        }

        public QuestDataStep(QuestDataStep toCopy)
        {
            StepName = toCopy.StepName;
            StepObjective = toCopy.StepObjective;
            Step = toCopy.Step;
            Type = toCopy.Type;
            Text = toCopy.Text;
            Own = toCopy.Own;
            Notes = toCopy.Notes;
        }
        public QuestDataStep(QuestDataInfo parent)
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
        public Dictionary<int, QuestDataStep> Steps { get; set; } = new Dictionary<int, QuestDataStep>();
        public string RegionName { get; set; } = string.Empty;

        public QuestDataInfo()
        { }

        public QuestDataInfo(QuestDataInfo toCopy)
        {
            CanRepeat = toCopy.CanRepeat;
            Reward = toCopy.Reward;
            QuestName = toCopy.QuestName;
            ID = toCopy.ID;
            Priority = toCopy.Priority;

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

        public int GetFreeStep()
        {
            if (Steps.Keys.Count > 0)
            {
                int max = Steps.Keys
                    .OrderByDescending(n => n)
                    .Distinct()
                    .Skip(1)
                    .FirstOrDefault();

                return max + 1;
            }
            else
                return 1;
        }

        public bool AddStep(QuestDataStep step)
        {
            int index = step.Step;

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
