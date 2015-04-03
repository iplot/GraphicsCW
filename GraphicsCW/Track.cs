using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{

    class Track
    {
        List<List<Point3D>> cylinders;  //верхний, правый нижний, левый нижний
        List<Point3D> centers;

        List<List<Point3D>> secondCylinders;
        List<Point3D> secondCenters;

        Point3D startPoint;

        int radius;//15
        int circleParts;
        int cylHeight = 20;

        int height = 75;    //высота гусениц
        int lowdist = 70;   //расстояние между нижними цилиндрами

        Color color;

        public Track(Point3D basePoint, int boxLength, int rad, Color col)
        {
            color = col;

            height = (boxLength / 2) + 25;

            radius = rad;
            circleParts = 24;
            startPoint = new Point3D(basePoint.x - boxLength/2, basePoint.y - boxLength/2, basePoint.z + boxLength/2);

            cylinders = new List<List<Point3D>>();
            centers = new List<Point3D>();
            secondCylinders = new List<List<Point3D>>();
            secondCenters = new List<Point3D>();

            List<Point3D> points = new List<Point3D>();

            //верхний главный цилиндр
            Point3D center = new Point3D(basePoint.x - boxLength, basePoint.y - boxLength / 2, basePoint.z + boxLength / 2);
            centers.Add(new Point3D(center.x, center.y, center.z));
            makeCylinder(center, points);

            cylinders.Add(new List<Point3D>(points));

            //задняя часть
            addCenterToList(center, points);
            AffineConverter.move(points, -cylHeight, 0, 0);
            removeCenterFromList(ref center, points);

            cylinders.Add(new List<Point3D>(points));
            centers.Add(new Point3D(center.x, center.y, center.z));

            //нижние цилиндры
            for (int i = -1; i < 2; i += 2)
            {
                center = new Point3D(centers[0].x, centers[0].y, centers[0].z);
                points = new List<Point3D>(cylinders[0]);

                addCenterToList(center, points);
                AffineConverter.move(points, 0, height, i*(lowdist / 2));
                removeCenterFromList(ref center, points);

                cylinders.Add(new List<Point3D>(points));
                centers.Add(new Point3D(center.x, center.y, center.z));

                //задняя часть
                addCenterToList(center, points);
                AffineConverter.move(points, -cylHeight, 0, 0);
                removeCenterFromList(ref center, points);

                cylinders.Add(new List<Point3D>(points));
                centers.Add(new Point3D(center.x, center.y, center.z));
            }

            makeSecondCylinders(boxLength);

        }

        private void makeCylinder(Point3D center, List<Point3D> points)
        {
            int angle = 360 / circleParts;
            points.Add(new Point3D(center.x, center.y - radius, center.z));

            for (int i = 0; i < circleParts; i++)
            {
                Point3D newPoint = new Point3D(points[0].x, points[0].y, points[0].z);
                List<Point3D> list = new List<Point3D> { newPoint };
                int ang = angle * i;

                AffineConverter.move(list, -center.x, -center.y, -center.z);
                AffineConverter.Xrotate(null, list, ang);
                AffineConverter.move(list, center.x, center.y, center.z);

                points.Add(list[0]);
            }
        }

        private void addCenterToList(Point3D center, List<Point3D> points)
        {
            points.Add(center);
        }

        private void removeCenterFromList(ref Point3D center, List<Point3D> points)
        {
            //center = new Point3D(points[points.Count - 1].x, points[points.Count - 1].y, points[points.Count - 1].z);
            center = points[points.Count - 1];
            points.Remove(center);
            
        }

        private void makeSecondCylinders(int boxLength)
        {
            for (int i = 0; i < cylinders.Count; i++)
            {
                List<Point3D> points = new List<Point3D>(cylinders[i]);
                Point3D center = new Point3D(centers[i].x, centers[i].y, centers[i].z);

                addCenterToList(center, points);
                AffineConverter.move(points, boxLength + cylHeight, 0, 0);
                removeCenterFromList(ref center, points);

                secondCenters.Add(center);
                secondCylinders.Add(points);
            }
        }

        public void moveTrack(int xleng, int yleng, int zleng)
        {
            startPoint.x += xleng;
            startPoint.y += yleng;
            startPoint.z += zleng;

            for (int i = 0; i < cylinders.Count; i++)
            {
                addCenterToList(centers[i], cylinders[i]);
                addCenterToList(secondCenters[i], secondCylinders[i]);

                AffineConverter.move(cylinders[i], xleng, yleng, zleng);
                AffineConverter.move(secondCylinders[i], xleng, yleng, zleng);

                Point3D cent = centers[i];
                removeCenterFromList(ref cent, cylinders[i]);
                centers[i] = cent;

                cent = secondCenters[i];
                removeCenterFromList(ref cent, secondCylinders[i]);
                secondCenters[i] = cent;
            }
        }

        public void XrotateTrack(int angle)
        {
            for (int i = 0; i < cylinders.Count; i++)
            {
                addCenterToList(centers[i], cylinders[i]);
                addCenterToList(secondCenters[i], secondCylinders[i]);

                AffineConverter.move(cylinders[i], -startPoint.x, -startPoint.y, -startPoint.z);
                AffineConverter.move(secondCylinders[i], -startPoint.x, -startPoint.y, -startPoint.z);

                AffineConverter.Xrotate(null, cylinders[i], angle);
                AffineConverter.Xrotate(null, secondCylinders[i], angle);

                AffineConverter.move(cylinders[i], startPoint.x, startPoint.y, startPoint.z);
                AffineConverter.move(secondCylinders[i], startPoint.x, startPoint.y, startPoint.z);

                Point3D cent = centers[i];
                removeCenterFromList(ref cent, cylinders[i]);
                centers[i] = cent;
                cent = secondCenters[i];
                removeCenterFromList(ref cent, secondCylinders[i]);
                secondCenters[i] = cent;
            }
        }

        public void YrotateTrack(int angle)
        {
            for (int i = 0; i < cylinders.Count; i++)
            {
                addCenterToList(centers[i], cylinders[i]);
                addCenterToList(secondCenters[i], secondCylinders[i]);

                AffineConverter.move(cylinders[i], -startPoint.x, -startPoint.y, -startPoint.z);
                AffineConverter.move(secondCylinders[i], -startPoint.x, -startPoint.y, -startPoint.z);

                AffineConverter.Yrotate(null, cylinders[i], angle);
                AffineConverter.Yrotate(null, secondCylinders[i], angle);

                AffineConverter.move(cylinders[i], startPoint.x, startPoint.y, startPoint.z);
                AffineConverter.move(secondCylinders[i], startPoint.x, startPoint.y, startPoint.z);

                Point3D cent = centers[i];
                removeCenterFromList(ref cent, cylinders[i]);
                centers[i] = cent;
                cent = secondCenters[i];
                removeCenterFromList(ref cent, secondCylinders[i]);
                secondCenters[i] = cent;
            }
        }

        public void ZrotateTrack(int angle)
        {
            for (int i = 0; i < cylinders.Count; i++)
            {
                addCenterToList(centers[i], cylinders[i]);
                addCenterToList(secondCenters[i], secondCylinders[i]);

                AffineConverter.move(cylinders[i], -startPoint.x, -startPoint.y, -startPoint.z);
                AffineConverter.move(secondCylinders[i], -startPoint.x, -startPoint.y, -startPoint.z);

                AffineConverter.Zrotate(null, cylinders[i], angle);
                AffineConverter.Zrotate(null, secondCylinders[i], angle);

                AffineConverter.move(cylinders[i], startPoint.x, startPoint.y, startPoint.z);
                AffineConverter.move(secondCylinders[i], startPoint.x, startPoint.y, startPoint.z);

                Point3D cent = centers[i];
                removeCenterFromList(ref cent, cylinders[i]);
                centers[i] = cent;
                cent = secondCenters[i];
                removeCenterFromList(ref cent, secondCylinders[i]);
                secondCenters[i] = cent;
            }
        }

        public void scaleTrack(double scaleX, double scaleY, double scaleZ)
        {
            for (int i = 0; i < cylinders.Count; i++)
            {
                addCenterToList(centers[i], cylinders[i]);
                addCenterToList(secondCenters[i], secondCylinders[i]);

                AffineConverter.move(cylinders[i], -startPoint.x, -startPoint.y, -startPoint.z);
                AffineConverter.move(secondCylinders[i], -startPoint.x, -startPoint.y, -startPoint.z);

                AffineConverter.scale(cylinders[i], scaleX, scaleY, scaleZ);
                AffineConverter.scale(secondCylinders[i], scaleX, scaleY, scaleZ);

                AffineConverter.move(cylinders[i], startPoint.x, startPoint.y, startPoint.z);
                AffineConverter.move(secondCylinders[i], startPoint.x, startPoint.y, startPoint.z);

                Point3D cent = centers[i];
                removeCenterFromList(ref cent, cylinders[i]);
                centers[i] = cent;
                cent = secondCenters[i];
                removeCenterFromList(ref cent, secondCylinders[i]);
                secondCenters[i] = cent;
            }
        }

        //не проверялась!
        public void camera2world(double[,] transformMatrix)
        {
            int[] cords;
            int[] result;

            for (int i = 0; i < cylinders.Count; i++)
            {
                addCenterToList(centers[i], cylinders[i]);
                addCenterToList(secondCenters[i], secondCylinders[i]);

                for (int j = 0; j < cylinders[i].Count; j++)
                {
                    //первая гусеница
                    cords = new int[4] { cylinders[i][j].x, cylinders[i][j].y, cylinders[i][j].z, 1 };
                    result = PointsColculator.matrixsMultiplication(cords, transformMatrix);
                    cylinders[i][j] = new Point3D(result);

                    //вторая гусеница
                    cords = new int[4] { secondCylinders[i][j].x, secondCylinders[i][j].y, secondCylinders[i][j].z, 1 };
                    result = PointsColculator.matrixsMultiplication(cords, transformMatrix);
                    secondCylinders[i][j] = new Point3D(result);
                }

                Point3D cent = centers[i];
                removeCenterFromList(ref cent, cylinders[i]);
                centers[i] = cent;

                cent = secondCenters[i];
                removeCenterFromList(ref cent, secondCylinders[i]);
                secondCenters[i] = cent;
            }

            cords = new int[4] { startPoint.x, startPoint.y, startPoint.z, 1 };
            result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

            startPoint = new Point3D(result);
        }

        public List<List<Point3D>> getTrack1()
        {
            List<List<Point3D>> points = new List<List<Point3D>>();

            for (int i = 0; i < cylinders.Count; i++)
            {
                addCenterToList(centers[i], cylinders[i]);
                points.Add(new List<Point3D>(cylinders[i]));
            }

            //points = new List<List<Point3D>>(cylinders);    //!!!

            for (int i = 0; i < cylinders.Count; i++)
            {
                Point3D cen = centers[i];
                removeCenterFromList(ref cen, cylinders[i]);
                centers[i] = cen;
            }

            return points;
        }

        public List<List<Point3D>> getTrack2()
        {
            List<List<Point3D>> points = new List<List<Point3D>>();

            for (int i = 0; i < secondCylinders.Count; i++)
            {
                addCenterToList(secondCenters[i], secondCylinders[i]);
                points.Add(new List<Point3D>(secondCylinders[i]));
            }

            //points = new List<List<Point3D>>(secondCylinders);

            for (int i = 0; i < secondCylinders.Count; i++)
            {
                Point3D cen = secondCenters[i];
                removeCenterFromList(ref cen, secondCylinders[i]);
                secondCenters[i] = cen;
            }

            return points;
        }

        public List<List<Point3D>> getTrackTape()
        {
            List<List<Point3D>> points = new List<List<Point3D>>();

            points.Add(new List<Point3D> { new Point3D(cylinders[0][60 / (360 / circleParts)]), new Point3D(cylinders[1][60 / (360 / circleParts)]) });
            points.Add(new List<Point3D> { new Point3D(cylinders[2][60 / (360 / circleParts)]), new Point3D(cylinders[3][60 / (360 / circleParts)]) });

            points.Add(new List<Point3D> { new Point3D(cylinders[0][300 / (360 / circleParts)]), new Point3D(cylinders[1][300 / (360 / circleParts)]) });
            points.Add(new List<Point3D> { new Point3D(cylinders[4][300 / (360 / circleParts)]), new Point3D(cylinders[5][300 / (360 / circleParts)]) });

            points.Add(new List<Point3D> { new Point3D(cylinders[2][180 / (360 / circleParts)]), new Point3D(cylinders[3][180 / (360 / circleParts)]) });
            points.Add(new List<Point3D> { new Point3D(cylinders[4][180 / (360 / circleParts)]), new Point3D(cylinders[5][180 / (360 / circleParts)]) });

            points.Add(new List<Point3D> { new Point3D(secondCylinders[0][60 / (360 / circleParts)]), new Point3D(secondCylinders[1][60 / (360 / circleParts)]) });
            points.Add(new List<Point3D> { new Point3D(secondCylinders[2][60 / (360 / circleParts)]), new Point3D(secondCylinders[3][60 / (360 / circleParts)]) });

            points.Add(new List<Point3D> { new Point3D(secondCylinders[0][300 / (360 / circleParts)]), new Point3D(secondCylinders[1][300 / (360 / circleParts)]) });
            points.Add(new List<Point3D> { new Point3D(secondCylinders[4][300 / (360 / circleParts)]), new Point3D(secondCylinders[5][300 / (360 / circleParts)]) });

            points.Add(new List<Point3D> { new Point3D(secondCylinders[2][180 / (360 / circleParts)]), new Point3D(secondCylinders[3][180 / (360 / circleParts)]) });
            points.Add(new List<Point3D> { new Point3D(secondCylinders[4][180 / (360 / circleParts)]), new Point3D(secondCylinders[5][180 / (360 / circleParts)]) });

            return points;
        }

        public List<Color> getColor()
        {
            List<Color> col = new List<Color>();

            for (int i = 0; i < cylinders.Count; i++)
            {
                col.Add(color);
                col.Add(color);
            }

            return col;
        }
    }

}
