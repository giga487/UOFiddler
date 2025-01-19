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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeaHatsExternal.Quests;

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
        string tempStepNotes { get; set; } = string.Empty;
        string tempNPCText { get; set; } = string.Empty;
        bool tempCanRepeat { get; set; } = false;
        string tempQuestRegionName { get; set; } = string.Empty;

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
            regionNameTxt.Text = data.Own.Group;

            foreach (var value in Enum.GetValues(typeof(QuestType_T)))
            {
                steptype.Items.Add(value);
            }

            steptype.SelectedIndex = (int)data.Type;

            questNotesTxtbox.Text = data.Notes;
            npcQuestGump.Text = data.NpcGumpText;

            canRepeatCheck.CheckState = data.Own.CanRepeat ? CheckState.Checked : CheckState.Unchecked;

            _selectdTextbox = stepText;
            paramsCombobox.Items.Add("----");

            foreach (var value in Enum.GetValues(typeof(QuestParameters)))
            {
                paramsCombobox.Items.Add(value);
            }

            paramsCombobox.SelectedIndex = 0;

        }

        public void ResetTempData(QuestDataStep data)
        {
            tempTitle = titleTextbox.Text = $"{data.Own.QuestName}";
            tempStepName = stepNameTextbox.Text = $"{data.StepName}";
            tempStepText = stepText.Text = data.Text;
            questPriorityCB.SelectedItem = data.Own.Priority;
            steptype.SelectedItem = data.Type;
            tempStepNotes = data.Notes;
            tempNPCText = data.NpcGumpText;
            tempQuestRegionName = data.Own.Group;
            canRepeatCheck.CheckState = data.Own.CanRepeat ? CheckState.Checked : CheckState.Unchecked;
        }

        private void titleTextbox_TextChanged(object sender, EventArgs e)
        {
            tempTitle = titleTextbox.Text;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Apply();

            MessageBox.Show("RICORDATI DI SALVARE IL FILE, altrimenti perderai tutto.");
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
                questStep.Notes = tempStepNotes;
                questStep.NpcGumpText = tempNPCText;

                questStep.Own.CanRepeat = tempCanRepeat;
                questStep.Own.Group = tempQuestRegionName;

                _manager.UpdateStep(datasInfo.Own.ID, questStep);

                _manager.ChangeTitleName(datasInfo.Own.ID, tempTitle);
            }

            //_manager.ChangeStepName(datasInfo.Own.ID, datasInfo.Step, tempStepName);
        }

        private void stepNameTextbox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                _selectdTextbox = textBox;
                selectionStart = textBox.SelectionStart;
                tempStepName = stepNameTextbox.Text;
            }
        }

        private void stepText_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                _selectdTextbox = textBox;
                selectionStart = textBox.SelectionStart;
                tempStepText = stepText.Text;
            }

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

        private void questNotesTxtbox_TextChanged(object sender, EventArgs e)
        {
            tempStepNotes = questNotesTxtbox.Text;
        }

        private void npcQuestGump_TextChanged(object sender, EventArgs e)
        {
            tempNPCText = npcQuestGump.Text;
        }

        private void canRepeatCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (canRepeatCheck.Checked)
            {
                tempCanRepeat = true;
            }
            else
            {
                tempCanRepeat = false;
            }
        }

        private void regionNameTxt_TextChanged(object sender, EventArgs e)
        {
            tempQuestRegionName = regionNameTxt.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (paramsCombobox.SelectedIndex == 0)
            {
                MessageBox.Show("Seleziona un parametro valido.");
                return;
            }

            if (_selectdTextbox == null)
            {
                MessageBox.Show("Seleziona un text box valido.");
                return;
            }

            var param = (QuestParameters)paramsCombobox.SelectedIndex;

            string generated = _manager.AddParameter(param, paramsCustom.Text);

            string text = _selectdTextbox.Text;

            var newT = text.Insert(selectionStart, generated);

            _selectdTextbox.Text = newT;
        }

        TextBox _selectdTextbox { get; set; } = null;
        int selectionStart { get; set; } = 0;
        private void stepText_Enter(object sender, EventArgs e)
        {
            _selectdTextbox = stepText;
            selectionStart = _selectdTextbox.SelectionStart;
        }

        private void npcQuestGump_Enter(object sender, EventArgs e)
        {
            _selectdTextbox = npcQuestGump;
            selectionStart = _selectdTextbox.SelectionStart;


            var npcQuestParams = _manager.GetParameters(npcQuestGump.Text);
        }

        private void npcQuestGump_MouseDown(object sender, MouseEventArgs e)
        {
            selectionStart = npcQuestGump.SelectionStart;
        }

        private void stepText_MouseDown(object sender, MouseEventArgs e)
        {
            selectionStart = stepText.SelectionStart;
        }
    }
}
