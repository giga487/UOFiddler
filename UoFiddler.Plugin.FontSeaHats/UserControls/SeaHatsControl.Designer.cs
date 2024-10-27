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
            _pictureBox = new System.Windows.Forms.PictureBox();
            createButton = new System.Windows.Forms.Button();
            FontCreator = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            fontListBox = new System.Windows.Forms.ListBox();
            otherStuffsPage = new System.Windows.Forms.TabPage();
            fontDialog1 = new System.Windows.Forms.FontDialog();
            tabControl1.SuspendLayout();
            fontPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_pictureBox).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(fontPage);
            tabControl1.Controls.Add(otherStuffsPage);
            tabControl1.Location = new System.Drawing.Point(0, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(785, 409);
            tabControl1.TabIndex = 0;
            // 
            // fontPage
            // 
            fontPage.Controls.Add(_pictureBox);
            fontPage.Controls.Add(createButton);
            fontPage.Controls.Add(FontCreator);
            fontPage.Controls.Add(label1);
            fontPage.Controls.Add(fontListBox);
            fontPage.Location = new System.Drawing.Point(4, 24);
            fontPage.Name = "fontPage";
            fontPage.Padding = new System.Windows.Forms.Padding(3);
            fontPage.Size = new System.Drawing.Size(777, 381);
            fontPage.TabIndex = 0;
            fontPage.Text = "Font";
            fontPage.UseVisualStyleBackColor = true;
            fontPage.Click += fontPage_Click;
            // 
            // _pictureBox
            // 
            _pictureBox.Location = new System.Drawing.Point(209, 40);
            _pictureBox.Name = "_pictureBox";
            _pictureBox.Size = new System.Drawing.Size(568, 335);
            _pictureBox.TabIndex = 3;
            _pictureBox.TabStop = false;
            // 
            // createButton
            // 
            createButton.Location = new System.Drawing.Point(209, 3);
            createButton.Name = "createButton";
            createButton.Size = new System.Drawing.Size(75, 23);
            createButton.TabIndex = 2;
            createButton.Text = "Draw";
            createButton.UseVisualStyleBackColor = true;
            createButton.Click += createButton_Click;
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
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(299, 10);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(99, 15);
            label1.TabIndex = 1;
            label1.Text = "ABCDEFGHILMIN";
            // 
            // fontListBox
            // 
            fontListBox.FormattingEnabled = true;
            fontListBox.ItemHeight = 15;
            fontListBox.Location = new System.Drawing.Point(6, 40);
            fontListBox.Name = "fontListBox";
            fontListBox.Size = new System.Drawing.Size(197, 184);
            fontListBox.TabIndex = 0;
            // 
            // otherStuffsPage
            // 
            otherStuffsPage.Location = new System.Drawing.Point(4, 24);
            otherStuffsPage.Name = "otherStuffsPage";
            otherStuffsPage.Padding = new System.Windows.Forms.Padding(3);
            otherStuffsPage.Size = new System.Drawing.Size(777, 381);
            otherStuffsPage.TabIndex = 1;
            otherStuffsPage.Text = "Not Implemented Yet";
            otherStuffsPage.UseVisualStyleBackColor = true;
            // 
            // SeaHatsControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tabControl1);
            DoubleBuffered = true;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "SeaHatsControl";
            Size = new System.Drawing.Size(785, 412);
            tabControl1.ResumeLayout(false);
            fontPage.ResumeLayout(false);
            fontPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_pictureBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage fontPage;
        private System.Windows.Forms.TabPage otherStuffsPage;
        private System.Windows.Forms.ListBox fontListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button FontCreator;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.PictureBox _pictureBox;
    }
}
