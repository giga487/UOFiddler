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
using System.Windows;
using System.Windows.Forms;
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
            string filename = (string)imageLoadedCombobox.SelectedItem;

            if (Images.TryGetValue(filename, out var image) && Int32.TryParse(widthTxt.Text, out int width) && Int32.TryParse(heightTxt.Text, out int height))
            {
                pictureBoxImage.Image = image.Stretch(width, height);

            };

            pictureBoxImage.Invalidate();
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

        private void drawBorderBtn_Click(object sender, EventArgs e)
        {
            CustomImage newImage = new CustomImage();

            var polygon = newImage.CreatePolygon(0, 0, 5);
            var newBitmap = newImage.CreateBitmapFromPoints(polygon, Color.AliceBlue);

            pictureBoxImage.Image = newBitmap;
        }

        private void splitinTileBtn_Click(object sender, EventArgs e)
        {

        }

        Dictionary<System.Drawing.Point, Bitmap> TileSplitted = new Dictionary<System.Drawing.Point, Bitmap>();

        private void button2_Click_1(object sender, EventArgs e)
        {
            string filename = (string)imageLoadedCombobox.SelectedItem;

            CustomImage image = null;
            if (Images.TryGetValue(filename, out image))
            {
                TileSplitted = image.SplitInTile();

                if (TileSplitted is null)
                    return;

                Bitmap newBitmap = new Bitmap(1000, 1000);

                int x = 0;
                int xOffset = image.Image.Width + 100;
                using (Graphics g = Graphics.FromImage(newBitmap))
                {
                    g.DrawImage(image.Image, new Rectangle(0 + xOffset, 0, image.Image.Width, image.Image.Height));

                    foreach (var keyValueBitmap in TileSplitted)
                    {
                        Rectangle rect = new Rectangle(keyValueBitmap.Key.X , keyValueBitmap.Key.Y , 44, 44);
                        g.DrawImage(keyValueBitmap.Value, rect);
                    }
                }

                pictureBoxImage.Image = newBitmap;
            }

        }

        private void saveTileBtn_Click(object sender, EventArgs e)
        {
            if (TileSplitted.Values.Count > 0)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "";// "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                saveFileDialog1.Title = "Save an Image File";
                saveFileDialog1.ShowDialog();

                // If the file name is not an empty string open it for saving.
                if (saveFileDialog1.FileName != "")
                {
                    foreach (var keyValue in TileSplitted)
                    {
                        keyValue.Value.Save(saveFileDialog1.FileName + $"_{keyValue.Key.X}_{keyValue.Key.Y}.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }



            }
        }
    }
}
