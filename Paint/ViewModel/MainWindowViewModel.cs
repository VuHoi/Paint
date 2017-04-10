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

namespace Paint.ViewModel
{

    enum DrawType { pencil, brush, line, ellipse, rectangle, triangle, arrow, heart, fill, erase, text,bucket };
    public class MainWindowViewModel:Control, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private Line curLine;
        private Shape curShape;
        private int Outline = 1;
        private int STT = 0;
        private int Isdelete = 0;
        private ContentControl curControl;
        private bool IsShape = false;
        private bool IsColor1 = true;
        private bool IsCheckFill=false;
        private bool isCanvas_MouseDown = false;
        System.Windows.Point CurrentPointDown = new System.Windows.Point();
        System.Windows.Point CurrentPointMove = new System.Windows.Point();
        private ICommand _Canvas_MouseDown;
        private System.Windows.Media.Brush _color1 =new SolidColorBrush(Colors.Black), _color2=new SolidColorBrush(Colors.White);
        private System.Windows.Media.Brush _colorFill;
        private int StrokeThickness = 1;
        private bool isItemMenu = false;
        private List<System.Windows.Controls.Image> StkShape = new List<System.Windows.Controls.Image>();
        private ICommand _Canvas_MouseMove;
        private ICommand _Canvas_MouseUp;
        private ICommand _StrokeThicknessCommand;
        private ICommand _StrokeCommand;
        private ICommand _ChooseColor1Command;
        private ICommand _ChooseColor2Command;
        private ICommand _ColorCommand;
        private ICommand _LineCommand;
        private ICommand _BrushCommand;
        private ICommand _EraserCommand;
        private ICommand _PenCommand;
        private ICommand _SmoothCommand;
        private ICommand _DarkCommand;
        private ICommand _MixCommand;
        private ICommand _RectangleCommand;
        private ICommand _TriangleCommand;
        private ICommand _ArrowCommand;
        private ICommand _CircleCommand;
        private ICommand _HeartCommand;
        private ICommand _FillCommand;
        private ICommand _BucketCommand;
        private ICommand _UndoCommand;
        private ICommand _RedoCommand;
        public MainWindowViewModel()
        {
            ColorFill = Color1;
            drawType = DrawType.brush;
        }
        public ICommand Canvas_MouseUp
        {
            get
            {
                _Canvas_MouseUp = new RelayCommand<Canvas>((p) => true, OnCanvas_MouseUp);
                return _Canvas_MouseUp;
            }

            set
            {
                _Canvas_MouseUp = value;
            }
        }

