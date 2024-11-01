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
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Reflection;


namespace UoFiddler.Plugin.ExamplePlugin.UserControls
{
    public partial class SeaHatsControl : UserControl
    {
        string _firstNumber => " !\"#$%&'[]*+',./0123456789:;<=>?@";
        string _stringLetter => "abcdefghijklmnopqrstuvwxyz";
        string _charUseful => "[\\]^-'";
        string _charUseful2 => "{|}~";
        string lastString { get; set; } = " ";

        public int FirstAsciiValue = 33;//33;
        public int MaxAsciiValue = 126;
        public int SpaceReference = 73; // 73 I, 79 O

        public string TextToCheck = "AGLI IRTI COLLI, IL MAESTRALE FA COSE";
        public SeaHatsControl()
        {
            InitializeComponent();

            //lastString = _firstNumber + _stringLetter.ToUpper() + _charUseful + _stringLetter + _charUseful2;

            for (int i = FirstAsciiValue; i <= MaxAsciiValue; i++)
            {
                lastString += ((char)i).ToString();
            }

            LoadFont();

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

                _pictureBox.Refresh();
            }

            //var fontDialogResult = _fontDialog.
        }

        System.Drawing.Font _font { get; set; } = null;

        private void DrawButton_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(_pictureBox.Width, _pictureBox.Height);
            _pictureBox.Image = bmp;
            Graphics g = Graphics.FromImage(bmp);

