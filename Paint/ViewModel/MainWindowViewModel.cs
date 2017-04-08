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

namespace Paint.ViewModel
{
   public class MainWindowViewModel:ViewModelBase
    {
        private Line curLine;
        private Canvas canvas;
        private Document doc;
        private bool IsColor1 = true;
        private bool isCanvas_MouseDown = false;
        Point CurrentPointDown = new Point();
        Point CurrentPointMove = new Point();
        private ICommand _Canvas_MouseDown;
        private Brush _color1=new SolidColorBrush(Colors.Black), _color2=new SolidColorBrush(Colors.White);
        private int StrokeThickness = 1;
        private bool isItemMenu = false;
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
        public MainWindowViewModel()
        {
            doc = new Document(canvas);
            doc.drawType = DrawType.brush;
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
            isCanvas_MouseDown = false;
            if (doc.drawType == DrawType.line)
            {

               Line Line = new Line();

                Line.X1 = CurrentPointDown.X;
                Line.Y1 = CurrentPointDown.Y;
                Line.X2 = CurrentPointMove.X;
                Line.Y2 = CurrentPointMove.Y;

                Line.StrokeThickness = StrokeThickness;
                Line.Stroke = Color1;
                canvas.Children.Add(Line);
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
                if (doc.drawType == DrawType.brush|| doc.drawType == DrawType.erase|| doc.drawType == DrawType.pencil)
                {
                    Line line = new Line();
                    line.Stroke = Color1;

                    if (doc.drawType == DrawType.erase)
                    {
                        line.Stroke = Color2;
                        StrokeThickness = 15;
                    }
                    else if (doc.drawType == DrawType.brush && !isItemMenu)
                    {
                        StrokeThickness = 3;
                    }
                    else if(!isItemMenu)
                        StrokeThickness = 1;
                        line.StrokeThickness = StrokeThickness;
                        
                    
                    line.X1 = CurrentPointDown.X;
                    line.Y1 = CurrentPointDown.Y;
                    line.X2 = Mouse.GetPosition(canvas).X;
                    line.Y2 = Mouse.GetPosition(canvas).Y;
                  

                    CurrentPointDown = Mouse.GetPosition(canvas);

                    canvas.Children.Add(line);
                }
                else if(doc.drawType==DrawType.line)
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
                    double[] dashes = { 2, 2 };
                    curLine.StrokeDashArray = new System.Windows.Media.DoubleCollection(dashes);
                    if (addShape)
                    {
                        canvas.Children.Add(curLine);
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
        public Brush Color1
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

        public Brush Color2
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
                _LineCommand = new RelayCommand<object>((p) => true, OnLineCommand);
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
                _BrushCommand = new RelayCommand<object>((p) => true, OnBrushCommand);
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
                _EraserCommand = new RelayCommand<object>((p) => true, OnEraserCommand);
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
                _PenCommand = new RelayCommand<object>((p) => true, OnPenCommand);
                return _PenCommand;
            }

            set
            {
                _PenCommand = value;
            }
        }

        private void OnPenCommand(object obj)
        {
            isItemMenu = false;
            doc.drawType = DrawType.pencil;
        }

        private void OnEraserCommand(object obj)
        {
            isItemMenu = false;
            doc.drawType = DrawType.erase;
        }

        private void OnBrushCommand(object obj)
        {
            isItemMenu = false;
            doc.drawType = DrawType.brush;
        }

        private void OnLineCommand(object obj)
        {
            isItemMenu = false;
            doc.drawType = DrawType.line;
        }

        private void OnColorCommand(RibbonButton canvas)
        {
            System.Windows.Forms.ColorDialog dlg = new System.Windows.Forms.ColorDialog();
            dlg.AllowFullOpen = true;
            dlg.ShowDialog();
            Color color = new Color();
            color.A = dlg.Color.A;
            color.R = dlg.Color.R;
            color.G = dlg.Color.G;
            color.B = dlg.Color.B;
            if (IsColor1)
                Color1 = new SolidColorBrush(color);
            else Color2 = new SolidColorBrush(color);


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
                Color1 = (Brush)(canvas.Background);
            }
           else
            {
                Color2 = (Brush)(canvas.Background);
            }
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

        
    }
}
