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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UoFiddler.Plugin.FontSeaHats.QuestSH;

namespace UoFiddler.Plugin.FontSeaHats.UserControls
{
    public partial class QuestControl : UserControl
    {
        QuestDataStep tempDataInfo { get; set; } = null;
        QuestDataStep datasInfo { get; set; } = null;

        public string QuestIdLabel { get; set; } = "QuestID: ";
        public string StepIdLabel { get; set; } = "StepID: ";

        string tempTitle { get; set; } = string.Empty;
        string tempStepName { get; set; } = string.Empty;
        string tempStepText { get; set; } = string.Empty;
        QuestSeaHatsManager _manager { get; set; } = null;
        public QuestControl(QuestDataStep data, QuestSeaHatsManager manager)
        {
            InitializeComponent();

            datasInfo = data;

            stepLabel.Text = $"{StepIdLabel}{data.Step}";
            questIdlabel.Text = $"{QuestIdLabel}{data.Own.ID}";

            ResetTempData(datasInfo);

            _manager = manager;

            foreach (var value in Enum.GetValues(typeof(QuestPriority_T)))
            {
                questPriorityCB.Items.Add(value);
            }

            questPriorityCB.SelectedIndex = (int)data.Own.Priority;

            foreach (var value in Enum.GetValues(typeof(QuestType_T)))
            {
                steptype.Items.Add(value);
            }

            steptype.SelectedIndex = (int)data.Type;
        }

        public void ResetTempData(QuestDataStep data)
        {
            tempTitle = titleTextbox.Text = $"{data.Own.QuestName}";
            tempStepName = stepNameTextbox.Text = $"{data.StepName}";
            tempStepText = stepText.Text = data.Text;
            questPriorityCB.SelectedItem = data.Own.Priority;
            steptype.SelectedItem = data.Type;
        }

        private void titleTextbox_TextChanged(object sender, EventArgs e)
        {
            tempTitle = titleTextbox.Text;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Apply();
        }

        public void Apply(bool update = true)
        {
            if (update)
            {
                datasInfo.Own.Priority = (QuestPriority_T)questPriorityCB.SelectedIndex;

                var questStep = new QuestDataStep(datasInfo);
                questStep.StepName = tempStepName;
                questStep.Text = tempStepText;
                questStep.Type = (QuestType_T)steptype.SelectedIndex;

                _manager.UpdateStep(datasInfo.Own.ID, questStep);

                _manager.ChangeTitleName(datasInfo.Own.ID, tempTitle);
            }

            //_manager.ChangeStepName(datasInfo.Own.ID, datasInfo.Step, tempStepName);
        }

        private void stepNameTextbox_TextChanged(object sender, EventArgs e)
        {
            tempStepName = stepNameTextbox.Text;
        }

        private void stepText_TextChanged(object sender, EventArgs e)
        {
            tempStepText = stepText.Text;
        }

        private void resetStepBtn_Click(object sender, EventArgs e)
        {
            ResetTempData(datasInfo);
        }

        private void removeStepBtn_Click(object sender, EventArgs e)
        {
            _manager.DeleteStep(datasInfo.Own.ID, datasInfo.Step);
        }

        private void questPriorityCB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void steptype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
