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
using System.Xml.Linq;
using Ultima;
using UoFiddler.Controls.Classes;

namespace UoFiddler.Plugin.ExamplePlugin.UserControls
{
    public partial class SeaHatsControl : UserControl
    {
        public SeaHatsControl()
        {
            InitializeComponent();

            LoadFont();
            label1.Text = _exampleString;

            _pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            _pictureBox.Paint += OnPaint;

            Invalidate();

        }

        private void _pictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Esempio di testo da disegnare
            string text = "Hello, PictureBox!";
            Font font = new Font("Arial", 16);
            Brush brush = Brushes.Black;

            // Disegna il testo
            g.DrawString(text, font, brush, new PointF(50, 50));
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

        string _exampleString => "ABCDEFGHILMNOPQRSTUVZ[];:!?";
        FontDialog _fontDialog { get; set; } = null;
        private void FontCreator_Click(object sender, EventArgs e)
        {
            _fontDialog = new FontDialog();
            if (_fontDialog.ShowDialog() == DialogResult.OK)
            {
                var font = _fontDialog.Font;
                label1.Font = font;
                label1.Text = _exampleString;
            }

            //var fontDialogResult = _fontDialog.
        }

        Font _font { get; set; } = null;

        //private void createButton_Click(object sender, PaintEventArgs e)
        //{

        //}

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //Font font = new Font("Arial", 16);
            string text = "Ciao, mondo!";

            SizeF size = g.MeasureString(text, _font);

            g.DrawString(text, _font, Brushes.Black, new PointF(10, 10));
            g.DrawString($"Width: {size.Width}, Height: {size.Height}", this.Font, Brushes.Black, new PointF(10, 50));
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            _pictureBox.Update();

        }
    }
}
