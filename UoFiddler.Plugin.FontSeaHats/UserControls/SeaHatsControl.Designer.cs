﻿/***************************************************************************
 *
 * $Author: Turley
 * 
 * "THE BEER-WARE LICENSE"
 * As long as you retain this notice you can do whatever you want with 
 * this stuff. If we meet some day, and you think this stuff is worth it,
 * you can buy me a beer in return.
 *
 ***************************************************************************/

namespace UoFiddler.Plugin.ExamplePlugin.UserControls
{
    partial class SeaHatsControl
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
            tabControl1 = new System.Windows.Forms.TabControl();
            fontPage = new System.Windows.Forms.TabPage();
            label1 = new System.Windows.Forms.Label();
            trackBar1 = new System.Windows.Forms.TrackBar();
            clearBtn = new System.Windows.Forms.Button();
            reloadBtn = new System.Windows.Forms.Button();
            autoFillFontBtn = new System.Windows.Forms.Button();
            AddFontBtn = new System.Windows.Forms.Button();
            _pictureBox = new System.Windows.Forms.PictureBox();
            DrawBtn = new System.Windows.Forms.Button();
            FontCreator = new System.Windows.Forms.Button();
            fontListBox = new System.Windows.Forms.ListBox();
            QuestPage = new System.Windows.Forms.TabPage();
            removeStepBtn = new System.Windows.Forms.Button();
            addStepBtn = new System.Windows.Forms.Button();
            questGroupBox = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            deleteButton = new System.Windows.Forms.Button();
            newQuest = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            questIDListBox = new System.Windows.Forms.ListBox();
            fontDialog1 = new System.Windows.Forms.FontDialog();
            tabControl1.SuspendLayout();
            fontPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_pictureBox).BeginInit();
            QuestPage.SuspendLayout();
            questGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(fontPage);
            tabControl1.Controls.Add(QuestPage);
            tabControl1.Location = new System.Drawing.Point(0, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(2068, 524);
            tabControl1.TabIndex = 0;
            // 
            // fontPage
            // 
            fontPage.Controls.Add(label1);
            fontPage.Controls.Add(trackBar1);
            fontPage.Controls.Add(clearBtn);
            fontPage.Controls.Add(reloadBtn);
            fontPage.Controls.Add(autoFillFontBtn);
            fontPage.Controls.Add(AddFontBtn);
            fontPage.Controls.Add(_pictureBox);
            fontPage.Controls.Add(DrawBtn);
            fontPage.Controls.Add(FontCreator);
            fontPage.Controls.Add(fontListBox);
            fontPage.Location = new System.Drawing.Point(4, 24);
            fontPage.Name = "fontPage";
            fontPage.Padding = new System.Windows.Forms.Padding(3);
            fontPage.Size = new System.Drawing.Size(2060, 496);
            fontPage.TabIndex = 0;
            fontPage.Text = "Font";
            fontPage.UseVisualStyleBackColor = true;
            fontPage.Click += fontPage_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(265, 411);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(69, 15);
            label1.TabIndex = 8;
            label1.Text = "Char mover";
            // 
            // trackBar1
            // 
            trackBar1.Location = new System.Drawing.Point(209, 381);
            trackBar1.Minimum = -10;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new System.Drawing.Size(175, 45);
            trackBar1.TabIndex = 1;
            trackBar1.Scroll += trackBar1_Scroll;
            trackBar1.ValueChanged += trackBar1_ValueChanged;
            // 
            // clearBtn
            // 
            clearBtn.Location = new System.Drawing.Point(6, 286);
            clearBtn.Name = "clearBtn";
            clearBtn.Size = new System.Drawing.Size(102, 38);
            clearBtn.TabIndex = 7;
            clearBtn.Text = "Clear Selected Font";
            clearBtn.Click += button1_Click;
            // 
            // reloadBtn
            // 
            reloadBtn.Location = new System.Drawing.Point(125, 286);
            reloadBtn.Name = "reloadBtn";
            reloadBtn.Size = new System.Drawing.Size(78, 27);
            reloadBtn.TabIndex = 6;
            reloadBtn.Text = "Reload";
            reloadBtn.Click += reloadBtn_Click;
            // 
            // autoFillFontBtn
            // 
            autoFillFontBtn.Location = new System.Drawing.Point(125, 319);
            autoFillFontBtn.Name = "autoFillFontBtn";
            autoFillFontBtn.Size = new System.Drawing.Size(78, 27);
            autoFillFontBtn.TabIndex = 5;
            autoFillFontBtn.Text = "Auto Fill Font";
            autoFillFontBtn.Click += AutoFillBtn_Click;
            // 
            // AddFontBtn
            // 
            AddFontBtn.Location = new System.Drawing.Point(125, 352);
            AddFontBtn.Name = "AddFontBtn";
            AddFontBtn.Size = new System.Drawing.Size(78, 23);
            AddFontBtn.TabIndex = 4;
            AddFontBtn.Text = "Create ";
            AddFontBtn.Click += AddFontBtn_Click;
            // 
            // _pictureBox
            // 
            _pictureBox.Location = new System.Drawing.Point(209, 40);
            _pictureBox.Name = "_pictureBox";
            _pictureBox.Size = new System.Drawing.Size(1826, 335);
            _pictureBox.TabIndex = 3;
            _pictureBox.TabStop = false;
            _pictureBox.Paint += _pictureBox_Paint_1;
            // 
            // DrawBtn
            // 
            DrawBtn.Location = new System.Drawing.Point(209, 3);
            DrawBtn.Name = "DrawBtn";
            DrawBtn.Size = new System.Drawing.Size(75, 23);
            DrawBtn.TabIndex = 2;
            DrawBtn.Text = "Draw";
            DrawBtn.UseVisualStyleBackColor = true;
            DrawBtn.Click += DrawButton_Click;
            // 
            // FontCreator
            // 
            FontCreator.Location = new System.Drawing.Point(128, 3);
            FontCreator.Name = "FontCreator";
            FontCreator.Size = new System.Drawing.Size(75, 23);
            FontCreator.TabIndex = 0;
            FontCreator.Text = "Create ";
            FontCreator.Click += FontCreator_Click;
            // 
            // fontListBox
            // 
            fontListBox.FormattingEnabled = true;
            fontListBox.ItemHeight = 15;
            fontListBox.Location = new System.Drawing.Point(6, 40);
            fontListBox.Name = "fontListBox";
            fontListBox.Size = new System.Drawing.Size(197, 229);
            fontListBox.TabIndex = 0;
            fontListBox.SelectedIndexChanged += fontListBox_SelectedIndexChanged;
            // 
            // QuestPage
            // 
            QuestPage.Controls.Add(removeStepBtn);
            QuestPage.Controls.Add(addStepBtn);
            QuestPage.Controls.Add(questGroupBox);
            QuestPage.Controls.Add(deleteButton);
            QuestPage.Controls.Add(newQuest);
            QuestPage.Controls.Add(label2);
            QuestPage.Controls.Add(questIDListBox);
            QuestPage.Location = new System.Drawing.Point(4, 24);
            QuestPage.Name = "QuestPage";
            QuestPage.Padding = new System.Windows.Forms.Padding(3);
            QuestPage.Size = new System.Drawing.Size(2060, 496);
            QuestPage.TabIndex = 1;
            QuestPage.Text = "Quest";
            QuestPage.UseVisualStyleBackColor = true;
            // 
            // removeStepBtn
            // 
            removeStepBtn.Location = new System.Drawing.Point(276, 128);
            removeStepBtn.Name = "removeStepBtn";
            removeStepBtn.Size = new System.Drawing.Size(88, 23);
            removeStepBtn.TabIndex = 8;
            removeStepBtn.Text = "Remove step";
            removeStepBtn.UseVisualStyleBackColor = true;
            // 
            // addStepBtn
            // 
            addStepBtn.Location = new System.Drawing.Point(276, 99);
            addStepBtn.Name = "addStepBtn";
            addStepBtn.Size = new System.Drawing.Size(88, 23);
            addStepBtn.TabIndex = 7;
            addStepBtn.Text = "Add Step";
            addStepBtn.UseVisualStyleBackColor = true;
            addStepBtn.Click += addStepBtn_Click;
            // 
            // questGroupBox
            // 
            questGroupBox.Controls.Add(tabPage1);
            questGroupBox.Location = new System.Drawing.Point(370, 24);
            questGroupBox.Name = "questGroupBox";
            questGroupBox.SelectedIndex = 0;
            questGroupBox.Size = new System.Drawing.Size(827, 447);
            questGroupBox.TabIndex = 6;
            // 
            // tabPage1
            // 
            tabPage1.Location = new System.Drawing.Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(819, 419);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // deleteButton
            // 
            deleteButton.Location = new System.Drawing.Point(276, 70);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new System.Drawing.Size(88, 23);
            deleteButton.TabIndex = 5;
            deleteButton.Text = "Delete";
            deleteButton.UseVisualStyleBackColor = true;
            // 
            // newQuest
            // 
            newQuest.Location = new System.Drawing.Point(276, 41);
            newQuest.Name = "newQuest";
            newQuest.Size = new System.Drawing.Size(88, 23);
            newQuest.TabIndex = 4;
            newQuest.Text = "New";
            newQuest.UseVisualStyleBackColor = true;
            newQuest.Click += newQuest_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 24);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(72, 15);
            label2.TabIndex = 2;
            label2.Text = "QUEST TITLE";
            // 
            // questIDListBox
            // 
            questIDListBox.FormattingEnabled = true;
            questIDListBox.ItemHeight = 15;
            questIDListBox.Location = new System.Drawing.Point(6, 41);
            questIDListBox.Name = "questIDListBox";
            questIDListBox.Size = new System.Drawing.Size(264, 424);
            questIDListBox.TabIndex = 1;
            questIDListBox.SelectedIndexChanged += questIDListBox_SelectedIndexChanged;
            // 
            // SeaHatsControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tabControl1);
            DoubleBuffered = true;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "SeaHatsControl";
            Size = new System.Drawing.Size(2071, 635);
            tabControl1.ResumeLayout(false);
            fontPage.ResumeLayout(false);
            fontPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)_pictureBox).EndInit();
            QuestPage.ResumeLayout(false);
            QuestPage.PerformLayout();
            questGroupBox.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage fontPage;
        private System.Windows.Forms.TabPage QuestPage;
        private System.Windows.Forms.ListBox fontListBox;
        private System.Windows.Forms.Button FontCreator;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button DrawBtn;
        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.Button AddFontBtn;
        private System.Windows.Forms.Button autoFillFontBtn;
        private System.Windows.Forms.Button reloadBtn;
        private System.Windows.Forms.Button clearBtn;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox questIDListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button newQuest;
        private System.Windows.Forms.Button removeStepBtn;
        private System.Windows.Forms.Button addStepBtn;
        private System.Windows.Forms.TabControl questGroupBox;
        private System.Windows.Forms.TabPage tabPage1;
        private FontSeaHats.UserControls.QuestControl questControl1;
    }
}
