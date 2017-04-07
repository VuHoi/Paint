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

namespace Paint.ViewModel
{
   public class MainWindowViewModel:ViewModelBase
    {
        Point currentPoint = new Point();
        private ICommand _Canvas_MouseDown;
        private bool isCanvas_MouseDown = false;
        private bool isCanvas_MouseMove = false;
        private bool isCanvas_MouseUp = false;
        private ICommand _Canvas_MouseMove;
        private ICommand _Canvas_MouseUp;

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

                line.Stroke = new SolidColorBrush(Colors.Black);
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

        private void OnCanvas_MouseDown(Canvas obj)
        {
            currentPoint= Mouse.GetPosition(obj);
            isCanvas_MouseDown = true;
            //if (e.ButtonState == MouseButtonState.Pressed)
            //    currentPoint = e.GetPosition((IInputElement)(e.Source));
        }
    }
}
