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
        private bool IsColor1 = true;
        private bool isCanvas_MouseDown = false;
        Point currentPoint = new Point();
        private ICommand _Canvas_MouseDown;
        private Brush _color1=new SolidColorBrush(Colors.Black), _color2=new SolidColorBrush(Colors.White);
        private int StrokeThickness = 3;
        private ICommand _Canvas_MouseMove;
        private ICommand _Canvas_MouseUp;
        private ICommand _StrokeThicknessCommand;
        private ICommand _StrokeCommand;
        private ICommand _ChooseColor1Command;
        private ICommand _ChooseColor2Command;
        private ICommand _ColorCommand;
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

        private void OnCanvas_MouseUp(Canvas obj)
        {
            isCanvas_MouseDown = false;
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

        private void OnCanvas_MouseMove(Canvas obj)
        {
            if (isCanvas_MouseDown == true)
            {
                Line line = new Line();

                line.Stroke = Color1;
                line.StrokeThickness = StrokeThickness;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = Mouse.GetPosition(obj).X;
                line.Y2 = Mouse.GetPosition(obj).Y;

                currentPoint = Mouse.GetPosition(obj);
               
                obj.Children.Add(line);
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

        private void OnColorCommand(RibbonButton obj)
        {
            
            
          
        }

        private void OnChooseColor2Command(object obj)
        {
            IsColor1 = false;
        }

        private void OnChooseColor1Command(object obj)
        {
            IsColor1 = true;
        }

        private void OnStrokeCommand(RibbonButton obj)
        {
           if(IsColor1)
            {
                Color1 = (Brush)(obj.Background);
            }
           else
            {
                Color2 = (Brush)(obj.Background);
            }
        }

        private void OnStrokeThicknessCommand(RibbonMenuItem obj)
        {
            if (obj.Header.Equals("1px"))
            {
                StrokeThickness = 1;
            }
            else if (obj.Header.Equals("3px"))
            {
                StrokeThickness = 3;
            }
            else if (obj.Header.Equals("5px"))
            {
                StrokeThickness = 5;
            }
            else if (obj.Header.Equals("10px"))
            {
                StrokeThickness = 10;
            }
        }

        private void OnCanvas_MouseDown(Canvas obj)
        {
            currentPoint= Mouse.GetPosition(obj);
            isCanvas_MouseDown = true;
        }
    }
}
