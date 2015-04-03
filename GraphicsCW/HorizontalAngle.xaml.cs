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
    /// Логика взаимодействия для HorizontalAngle.xaml
    /// </summary>
    public partial class HorizontalAngle : Window
    {
        MainWindow collection;

        public HorizontalAngle()
        {
            InitializeComponent();
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (horizontalText != null)
            {
                horizontalText.Text = horizontalSlider.Value.ToString();
            }
        }

        public void setCollection(MainWindow collection)
        {
            this.collection = collection;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            int angle = Convert.ToInt32(horizontalText.Text);

            collection.setHorizontAngle(angle);

            this.Close();
        }


    }
}
