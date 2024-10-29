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


namespace UoFiddler.Plugin.ExamplePlugin.UserControls
{
    public partial class SeaHatsControl : UserControl
    {
        public SeaHatsControl()
        {
            InitializeComponent();

            LoadFont();
            label1.Text = _exampleStringLetter; ;

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

        string _exampleStringLetter => "abcdefghijklmnopqrstuvwxyz";
        string _charUseul => "[];:!?";
        FontDialog _fontDialog { get; set; } = null;
        private void FontCreator_Click(object sender, EventArgs e)
        {
            _fontDialog = new FontDialog();
            if (_fontDialog.ShowDialog() == DialogResult.OK)
            {
                _font = _fontDialog.Font;
                label1.Font = _font;
                label1.Text = _exampleStringLetter;

                _pictureBox.Invalidate();
            }

            //var fontDialogResult = _fontDialog.
        }

        System.Drawing.Font _font { get; set; } = null;


        private void createButton_Click(object sender, EventArgs e)
        {
            if (_font is not null)
            {
                _pictureBox.Refresh();
            }

        }

        private async void _pictureBox_Paint_1(object sender, PaintEventArgs e)
        {

            if (_font is not null)
            {
                Graphics g = e.Graphics;
                //Font font = new Font("Arial", 16);
                int x = 0;
                int y = 0;
                char[] charArray = _exampleStringLetter.ToCharArray(); 
                char[] usefulArray = _charUseul.ToCharArray();
                char[] charArrayUpper = _exampleStringLetter.ToUpper().ToCharArray();

                var array = charArray.Concat(usefulArray).Concat(charArrayUpper).ToArray();

                int border = 5;
                int i = 0;

                foreach (char c in array)
                {
                    SizeF size = g.MeasureString(c.ToString(), _font);

                    System.Drawing.Brush brush = System.Drawing.Brushes.Gray;

                    try
                    {
                        using (Bitmap bitmap = new Bitmap((int)size.Width, (int)size.Height))
                        {
                            using (Graphics g2 = Graphics.FromImage(bitmap))
                            {
                                //g2.Clear(System.Drawing.Color.Blue);
                                x = (int)(0);
                                PointF position = new PointF(x, y);
                                g2.DrawString(c.ToString(), _font, brush, position);
                            }

                            string nam = string.Empty;
                            if (charArrayUpper.Contains(c))
                            {
                                nam = $"{c.ToString()}_UPPER.tiff";
                                bitmap.Save(nam, System.Drawing.Imaging.ImageFormat.Tiff);
                            }
                            else
                            {
                                nam = $"{c.ToString()}.tiff";
                                bitmap.Save(nam, System.Drawing.Imaging.ImageFormat.Tiff);
                            }

                            AsciiText.Fonts[10].Characters[i++] = bitmap;

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
    }
}
