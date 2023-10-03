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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Ultima;
using UoFiddler.Plugin.ImageParser;
using UoFiddler.Plugin.MultiEditor.Classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace UoFiddler.Plugin.ExamplePlugin.UserControls
{
    public partial class ImageParserBase : UserControl
    {
        public Dictionary<string, CustomImage> Images { get; set; } = new Dictionary<string, CustomImage>();
        public ImageParserBase()
        {
            InitializeComponent();

        }

        private void loadImagebtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var image = new CustomImage(dlg.FileName);
                FileInfo f = new FileInfo(dlg.FileName);

                string name = f.Name.Replace(f.Extension, "");
                Images.TryAdd(name, image);
                imageLoadedCombobox.Items.Add(name);

                if (imageLoadedCombobox.SelectedIndex == -1)
                    imageLoadedCombobox.SelectedIndex = 0;
            }

            dlg.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }



        private void imageLoadedCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filename = (string)imageLoadedCombobox.SelectedItem;

            if (Images.TryGetValue(filename, out var image))
                pictureBoxImage.Image = image.Image;

            pictureBoxImage.Invalidate();
        }

        private void matrixBtn_Click(object sender, EventArgs e)
        {
            string filename = (string)imageLoadedCombobox.SelectedItem;

            if (Images.TryGetValue(filename, out var image))
                pictureBoxImage.Image = image.FillWithTileMatrix();

            //pictureBoxImage.Invalidate();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                pictureBoxImage.Image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void extractPieceBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
