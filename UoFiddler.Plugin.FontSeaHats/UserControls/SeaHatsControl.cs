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
using System.Runtime.InteropServices;
using UoFiddler.Plugin.FontSeaHats.QuestSH;
using UoFiddler.Plugin.FontSeaHats.UserControls;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using UoFiddler.Plugin.FontSeaHats;

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
        public char SpaceReference = (char)73; // 73 I, 79 O

        QuestSeaHatsManager _questManager { get; set; } = null;
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

            _questManager = new QuestSeaHatsManager();
            _questManager.QuestListChangeRequest += _questManager_UpdateQuestsRequest;
            _questManager.StepListChangeRequest += _questManager_StepListChangeRequest;
            for (int i = 0; i < 100; i++)
            {
                maxStepCombo.Items.Add(i);
            }

            maxStepCombo.SelectedIndex = 4;
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

        public Size CharSize(char c, Graphics g)
        {
            /* metodo alternativo */
            RectangleF layoutRect = new RectangleF(0, 0, 100, 100);
            StringFormat format = new StringFormat();

            CharacterRange[] characterRanges = { new CharacterRange(0, 1) };
            format.SetMeasurableCharacterRanges(characterRanges);
            format.Alignment = StringAlignment.Near;

            Region[] regions = g.MeasureCharacterRanges($"{c}", _font, layoutRect, format);

            g.MeasureCharacterRanges(c.ToString(), _font, layoutRect, format);
            RectangleF bounds = regions[0].GetBounds(g);

            return new Size((int)bounds.Width, (int)bounds.Height);
            /* fine metodo alternativo di misurazione */
        }


        public Bitmap CreateChar(Bitmap bmp, Char c, System.Drawing.Brush brush, double lOffset, Size spaceSize, float heightOffset)
        {
            if (c == 0)
                return null;

            Bitmap drawn = null;

            Graphics g = Graphics.FromImage(bmp);
            //SizeF size = g.MeasureString(c.ToString(), _font);

            Size size = CharSize(c, g);

            try
            {

                if (c == 32)
                {
                    drawn = new Bitmap((int)(spaceSize.Width), (int)(spaceSize.Height));
                }
                else
                {
                    drawn = new Bitmap((int)(size.Width), (int)(size.Height));

                }

                StringFormat format1 = new StringFormat()
                {
                    Alignment = StringAlignment.Near,
                };

                int leftOffset = (int)(Math.Round(drawn.Width * lOffset, 0));

                using (Graphics g2 = Graphics.FromImage(drawn))
                {
                    g2.Clear(System.Drawing.Color.Transparent);

                    float posX = 0;

                    //if ( c == '6' || c == '8')
                    //{
                    //    heightOffset = (int)(Math.Ceiling(size.Height * 0.08));
                    //}
                    //else if ((c >= 65 && c < 91))
                    //{
                    //    heightOffset = -(int)(Math.Ceiling(size.Height * 0.3));
                    //}

                    PointF position = new PointF(posX + leftOffset, heightOffset);
                    g2.DrawString(c.ToString(), _font, brush, position, format1);
                }
            }
            catch
            {

            }

            return drawn;
        }

        public List<Bitmap> CreateChars(Bitmap bmp, double leftOffset, System.Drawing.Brush brush)
        {
            List<Bitmap> chars = new List<Bitmap>();

            Graphics g = Graphics.FromImage(bmp);

            if (_font is not null)
            {
                //Font font = new Font("Arial", 16);
                int x = 0;
                int y = 30;

                var array = lastString.ToCharArray();

                int sizeToAdd = 224 - array.Length;
                var newCharArray = new char[sizeToAdd];

                array = [.. array, .. newCharArray];

                int border = 2;
                int i = 0;

                int lastWidth = 0;

                Size space = CharSize((char)SpaceReference, g);

                foreach (char toAnalyze in array)
                {
                    var charBmp = CreateChar(bmp, toAnalyze, brush, leftOffset, space, 0);

                    if (charBmp is null)
                    {
                        charBmp = CreateChar(bmp, 'O', brush, leftOffset, space, 0);
                    }

                    chars.Add(charBmp);
                }


                return chars;
                //MaxHeight = (int)Math.Ceiling((double)MaxHeight * 0.8); ;

                /*
                foreach (char toAnalyze in array)
                {
                    char c = toAnalyze;

                    //SizeF size = g.MeasureString(c.ToString(), _font);

                    if (c == 0)
                        continue;

#region metodoAlternativo
                    RectangleF layoutRect = new RectangleF(0, 0, 100, 100);
                    StringFormat format = new StringFormat();

                    CharacterRange[] characterRanges = { new CharacterRange(0, 1) };
                    format.SetMeasurableCharacterRanges(characterRanges);
                    format.Alignment = StringAlignment.Near;

                    Region[] regions = g.MeasureCharacterRanges($"{c}", _font, layoutRect, format);

                    g.MeasureCharacterRanges(c.ToString(), _font, layoutRect, format);
                    RectangleF bounds = regions[0].GetBounds(g);

                    Size size = new Size((int)bounds.Width, (int)bounds.Height);
#endregion

                    System.Drawing.Brush brush = System.Drawing.Brushes.Gray;
                    System.Drawing.Brush violetBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Violet);

                    try
                    {
                        //Scrittura su bitmap
                        //Bitmap bitmap = new Bitmap(boundsSize.Width, boundsSize.Height))

                        //if (c == 0)
                        //{
                        //    c = (char)32;
                        //}

                        Bitmap bitmap = new Bitmap((int)(size.Width), (int)(size.Height));
                        if (c == 32)
                        {
                            bitmap = new Bitmap((int)(SpaceWidth), (int)(size.Height));
                        }

                        int leftOffset = (int)(Math.Round(bitmap.Width * xOffset, 0));
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

                        StringFormat format1 = new StringFormat()
                        {
                            Alignment = StringAlignment.Near,
                        };

                        g.DrawRectangle(new System.Drawing.Pen(violetBrush), rectangle);
                        g.DrawString(c.ToString(), _font, brush, positionLetter, format1);

                        using (Graphics g2 = Graphics.FromImage(bitmap))
                        {
                            g2.Clear(System.Drawing.Color.Transparent);

                            float posX = 0;

                            //if ( c == '6' || c == '8')
                            //{
                            //    heightOffset = (int)(Math.Ceiling(size.Height * 0.08));
                            //}
                            //else if ((c >= 65 && c < 91))
                            //{
                            //    heightOffset = -(int)(Math.Ceiling(size.Height * 0.3));
                            //}

                            PointF position = new PointF(posX - leftOffset, heightOffset);
                            g2.DrawString(c.ToString(), _font, brush, position, format1);
                        }

                        chars.Add(bitmap);

                        lastWidth = (int)bitmap.Width + border;

                    }
                    catch
                    {

                    }
                }
            }

            return chars;
                */
            }

            return null;
        }



        private void DrawButton_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(_pictureBox.Width, _pictureBox.Height);
            _pictureBox.Image = bmp;



            _listBitmap = CreateChars(bmp, xOffset, System.Drawing.Brushes.DarkGray);

            ControlEvents.FontLoaderReload();
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

            char[] test = "Perche' Con Quella C Aspirata E Quel Senso Dell'Umorismo Da 4 Soldi I Toscani Hanno Devastato Questo Paese, E Questo Lo Devi Scrivere, Per favore ZYJKX!".ToCharArray();

            int yTest = 1;
            int xTest = 0;
            i = 1;

            foreach (var characterBitmap in test)
            {
                if (AsciiText.Fonts[_selected] is null)
                {
                    break;
                }
                var bitmap = AsciiText.Fonts[_selected].GetBitmap(characterBitmap);
                PointF point = new PointF(xTest, yTest);

                if (characterBitmap == 32)
                {

                }

                if (bitmap is null)
                    continue;

                if (i++ % 90 == 0)
                {
                    yTest += (int)bitmap.Height;
                    xTest = 0;

                    point = new PointF(xTest, yTest);
                }

                try
                {
                    e.Graphics.DrawRectangle(new System.Drawing.Pen(violetBrush), new RectangleF(point, bitmap.Size));
                    e.Graphics.DrawImage(bitmap, point);
                    xTest += bitmap.Width;
                }
                catch
                {

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

            try
            {
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
            catch (Exception ex)
            {

            }
        }

        private void fontListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selected = fontListBox.SelectedIndex;

            _listBitmap = AsciiText.Fonts[_selected]?.Characters.ToList();

            _pictureBox.Update();
        }

        private void reloadBtn_Click(object sender, EventArgs e)
        {
            ControlEvents.FontLoaderReload();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _selected = fontListBox.SelectedIndex;

            if (_selected != -1)
            {
                AsciiText.Fonts[_selected].ClearBitmaps();
                ControlEvents.FontLoaderReload();
            }

        }

        double xOffset = 0;
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int value = trackBar1.Value;

            xOffset = (double)value / 10;

            Bitmap bmp = new Bitmap(_pictureBox.Width, _pictureBox.Height);

            _listBitmap = CreateChars(bmp, xOffset, System.Drawing.Brushes.DarkGray);
            _pictureBox.Invalidate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void questIDListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (questIDListBox.SelectedIndex != -1)
            {
                string selectedName = string.Empty;
                if (string.IsNullOrEmpty(selectedName = (string)questIDListBox.SelectedItem))
                {
                    string name = _questManager.Data.Quests.Values.First().QuestName;
                    selectedName = name;
                }

                var questData2 = _questManager.GetQuest(selectedName);

                RefillQuestControl(questData2);
            }
        }

        public void RefillQuestControl(QuestDataInfo dataInfo)
        {
            StepTab.TabPages.Clear();
            if (dataInfo is null)
            {
                return;
            }

            foreach (var idStep in dataInfo.Steps)
            {
                var tabPage = new TabPage()
                {
                    Text = idStep.Value.StepName,
                };

                tabPage.Controls.Add(new QuestControl(idStep.Value, _questManager));

                StepTab.TabPages.Add(tabPage);
            }
        }

        public void RefillQuestTitle()
        {
            questIDListBox.Items.Clear();

            foreach (var questData in _questManager.Data.Quests.Values)
            {
                questIDListBox.Items.Add(questData.QuestName);
            }

            QuestDataInfo questData2 = null;
            string selectedName = string.Empty;
            if (string.IsNullOrEmpty(selectedName = (string)questIDListBox.SelectedItem))
            {
                try
                {
                    string name = _questManager.Data.Quests.Values.First().QuestName;
                    selectedName = name;
                    questData2 = _questManager.GetQuest(selectedName);
                }
                catch
                {

                }
            }
            else
            {
                var questName = (string)questIDListBox.Items[lastIndex];
                questData2 = _questManager.GetQuest(questName);
            }

            RefillQuestControl(questData2);
        }


        private void _questManager_StepListChangeRequest(object sender, StepListEventArgs e)
        {
            string selectedName = string.Empty;
            if (string.IsNullOrEmpty(selectedName = (string)questIDListBox.SelectedItem))
            {
                string name = _questManager.Data.Quests.Values.First().QuestName;
                selectedName = name;
            }
            else if (e.StepData is not null)
            {
                lastStep = e.StepData;

            }
        }

        QuestDataStep lastStep { get; set; } = null;
        short lastIndex = -1;
        private void _questManager_UpdateQuestsRequest(object sender, QuestListEventArgs e)
        {
            lastIndex = (short)questIDListBox.SelectedIndex;
            RefillQuestTitle();

            try
            {
                questIDListBox.SelectedIndex = lastIndex;

                if (lastStep is not null)
                {
                    TabPage tabPage = null;

                    foreach (TabPage tab in StepTab.TabPages)
                    {
                        if (tab.Text == lastStep.StepName)
                        {
                            tabPage = tab;
                            break;
                        }
                    }

                    StepTab.SelectedTab = tabPage;
                }
                else
                {
                    StepTab.SelectedIndex = 0;
                }
            }
            catch
            {
                questIDListBox.SelectedIndex = questIDListBox.Items.Count - 1;
            }
        }

        private void newQuest_Click(object sender, EventArgs e)
        {
            var result = _questManager.AddQuest();

            if (result is not null)
            {
                RefillQuestTitle();
            }

            questIDListBox.SelectedIndex = (short)(questIDListBox.Items.Count - 1);
        }

        private void addStepBtn_Click(object sender, EventArgs e)
        {
            if (questIDListBox.SelectedItem is not null)
            {
                var questData = _questManager.GetQuest((string)questIDListBox.SelectedItem);

                bool value = _questManager.AddStep(questData.ID);
                RefillQuestControl(questData);
            }
        }

        private void maxStepCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            if (maxStepCombo.SelectedValue is not null)
                _questManager.ChangeMaxStep((int)maxStepCombo.SelectedValue);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            var questData = _questManager.GetQuest((string)questIDListBox.SelectedItem);
            try
            {
                _questManager.DeleteQuest(questData.ID);
            }
            catch
            {

            }
        }

        public string filePath { get; private set; } = string.Empty;

        private void saveBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 0;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(saveFileDialog1.FileName))
                {
                    filePath = new DirectoryInfo(saveFileDialog1.FileName).FullName;

                    _questManager.Data.SecretKey = secretKeyTxt.Text;
                    _questManager.SaveJsonQuest(saveFileDialog1.FileName);
                }

            }
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = "c:\\";
            }

            openFileDialog.InitialDirectory = _questManager.FileQuests;
            openFileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = new DirectoryInfo(openFileDialog.FileName).FullName;
                _questManager.LoadQuestJson(openFileDialog.FileName);

                secretKeyTxt.Text = _questManager.Data.SecretKey;
            }

            try
            {
                questIDListBox.SelectedIndex = 0;
            }
            catch
            {

            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 1)
            {
                _questManager.OnQuestListChangeRequest(new QuestListEventArgs());
            }


        }

        private void cloneQuestBtn_Click(object sender, EventArgs e)
        {
            if (questIDListBox.SelectedItem is not null)
            {
                _questManager.CloneQuest((string)questIDListBox.SelectedItem);

                questIDListBox.SelectedIndex = (short)(questIDListBox.Items.Count - 1);
            }
        }

        private void SaveBySecretBtn_Click(object sender, EventArgs e)
        {
            if(File.Exists(filePath))
            {
                FileInfo f = new FileInfo(filePath);
                string fileName = f.Name.Replace(f.Extension, "");

                var data = new QuestData(_questManager.Data);
                data.SecretKey = "";

                string toSave = _questManager.ExtractDataJson();

                string pwd = secretKeyTxt.Text;
                if (!string.IsNullOrEmpty(pwd))
                {

                    FileManagerHelper.EncryptStringToFile(toSave, fileName + ".MUL", pwd);
                }
            }

        }

        private void loadBySecretBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = _questManager.FileQuests;
            openFileDialog.Filter = "mul files (*.mul)|*.mul|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileToLoad = new DirectoryInfo(openFileDialog.FileName).FullName;

                    string decrypted = FileManagerHelper.DecryptFile(fileToLoad, secretKeyTxt.Text);
                    _questManager.DeserializeQuest(decrypted);

                    _questManager.OnQuestListChangeRequest(new QuestListEventArgs());
                }

                questIDListBox.SelectedIndex = 0;
            }
            catch
            {

            }



        }
    }
}
