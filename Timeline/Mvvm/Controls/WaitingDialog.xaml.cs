using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShiningMeeting.Mvvm.Controls
{
    /// <summary>
    /// Interaction logic for WaitingDialog.xaml
    /// </summary>
    public partial class WaitingDialog : UserControl
    {
        public WaitingDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }



        public int Circle
        {
            get { return (int)GetValue(CircleProperty); }
            set { SetValue(CircleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Circle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CircleProperty =
            DependencyProperty.Register("Circle", typeof(int), typeof(WaitingDialog), new UIPropertyMetadata(25));


        public int GeometryRadiusX
        {
            get { return (int)GetValue(GeometryRadiusXProperty); }
            set { SetValue(GeometryRadiusXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GeometryRadiusX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GeometryRadiusXProperty =
            DependencyProperty.Register("GeometryRadiusX", typeof(int), typeof(WaitingDialog), new UIPropertyMetadata(1));



        public int GeometryRadiusY
        {
            get { return (int)GetValue(GeometryRadiusYProperty); }
            set { SetValue(GeometryRadiusYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GeometryRadiusY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GeometryRadiusYProperty =
            DependencyProperty.Register("GeometryRadiusY", typeof(int), typeof(WaitingDialog), new UIPropertyMetadata(3));



    }
}
