using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphicsCW
{
    public class Point3D
    {
        public int x;
        public int y;
        public int z;

        public Point3D(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point3D(int[] points)
        {
            this.x = points[0];
            this.y = points[1];
            this.z = points[2];
        }

        public Point3D(Point3D point)
        {
            this.x = point.x;
            this.y = point.y;
            this.z = point.z;
        }

        public Point point3dToPoint()
        {
            return new Point(x, y);
        }
    }
}
