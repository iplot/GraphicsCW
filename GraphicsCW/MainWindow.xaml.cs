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
using System.IO;

namespace GraphicsCW
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NewWalle n;
        HorizontalAngle horizAng;
        VerticalAngle verticAng;

        PicturesCollection myPicturesCollection;
        Walle workWalle;

        int moveCameraStep;
        int rotateCameraAngle;
        int zoomDist;

        int nameInd = 0;

        public MainWindow()
        {
            InitializeComponent();

            myPicturesCollection = new PicturesCollection((int)screen.Width, (int)screen.Height);

            setViewCollection();

            myPicturesCollection.redrawCollection();

            display();
        }

        private void display()
        {
            System.Drawing.Bitmap scene = myPicturesCollection.getBitmap();

            scene = myPicturesCollection.getBitmap();

            screen.Source = Bitmap2BitmapImage(scene);
        }

        private BitmapImage Bitmap2BitmapImage(System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Position = 0;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            return bi;
        }

        public void setHorizontAngle(int angle)
        {
            myPicturesCollection.getCamera().HorizontalAngle = angle;

            myPicturesCollection.redrawCollection();
            display();
        }

        public void setVerticalAngle(int angle)
        {
            myPicturesCollection.getCamera().VerticalAngle = angle;

            myPicturesCollection.redrawCollection();
            display();
        }

        private void saveScene(String path)
        {
            StreamWriter writer = new StreamWriter(path);

            Camera cam = myPicturesCollection.getCamera();

            writer.WriteLine("<Camera>");
            writer.WriteLine("<VisionPoint>{0} {1} {2}</VisionPoint>", cam.getRealVisionPoint().x, cam.getRealVisionPoint().y, cam.getRealVisionPoint().z);
            writer.WriteLine("<ScreenCenter>{0} {1} {2}</ScreenCenter>", cam.getRealScreenCenter().x, cam.getRealScreenCenter().y, cam.getRealScreenCenter().z);
            writer.WriteLine("<HorizontalAngle>{0}</HorizontalAngle>", cam.HorizontalAngle);
            writer.WriteLine("<VerticalAngle>{0}</VerticalAngle>", cam.VerticalAngle);
            writer.WriteLine("<MinDistance>{0}</MinDistance>", cam.MinDistance);
            writer.WriteLine("<CentralProjection>{0}</CentralProjection>", cam.IsCentralProjection);
            writer.WriteLine("<WireMod>{0}</WireMod>", myPicturesCollection.WireMod);
            writer.WriteLine("</Camera>");

            Drawer dr = myPicturesCollection.getDrawer();

            writer.WriteLine("<Drawer>");
            writer.WriteLine("<UnFillMode>{0}</UnFillMode>", dr.UnFillMode);
            writer.WriteLine("</Drawer>");

            int width = (int)screen.Width;
            int height = (int)screen.Height;

            foreach (Walle w in myPicturesCollection.Pictures)
            {
                WallesParams param = w.Params;
                List<System.Drawing.Color> col = w.Colors;

                writer.WriteLine("<Walle>");
                writer.WriteLine("<WalleVisionPoint>{0} {1} {2}</WalleVisionPoint>", w.WallesViewPoint.x, w.WallesViewPoint.y, w.WallesViewPoint.z);
                writer.WriteLine("<WalleScreenCenter>{0} {1} {2}</WalleScreenCenter>", w.WallesScreenCenter.x, w.WallesScreenCenter.y, w.WallesScreenCenter.z);
                writer.WriteLine("<BasePoint>{0} {1} {2}</BasePoint>", w.BasePoint.x + width / 2, w.BasePoint.y + height / 2, w.BasePoint.z);
                writer.WriteLine("<HousingLength>{0}</HousingLength>", param.housingLength);
                writer.WriteLine("<HandsWidth>{0}</HandsWidth>", param.handsWidth);
                writer.WriteLine("<HandsLength>{0}</HandsLength>", param.handsLength);
                writer.WriteLine("<TrunkWidth>{0}</TrunkWidth>", param.trunkWidth);
                writer.WriteLine("<TrunkHeight>{0}</TrunkHeight>", param.trunkHeight);
                writer.WriteLine("<NeckWidth>{0}</NeckWidth>", param.neckWidth);
                writer.WriteLine("<NeckHeight>{0}</NeckHeight>", param.neckHeight);
                writer.WriteLine("<TrackCylinderRad>{0}</TrackCylinderRad>", param.trackCylinderRad);
                writer.WriteLine("<EyeCylinderHeight>{0}</EyeCylinderHeight>", param.eyeCylinderHeight);
                writer.WriteLine("<PanelsNumber>{0}</PanelsNumber>", param.panelsNumber);

                writer.WriteLine("<HousingColor>{0}</HousingColor>", System.Drawing.ColorTranslator.ToHtml(col[0]));
                writer.WriteLine("<HandsColor>{0}</HandsColor>", System.Drawing.ColorTranslator.ToHtml(col[1]));
                writer.WriteLine("<NeckColor>{0}</NeckColor>", System.Drawing.ColorTranslator.ToHtml(col[2]));
                writer.WriteLine("<TrunkColor>{0}</TrunkColor>", System.Drawing.ColorTranslator.ToHtml(col[3]));
                writer.WriteLine("<PanellsColor>{0}</PanellsColor>", System.Drawing.ColorTranslator.ToHtml(col[4]));
                writer.WriteLine("<EyesColor>{0}</EyesColor>", System.Drawing.ColorTranslator.ToHtml(col[5]));
                writer.WriteLine("<TracksColor>{0}</TracksColor>", System.Drawing.ColorTranslator.ToHtml(col[6]));

                writer.WriteLine("</Walle>");
            }

            writer.Close();
        }

        private void loadScene(String path)
        {
            StreamReader reader = new StreamReader(path);

            String sceneCode = reader.ReadToEnd();

            SceneCodeParser.parse(sceneCode, (int)screen.Width, (int)screen.Height);

            myPicturesCollection.setCamera(SceneCodeParser.getCamera());
            myPicturesCollection.setDrawer(SceneCodeParser.getDrawer());
            myPicturesCollection.setCollection(SceneCodeParser.getWalles());
            myPicturesCollection.WireMod = SceneCodeParser.wireMod;
        }

        private void stepField_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox field = (TextBox)sender;

            try
            {
                moveCameraStep = Convert.ToInt32(field.Text);
            }
            catch (System.FormatException exc)
            {
                stepField.Text = "0";
                MessageBox.Show("Step is uncorrect", "FormatError", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void moveCamLeft_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.moveCamera(-moveCameraStep, 0, 0);

            int loc = Convert.ToInt32(XDest.Text);
            loc -= moveCameraStep;

            XDest.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void moveCamRight_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.moveCamera(moveCameraStep, 0, 0);

            int loc = Convert.ToInt32(XDest.Text);
            loc += moveCameraStep;

            XDest.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void moveCamUp_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.moveCamera(0, -moveCameraStep, 0);

            int loc = Convert.ToInt32(YDest.Text);
            loc -= moveCameraStep;

            YDest.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void moveCamDown_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.moveCamera(0, moveCameraStep, 0);

            int loc = Convert.ToInt32(YDest.Text);
            loc += moveCameraStep;

            YDest.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void moveCamForw_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.moveCamera(0, 0, moveCameraStep);

            int loc = Convert.ToInt32(ZDest.Text);
            loc += moveCameraStep;

            ZDest.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void moveCamBack_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.moveCamera(0, 0, -moveCameraStep);

            int loc = Convert.ToInt32(ZDest.Text);
            loc -= moveCameraStep;

            ZDest.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void rotateCamLeft_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.YrotateCamera(-rotateCameraAngle);

            int loc = Convert.ToInt32(YAngle.Text);

            loc -= rotateCameraAngle;

            YAngle.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void rotateCamRight_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.YrotateCamera(rotateCameraAngle);

            int loc = Convert.ToInt32(YAngle.Text);

            loc += rotateCameraAngle;

            YAngle.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void rotateCamUp_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.XrotateCamera(rotateCameraAngle);

            int loc = Convert.ToInt32(XAngle.Text);

            loc += rotateCameraAngle;

            XAngle.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void rotateCamDown_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.XrotateCamera(-rotateCameraAngle);

            int loc = Convert.ToInt32(XAngle.Text);

            loc -= rotateCameraAngle;

            XAngle.Text = loc.ToString();

            myPicturesCollection.redrawCollection();
            display();
        }

        private void textBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                rotateCameraAngle = Convert.ToInt32(rotateStep.Text);
            }
            catch (System.FormatException exc)
            {
                stepField.Text = "0";
                MessageBox.Show("Angle is uncorrect", "FormatError", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void zoomPlus_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            zoomDist = Convert.ToInt32(zoomField.Text);

            zoomDist += 10;

            if (zoomDist <= 100)
            {
                camera.zoom(10);

                zoomField.Text = zoomDist.ToString();

                myPicturesCollection.redrawCollection();
                display();
            }

            
        }

        private void zoomMinus_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            zoomDist = Convert.ToInt32(zoomField.Text);

            zoomDist -= 10;

            if (zoomDist > -10)
            {
                camera.zoom(-10);

                zoomField.Text = zoomDist.ToString();

                myPicturesCollection.redrawCollection();
                display();
            }  
        }

        private void paralelRB_Checked(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.IsCentralProjection = false;

            myPicturesCollection.redrawCollection();
            display();
        }

        private void centralRB_Checked(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.IsCentralProjection = true;

            myPicturesCollection.redrawCollection();
            display();
        }

        private void surfaceRB_Checked(object sender, RoutedEventArgs e)
        {
            myPicturesCollection.WireMod = false;
            myPicturesCollection.getDrawer().UnFillMode = true;
            myPicturesCollection.redrawCollection();
            display();
        }

        private void wireRB_Checked(object sender, RoutedEventArgs e)
        {
            myPicturesCollection.WireMod = true;
            myPicturesCollection.redrawCollection();
            display();
        }

        private void fillRB_Checked(object sender, RoutedEventArgs e)
        {
            myPicturesCollection.WireMod = false;
            myPicturesCollection.getDrawer().UnFillMode = false;
            myPicturesCollection.redrawCollection();
            display();
        }


        private void setViewCollection()
        {
            viewCollection.Items.Clear();

            for (int i = 0; i < myPicturesCollection.Pictures.Count; i++)
            {
                viewCollection.Items.Add("Walle" + nameInd.ToString());
                nameInd++;
            }
        }

        private void viewCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int ind = viewCollection.SelectedIndex;

            if (ind != -1)
                workWalle = myPicturesCollection.getPicture(ind);
        }

        private void deletePictureBut_Click(object sender, RoutedEventArgs e)
        {
            int ind = viewCollection.SelectedIndex;

            if (ind != -1)
            {
                myPicturesCollection.removePicture(ind);
                viewCollection.Items.RemoveAt(ind);

                myPicturesCollection.redrawCollection();
                display();
            }
        }

        private void gotoPictureBut_Click(object sender, RoutedEventArgs e)
        {
            Camera camera = myPicturesCollection.getCamera();

            camera.setPoints(workWalle.WallesViewPoint, workWalle.WallesScreenCenter);

            myPicturesCollection.redrawCollection();
            display();
        }

        private void moveWalleBut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = Convert.ToInt32(XmoveText.Text);
                int y = Convert.ToInt32(YmoveText.Text);
                int z = Convert.ToInt32(ZmoveText.Text);

                if (workWalle != null)
                {
                    workWalle.moveWalle(x, y, z);
                    myPicturesCollection.redrawCollection();
                    display();
                }
            }
            catch (System.FormatException exc)
            {
                MessageBox.Show("Uncorrect length", "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ang = Convert.ToInt32(XWalleAngle.Text);

                if (workWalle != null)
                {
                    workWalle.XrotateWalle(ang);
                    myPicturesCollection.redrawCollection();
                    display();
                }
            }
            catch (System.FormatException exc)
            {
                MessageBox.Show("Uncorrect angle", "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void YRotate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ang = Convert.ToInt32(YWalleAngle.Text);

                if (workWalle != null)
                {
                    workWalle.YrotateWalle(ang);
                    myPicturesCollection.redrawCollection();
                    display();
                }
            }
            catch (System.FormatException exc)
            {
                MessageBox.Show("Uncorrect angle", "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ZRotate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ang = Convert.ToInt32(ZWalleAngle.Text);

                if (workWalle != null)
                {
                    workWalle.ZrotateWalle(ang);
                    myPicturesCollection.redrawCollection();
                    display();
                }
            }
            catch (System.FormatException exc)
            {
                MessageBox.Show("Uncorrect angle", "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void scaleWalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double x = Convert.ToDouble(XScale.Text);
                double y = Convert.ToDouble(YScale.Text);
                double z = Convert.ToDouble(ZScale.Text);

                if (x <= 0 || y <= 0 || z <= 0)
                    throw new System.FormatException();

                if (workWalle != null)
                {
                    workWalle.scaleWalle(x, y, z);
                    myPicturesCollection.redrawCollection();
                    display();
                }
            }
            catch (System.FormatException exc)
            {
                MessageBox.Show("Uncorrect coefficient", "Format Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog();

            fd.ShowDialog();

            String path = fd.FileName;

            if (!path.Equals(""))
            {
                loadScene(path);
                setViewCollection();
                myPicturesCollection.redrawCollection();
                display();
            }
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();

            sd.ShowDialog();

            String path = sd.FileName;

            if (!path.Equals(""))
                saveScene(path);
        }

        private void addPictureBut_Click(object sender, RoutedEventArgs e)
        {
            n = new NewWalle();
            n.setCollect(this);
            n.Show();

            //if (!n.isClose)
            //{
            //    myPicturesCollection.addWalle(n.baseP, n.depth, n.param);
            //    setViewCollection();
            //    myPicturesCollection.redrawCollection();
            //    display();
            //}
        }

        public void addWalle()
        {
            myPicturesCollection.addWalle(n.baseP, n.depth, n.param, n.colors);
            setViewCollection();
            myPicturesCollection.redrawCollection();
            display();
        }

        public void changeWalle()
        {
            //workWalle = new Walle(n.baseP, n.depth, n.param, n.colors);
            myPicturesCollection.removePicture(viewCollection.SelectedIndex);
            myPicturesCollection.addWalle(n.baseP, n.depth, n.param, n.colors);
            setViewCollection();
            myPicturesCollection.redrawCollection();
            display();

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            horizAng = new HorizontalAngle();

            horizAng.setCollection(this);

            horizAng.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            verticAng = new VerticalAngle();

            verticAng.setCollection(this);

            verticAng.Show();
        }

        private void changeObject_Click(object sender, RoutedEventArgs e)
        {
            if (viewCollection.SelectedIndex == -1)
                return;

            n = new NewWalle();
            n.setCollect(this);
            n.setWalleParam(workWalle, viewCollection.SelectedIndex, (int)screen.Width, (int)screen.Height);

            n.Show();
        }
        

    }
}
