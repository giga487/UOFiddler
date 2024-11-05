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
            textBox3 = new System.Windows.Forms.TextBox();
            titleTextbox = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            stepNameTextbox = new System.Windows.Forms.TextBox();
            saveBtn = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // stepLabel
            // 
            stepLabel.AutoSize = true;
            stepLabel.Location = new System.Drawing.Point(497, 36);
            stepLabel.Name = "stepLabel";
            stepLabel.Size = new System.Drawing.Size(57, 15);
            stepLabel.TabIndex = 17;
            stepLabel.Text = "Step ID: X";
            // 
            // questIdlabel
            // 
            questIdlabel.AutoSize = true;
            questIdlabel.Location = new System.Drawing.Point(497, 21);
            questIdlabel.Name = "questIdlabel";
            questIdlabel.Size = new System.Drawing.Size(65, 15);
            questIdlabel.TabIndex = 16;
            questIdlabel.Text = "Quest ID: X";
            // 
            // textBox3
            // 
            textBox3.Location = new System.Drawing.Point(13, 109);
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.Size = new System.Drawing.Size(387, 277);
            textBox3.TabIndex = 15;
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
            // 
            // saveBtn
            // 
            saveBtn.Location = new System.Drawing.Point(641, 351);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new System.Drawing.Size(75, 43);
            saveBtn.TabIndex = 18;
            saveBtn.Text = "Save";
            saveBtn.UseVisualStyleBackColor = true;
            saveBtn.Click += saveBtn_Click;
            // 
            // QuestControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ActiveCaption;
            Controls.Add(saveBtn);
            Controls.Add(stepLabel);
            Controls.Add(questIdlabel);
            Controls.Add(textBox3);
            Controls.Add(titleTextbox);
            Controls.Add(label6);
            Controls.Add(label7);
            Controls.Add(stepNameTextbox);
            Name = "QuestControl";
            Size = new System.Drawing.Size(726, 397);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label stepLabel;
        private System.Windows.Forms.Label questIdlabel;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox titleTextbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox stepNameTextbox;
        private System.Windows.Forms.Button saveBtn;
    }
}
