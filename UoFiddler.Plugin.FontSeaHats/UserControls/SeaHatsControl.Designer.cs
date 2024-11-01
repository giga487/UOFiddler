/***************************************************************************
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
            clearBtn = new System.Windows.Forms.Button();
            reloadBtn = new System.Windows.Forms.Button();
            autoFillFontBtn = new System.Windows.Forms.Button();
            AddFontBtn = new System.Windows.Forms.Button();
            _pictureBox = new System.Windows.Forms.PictureBox();
            DrawBtn = new System.Windows.Forms.Button();
            FontCreator = new System.Windows.Forms.Button();
            fontListBox = new System.Windows.Forms.ListBox();
            otherStuffsPage = new System.Windows.Forms.TabPage();
            fontDialog1 = new System.Windows.Forms.FontDialog();
            trackBar1 = new System.Windows.Forms.TrackBar();
            label1 = new System.Windows.Forms.Label();
            tabControl1.SuspendLayout();
            fontPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_pictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(fontPage);
            tabControl1.Controls.Add(otherStuffsPage);
            tabControl1.Location = new System.Drawing.Point(0, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(2068, 499);
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
            fontPage.Size = new System.Drawing.Size(2060, 471);
            fontPage.TabIndex = 0;
            fontPage.Text = "Font";
            fontPage.UseVisualStyleBackColor = true;
            fontPage.Click += fontPage_Click;
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
            // otherStuffsPage
            // 
            otherStuffsPage.Location = new System.Drawing.Point(4, 24);
            otherStuffsPage.Name = "otherStuffsPage";
            otherStuffsPage.Padding = new System.Windows.Forms.Padding(3);
            otherStuffsPage.Size = new System.Drawing.Size(2060, 471);
            otherStuffsPage.TabIndex = 1;
            otherStuffsPage.Text = "Not Implemented Yet";
            otherStuffsPage.UseVisualStyleBackColor = true;
            // 
            // trackBar1
            // 
            trackBar1.Location = new System.Drawing.Point(209, 381);
            trackBar1.Minimum = -10;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new System.Drawing.Size(175, 45);
            trackBar1.TabIndex = 1;
            trackBar1.ValueChanged += trackBar1_ValueChanged;
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
            ((System.ComponentModel.ISupportInitialize)_pictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage fontPage;
        private System.Windows.Forms.TabPage otherStuffsPage;
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
    }
}
