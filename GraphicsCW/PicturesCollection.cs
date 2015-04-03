using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GraphicsCW
{
    class PicturesCollection
    {
        byte[] drawArea;
        int areaWidth;
        int areaHeight;

        List<Walle> pictures;

        Camera myCamera;
        Drawer myDrawer;

        bool wiremod = false;

        public List<Walle> Pictures
        {
            get { return pictures; }
        }

        public bool WireMod
        {
            get{return wiremod;}
            set { wiremod = value; }
        }

        public PicturesCollection(int imageWidth, int imageHeight)
        {
            areaWidth = imageWidth;
            areaHeight = imageHeight;

            int length = imageWidth * imageHeight * 3;
            drawArea = new byte[length];

            pictures = new List<Walle>();

            myCamera = new Camera(120, 98, imageWidth, imageHeight);  //потом углы должны задаваться пользователем
            myDrawer = new Drawer(imageWidth, imageHeight);

        }

        public void addWalle(Point basePoint, int depth, WallesParams param, List<Color> colors)
        {
            Point3D pictureStartPoint = new Point3D(basePoint.X - areaWidth / 2, basePoint.Y - areaHeight / 2, depth);  //начальная точка объекта в координатах текущей камеры
            //Walle newWalle = new Walle(pictureStartPoint, param, myCamera.getVisionPoint(), myCamera.getScreenCenter());
            Walle newWalle = new Walle(pictureStartPoint, param, myCamera.getRealVisionPoint(), myCamera.getRealScreenCenter(), colors);
            //Walle создан в координатах камеры. Надо представить их в мировых
            newWalle.cameraCoordsAndWorld(myCamera.getInverseUVNMatrix());
            pictures.Add(newWalle);
        }

        public Bitmap getBitmap()
        {
            Bitmap bm = new Bitmap(areaWidth, areaHeight);
            Rectangle rect = new Rectangle(0, 0, areaWidth, areaHeight);
            BitmapData bd = new BitmapData();         

            bm.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb, bd);
            IntPtr ptr = bd.Scan0;
            bd.Stride = areaWidth * 3;
            Marshal.Copy(drawArea, 0, ptr, drawArea.Length);

            bm.UnlockBits(bd);

            return bm;
        }

        public Walle getPicture(int index)
        {
            return pictures[index];
        }

        public void removePicture(int index)
        {
            pictures.RemoveAt(index);
        }

        public void redrawCollection()
        {
            myDrawer.update();

            foreach (Walle w in pictures)
            {
                w.cameraCoordsAndWorld(myCamera.getUVNMatrix());    //переводим координат объекта в координаты текущей камеры

                List<List<Point3D>> cameraPoints = w.getPoints();
                List<List<Point3D>> points = w.getPoints();
                List<List<bool[,]>> transmissions = w.getTransmissionTabs();
                List<List<TrianglesIndexes>> tr = w.getIndexes();
                List<Color> colors = w.getColors();

                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = myCamera.checkPoints(points[i]);
                    points[i] = myCamera.projectingOnScreen(points[i]);
                }


                if (wiremod)
                    drawWithWiremod(cameraPoints, points, transmissions, w.getPanellsCount(), colors);
                else
                    drawWithoutWiremod(cameraPoints, points, tr, w.getPanellsCount(), colors);

                w.cameraCoordsAndWorld(myCamera.getInverseUVNMatrix()); //возвращаем координаты объекта в мировую систему
            }

            drawArea = myDrawer.getCanvas();
        }

        private void drawWithWiremod(List<List<Point3D>> cameraPoints, List<List<Point3D>> points, List<List<bool[,]>> transmissions, int panelsCount, List<Color> colors)
        {
            //это функция!!!

            //кубики и клины
            for (int i = 0; i < transmissions.Count; i++)
                myDrawer.drawObjectWithTable(points[i], transmissions[i], colors[i]);

            //цилиндры и конусы
            for (int i = transmissions.Count; i < 22 + panelsCount; i += 2)
            {
                myDrawer.drawRotationObject(points[i], points[i + 1], true, colors[i]);
            }

            //ленты на гусенице
            for (int i = 22 + panelsCount; i < points.Count - 1; i += 2)
                myDrawer.drawRotationObject(points[i], points[i + 1], false, colors[22 + panelsCount-1]);
        }

        private void drawWithoutWiremod(List<List<Point3D>> cameraPoints, List<List<Point3D>> points, List<List<TrianglesIndexes>> tr, int panelsCount, List<Color> col)
        {
            //это функция!!!
            for (int i = 0; i < tr.Count; i++)
            {
                List<Triangle> triang = makeTriangles(cameraPoints[i], points[i], tr[i], col[i]);
                triang.Sort(new CompareTriangles());

                myDrawer.zBuffer(triang, myCamera.isCentralProj());
            }

            for (int i = tr.Count; i < 22 + panelsCount; i += 2)
            {
                List<Triangle> triang = makeCylinderBaseTriangle(cameraPoints[i], points[i], cameraPoints[i + 1], points[i + 1], col[i]);
                triang.Sort(new CompareTriangles());

                myDrawer.zBuffer(triang, myCamera.isCentralProj());
            }

            for (int i = 22 + panelsCount; i < points.Count - 1; i += 2)
            {
                List<Triangle> triang = makeCylinderTriangle(cameraPoints[i], points[i], cameraPoints[i + 1], points[i + 1], false, col[22 + panelsCount-1]);
                triang.Sort(new CompareTriangles());

                myDrawer.zBuffer(triang, myCamera.isCentralProj());
            }

            myDrawer.drawbuf();
        }

        public Camera getCamera()
        {
            return myCamera;
        }

        public void setCamera(Camera cam)
        {
            myCamera = cam;
        }

        public Drawer getDrawer()
        {
            return myDrawer;
        }

        public void setDrawer(Drawer draw)
        {
            myDrawer = draw;
        }

        public void setCollection(List<Walle> collection)
        {
            pictures = new List<Walle>();

            foreach (Walle w in collection)
                pictures.Add(w);
        }

        private List<Triangle> makeTriangles(List<Point3D> cameraPoints, List<Point3D> screenPoints, List<TrianglesIndexes> indexes, Color col)
        {
            List<Triangle> triangles = new List<Triangle>();

            for(int i = 0; i < indexes.Count; i++)
            {
                TrianglesIndexes ind = indexes[i];
                List<Point3D> camP = new List<Point3D> { cameraPoints[ind.fPointIndex], cameraPoints[ind.sPointIndex], cameraPoints[ind.tPointIndex] };
                List<Point3D> screenP = new List<Point3D> { screenPoints[ind.fPointIndex], screenPoints[ind.sPointIndex], screenPoints[ind.tPointIndex] };

                if (screenP.IndexOf(null) != -1)
                    continue;

                triangles.Add(new Triangle(screenP, camP, col));
            }

            return triangles;
        }

        //треугольники по основаниям
        private List<Triangle> makeCylinderBaseTriangle(List<Point3D> cameraFirst, List<Point3D> screenFirst, List<Point3D> cameraSecond, List<Point3D> screenSecond, Color col)
        {
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 0; i < cameraFirst.Count - 2; i++)
            {
                List<Point3D> camera1 = new List<Point3D> { cameraFirst[i], cameraFirst[i + 1], cameraFirst[cameraFirst.Count - 1] };
                List<Point3D> screen1 = new List<Point3D> { screenFirst[i], screenFirst[i + 1], screenFirst[screenFirst.Count - 1] };

                List<Point3D> camera2 = new List<Point3D> { cameraSecond[i], cameraSecond[i + 1], cameraSecond[cameraSecond.Count - 1] };
                List<Point3D> screen2 = new List<Point3D> { screenSecond[i], screenSecond[i + 1], screenSecond[screenSecond.Count - 1] };

                if (screen1.IndexOf(null) == -1)
                //    continue;
                //else
                    triangles.Add(new Triangle(screen1, camera1, col));

                if (screen2.IndexOf(null) == -1)
                //    continue;
                //else
                    triangles.Add(new Triangle(screen2, camera2, col));
            }

            triangles.AddRange(makeCylinderTriangle(cameraFirst, screenFirst, cameraSecond, screenSecond, true, col));

            return triangles;
        }

        //треугольники по телу цилиндра
        private List<Triangle> makeCylinderTriangle(List<Point3D> cameraFirst, List<Point3D> screenFirst, List<Point3D> cameraSecond, List<Point3D> screenSecond, bool flag, 
            Color col)
        {
            List<Triangle> triangles = new List<Triangle>();
            int k = (flag) ? 2 : 1;
            for (int i = 0; i < cameraFirst.Count - k; i++)
            {
                List<Point3D> camera1 = new List<Point3D> { cameraFirst[i], cameraFirst[i + 1], cameraSecond[i] };
                List<Point3D> screen1 = new List<Point3D> { screenFirst[i], screenFirst[i + 1], screenSecond[i] };

                List<Point3D> camera2 = new List<Point3D> { cameraFirst[i + 1], cameraSecond[i], cameraSecond[i + 1] };
                List<Point3D> screen2 = new List<Point3D> { screenFirst[i + 1], screenSecond[i], screenSecond[i + 1] };

                if (screen1.IndexOf(null) == -1)
                //    continue;
                //else
                    triangles.Add(new Triangle(screen1, camera1, col));

                if (screen2.IndexOf(null) == -1)
                //    continue;
                //else
                    triangles.Add(new Triangle(screen2, camera2, col));
            }

            return triangles;
        }

    }

    class CompareTriangles : IComparer<Triangle>
    {
        public int Compare(Triangle tr1, Triangle tr2)
        {
            return (int)(tr1.getAverageZ() - tr1.getAverageZ());
        }
    }
}