        private void OnCanvas_MouseUp(Canvas canvas)
        {
            if (STT < StkShape.Count - 1)
            {
                StkShape.RemoveRange(STT, StkShape.Count - (STT));
            }
            isCanvas_MouseDown = false;
            if (drawType == DrawType.line)
            {
                 curControl = new ContentControl();
                Line Line = new Line();

                Line.X1 = CurrentPointDown.X;
                Line.Y1 = CurrentPointDown.Y;
                Line.X2 = CurrentPointMove.X;
                Line.Y2 = CurrentPointMove.Y;

                Line.StrokeThickness = StrokeThickness;
                Line.Stroke = Color1;
                Canvas.SetLeft(curControl, Line.Margin.Left);
                Canvas.SetTop(curControl, Line.Margin.Top);
                curControl.Width = Line.Width;
                curControl.Height = Line.Height;
                curControl.Content = Line;
                curControl.Background = Color1;
                DrawShape(curControl, Outline, canvas);
               
                curLine = null;
            }
            else if (drawType == DrawType.ellipse || drawType == DrawType.rectangle || drawType == DrawType.triangle || drawType == DrawType.arrow || drawType == DrawType.heart)
            {
                curControl = new ContentControl();
                Shape temp;
                if (drawType == DrawType.ellipse)
                {
                    temp = new Ellipse();
                 
                }
                else if (drawType == DrawType.rectangle)
                {
                    temp = new System.Windows.Shapes.Rectangle();
                }
                else if (drawType == DrawType.triangle)
                {
                    temp = new Triangle();
                    ((Triangle)temp).Start = ((Triangle)curShape).Start;
                    temp.Width = curShape.Width;
                    temp.Height = curShape.Height;
                }
                else if (drawType == DrawType.arrow)
                {
                    temp = new Arrow();
                    ((Arrow)temp).Start = ((Arrow)curShape).Start;
                    temp.Width = curShape.Width;
                    temp.Height = curShape.Height;
                }
                else
                {
                    temp = new Heart();
                    ((Heart)temp).Start = ((Heart)curShape).Start;
                    temp.Width = curShape.Width;
                    temp.Height = curShape.Height;
                }
                temp.Stroke = Color1;
                temp.StrokeThickness = StrokeThickness;
                temp.IsHitTestVisible = true;
                if(IsCheckFill)temp.Fill = ColorFill;
                Canvas.SetLeft(curControl, curShape.Margin.Left);
                Canvas.SetTop(curControl, curShape.Margin.Top);
                curControl.Width = curShape.Width;
                curControl.Height = curShape.Height;
                curControl.Content = temp;
                curControl.Background = Color1;
                DrawShape(curControl, Outline, canvas);
               

                curShape = null;
            }
            else if(drawType==DrawType.bucket)
            {
                System.Drawing.Color color = new System.Drawing.Color();
                color = System.Drawing.Color.FromArgb(((System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty))).A,
                    ((System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty))).R,
                    ((System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty))).G,
                    ((System.Windows.Media.Color)(Color1.GetValue(SolidColorBrush.ColorProperty))).B);
                Bitmap bm = CanvasToBitmap(canvas);
                FloodFill(bm, new System.Drawing.Point((int)Mouse.GetPosition(canvas).X, (int)Mouse.GetPosition(canvas).Y), color, canvas);
               
               
            }
            else
            {
                
                RefreshCanvas(canvas);
            }
           
        }

        public ICommand Canvas_MouseMove
        {
            get
            {
                _Canvas_MouseMove = new RelayCommand<Canvas>((p) => true, OnCanvas_MouseMove);
                return _Canvas_MouseMove;
            }

            set
            {
                _Canvas_MouseMove = value;
            }
        }

        private void OnCanvas_MouseMove(Canvas canvas)
        {
            CurrentPointMove = Mouse.GetPosition(canvas);
            bool addShape = false;
            if (isCanvas_MouseDown == true)
            {

                if ((drawType == DrawType.ellipse || drawType == DrawType.rectangle || drawType == DrawType.triangle || drawType == DrawType.arrow || drawType == DrawType.heart) && IsShape)
                {

                    if (curShape == null)
                    {

                        if (drawType == DrawType.ellipse)
                        {
                            curShape = new Ellipse();
                        }
                        else if (drawType == DrawType.rectangle)
                        {
                            curShape = new System.Windows.Shapes.Rectangle();
                        }
                        else if (drawType == DrawType.triangle)
                        {
                            curShape = new Triangle();
                            ((Triangle)curShape).Start = CurrentPointDown;
                        }
                        else if (drawType == DrawType.arrow)
                        {

                            curShape = new Arrow();
                            ((Arrow)curShape).Start = CurrentPointDown;
                        }
                        else
                        {
                            curShape = new Heart();
                            ((Heart)curShape).Start = CurrentPointDown;
                        }
                        addShape = true;
                        curShape.StrokeThickness = StrokeThickness;
                        curShape.Stroke = Color1;
                    }

                    if (CurrentPointMove.X <= CurrentPointDown.X && CurrentPointMove.Y <= CurrentPointDown.Y)  //Góc phần tư thứ nhất
                    {
                        curShape.Margin = new Thickness(CurrentPointMove.X, CurrentPointMove.Y, 0, 0);
                    }
                    else if (CurrentPointMove.X >= CurrentPointDown.X && CurrentPointMove.Y <= CurrentPointDown.Y)
                    {
                        curShape.Margin = new Thickness(CurrentPointDown.X, CurrentPointMove.Y, 0, 0);
                    }
                    else if (CurrentPointMove.X >= CurrentPointDown.X && CurrentPointMove.Y >= CurrentPointDown.Y)
                    {
                        curShape.Margin = new Thickness(CurrentPointDown.X, CurrentPointDown.Y, 0, 0);
                    }
                    else if (CurrentPointMove.X <= CurrentPointDown.X && CurrentPointMove.Y >= CurrentPointDown.Y)
                    {
                        curShape.Margin = new Thickness(CurrentPointDown.X, CurrentPointDown.Y, 0, 0);
                    }
                    if (IsCheckFill) curShape.Fill = ColorFill;
                    curShape.Width = Math.Abs(CurrentPointMove.X - CurrentPointDown.X);
                    curShape.Height = Math.Abs(CurrentPointMove.Y - CurrentPointDown.Y);
                    
                  
                    if(addShape)
                    DrawCapture(curShape, canvas);
                }else
                    if (drawType == DrawType.brush || drawType == DrawType.erase || drawType == DrawType.pencil)
                    {
                        Line line = new Line();
                        line.Stroke = Color1;

                        if (drawType == DrawType.erase)
                        {
                            line.Stroke = Color2;
                            StrokeThickness = 15;
                        }
                        else if (drawType == DrawType.brush && !isItemMenu)
                        {
                            StrokeThickness = 3;
                        }
                        else if (!isItemMenu)
                            StrokeThickness = 1;
                        line.StrokeThickness = StrokeThickness;
                        line.X1 = CurrentPointDown.X;
                        line.Y1 = CurrentPointDown.Y;
                        line.X2 = Mouse.GetPosition(canvas).X;
                        line.Y2 = Mouse.GetPosition(canvas).Y;


                        CurrentPointDown = Mouse.GetPosition(canvas);
                        canvas.Children.Add(line);


                    }
                    else if (drawType == DrawType.line)
                    {
                        if (curLine == null)
                        {
                            curLine = new Line();
                            addShape = true;
                        }
                        curLine.X1 = CurrentPointDown.X;
                        curLine.Y1 = CurrentPointDown.Y;
                        curLine.X2 = CurrentPointMove.X;
                        curLine.Y2 = CurrentPointMove.Y;

                        curLine.StrokeThickness = StrokeThickness;
                        curLine.Stroke = Color1;

                        if (addShape)
                        {
                            DrawCapture(curLine, canvas);
                        }
                    }
                }
            
        }
        public ICommand Canvas_MouseDown
        {
            get
            {
                _Canvas_MouseDown = new RelayCommand<Canvas>((p) => true, OnCanvas_MouseDown);
                return _Canvas_MouseDown;
            }
            set
            {
                _Canvas_MouseDown = value;
            }
        }
        private void OnCanvas_MouseDown(Canvas canvas)
        {
            CurrentPointDown = Mouse.GetPosition(canvas);
            isCanvas_MouseDown = true;

         
                
            
        }
        public System.Windows.Media.Brush Color1
        {
            get
            {
                return _color1;
            }

            set
            {
                _color1 = value; NotifyPropertyChanged("Color1");
            }
        }

        public System.Windows.Media.Brush Color2
        {
            get
            {
                return _color2;
            }

            set
            {
                _color2 = value;NotifyPropertyChanged("Color2");
            }
        }

        public ICommand StrokeThicknessCommand
        {
            get
            {
                _StrokeThicknessCommand = new RelayCommand<RibbonMenuItem>((p) => true, OnStrokeThicknessCommand);
                return _StrokeThicknessCommand;
            }

            set
            {
                _StrokeThicknessCommand = value;
            }
        }

        public ICommand StrokeCommand
        {
            get
            {
                _StrokeCommand = new RelayCommand<RibbonButton>((p) => true, OnStrokeCommand);
                return _StrokeCommand;
            }

            set
            {
                _StrokeCommand = value;
            }
        }

        public ICommand ChooseColor1Command
        {
            get
            {
                _ChooseColor1Command = new RelayCommand<object>((p) => true, OnChooseColor1Command);
                return _ChooseColor1Command;
            }

            set
            {
                _ChooseColor1Command = value;
            }
        }

        public ICommand ChooseColor2Command
        {
            get
            {
                _ChooseColor2Command = new RelayCommand<object>((p) => true, OnChooseColor2Command);
                return _ChooseColor2Command;
            }

            set
            {
                _ChooseColor2Command = value;
            }
        }

        public ICommand ColorCommand
        {
            get
            {
                _ColorCommand = new RelayCommand<RibbonButton>((p) => true, OnColorCommand);


                return _ColorCommand;
            }

            set
            {
                _ColorCommand = value;
            }
        }

        public ICommand LineCommand
        {
            get
            {
                _LineCommand = new RelayCommand<Canvas>((p) => true, OnLineCommand);
                return _LineCommand;
            }

            set
            {
                _LineCommand = value;
            }
        }

        public ICommand BrushCommand
        {
            get
            {
                _BrushCommand = new RelayCommand<Canvas>((p) => true, OnBrushCommand);
                return _BrushCommand;
            }

            set
            {
                _BrushCommand = value;
            }
        }

        public ICommand EraserCommand
        {
            get
            {
                _EraserCommand = new RelayCommand<Canvas>((p) => true, OnEraserCommand);
                return _EraserCommand;
            }

            set
            {
                _EraserCommand = value;
            }
        }

        public ICommand PenCommand
        {
            get
            {
                _PenCommand = new RelayCommand<Canvas>((p) => true, OnPenCommand);
                return _PenCommand;
            }

            set
            {
                _PenCommand = value;
            }
        }
        
        public Canvas Canvas
        {
            get { return (Canvas)GetValue(MainWindowViewModel.CanvasProperty); }
            set { SetValue(MainWindowViewModel.CanvasProperty, value); }
        }

        public ICommand SmoothCommand
        {
            get
            {
                _SmoothCommand = new RelayCommand<object>((p) => true, OnSmoothCommand);
                return _SmoothCommand;
            }

            set
            {
                _SmoothCommand = value;
            }
        }

        private void OnSmoothCommand(object obj)
        {
            Outline = 1;
        }

        public ICommand DarkCommand
        {
            get
            {
                _DarkCommand = new RelayCommand<object>((p) => true, OnDarkCommand);
                return _DarkCommand;
            }

            set
            {
                _DarkCommand = value;
            }
        }

        private void OnDarkCommand(object obj)
        {
            Outline = 2;
        }

        public ICommand MixCommand
        {
            get
            {
                _MixCommand = new RelayCommand<object>((p) => true, OnMixCommand);
                return _MixCommand;
            }

            set
            {
                _MixCommand = value;
            }
        }

        public ICommand RectangleCommand
        {
            get
            {
                _RectangleCommand = new RelayCommand<Canvas>((p) => true, OnRectangleCommand);
                return _RectangleCommand;
            }

            set
            {
                _RectangleCommand = value;
            }
        }

        public ICommand TriangleCommand
        {
            get
            {
                _TriangleCommand = new RelayCommand<Canvas>((p) => true, OnTriangleCommand);
                return _TriangleCommand;
            }

            set
            {
                _TriangleCommand = value;
            }
        }

        public ICommand ArrowCommand
        {
            get
            {
                _ArrowCommand = new RelayCommand<Canvas>((p) => true, OnArrowCommand);
                return _ArrowCommand;
            }

            set
            {
                _ArrowCommand = value;
            }
        }

        public ICommand CircleCommand
        {
            get
            {
                _CircleCommand = new RelayCommand<Canvas>((p) => true, OnCircleCommand);
                return _CircleCommand;
            }

            set
            {
                _CircleCommand = value;
            }
        }

        public ICommand HeartCommand
        {
            get
            {
                _HeartCommand = new RelayCommand<Canvas>((p) => true, OnHeartCommand);
                return _HeartCommand;
            }

            set
            {
                _HeartCommand = value;
            }
        }

        public System.Windows.Media.Brush ColorFill
        {
            get
            {
                return _colorFill;
            }
            set
            {
                _colorFill = value;NotifyPropertyChanged("ColorFill");
            }
        }

        public ICommand FillCommand
        {
            get
            {
                _FillCommand = new RelayCommand<RibbonCheckBox>((p) => true, OnFillCommand);
                return _FillCommand;
            }

            set
            {
                _FillCommand = value;
            }
        }

        public ICommand BucketCommand
        {
            get
            {
                _BucketCommand = new RelayCommand<Canvas>((p) => true, OnBucketCommand);
                return _BucketCommand;
            }

            set
            {
                _BucketCommand = value;
            }
        }

        public ICommand UndoCommand
        {
            get
            {
                _UndoCommand = new RelayCommand<Canvas>((p) => true, OnUndoCommand);
                return _UndoCommand;
            }

            set
            {
                _UndoCommand = value;
            }
        }

        public ICommand RedoCommand
        {
            get
            {
                _RedoCommand = new RelayCommand<Canvas>((p) => true, OnRedoCommand);
                return _RedoCommand;
            }

            set
            {
                _RedoCommand = value;
            }
        }

        private void OnRedoCommand(Canvas canvas)
        {

            if (STT < StkShape.Count)
            {
                STT++;
                canvas.Children.Clear();
                canvas.Children.Add(StkShape[STT - 1]);
            }

        }

        private void OnUndoCommand(Canvas canvas)
        {
            if(STT==StkShape.Count&& Isdelete==0)
            {
                canvas.Children.Remove(curControl);
                Isdelete = 1;
              
            }
           else if (STT - 1 > 0 && Isdelete==1)
            {
                STT--;
                canvas.Children.Clear();
                canvas.Children.Add(StkShape[STT - 1]);
            } else if(STT-1==0)
            {
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Width = canvas.ActualWidth;
                img.Height = canvas.ActualHeight;
                canvas.Children.Clear();
                canvas.Children.Add(img);
            }
        }

        private void OnBucketCommand(Canvas canvas)
        {
            drawType = DrawType.bucket;
            canvas.Cursor = Cursors.Arrow;
        }

        private void OnFillCommand(RibbonCheckBox cbk)
        {
            if (cbk.IsChecked == true)
            {
                IsCheckFill = true;
            }
            else IsCheckFill=false;
        }

        private void OnHeartCommand(Canvas obj)
        {
            drawType = DrawType.heart;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnCircleCommand(Canvas obj)
        {
            drawType = DrawType.ellipse;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnArrowCommand(Canvas obj)
        {
            drawType = DrawType.arrow;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnTriangleCommand(Canvas obj)
        {
            drawType = DrawType.triangle;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnRectangleCommand(Canvas obj)
        {
            drawType = DrawType.rectangle;
            obj.Cursor = Cursors.Cross;
            IsShape = true;
        }

        private void OnMixCommand(object obj)
        {
            Outline = 3;
        }

        public static readonly DependencyProperty CanvasProperty =
            DependencyProperty.Register("Canvas", typeof(Canvas), typeof(MainWindowViewModel));


        private void OnPenCommand(Canvas obj)
        {
            isItemMenu = false;
            drawType = DrawType.pencil;
            obj.Cursor= Cursors.Pen;
        }

        private void OnEraserCommand(Canvas obj)
        {
            isItemMenu = false;
            drawType = DrawType.erase;
            obj.Cursor = Cursors.Arrow;
        }

        private void OnBrushCommand(Canvas obj)
        {
            isItemMenu = false;
            drawType = DrawType.brush;
            obj.Cursor = Cursors.Pen;
        }

        private void OnLineCommand(Canvas obj)
        {
            isItemMenu = false;
            drawType = DrawType.line;
            obj.Cursor = Cursors.Cross;
        }

        private void OnColorCommand(RibbonButton canvas)
        {
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            dlg.AllowFullOpen = true;
            dlg.ShowDialog();
            System.Windows.Media.Color color = new System.Windows.Media.Color();
            color.A = dlg.Color.A;
            color.R = dlg.Color.R;
            color.G = dlg.Color.G;
            color.B = dlg.Color.B;
            if (IsColor1)
                Color1 = new SolidColorBrush(color);
            else Color2 = new SolidColorBrush(color);
            if (IsCheckFill) ColorFill = Color1;
            else ColorFill = Color2;

        }

        private void OnChooseColor2Command(object obj)
        {
            IsColor1 = false;
        }

        private void OnChooseColor1Command(object obj)
        {
            IsColor1 = true;
        }

        private void OnStrokeCommand(RibbonButton canvas)
        {
           if(IsColor1)
            {
                Color1 = canvas.Background;
            }
           else
            {
                Color2 = canvas.Background;
            }
            if (IsCheckFill) ColorFill = Color1;
            else ColorFill = Color2;
        }

        private void OnStrokeThicknessCommand(RibbonMenuItem canvas)
        {
            isItemMenu = true;
            if (canvas.Header.Equals("1px"))
            {
                StrokeThickness = 1;
            }
            else if (canvas.Header.Equals("3px"))
            {
                StrokeThickness = 3;
            }
            else if (canvas.Header.Equals("5px"))
            {
                StrokeThickness = 5;
            }
            else if (canvas.Header.Equals("10px"))
            {
                StrokeThickness = 10;
            }
        }

        private DrawType drawType;
       
        public void DrawCapture(Shape shape,Canvas canvas)
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

        public void RefreshCanvas( Canvas canvas)
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

        public void DrawShape(Shape shape, int outline,Canvas canvas)
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
        public void FloodFill(System.Drawing.Bitmap bm, System.Drawing.Point p, System.Drawing.Color Color,Canvas canvas)
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
            canvas.Children.Clear();
            canvas.Children.Add(img);
        }
        public void RemoveShape(ContentControl shape,Canvas canvas)
        {
            canvas.Children.Remove(shape);
        }
    }
}
