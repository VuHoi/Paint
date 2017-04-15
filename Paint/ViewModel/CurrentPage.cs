using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Windows.Controls.Ribbon;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Controls.Primitives;

namespace Paint.ViewModel
{
    class CurrentPage
    {
        public static CurrentPage Instance = new CurrentPage();
        public CurrentPage()
        { }
        public int STT = 0;
        public List<System.Windows.Controls.Image> StkShape = new List<System.Windows.Controls.Image>();
        public void DrawCapture(Shape shape, Canvas canvas)
        {
            double[] dashes = { 2, 2 };
            shape.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            canvas.Children.Add(shape);
        }
        public void DrawShape(ContentControl control, int outline, Canvas canvas)
        {

            canvas.Children.RemoveAt(canvas.Children.Count - 1);
            RefreshCanvas(canvas);
            if (outline == 1)
            {
                ((Shape)control.Content).StrokeDashArray = null;
            }
            else if (outline == 2)
            {
                double[] dashes = { 4, 4 };
                ((Shape)control.Content).StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
            else
            {
                double[] dashes = { 4, 1, 4, 1 };
                ((Shape)control.Content).StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }

            canvas.Children.Add(control);


        }

        public void RefreshCanvas(Canvas canvas)
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Width = canvas.ActualWidth;
            img.Height = canvas.ActualHeight;
            img.Source = BitmapToImageSource(CanvasToBitmap(canvas));
            if (STT <= 10)
            {
                StkShape.Add(img);
                STT++;
            }
            canvas.Children.Clear();

            canvas.Children.Add(img);
        }
        private ImageSource BitmapToImageSource(Bitmap bm)
        {
            System.Windows.Media.Imaging.BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bm.Width, bm.Height));
            return b;
        }

        public Bitmap CanvasToBitmap(Canvas cv)
        {
            Bitmap bm;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(cv);
            double dpi = 96d;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);


            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(cv);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            renderBitmap.Render(dv);

            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
            encoder.Save(stream);
            bm = new System.Drawing.Bitmap(stream);
            return bm;
        }

        public void DrawShape(Shape shape, int outline, Canvas canvas)
        {
            RefreshCanvas(canvas);
            if (outline == 1)
            {
                shape.StrokeDashArray = null;
            }
            else if (outline == 2)
            {
                double[] dashes = { 4, 4 };
                shape.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }
            else
            {
                double[] dashes = { 4, 1, 4, 1 };
                shape.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
            }

            canvas.Children.Add(shape);

        }
        private bool SameColor(System.Drawing.Color c1, System.Drawing.Color c2)
        {
            return ((c1.A == c2.A) && (c1.B == c2.B) && (c1.G == c2.G) && (c1.R == c2.R));
        }
        public void FloodFill(System.Drawing.Bitmap bm, System.Drawing.Point p, System.Drawing.Color Color, Canvas canvas)
        {
            Stack<System.Drawing.Point> S = new Stack<System.Drawing.Point>();
            System.Drawing.Color OriColor = bm.GetPixel(p.X, p.Y);
            bm.SetPixel(p.X, p.Y, Color);
            S.Push(p);
            while (S.Count != 0)
            {
                p = S.Pop();
                if ((p.X - 1 >= 0) && SameColor(OriColor, bm.GetPixel(p.X - 1, p.Y)))
                {
                    bm.SetPixel(p.X - 1, p.Y, Color);
                    S.Push(new System.Drawing.Point(p.X - 1, p.Y));
                }
                if ((p.X + 1 < bm.Width) && SameColor(OriColor, bm.GetPixel(p.X + 1, p.Y)))
                {
                    bm.SetPixel(p.X + 1, p.Y, Color);
                    S.Push(new System.Drawing.Point(p.X + 1, p.Y));
                }
                if ((p.Y - 1 >= 0) && SameColor(OriColor, bm.GetPixel(p.X, p.Y - 1)))
                {
                    bm.SetPixel(p.X, p.Y - 1, Color);
                    S.Push(new System.Drawing.Point(p.X, p.Y - 1));
                }
                if ((p.Y + 1 < bm.Height) && SameColor(OriColor, bm.GetPixel(p.X, p.Y + 1)))
                {
                    bm.SetPixel(p.X, p.Y + 1, Color);
                    S.Push(new System.Drawing.Point(p.X, p.Y + 1));
                }
            }
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Width = canvas.ActualWidth;
            img.Height = canvas.ActualHeight;
            img.Source = BitmapToImageSource(bm);
            if (STT <= 10)
            {
                StkShape.Add(img);
                STT++;
            }
            canvas.Children.Clear();
            canvas.Children.Add(img);
        }

    }
}
