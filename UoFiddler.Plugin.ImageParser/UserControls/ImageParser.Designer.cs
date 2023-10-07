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
            widthTxt = new System.Windows.Forms.TextBox();
            heightTxt = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            stretchBtn = new System.Windows.Forms.Button();
            imageLoadedCombobox = new System.Windows.Forms.ComboBox();
            saveBtn = new System.Windows.Forms.Button();
            extractPieceBtn = new System.Windows.Forms.Button();
            drawBorderBtn = new System.Windows.Forms.Button();
            splitRectBtn = new System.Windows.Forms.Button();
            saveTileBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxImage
            // 
            pictureBoxImage.BackColor = System.Drawing.Color.MintCream;
            pictureBoxImage.Location = new System.Drawing.Point(307, 2);
            pictureBoxImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            pictureBoxImage.Name = "pictureBoxImage";
            pictureBoxImage.Size = new System.Drawing.Size(848, 495);
            pictureBoxImage.TabIndex = 0;
            pictureBoxImage.TabStop = false;
            // 
            // loadImagebtn
            // 
            loadImagebtn.Location = new System.Drawing.Point(219, 2);
            loadImagebtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            loadImagebtn.Name = "loadImagebtn";
            loadImagebtn.Size = new System.Drawing.Size(82, 22);
            loadImagebtn.TabIndex = 1;
            loadImagebtn.Text = "Load Image";
            loadImagebtn.UseVisualStyleBackColor = true;
            loadImagebtn.Click += loadImagebtn_Click;
            // 
            // widthTxt
            // 
            widthTxt.Location = new System.Drawing.Point(67, 69);
            widthTxt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            widthTxt.Name = "widthTxt";
            widthTxt.Size = new System.Drawing.Size(83, 23);
            widthTxt.TabIndex = 2;
            widthTxt.Text = "300";
            // 
            // heightTxt
            // 
            heightTxt.Location = new System.Drawing.Point(66, 93);
            heightTxt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            heightTxt.Name = "heightTxt";
            heightTxt.Size = new System.Drawing.Size(83, 23);
            heightTxt.TabIndex = 3;
            heightTxt.Text = "300";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(17, 70);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 15);
            label1.TabIndex = 4;
            label1.Text = "Width";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(17, 95);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(43, 15);
            label2.TabIndex = 5;
            label2.Text = "Height";
            // 
            // stretchBtn
            // 
            stretchBtn.Location = new System.Drawing.Point(156, 70);
            stretchBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            stretchBtn.Name = "stretchBtn";
            stretchBtn.Size = new System.Drawing.Size(82, 22);
            stretchBtn.TabIndex = 6;
            stretchBtn.Text = "Stretch";
            stretchBtn.UseVisualStyleBackColor = true;
            stretchBtn.Click += button1_Click;
            // 
            // imageLoadedCombobox
            // 
            imageLoadedCombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            imageLoadedCombobox.FormattingEnabled = true;
            imageLoadedCombobox.Location = new System.Drawing.Point(16, 42);
            imageLoadedCombobox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            imageLoadedCombobox.Name = "imageLoadedCombobox";
            imageLoadedCombobox.Size = new System.Drawing.Size(133, 23);
            imageLoadedCombobox.TabIndex = 7;
            imageLoadedCombobox.SelectedIndexChanged += imageLoadedCombobox_SelectedIndexChanged;
            // 
            // saveBtn
            // 
            saveBtn.Location = new System.Drawing.Point(219, 144);
            saveBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            saveBtn.Name = "saveBtn";
            saveBtn.Size = new System.Drawing.Size(82, 36);
            saveBtn.TabIndex = 9;
            saveBtn.Text = "Save Image";
            saveBtn.UseVisualStyleBackColor = true;
            saveBtn.Click += button2_Click;
            // 
            // extractPieceBtn
            // 
            extractPieceBtn.Location = new System.Drawing.Point(219, 184);
            extractPieceBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            extractPieceBtn.Name = "extractPieceBtn";
            extractPieceBtn.Size = new System.Drawing.Size(82, 36);
            extractPieceBtn.TabIndex = 10;
            extractPieceBtn.Text = "DrawPiece";
            extractPieceBtn.UseVisualStyleBackColor = true;
            extractPieceBtn.Click += extractPieceBtn_Click;
            // 
            // drawBorderBtn
            // 
            drawBorderBtn.Location = new System.Drawing.Point(219, 225);
            drawBorderBtn.Name = "drawBorderBtn";
            drawBorderBtn.Size = new System.Drawing.Size(82, 41);
            drawBorderBtn.TabIndex = 11;
            drawBorderBtn.Text = "Draw Border Matrix Polygon";
            drawBorderBtn.UseVisualStyleBackColor = true;
            drawBorderBtn.Click += drawBorderBtn_Click;
            // 
            // splitRectBtn
            // 
            splitRectBtn.Location = new System.Drawing.Point(3, 144);
            splitRectBtn.Name = "splitRectBtn";
            splitRectBtn.Size = new System.Drawing.Size(78, 41);
            splitRectBtn.TabIndex = 13;
            splitRectBtn.Text = "split in tile";
            splitRectBtn.UseVisualStyleBackColor = true;
            splitRectBtn.Click += button2_Click_1;
            // 
            // saveTileBtn
            // 
            saveTileBtn.Location = new System.Drawing.Point(3, 186);
            saveTileBtn.Name = "saveTileBtn";
            saveTileBtn.Size = new System.Drawing.Size(78, 33);
            saveTileBtn.TabIndex = 14;
            saveTileBtn.Text = "Save Tile";
            saveTileBtn.UseVisualStyleBackColor = true;
            saveTileBtn.Click += saveTileBtn_Click;
            // 
            // ImageParserBase
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(saveTileBtn);
            Controls.Add(splitRectBtn);
            Controls.Add(drawBorderBtn);
            Controls.Add(extractPieceBtn);
            Controls.Add(saveBtn);
            Controls.Add(imageLoadedCombobox);
            Controls.Add(stretchBtn);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(heightTxt);
            Controls.Add(widthTxt);
            Controls.Add(loadImagebtn);
            Controls.Add(pictureBoxImage);
            DoubleBuffered = true;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "ImageParserBase";
            Size = new System.Drawing.Size(1158, 499);
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxImage;
        private System.Windows.Forms.Button loadImagebtn;
        private System.Windows.Forms.TextBox widthTxt;
        private System.Windows.Forms.TextBox heightTxt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button stretchBtn;
        private System.Windows.Forms.ComboBox imageLoadedCombobox;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button extractPieceBtn;
        private System.Windows.Forms.Button drawBorderBtn;
        private System.Windows.Forms.Button splitRectBtn;
        private System.Windows.Forms.Button saveTileBtn;
    }
}
