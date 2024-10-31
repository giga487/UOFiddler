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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Media;
using System.Xml.Linq;
using Ultima;
using UoFiddler.Controls.Classes;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace UoFiddler.Plugin.ExamplePlugin.UserControls
{
    public partial class SeaHatsControl : UserControl
    {
        string _firstNumber => " !\"#$%&'[]*+',./0123456789:;<=>?@";
        string _stringLetter => "abcdefghijklmnopqrstuvwxyz";
        string _charUseful => "[\\] - ";

        string lastString { get; set; } = string.Empty;
        public SeaHatsControl()
        {
            InitializeComponent();

            lastString = _firstNumber + _stringLetter.ToUpper() + _charUseful + _stringLetter;


            LoadFont();
            label1.Text = _stringLetter; ;

            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.White
            };

            Invalidate();

        }

        private void OnClickSayHello(object sender, EventArgs e)
        {
            if (Client.Running)
            {
                Client.SendText("Hello World... I am an example plugIn form.");
            }
            else
            {
                MessageBox.Show("UO client is not running so I will say hello here. Hi!");
            }
        }

        public int LastAscii { get; set; } = 0;

        public void LoadFont()
        {
            Cursor.Current = Cursors.WaitCursor;
            Options.LoadedUltimaClass["ASCIIFont"] = true;

            fontListBox.Items.Clear();

            for (int i = 0; i < AsciiText.Fonts.Length; ++i)
            {
                fontListBox.Items.Add(i);
                LastAscii = i;
            }
        }

        private void fontPage_Click(object sender, EventArgs e)
        {

        }

        FontDialog _fontDialog { get; set; } = null;
        private void FontCreator_Click(object sender, EventArgs e)
        {
            _fontDialog = new FontDialog();
            if (_fontDialog.ShowDialog() == DialogResult.OK)
            {
                _font = _fontDialog.Font;
                label1.Font = _font;
                label1.Text = _stringLetter;

                _pictureBox.Refresh();
            }

            //var fontDialogResult = _fontDialog.
        }

        System.Drawing.Font _font { get; set; } = null;

        private void DrawButton_Click(object sender, EventArgs e)
        {
            if (_font is not null)
            {
                _pictureBox.Invalidate();
            }

        }

        List<Bitmap> _listBitmap = new List<Bitmap>();
        private async void _pictureBox_Paint_1(object sender, PaintEventArgs e)
        {

            if (_font is not null)
            {
                Graphics g = e.Graphics;
                //Font font = new Font("Arial", 16);
                int x = 0;
                int y = 0;

                var array = lastString.ToCharArray();

                int border = 5;
                int i = 0;

                _listBitmap.Clear();
                foreach (char c in array)
                {
                    SizeF size = g.MeasureString(c.ToString(), _font);

                    System.Drawing.Brush brush = System.Drawing.Brushes.Gray;

                    try
                    {

                        //Scrittura su bitmap
                        using (Bitmap bitmap = new Bitmap((int)size.Width, (int)size.Height))
                        {
                            System.Drawing.Brush rectanglebrush = System.Drawing.Brushes.Blue;
                            var pen = new System.Drawing.Pen(brush);
                            pen.PenType = System.Drawing.Drawing2D.PenType.SolidColor;
                            x += bitmap.Width;
                            Point positionInPicturebox = new Point(x, y);

                            e.Graphics.DrawRectangle(new System.Drawing.Pen(brush), new Rectangle(positionInPicturebox, bitmap.Size));
                            e.Graphics.DrawString(c.ToString(), _font, brush, positionInPicturebox);

                            using (Graphics g2 = Graphics.FromImage(bitmap))
                            {
                                g2.Clear(System.Drawing.Color.Blue);
   
                                PointF position = new PointF(0, 0);
                                g2.DrawString(c.ToString(), _font, brush, position);
                            }

                            _listBitmap.Add(bitmap);

                            //string nam = string.Empty;
                            //if (array.Contains(c))
                            //{
                            //    nam = $"{c.ToString()}_UPPER.tiff";
                            //    bitmap.Save(nam, System.Drawing.Imaging.ImageFormat.Tiff);
                            //}
                            //else
                            //{
                            //    nam = $"{c.ToString()}.tiff";
                            //    bitmap.Save(nam, System.Drawing.Imaging.ImageFormat.Tiff);
                            //}

                            //AsciiText.Fonts[10].Characters[i++] = bitmap;
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void AddFontBtn_Click(object sender, EventArgs e)
        {
            AsciiFont lastFont = null;

            if (AsciiText.FreeFontIndex(out int freefont))
            {
                lastFont = AsciiText.Fonts[freefont - 1];
                AsciiText.Fonts[freefont] = new AsciiFont((byte)freefont, lastFont);

                MessageBox.Show($"You have created the {freefont} of ASCII font copied from {lastFont.Header}");

                ControlEvents.FontLoaderReload();
            }
        }

        private void AutoFillBtn_Click(object sender, EventArgs e)
        {
            AsciiFont lastFont = null;

            if (AsciiText.FreeFontIndex(out int freefont))
            {
                lastFont = AsciiText.Fonts[freefont - 1];
                AsciiText.Fonts[freefont] = new AsciiFont((byte)freefont, lastFont);

                for (int i = 0; i < _listBitmap.Count; i++)
                {
                    if (_listBitmap[i] == null)
                    {
                        continue;
                    }

                    AsciiText.Fonts[freefont].ReplaceCharacter(i, _listBitmap[i]);
                }


                MessageBox.Show($"You have created the {freefont} of ASCII font copied from {lastFont.Header}");

                ControlEvents.FontLoaderReload();
            }
        }
    }
}
