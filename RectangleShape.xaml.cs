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

namespace SimpleFlowChart
{
    /// <summary>
    /// Interaction logic for RectangleShape.xaml
    /// </summary>
    public partial class RectangleShape : UserControl
    {
        public RectangleShape(string text)
        {
            InitializeComponent();
            ShapeText = text;
        }

        public string ShapeText
        {
            get { return TextBlock.Text; }
            set { TextBlock.Text = value; }
        }
    }
}
