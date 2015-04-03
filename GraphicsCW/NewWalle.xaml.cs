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
    /// Логика взаимодействия для NewWalle.xaml
    /// </summary>
    public partial class NewWalle : Window
    {
        public bool isClose = true;

        public List<System.Drawing.Color> colors;
        public WallesParams param;
        public System.Drawing.Point baseP;
        public int depth;

        public MainWindow main;

        public int index;

        public NewWalle()
        {
            InitializeComponent();

            index = -1;
        }

        public void setCollect(MainWindow collect)
        {
            main = collect;
        }

        public void setWalleParam(Walle wal, int ind, int width, int height)
        {
            this.index = ind;

            housingLengthSlider.Value = wal.Params.housingLength;
            handsLengthSlider.Value = wal.Params.handsLength;
            handsWidthSlider.Value = wal.Params.handsWidth;
            trunkWidthSlider.Value = wal.Params.trunkWidth;
            trunkHeightSlider.Value = wal.Params.trunkHeight;
            neckWidthSlider.Value = wal.Params.neckWidth;
            neckHeightSlider.Value = wal.Params.neckHeight;
            eyeHeightSlider.Value = wal.Params.eyeCylinderHeight;
            trackRadiusSlider.Value = wal.Params.trackCylinderRad;

            panellsNum.Text = wal.Params.panelsNumber.ToString();

            housingColorText.Text = System.Drawing.ColorTranslator.ToHtml(wal.Colors[0]);
            handsColorText.Text = System.Drawing.ColorTranslator.ToHtml(wal.Colors[1]);
            trunkColorText.Text = System.Drawing.ColorTranslator.ToHtml(wal.Colors[2]);
            neckColorText.Text = System.Drawing.ColorTranslator.ToHtml(wal.Colors[3]);
            eyeColorText.Text = System.Drawing.ColorTranslator.ToHtml(wal.Colors[4]);
            trackColorText.Text = System.Drawing.ColorTranslator.ToHtml(wal.Colors[5]);
            panellsColorText.Text = System.Drawing.ColorTranslator.ToHtml(wal.Colors[6]);

            baseX.Text = (wal.BasePoint.x + width/2).ToString();
            baseY.Text = (wal.BasePoint.y + height/2).ToString();
            baseZ.Text = wal.BasePoint.z.ToString();
            //baseP = new System.Drawing.Point(wal.BasePoint.x, wal.BasePoint.y);
            //depth = wal.BasePoint.z;

        }

        private void housingLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (housingLengthText != null)
            {
                Slider slider = (Slider)sender;

                switch (slider.Name)
                {
                    case "housingLengthSlider": { housingLengthText.Text = slider.Value.ToString(); break; }
                    case "handsLengthSlider": { handsLengthText.Text = slider.Value.ToString(); break; }
                    case "handsWidthSlider": { handsWidthText.Text = slider.Value.ToString(); break; }
                    case "trunkWidthSlider": { trunkWidthText.Text = slider.Value.ToString(); break; }
                    case "trunkHeightSlider": { trunkHeightText.Text = slider.Value.ToString(); break; }
                    case "neckHeightSlider": { neckHeightText.Text = slider.Value.ToString(); break; }
                    case "neckWidthSlider": { neckWidthText.Text = slider.Value.ToString(); break; }
                    case "trackRadiusSlider": { trackRadiusText.Text = slider.Value.ToString(); break; }
                    case "eyeHeightSlider": { eyeHeightText.Text = slider.Value.ToString(); break; }
                }
                
            }
        }

        private void cancelBut_Click(object sender, RoutedEventArgs e)
        {
            isClose = true;
            this.Close();
        }

        private void housingCoor_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            cd.ShowDialog();

            System.Drawing.Color col = cd.Color;

            Button but = (Button)sender;

            switch (but.Name)
            {
                case "housingCoor": { housingColorText.Text = System.Drawing.ColorTranslator.ToHtml(col); break; }
                case "handsColor": { handsColorText.Text = System.Drawing.ColorTranslator.ToHtml(col); break; }
                case "trunkColor": { trunkColorText.Text = System.Drawing.ColorTranslator.ToHtml(col); break; }
                case "neckColor": { neckColorText.Text = System.Drawing.ColorTranslator.ToHtml(col); break; }
                case "eyeColor": { eyeColorText.Text = System.Drawing.ColorTranslator.ToHtml(col); break; }
                case "trackColor": { trackColorText.Text = System.Drawing.ColorTranslator.ToHtml(col); break; }
                case "panellsColor": { panellsColorText.Text = System.Drawing.ColorTranslator.ToHtml(col); break; }
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            try
            {
                param = new WallesParams();

                param.housingLength = Convert.ToInt32(housingLengthText.Text);
                param.handsLength = Convert.ToInt32(handsLengthText.Text);
                param.handsWidth = Convert.ToInt32(handsWidthText.Text);
                param.trunkWidth = Convert.ToInt32(trunkWidthText.Text);
                param.trunkHeight = Convert.ToInt32(trunkHeightText.Text);
                param.neckHeight = Convert.ToInt32(neckHeightText.Text);
                param.neckWidth = Convert.ToInt32(neckWidthText.Text);
                param.trackCylinderRad = Convert.ToInt32(trackRadiusText.Text);
                param.eyeCylinderHeight = Convert.ToInt32(eyeHeightText.Text);

                if (param.handsWidth > (param.housingLength / 4))
                    throw new Exception();

                if (param.neckWidth > param.housingLength)
                    throw new Exception();

                if (param.trunkHeight > param.housingLength || param.trunkWidth > param.housingLength)
                    throw new Exception();

                if (param.trackCylinderRad > param.housingLength / 4)
                    throw new Exception();

                if (param.handsLength <= param.housingLength / 2)
                    throw new Exception();
            }
            catch (System.FormatException exc)
            {
                MessageBox.Show("Walle's params must be numbers", "Format Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                flag = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Not possible relations", "Format Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                flag = false;
            }

            try
            {
                System.Drawing.Color housing = (housingColorText.Text.Equals("")) ? System.Drawing.Color.Red : System.Drawing.ColorTranslator.FromHtml(housingColorText.Text);
                System.Drawing.Color hands = (handsColorText.Text.Equals("")) ? System.Drawing.Color.Red : System.Drawing.ColorTranslator.FromHtml(handsColorText.Text);
                System.Drawing.Color trunk = (trunkColorText.Text.Equals("")) ? System.Drawing.Color.Red : System.Drawing.ColorTranslator.FromHtml(trunkColorText.Text);
                System.Drawing.Color neck = (neckColorText.Text.Equals("")) ? System.Drawing.Color.Red : System.Drawing.ColorTranslator.FromHtml(neckColorText.Text);
                System.Drawing.Color eyes = (eyeColorText.Text.Equals("")) ? System.Drawing.Color.Red : System.Drawing.ColorTranslator.FromHtml(eyeColorText.Text);
                System.Drawing.Color track = (trackColorText.Text.Equals("")) ? System.Drawing.Color.Red : System.Drawing.ColorTranslator.FromHtml(trackColorText.Text);
                System.Drawing.Color panells = (panellsColorText.Text.Equals("")) ? System.Drawing.Color.Red : System.Drawing.ColorTranslator.FromHtml(panellsColorText.Text);

                
                colors = new List<System.Drawing.Color> { housing, hands, trunk, neck, eyes, track, panells };
            }
            catch (Exception exc)
            {
                MessageBox.Show("Uncorrect color", "Format Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                flag = false;
            }

            try
            {
                int x = Convert.ToInt32(baseX.Text);
                int y = Convert.ToInt32(baseY.Text);
                int z = Convert.ToInt32(baseZ.Text);

                if (x < 0 || x > (int)main.screen.Width)
                    throw new Exception();

                if (y < 0 || y > (int)main.screen.Height)
                    throw new Exception();

                if (z < 0 || z > 540)
                    throw new Exception();

                baseP = new System.Drawing.Point(x, y);
                depth = z;

                param.panelsNumber = Convert.ToInt32(panellsNum.Text);


            }
            catch (System.FormatException exc)
            {
                MessageBox.Show("Base point uncorrect", "Format Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                flag = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Base point not correct", "Format Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                flag = false;
            }

            if (flag)
            {
                if (index == -1)
                    main.addWalle();
                else
                    main.changeWalle();

                isClose = false;
                this.Close();
            }
        }
        
    }
}