            if (_font is not null)
            {
                //Font font = new Font("Arial", 16);
                int x = 0;
                int y = 30;

                var array = lastString.ToCharArray();

                int sizeToAdd = 224 - array.Length;
                var newCharArray = new char[sizeToAdd];

                array = [..array, ..newCharArray];

                int border = 2;
                int i = 0;

                int lastWidth = 0;
                _listBitmap = new List<Bitmap>();

                //int Width = 0;
                int MaxHeight = 0;
                int SpaceWidth = 0;

                foreach (char c in array)
                {
                    SizeF size = g.MeasureString(c.ToString(), _font);

                    if (size.Height > MaxHeight)
                    {
                        MaxHeight = (int)size.Height;
                    }

                    if (c == SpaceReference)
                    {
                        SpaceWidth = (int)(Math.Round(size.Width * 0.8, 0));
                    }
                }

                foreach (char toAnalyze in array)
                {
                    char c = toAnalyze;

                    SizeF size = g.MeasureString(c.ToString(), _font);

                    //RectangleF layoutRect = new RectangleF(10, 10, 300, 50);
                    //StringFormat format = new StringFormat();

                    //CharacterRange[] characterRanges = { new CharacterRange(0, 1) };
                    //format.SetMeasurableCharacterRanges(characterRanges);

                    //Region[] regions = g.MeasureCharacterRanges($"{c}", _font, layoutRect, format);

                    //g.MeasureCharacterRanges(c.ToString(), _font, layoutRect, format);
                    //RectangleF bounds = regions[0].GetBounds(g);

                    //Size boundsSize = new Size((int)bounds.Width, Height);

                    System.Drawing.Brush brush = System.Drawing.Brushes.Gray;
                    System.Drawing.Brush violetBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Violet);

                    try
                    {
                        //Scrittura su bitmap
                        //Bitmap bitmap = new Bitmap(boundsSize.Width, boundsSize.Height))

                        if (c == 0)
                        {
                            c = ' ';
                        }

                        Bitmap bitmap = new Bitmap((int)(size.Width * 0.7), MaxHeight);
                        if (c == ' ')
                        {
                            bitmap = new Bitmap((int)(SpaceWidth), MaxHeight);
                        }

                        int leftOffset = (int)(Math.Round(bitmap.Width * 0.15, 0));
                        int heightOffset = 0;

                        System.Drawing.Brush rectanglebrush = System.Drawing.Brushes.Blue;

                        var pen = new System.Drawing.Pen(brush);

                        x += lastWidth;
                        if (i++ % 40 == 0)
                        {
                            y += (int)size.Height + border;
                            x = 0;
                        }

                        System.Drawing.Point positionInPicturebox = new System.Drawing.Point(x, y);
                        System.Drawing.Point positionLetter = new System.Drawing.Point(x - leftOffset, y);
                        //foreach (var region in regions)
                        //{
                        //    var size2 = new Size((int)region.GetBounds(g).Size.Width, (int)region.GetBounds(g).Size.Height);
                        //    var rectangle = new Rectangle(positionInPicturebox, size2);

                        //    e.Graphics.DrawRectangle(new System.Drawing.Pen(brush), rectangle);
                        //}

                        var rectangle = new System.Drawing.Rectangle(positionInPicturebox, bitmap.Size);
                        //var rectangle = new Rectangle(positionInPicturebox, boundsSize);

                        g.DrawRectangle(new System.Drawing.Pen(violetBrush), rectangle);
                        g.DrawString(c.ToString(), _font, brush, positionLetter);

                        using (Graphics g2 = Graphics.FromImage(bitmap))
                        {
                            g2.Clear(System.Drawing.Color.Transparent);

                            float posX = 0;

                            if ( c == '6' || c == '8')
                            {
                                heightOffset = (int)(Math.Ceiling(size.Height * 0.08));
                            }
                            else if ((c >= 65 && c < 91) || c == '6' || c == '8')
                            {
                                heightOffset = (int)(Math.Ceiling(size.Height * 0.05));
                            }

                            PointF position = new PointF(posX - leftOffset, heightOffset);
                            g2.DrawString(c.ToString(), _font, brush, position);
                        }

                        _listBitmap.Add(bitmap);

                        lastWidth = (int)bitmap.Width + border;

                    }
                    catch
                    {

                    }
                }
            }

            _pictureBox.Refresh();
        }

        int _selected = 0;

        List<Bitmap> _listBitmap = new List<Bitmap>();
        private void _pictureBox_Paint_1(object sender, PaintEventArgs e)
        {
            int x = 0;
            int y = 30;

            e.Graphics.Clear(System.Drawing.Color.White);
            System.Drawing.Brush violetBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Violet);

            int lastWidth = 0;
            int i = 0;
            int border = 10;

            if (_listBitmap is null)
                return;

            foreach (var characterBitmap in _listBitmap)
            {
                if (characterBitmap is null)
                {
                    continue;
                }

                x += lastWidth;
                if (i++ % 40 == 0)
                {
                    y += (int)characterBitmap.Height + 2 * border;
                    x = 0;
                }

                System.Drawing.Point positionInPicturebox = new System.Drawing.Point(x, y);
                var rectangle = new System.Drawing.Rectangle(positionInPicturebox, characterBitmap.Size);
                e.Graphics.DrawRectangle(new System.Drawing.Pen(violetBrush), rectangle);
                e.Graphics.DrawImage(characterBitmap, positionInPicturebox);

                lastWidth = (int)characterBitmap.Width + border;
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
            AsciiFont defaultFont = AsciiText.Fonts[0];

            int index = fontListBox.SelectedIndex;

            if (index == -1 && AsciiText.FreeFontIndex(out int freefont))
            {
                index = freefont;
            }

            var result = MessageBox.Show($"You are copying the font at the {index} font", "overriding?", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                return;
            }

            AsciiText.Fonts[index] = new AsciiFont((byte)index, defaultFont);
            AsciiText.Fonts[index].ClearBitmaps();

            for (int i = 0; i < _listBitmap.Count; i++)
            {
                if (_listBitmap[i] == null)
                {
                    continue;
                }

                AsciiText.Fonts[index].ReplaceCharacter(i, _listBitmap[i]);
            }

            ControlEvents.FontLoaderReload();

        }

        private void fontListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = fontListBox.SelectedIndex;

            _listBitmap = AsciiText.Fonts[index]?.Characters.ToList();

            _pictureBox.Update();
        }

        private void reloadBtn_Click(object sender, EventArgs e)
        {
            ControlEvents.FontLoaderReload();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = fontListBox.SelectedIndex;

            if (index != -1)
            {
                AsciiText.Fonts[index].ClearBitmaps();
                ControlEvents.FontLoaderReload();
            }

        }
    }
}
