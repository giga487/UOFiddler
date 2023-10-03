// /***************************************************************************
//  *
//  * $Author: Turley
//  * 
//  * "THE BEER-WARE LICENSE"
//  * As long as you retain this notice you can do whatever you want with 
//  * this stuff. If we meet some day, and you think this stuff is worth it,
//  * you can buy me a beer in return.
//  *
//  ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UoFiddler.Plugin.MultiEditor.Classes;

namespace UoFiddler.Plugin.ImageParser
{
    public class CustomImage
    {
        public string Path { get; set; } = string.Empty;
        private Bitmap _originalImage { get; set; } = null;
        public Bitmap Image { get; set; } = null;

        public CustomImage(string path)
        {
            if(File.Exists(path))
            {
                Path = path;
                _originalImage = new Bitmap(Path);
                Image = new Bitmap(Path);
            }
        }

        public Bitmap Stretch(int width, int height) 
        {
            Image = new Bitmap(_originalImage, new Size(width, height));

            return Image;
        }

        public int Width { get; private set; }
        public int XMax { get; private set; }
        private int XMaxOrg { get; set; }
        public int XMin { get; private set; }
        private int XMinOrg { get; set; }
        private int YMax { get; set; }
        public int YMaxOrg { get; private set; }
        public int YMin { get; private set; }
        public int YMinOrg { get; private set; }
        public int ZMax { get; private set; }
        public int ZMin { get; private set; }

        public Bitmap FillWithTileMatrix()
        {
            //var color = System.Drawing.Color.FromArgb(92, 32, 192, 32);
            var color = System.Drawing.Color.FromArgb(0, 00, 00, 0);
            Bitmap tileData = GetTIleDataBitmap(color);

            int width = Image.Width;
            int height = Image.Height;
            int tileDataWidth = tileData.Width;
            int tileDataHeight = tileData.Height;

            //int xStep = (int)Math.Floor((double)width / tileDataWidth);
            //int yStep = (int)Math.Floor((double)height / tileDataHeight);
            //
            int xStep = 50;
            int yStep = 50;
            Bitmap newBitmap = new Bitmap(width+44, height+44);
            Rectangle rectangle = new Rectangle(0, 0, width, height); 

            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.DrawImage(Image, rectangle);

                for (int x = 0; x < xStep; x++)
                {
                    for (int y = 0; y < yStep; y++)
                    {
                        g.DrawImage(tileData, new Point(x * tileDataWidth, y * tileDataHeight));
                        g.Save();
                    }
                }
            }

            return newBitmap;
        }

        public List<Bitmap> ExtractMatrixTile()
        {
            //var color = System.Drawing.Color.FromArgb(92, 32, 192, 32);
            var color = System.Drawing.Color.FromArgb(0, 00, 00, 0);
            Bitmap tileData = GetTIleDataBitmap(color);

            int width = Image.Width;
            int height = Image.Height;
            int tileDataWidth = tileData.Width;
            int tileDataHeight = tileData.Height;

            //int xStep = (int)Math.Floor((double)width / tileDataWidth);
            //int yStep = (int)Math.Floor((double)height / tileDataHeight);
            //
            int xStep = 50;
            int yStep = 50;
            Bitmap newBitmap = new Bitmap(width + 44, height + 44);
            Rectangle rectangle = new Rectangle(0, 0, width, height);

            List<Bitmap> bitmaps = new List<Bitmap>();
            Bitmap imageToSplit = new Bitmap(_originalImage);

            using (Graphics g = Graphics.FromImage(imageToSplit))
            {
                for (int x = 0; x < xStep; x++)
                {
                    for (int y = 0; y < yStep; y++)
                    {
                        var points = CreatePolygon(x * tileDataWidth, y * tileDataHeight);

                        //creare un bitmap con questi bitmap

                    }
                }
            }

            return bitmaps;
        }


        public Point[] CreatePolygon(int offsetX, int offsetY)
        {
            Point[] drawFloorPoint = new Point[4];
            drawFloorPoint[0].X = 22 + offsetX;
            drawFloorPoint[0].Y = 0 + offsetY;
            drawFloorPoint[1].X = 44 + offsetX;
            drawFloorPoint[1].Y = 22 + offsetY;
            drawFloorPoint[2].X = 22 + offsetX;
            drawFloorPoint[2].Y = 44 + offsetY;
            drawFloorPoint[3].X = 0 + offsetX;
            drawFloorPoint[3].Y = 22 + offsetY;

            return drawFloorPoint;
        }

        public static Bitmap GetTIleDataBitmap(System.Drawing.Color color)
        {
            Bitmap _floorBmp = null;

            if (_floorBmp != null)
            {
                return _floorBmp;
            }

            _floorBmp = new Bitmap(44, 44);

            using (Graphics g = Graphics.FromImage(_floorBmp))
            {
                //
                using (System.Drawing.Brush floorBrush = new SolidBrush(color))
                {
                    Point[] drawFloorPoint = new Point[4];
                    drawFloorPoint[0].X = 22;
                    drawFloorPoint[0].Y = 0;
                    drawFloorPoint[1].X = 44;
                    drawFloorPoint[1].Y = 22;
                    drawFloorPoint[2].X = 22;
                    drawFloorPoint[2].Y = 44;
                    drawFloorPoint[3].X = 0;
                    drawFloorPoint[3].Y = 22;

                    g.FillPolygon(floorBrush, drawFloorPoint);
                    g.DrawPolygon(Pens.White, drawFloorPoint);
                }
            }

            return _floorBmp;
        }
    }
}
