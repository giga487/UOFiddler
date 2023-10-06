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
using Ultima;
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

        public CustomImage()
        {
        }

        public Bitmap Stretch(int width, int height) 
        {
            Image = new Bitmap(_originalImage, new System.Drawing.Size(width, height));

            using (Graphics g = Graphics.FromImage(Image))
            {
                g.DrawImage(Image, new Rectangle(0,0,width, height));
                g.Save();
            }



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
            Bitmap tileData = GetTileDataBitmap(color);

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
                        g.DrawImage(tileData, new System.Drawing.Point(x * tileDataWidth, y * tileDataHeight));
                        g.Save();
                    }
                }
            }

            return newBitmap;
        }

        public Dictionary<Point, Bitmap> SplitInTile()
        {
            var dictToReturn = new Dictionary<Point, Bitmap>();

            Bitmap correctSizeBitmap = new Bitmap(Image.Width+44, Image.Height + 44);

            using (Graphics g = Graphics.FromImage(correctSizeBitmap))
            {
                g.DrawImage(_originalImage, new Rectangle(22, 22, Image.Width, Image.Height));
                g.Save();
            }

            return SplitInTile(correctSizeBitmap, 44, 44);

        }

        public Dictionary<Point, Bitmap> SplitInTile(Bitmap image, int width, int height)
        {
            Dictionary<Point, Bitmap> splittedDictionary = new Dictionary<Point, Bitmap>();

            int stepToDoX = (int)Math.Ceiling((double)image.Width / width);
            int stepToDoY = (int)Math.Ceiling((double)image.Height / height);
            System.Drawing.Imaging.PixelFormat format = image.PixelFormat;

            int bigWidth = stepToDoX * width;
            int bigHeight = stepToDoY * height;
            Bitmap correctSizeBitmap = new Bitmap(bigWidth + 1, bigHeight + 1);
            using (Graphics g = Graphics.FromImage(correctSizeBitmap))
            {
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
                g.Save();
            }

            int z = 0;
            int i = 0;

            int x = 0;
            int y = 0;
            Rectangle tileRectangle;
            try
            {
                for (y = 44 / 2; y < bigHeight - 22; y += 22)
                {
                    x = (i + 1) % 2 == 0 ? 0 : 44 / 2;
                    int t = 0;
                    for (; x < bigWidth - 22; x += 44)
                    {
                        tileRectangle = new Rectangle(x, y, width, height); //aggiungo metà rombo
                        splittedDictionary.Add(new Point(t++, z), correctSizeBitmap.Clone(tileRectangle, format));
                    }
                    i++;
                    z++;
                }
            }
            catch
            {
                return null;
            }

            Dictionary<Point, Bitmap> polygonTile = new Dictionary<Point, Bitmap>();
            var polygon = CreatePolygon(0,0);
            var borderPoints = FindBorderOfPolygon(polygon);

            foreach (var keyvaluepair in splittedDictionary)
            {
                Bitmap newBitmap = new Bitmap(keyvaluepair.Value);
                using (Graphics g = Graphics.FromImage(newBitmap))
                {
                    g.Clear(Color.Black);

                    for (int xF = 0; xF <= keyvaluepair.Value.Width; xF++)
                    {
                        for (int yF = 0; yF <= keyvaluepair.Value.Height; yF++)
                        {
                            if (FindIfPointsIsInConvexPolygon(borderPoints, new Point(xF, yF)))
                            {
                                Color c = keyvaluepair.Value.GetPixel(xF, yF);
                                newBitmap.SetPixel(xF, yF, c);
                                //g.FillRectangle(floorBrush, xF, yF, 1, 1);
                            }
                        }
                    }

                    g.Save();
                }

                polygonTile[keyvaluepair.Key] = newBitmap;
            }



            return polygonTile;
        }

        public Dictionary<Point, Bitmap> SplitImage(Bitmap image, int width, int height)
        {
            Dictionary<Point, Bitmap> splittedDictionary = new Dictionary<Point, Bitmap>();

            int stepToDoX = (int)Math.Ceiling((double)image.Width / width);
            int stepToDoY = (int)Math.Ceiling((double)image.Height / height);
            System.Drawing.Imaging.PixelFormat format = image.PixelFormat;

            Bitmap correctSizeBitmap = new Bitmap(stepToDoX * width, stepToDoY * height);
            using (Graphics g = Graphics.FromImage(correctSizeBitmap))
            {
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
                g.Save();
            }

            int t = 0;
            
            for (int x = 0; x < correctSizeBitmap.Width; x += width)
            {
                int z = 0;

                for (int y = 0; y < correctSizeBitmap.Height; y += height)
                {
                    Rectangle tileRectangle = new Rectangle(x, y, width, height); //aggiungo metà rombo
                    splittedDictionary.Add(new Point(t,z++) , correctSizeBitmap.Clone(tileRectangle, format));
                }
                t++;
            }

            return splittedDictionary;
        }



        public Bitmap ExtractMatrixTile(int xOffset, int yOffset)
        {

            return null;


            //using (Graphics g = Graphics.FromImage(imageToSplit))
            //{
            //    g.DrawImage(Image, rectangle);

            //    int i = 0;
            //    for (int y = tileDataHeight / 2; y < yStep; y += tileDataHeight)
            //    {
            //        int x = i % 2 == 0 ? 0 : tileDataWidth / 2;

            //        for (; x < xStep; x += tileDataWidth)
            //        {
            //            Bitmap littleTile = new Bitmap(tileDataWidth, tileDataHeight);

            //            var points = CreatePolygon(x * tileDataWidth, y * tileDataHeight);
            //            var border = FindBorderOfPolygon(points);

            //            creare un bitmap con questi bitmap

            //        }
            //    }
            //}

            //return bitmaps;
        }

        public Point[] CreatePolygon(int offsetX, int offsetY, double scale = 1)
        {
            Point[] drawFloorPoint = new Point[4];
            drawFloorPoint[0].X = (int)Math.Round(22 * scale,0) + offsetX;
            drawFloorPoint[0].Y = 0 + offsetY;
            drawFloorPoint[1].X = (int)Math.Round(44 * scale, 0) + offsetX;
            drawFloorPoint[1].Y = (int)Math.Round(22 * scale, 0) + offsetY;
            drawFloorPoint[2].X = (int)Math.Round(22 * scale,0) + offsetX;
            drawFloorPoint[2].Y = (int)Math.Round(44 * scale, 0) + offsetY;
            drawFloorPoint[3].X = 0 + offsetX;
            drawFloorPoint[3].Y = (int)Math.Round(22 * scale, 0) + offsetY;

            return drawFloorPoint;
        }

        public Bitmap CreateBitmapFromPoints(Point[] closedPointPolygon, Color color)
        {
            int xMax = closedPointPolygon.ToList().Max(x => x.X);
            int yMax = closedPointPolygon.ToList().Max(x => x.Y);
            int xMin = closedPointPolygon.ToList().Min(x => x.X);
            int yMin = closedPointPolygon.ToList().Min(x => x.Y);

            Bitmap newBitmap = new Bitmap(xMax, yMax);

            var borderPoints = FindBorderOfPolygon(closedPointPolygon);

            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                using (System.Drawing.Brush floorBrush = new SolidBrush(color))
                {
                    for (int x = xMin; x <= xMax; x++)
                    {
                        for (int y = yMin; y <= yMax; y++)
                        {
                            if (FindIfPointsIsInConvexPolygon(borderPoints, new Point(x, y)))
                            {
                                g.FillRectangle(floorBrush, x, y, 1, 1);
                            }
                        }
                    }
                }
            }

            return newBitmap;
        }

        public bool FindIfPointsIsInConvexPolygon(Point[] closedPointPolygon, Point check)
        {
            Point[] convexBorder = FindBorderOfPolygon(closedPointPolygon);

            int xMax = closedPointPolygon.ToList().Max(x => x.X);
            int yMax = closedPointPolygon.ToList().Max(x => x.Y);
            int xMin = closedPointPolygon.ToList().Min(x => x.X);
            int yMin = closedPointPolygon.ToList().Min(x => x.Y);

            if (check.Y < yMin || check.Y > yMax || check.X < xMin || check.X > xMax)
                return false;

            List<Point> pointsAtSameY = closedPointPolygon.Where(x => x.Y == check.Y).ToList();

            if (pointsAtSameY.Count == 1 && check.X == pointsAtSameY[0].X) //Is a vertex
            {
                return true;
            }

            if (pointsAtSameY.Count > 1) //Is not a vertex
            {
                int xLeft = pointsAtSameY.ToList().Min(x => x.X);
                int xRight = pointsAtSameY.ToList().Max(x => x.X);

                if (check.X > xLeft && check.X < xRight)
                    return true;
            }

            return false;
        }


        public Point[] PointBetweenPoints(Point point1, Point point2)
        {
            double mCoeff = (double)(point1.Y - point2.Y) / (point1.X - point2.X);

            List<Point> points = new List<Point>();
            int xMin = Math.Min(point1.X, point2.X);
            int xMax = Math.Max(point1.X, point2.X);

            for (int x = xMin; x <= xMax; x++)
            {
                int y = (int)Math.Round(mCoeff * x - mCoeff * point1.X + point1.Y, 0);

                Point newPoint = new Point(x, y);

                points.Add(newPoint);
            }

            return points.ToArray();
        }


        /// <summary>
        /// Consecutive points
        /// </summary>
        /// <param name="closedPointPolygon"></param>
        /// <returns></returns>
        public Point[] FindBorderOfPolygon(Point[] closedPointPolygon)
        {
            List<Point> result = new List<Point>();
            int xMin = 0;
            int xMax = 0;
            double mCoeff = 0;

            for (int i = 0; i < closedPointPolygon.Length - 1; i++)
            {
                result.AddRange(PointBetweenPoints(closedPointPolygon[i], closedPointPolygon[i + 1]));
            }

            result.AddRange(PointBetweenPoints(closedPointPolygon[closedPointPolygon.Length - 1], closedPointPolygon[0]));

            return result.ToArray();
        }


        public void ExtractPolygonFromImage(Point[] closedPointPolygon, Bitmap initialArea)
        {

            Bitmap newBitmap = new Bitmap(initialArea.Width, initialArea.Height);
            newBitmap = ColorAllTheBitmap(newBitmap, System.Drawing.Color.Black); 

            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.Clear(Color.Black);




            }
        }

        public Bitmap ColorAllTheBitmap(Bitmap initialBitmap, System.Drawing.Color color)
        {
            for (int Xcount = 0; Xcount < initialBitmap.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < initialBitmap.Height; Ycount++)
                {
                    initialBitmap.SetPixel(Xcount, Ycount, color);
                }
            }

            return initialBitmap;
        }


        public static Bitmap GetTileDataBitmap(System.Drawing.Color color)
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
