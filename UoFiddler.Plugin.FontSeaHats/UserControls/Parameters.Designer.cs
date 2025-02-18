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
    partial class Parameters
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
            AcceptBtn = new System.Windows.Forms.Button();
            CloseBtn = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // AcceptBtn
            // 
            AcceptBtn.Location = new System.Drawing.Point(424, 215);
            AcceptBtn.Name = "AcceptBtn";
            AcceptBtn.Size = new System.Drawing.Size(75, 23);
            AcceptBtn.TabIndex = 0;
            AcceptBtn.Text = "Accept";
            AcceptBtn.UseVisualStyleBackColor = true;
            AcceptBtn.Click += AcceptBtn_Click;
            // 
            // CloseBtn
            // 
            CloseBtn.Location = new System.Drawing.Point(343, 215);
            CloseBtn.Name = "CloseBtn";
            CloseBtn.Size = new System.Drawing.Size(75, 23);
            CloseBtn.TabIndex = 1;
            CloseBtn.Text = "Close";
            CloseBtn.UseVisualStyleBackColor = true;
            CloseBtn.Click += CloseBtn_Click;
            // 
            // Parameters
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.SystemColors.ActiveCaption;
            BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            Controls.Add(CloseBtn);
            Controls.Add(AcceptBtn);
            Name = "Parameters";
            Size = new System.Drawing.Size(502, 241);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button AcceptBtn;
        private System.Windows.Forms.Button CloseBtn;
    }
}
