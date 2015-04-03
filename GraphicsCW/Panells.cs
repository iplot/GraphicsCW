using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    class Panells
    {
        static /*const*/ List<TrianglesIndexes> triangleVertex = new List<TrianglesIndexes>{
            new TrianglesIndexes(0,1,2), new TrianglesIndexes(0,3,2), new TrianglesIndexes(0,1,5), new TrianglesIndexes(0,4,5), new TrianglesIndexes(0,3,4),
            new TrianglesIndexes(3,4,7), new TrianglesIndexes(3,2,7), new TrianglesIndexes(2,6,7), new TrianglesIndexes(1,5,6), new TrianglesIndexes(2,6,1),
            new TrianglesIndexes(4,5,7), new TrianglesIndexes(5,6,7)};

        static /*const*/ bool[,] transmisionMatrix = new bool[8, 8]{
            {false, true, true, true, true, true, false, false},
            {false, false, true, false, false, true, true, false},
            {true, true, false, true, false, false, true, true},
            {true, false, true, false, true, false, false, true},
            {true, false, false, true, false, true, false, true},
            {true, true, false, false, true, false, true, true},
            {false, true, true, false, false, true, false, true},
            {false, false, true, true, true, true, true, false}
        };

        List<List<Point3D>> panells;

        Point3D startPoint;

        Color color;

        public Panells(Point3D basePoint, int length, int count, Color col)
        {
            color = col;

            panells = new List<List<Point3D>>();

            startPoint = new Point3D(basePoint.x - length / 2, basePoint.y - length / 2, basePoint.z + length / 2);

            List<Point3D> points = new List<Point3D>();

            //int panelLeng = 10;
            int panelLeng = (length / 2 - 10) / 3;
            //if(count != 0)
            //    panelLeng = (length / count) - 10;
            //else
            //    panelLeng = 15;

            //толщина 5
            Point3D mainP = new Point3D(basePoint.x - length / 4, basePoint.y - length + panelLeng, basePoint.z - 5);
            // нижнее основание
            points.Add(mainP);
            points.Add(new Point3D(mainP.x, mainP.y, mainP.z + 5)); //аналог по z

            points.Add(new Point3D(mainP.x - panelLeng, mainP.y, mainP.z + 5)); //аналог по z
            points.Add(new Point3D(mainP.x - panelLeng, mainP.y, mainP.z));

            //верхнее основание
            points.Add(new Point3D(mainP.x, mainP.y - panelLeng, mainP.z));
            points.Add(new Point3D(mainP.x, mainP.y - panelLeng, mainP.z + 5)); // аналог по z

            points.Add(new Point3D(mainP.x - panelLeng, mainP.y - panelLeng, mainP.z + 5)); // аналог по z
            points.Add(new Point3D(mainP.x - panelLeng, mainP.y - panelLeng, mainP.z));

            for (int i = 0; i < count; i++)
            {
                List<Point3D> p = new List<Point3D>(points);
                AffineConverter.move(p, -i * (panelLeng + 5), 0, 0);
                panells.Add(p);
            }
        }

        public void movePanells(int xleng, int yleng, int zleng)
        {
            startPoint.x += xleng;
            startPoint.y += yleng;
            startPoint.z += zleng;
            for (int i = 0; i < panells.Count; i++)
            {
                AffineConverter.move(panells[i], xleng, yleng, zleng);
            }
        }

        public void XrotatePanells(int angle)
        {
            for (int i = 0; i < panells.Count; i++)
            {
                AffineConverter.move(panells[i], -startPoint.x, -startPoint.y, -startPoint.z);
                AffineConverter.Xrotate(null, panells[i], angle);
                AffineConverter.move(panells[i], startPoint.x, startPoint.y, startPoint.z);
            }
        }

        public void YrotatePanells(int angle)
        {
            for (int i = 0; i < panells.Count; i++)
            {
                AffineConverter.move(panells[i], -startPoint.x, -startPoint.y, -startPoint.z);
                AffineConverter.Yrotate(null, panells[i], angle);
                AffineConverter.move(panells[i], startPoint.x, startPoint.y, startPoint.z);
            }
        }

        public void ZrotatePanells(int angle)
        {
            for (int i = 0; i < panells.Count; i++)
            {
                AffineConverter.move(panells[i], -startPoint.x, -startPoint.y, -startPoint.z);
                AffineConverter.Zrotate(null, panells[i], angle);
                AffineConverter.move(panells[i], startPoint.x, startPoint.y, startPoint.z);
            }
        }

        public void scalePanells(double xscale, double yscale, double zscale)
        {
            for (int i = 0; i < panells.Count; i++)
            {
                AffineConverter.move(panells[i], -startPoint.x, -startPoint.y, -startPoint.z);
                AffineConverter.scale(panells[i], xscale, yscale, zscale);
                AffineConverter.move(panells[i], startPoint.x, startPoint.y, startPoint.z);
            }
        }

        public void camera2world(double[,] transformMatrix)
        {
            int[] cords;
            int[] result;

            for (int i = 0; i < panells.Count; i++)
            {
                for (int j = 0; j < panells[i].Count; j++)
                {
                    cords = new int[4] { panells[i][j].x, panells[i][j].y, panells[i][j].z, 1 };
                    result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

                    panells[i][j] = new Point3D(result);
                }
            }

            cords = new int[4] { startPoint.x, startPoint.y, startPoint.z, 1 };
            result = PointsColculator.matrixsMultiplication(cords, transformMatrix);

            startPoint = new Point3D(result);
        }

        public List<List<Point3D>> getPoints()
        {
            List<List<Point3D>> points = new List<List<Point3D>>();

            for (int i = 0; i < panells.Count; i++)
            {
                points.Add(new List<Point3D>(panells[i]));
            }
            return points;
        }

        public List<bool[,]> getTransmissionTab()
        {
            return new List<bool[,]> { transmisionMatrix };
        }

        public List<TrianglesIndexes> getIndexes()
        {
            return new List<TrianglesIndexes>(triangleVertex);
        }

        public List<Color> getColor()
        {
            List<Color> col = new List<Color>();

            for (int i = 0; i < panells.Count; i++)
                col.Add(color);

            return col;
        }
    }
}
