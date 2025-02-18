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

namespace UoFiddler.Plugin.FontSeaHats.UserControls
{
    partial class QuestControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            stepLabel = new System.Windows.Forms.Label();
            questIdlabel = new System.Windows.Forms.Label();
            stepText = new System.Windows.Forms.TextBox();
            titleTextbox = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            stepNameTextbox = new System.Windows.Forms.TextBox();
            saveBtn = new System.Windows.Forms.Button();
            resetStepBtn = new System.Windows.Forms.Button();
            removeStepBtn = new System.Windows.Forms.Button();
            questPriorityCB = new System.Windows.Forms.ComboBox();
            paramsType = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            questNotesTxtbox = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            npcQuestGump = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            canRepeatCheck = new System.Windows.Forms.CheckBox();
            regionNameTxt = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            paramsCustom = new System.Windows.Forms.TextBox();
            paramsCombobox = new System.Windows.Forms.ComboBox();
            parametersBtn = new System.Windows.Forms.Button();
            label10 = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            dataContainerList = new System.Windows.Forms.ListBox();
            SuspendLayout();
            // 
            // stepLabel
            // 
            stepLabel.AutoSize = true;
            stepLabel.Location = new System.Drawing.Point(422, 11);
            stepLabel.Name = "stepLabel";
            stepLabel.Size = new System.Drawing.Size(57, 15);
            stepLabel.TabIndex = 17;
            stepLabel.Text = "Step ID: X";
            // 
            // questIdlabel
            // 
            questIdlabel.AutoSize = true;
            questIdlabel.Location = new System.Drawing.Point(422, 25);
            questIdlabel.Name = "questIdlabel";
            questIdlabel.Size = new System.Drawing.Size(65, 15);
            questIdlabel.TabIndex = 16;
            questIdlabel.Text = "Quest ID: X";
            // 
            // stepText
            // 
            stepText.Location = new System.Drawing.Point(13, 118);
            stepText.Multiline = true;
            stepText.Name = "stepText";
            stepText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            stepText.Size = new System.Drawing.Size(312, 302);
            stepText.TabIndex = 15;
            stepText.TextChanged += stepText_TextChanged;
            stepText.Enter += stepText_Enter;
            stepText.MouseDown += stepText_MouseDown;
            // 
            // titleTextbox
            // 
            titleTextbox.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            titleTextbox.Location = new System.Drawing.Point(144, 10);
            titleTextbox.Name = "titleTextbox";
            titleTextbox.Size = new System.Drawing.Size(256, 33);
            titleTextbox.TabIndex = 14;
            titleTextbox.TextChanged += titleTextbox_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label6.Location = new System.Drawing.Point(3, 11);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(135, 32);
            label6.TabIndex = 13;
            label6.Text = "Quest Title:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(13, 55);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(55, 15);
            label7.TabIndex = 12;
            label7.Text = "Step Title";
            // 
            // stepNameTextbox
            // 
            stepNameTextbox.Location = new System.Drawing.Point(144, 52);
            stepNameTextbox.Name = "stepNameTextbox";
            stepNameTextbox.Size = new System.Drawing.Size(256, 23);
            stepNameTextbox.TabIndex = 11;
            stepNameTextbox.TextChanged += stepNameTextbox_TextChanged;
            // 
            // saveBtn
            // 
            saveBtn.Location = new System.Drawing.Point(992, 377);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new System.Drawing.Size(75, 43);
            saveBtn.TabIndex = 18;
            saveBtn.Text = "Applica modifica";
            saveBtn.UseVisualStyleBackColor = true;
            saveBtn.Click += saveBtn_Click;
            // 
            // resetStepBtn
            // 
            resetStepBtn.Location = new System.Drawing.Point(992, 299);
            resetStepBtn.Name = "resetStepBtn";
            resetStepBtn.Size = new System.Drawing.Size(75, 27);
            resetStepBtn.TabIndex = 19;
            resetStepBtn.Text = "Reset Step";
            resetStepBtn.UseVisualStyleBackColor = true;
            resetStepBtn.Click += resetStepBtn_Click;
            // 
            // removeStepBtn
            // 
            removeStepBtn.Location = new System.Drawing.Point(992, 332);
            removeStepBtn.Name = "removeStepBtn";
            removeStepBtn.Size = new System.Drawing.Size(75, 39);
            removeStepBtn.TabIndex = 20;
            removeStepBtn.Text = "Remove Step";
            removeStepBtn.UseVisualStyleBackColor = true;
            removeStepBtn.Click += removeStepBtn_Click;
            // 
            // questPriorityCB
            // 
            questPriorityCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            questPriorityCB.FormattingEnabled = true;
            questPriorityCB.Location = new System.Drawing.Point(846, 26);
            questPriorityCB.Name = "questPriorityCB";
            questPriorityCB.Size = new System.Drawing.Size(121, 23);
            questPriorityCB.TabIndex = 21;
            questPriorityCB.SelectedIndexChanged += questPriorityCB_SelectedIndexChanged;
            // 
            // paramsType
            // 
            paramsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            paramsType.FormattingEnabled = true;
            paramsType.Location = new System.Drawing.Point(846, 311);
            paramsType.Name = "paramsType";
            paramsType.Size = new System.Drawing.Size(121, 23);
            paramsType.TabIndex = 22;
            paramsType.SelectedIndexChanged += steptype_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(888, 8);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(79, 15);
            label1.TabIndex = 23;
            label1.Text = "Quest Priority";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(894, 293);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(73, 15);
            label2.TabIndex = 24;
            label2.Text = "Params Type";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(13, 100);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(77, 15);
            label3.TabIndex = 25;
            label3.Text = "Step Text Box";
            // 
            // questNotesTxtbox
            // 
            questNotesTxtbox.Location = new System.Drawing.Point(659, 118);
            questNotesTxtbox.Multiline = true;
            questNotesTxtbox.Name = "questNotesTxtbox";
            questNotesTxtbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            questNotesTxtbox.Size = new System.Drawing.Size(408, 125);
            questNotesTxtbox.TabIndex = 26;
            questNotesTxtbox.TextChanged += questNotesTxtbox_TextChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(659, 104);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(60, 15);
            label4.TabIndex = 27;
            label4.Text = "GM Notes";
            // 
            // npcQuestGump
            // 
            npcQuestGump.Location = new System.Drawing.Point(341, 118);
            npcQuestGump.Multiline = true;
            npcQuestGump.Name = "npcQuestGump";
            npcQuestGump.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            npcQuestGump.Size = new System.Drawing.Size(312, 302);
            npcQuestGump.TabIndex = 28;
            npcQuestGump.TextChanged += npcQuestGump_TextChanged;
            npcQuestGump.Enter += npcQuestGump_Enter;
            npcQuestGump.MouseDown += npcQuestGump_MouseDown;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(341, 100);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(78, 15);
            label5.TabIndex = 29;
            label5.Text = "NPC Text box";
            // 
            // canRepeatCheck
            // 
            canRepeatCheck.AutoSize = true;
            canRepeatCheck.Location = new System.Drawing.Point(725, 56);
            canRepeatCheck.Name = "canRepeatCheck";
            canRepeatCheck.Size = new System.Drawing.Size(84, 19);
            canRepeatCheck.TabIndex = 30;
            canRepeatCheck.Text = "Repeatable";
            canRepeatCheck.UseVisualStyleBackColor = true;
            canRepeatCheck.CheckedChanged += canRepeatCheck_CheckedChanged;
            // 
            // regionNameTxt
            // 
            regionNameTxt.Location = new System.Drawing.Point(674, 26);
            regionNameTxt.Name = "regionNameTxt";
            regionNameTxt.Size = new System.Drawing.Size(130, 23);
            regionNameTxt.TabIndex = 31;
            regionNameTxt.TextChanged += regionNameTxt_TextChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(725, 8);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(75, 15);
            label8.TabIndex = 32;
            label8.Text = "Group Name";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(588, 5);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(46, 15);
            label9.TabIndex = 34;
            label9.Text = "Params";
            // 
            // paramsCustom
            // 
            paramsCustom.Location = new System.Drawing.Point(540, 52);
            paramsCustom.Name = "paramsCustom";
            paramsCustom.Size = new System.Drawing.Size(94, 23);
            paramsCustom.TabIndex = 35;
            // 
            // paramsCombobox
            // 
            paramsCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            paramsCombobox.FormattingEnabled = true;
            paramsCombobox.Location = new System.Drawing.Point(513, 23);
            paramsCombobox.Name = "paramsCombobox";
            paramsCombobox.Size = new System.Drawing.Size(121, 23);
            paramsCombobox.TabIndex = 36;
            // 
            // parametersBtn
            // 
            parametersBtn.Location = new System.Drawing.Point(444, 51);
            parametersBtn.Name = "parametersBtn";
            parametersBtn.Size = new System.Drawing.Size(79, 24);
            parametersBtn.TabIndex = 37;
            parametersBtn.Text = "Add params";
            parametersBtn.UseVisualStyleBackColor = true;
            parametersBtn.Click += button1_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(659, 265);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(83, 15);
            label10.TabIndex = 39;
            label10.Text = "Step Objective";
            label10.Click += label10_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(894, 339);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 41;
            button1.Text = "Add Props";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(894, 368);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(75, 23);
            button2.TabIndex = 42;
            button2.Text = "Remove Props";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(894, 397);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(75, 23);
            button3.TabIndex = 43;
            button3.Text = "View";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // dataContainerList
            // 
            dataContainerList.FormattingEnabled = true;
            dataContainerList.ItemHeight = 15;
            dataContainerList.Location = new System.Drawing.Point(659, 283);
            dataContainerList.Name = "dataContainerList";
            dataContainerList.Size = new System.Drawing.Size(181, 139);
            dataContainerList.TabIndex = 44;
            dataContainerList.SelectedIndexChanged += dataContainerList_SelectedIndexChanged;
            // 
            // QuestControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Gray;
            Controls.Add(dataContainerList);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label10);
            Controls.Add(parametersBtn);
            Controls.Add(paramsCombobox);
            Controls.Add(paramsCustom);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(regionNameTxt);
            Controls.Add(canRepeatCheck);
            Controls.Add(label5);
            Controls.Add(npcQuestGump);
            Controls.Add(label4);
            Controls.Add(questNotesTxtbox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(paramsType);
            Controls.Add(questPriorityCB);
            Controls.Add(removeStepBtn);
            Controls.Add(resetStepBtn);
            Controls.Add(saveBtn);
            Controls.Add(stepLabel);
            Controls.Add(questIdlabel);
            Controls.Add(stepText);
            Controls.Add(titleTextbox);
            Controls.Add(label6);
            Controls.Add(label7);
            Controls.Add(stepNameTextbox);
            Name = "QuestControl";
            Size = new System.Drawing.Size(1082, 423);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label stepLabel;
        private System.Windows.Forms.Label questIdlabel;
        private System.Windows.Forms.TextBox stepText;
        private System.Windows.Forms.TextBox titleTextbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox stepNameTextbox;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button resetStepBtn;
        private System.Windows.Forms.Button removeStepBtn;
        private System.Windows.Forms.ComboBox questPriorityCB;
        private System.Windows.Forms.ComboBox paramsType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox questNotesTxtbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox npcQuestGump;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox canRepeatCheck;
        private System.Windows.Forms.TextBox regionNameTxt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox paramsCustom;
        private System.Windows.Forms.ComboBox paramsCombobox;
        private System.Windows.Forms.Button parametersBtn;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox dataContainerList;
    }
}
