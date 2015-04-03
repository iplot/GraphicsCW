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
using System.Windows.Shapes;

namespace GraphicsCW
{
    /// <summary>
    /// Логика взаимодействия для VerticalAngle.xaml
    /// </summary>
    public partial class VerticalAngle : Window
    {
        MainWindow collection;

        public VerticalAngle()
        {
            InitializeComponent();
        }

        private void verticalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (verticalText != null)
            {
                verticalText.Text = verticalSlider.Value.ToString();
            }
        }

        public void setCollection(MainWindow collection)
        {
            this.collection = collection;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            int angle = Convert.ToInt32(verticalText.Text);

            collection.setVerticalAngle(angle);

            this.Close();
        }
    }
}
