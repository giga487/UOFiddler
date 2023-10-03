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
    partial class ImageParserBase
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
            pictureBoxImage = new System.Windows.Forms.PictureBox();
            loadImagebtn = new System.Windows.Forms.Button();
            textBox1 = new System.Windows.Forms.TextBox();
            textBox2 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            imageLoadedCombobox = new System.Windows.Forms.ComboBox();
            matrixBtn = new System.Windows.Forms.Button();
            saveBtn = new System.Windows.Forms.Button();
            extractPieceBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxImage
            // 
            pictureBoxImage.BackColor = System.Drawing.Color.Fuchsia;
            pictureBoxImage.Location = new System.Drawing.Point(175, 3);
            pictureBoxImage.Name = "pictureBoxImage";
            pictureBoxImage.Size = new System.Drawing.Size(663, 660);
            pictureBoxImage.TabIndex = 0;
            pictureBoxImage.TabStop = false;
            // 
            // loadImagebtn
            // 
            loadImagebtn.Location = new System.Drawing.Point(75, 3);
            loadImagebtn.Name = "loadImagebtn";
            loadImagebtn.Size = new System.Drawing.Size(94, 29);
            loadImagebtn.TabIndex = 1;
            loadImagebtn.Text = "Load Image";
            loadImagebtn.UseVisualStyleBackColor = true;
            loadImagebtn.Click += loadImagebtn_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(75, 91);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(94, 27);
            textBox1.TabIndex = 2;
            textBox1.Text = "300";
            // 
            // textBox2
            // 
            textBox2.Location = new System.Drawing.Point(75, 124);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(94, 27);
            textBox2.TabIndex = 3;
            textBox2.Text = "300";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(19, 94);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(49, 20);
            label1.TabIndex = 4;
            label1.Text = "Width";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(19, 127);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(54, 20);
            label2.TabIndex = 5;
            label2.Text = "Height";
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(75, 157);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(94, 29);
            button1.TabIndex = 6;
            button1.Text = "Stretch";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // imageLoadedCombobox
            // 
            imageLoadedCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            imageLoadedCombobox.FormattingEnabled = true;
            imageLoadedCombobox.Location = new System.Drawing.Point(18, 56);
            imageLoadedCombobox.Name = "imageLoadedCombobox";
            imageLoadedCombobox.Size = new System.Drawing.Size(151, 28);
            imageLoadedCombobox.TabIndex = 7;
            imageLoadedCombobox.SelectedIndexChanged += imageLoadedCombobox_SelectedIndexChanged;
            // 
            // matrixBtn
            // 
            matrixBtn.Location = new System.Drawing.Point(75, 192);
            matrixBtn.Name = "matrixBtn";
            matrixBtn.Size = new System.Drawing.Size(94, 48);
            matrixBtn.TabIndex = 8;
            matrixBtn.Text = "Insert Matrix";
            matrixBtn.UseVisualStyleBackColor = true;
            matrixBtn.Click += matrixBtn_Click;
            // 
            // saveBtn
            // 
            saveBtn.Location = new System.Drawing.Point(75, 246);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new System.Drawing.Size(94, 48);
            saveBtn.TabIndex = 9;
            saveBtn.Text = "Save Image";
            saveBtn.UseVisualStyleBackColor = true;
            saveBtn.Click += button2_Click;
            // 
            // extractPieceBtn
            // 
            extractPieceBtn.Location = new System.Drawing.Point(75, 300);
            extractPieceBtn.Name = "extractPieceBtn";
            extractPieceBtn.Size = new System.Drawing.Size(94, 48);
            extractPieceBtn.TabIndex = 10;
            extractPieceBtn.Text = "DrawPiece";
            extractPieceBtn.UseVisualStyleBackColor = true;
            extractPieceBtn.Click += extractPieceBtn_Click;
            // 
            // ImageParserBase
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(extractPieceBtn);
            Controls.Add(saveBtn);
            Controls.Add(matrixBtn);
            Controls.Add(imageLoadedCombobox);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(loadImagebtn);
            Controls.Add(pictureBoxImage);
            DoubleBuffered = true;
            Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            Name = "ImageParserBase";
            Size = new System.Drawing.Size(841, 666);
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxImage;
        private System.Windows.Forms.Button loadImagebtn;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox imageLoadedCombobox;
        private System.Windows.Forms.Button matrixBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button extractPieceBtn;
    }
}
