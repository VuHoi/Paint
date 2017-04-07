using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           
        }
        Point currentPoint = new Point();
        //private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ButtonState == MouseButtonState.Pressed)
        //        currentPoint = e.GetPosition((IInputElement)(e.Source));
        //}

        //private void Canvas_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        Line line = new Line();

        //        line.Stroke = new SolidColorBrush(Colors.Black);
        //        line.X1 = currentPoint.X;
        //        line.Y1 = currentPoint.Y;
        //        line.X2 = e.GetPosition((IInputElement)(e.Source)).X;
        //        line.Y2 = e.GetPosition((IInputElement)(e.Source)).Y;

        //        currentPoint = e.GetPosition((IInputElement)(e.Source));

        //        Canvas.Children.Add(line);

        //    }
        //}
    }
}
