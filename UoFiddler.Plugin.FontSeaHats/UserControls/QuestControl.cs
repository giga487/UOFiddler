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
using UoFiddler.Plugin.FontSeaHats.QuestSH;

namespace UoFiddler.Plugin.FontSeaHats.UserControls
{
    public partial class QuestControl : UserControl
    {
        public event EventHandler<QuestStepUpdated> InvalidateStep;

        QuestDataStep tempDataInfo { get; set; } = null;
        QuestDataStep datasInfo { get; set; } = null;

        public string QuestIdLabel { get; set; } = "QuestID: ";
        public string SteIdLabel { get; set; } = "StepID: ";
        public QuestControl(QuestDataStep data)
        {
            InitializeComponent();

            datasInfo = data;

            tempDataInfo = new QuestDataStep(data);

            stepLabel.Text = $"{SteIdLabel}{data.Step}";
            questIdlabel.Text = $"{QuestIdLabel}{data.Own.ID}";
            titleTextbox.Text = $"{data.Own.QuestName}";
            stepNameTextbox.Text = $"{data.StepName}";
        }

        private void titleTextbox_TextChanged(object sender, EventArgs e)
        {
            string textTitle = titleTextbox.Text;
            tempDataInfo.Own.QuestName = textTitle;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Apply();
        }

        public void Apply()
        {
            datasInfo = new QuestDataStep(tempDataInfo);
            InvalidateStep.Invoke(this, new QuestStepUpdated(datasInfo.Own.ID, datasInfo.Step));
        }
    }
}
