﻿// /***************************************************************************
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
using System.Text;
using System.Threading.Tasks;

namespace UoFiddler.Plugin.FontSeaHats.QuestSH
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
        private QuestDataInfo _own { get; set; } = null;
        public QuestDataInfo Own => _own;
        public string Text { get; set; }
        public List<string> StepObjective { get; set; } = new List<string>();
        public QuestType_T Type { get; set; } = QuestType_T.Default;

        public QuestDataStep(QuestDataStep toCopy)
        {
            StepName = toCopy.StepName;
            StepObjective = toCopy.StepObjective;
            Step = toCopy.Step;
            _own = toCopy.Own;
            Type = toCopy.Type;
            Text = toCopy.Text;
        }
        public QuestDataStep(QuestDataInfo parent)
        {
            _own = parent;


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
        Daily
    }
    public class QuestDataInfo
    {
        public Dictionary<string, QuestRewardInfo> Reward = new Dictionary<string, QuestRewardInfo>();
        public bool CanRepeat { get; set; } = false;
        public string QuestName { get; set; } = "DEFAULT NAME";
        public ushort ID { get; set; } = 0;
        public QuestPriority_T Priority { get; set; }
        public Dictionary<int, QuestDataStep> Steps { get; } = new Dictionary<int, QuestDataStep>();

        public int GetFreeStep()
        {
            if (Steps.Keys.Count > 0)
                return Steps.Keys.Max() + 1;
            else
                return 1;
        }

        public bool AddStep(QuestDataStep step)
        {
            int index = GetFreeStep();

            if (!Steps.TryAdd(index, step))
            {
                return false;
            }
            else
            {
                step.Step = index;
                step.StepName = $"STEP {index}";
            }

            return true;
        }
    }
}