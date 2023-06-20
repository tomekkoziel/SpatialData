using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace SpatialDataLibrary
{
    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = 8000)]
    public class Figure
    {
        public List<Point> points;
        private double area;

        public Figure()
        {
            this.points = new List<Point>();
            this.area = 0.0;
        }

        public double Area
        {
            get { return this.area; }
            set { this.area = value; }
        }

        public void AddPoint(Point point)
        {
            if (point != null)
            {
                this.points.Add(point);
            }
            else
            {
                throw new ArgumentNullException(nameof(point), "Cannot add a null point to the figure.");
            }
        }

        public Point ImportPoints
        {
            set { this.points.Add(new Point(value.x, value.y)); }
        }

        public double CalculateArea()
        {
            if (this.points.Count < 3)
            {
                throw new InvalidOperationException("Area calculation requires at least 3 points.");
            }

            double area = 0;

            for (int i = 0; i < this.points.Count; i++)
            {
                Point currentPoint = this.points[i];
                Point nextPoint = this.points[(i + 1) % this.points.Count];

                area += (currentPoint.X * nextPoint.Y) - (currentPoint.Y * nextPoint.X);
            }

            return Math.Abs(area / 2);
        }

        public bool IsInside(Point point)
        {
            if (this.points.Count < 3)
            {
                throw new InvalidOperationException("Area calculation requires at least 3 points.");
            }

            bool isInside = false;
            int j = this.points.Count - 1;

            for (int i = 0; i < this.points.Count; i++)
            {
                if ((this.points[i].Y < point.Y && this.points[j].Y >= point.Y || this.points[j].Y < point.Y && this.points[i].Y >= point.Y)
                    && (this.points[i].X <= point.X || this.points[j].X <= point.X))
                {
                    if (this.points[i].X + (point.Y - this.points[i].Y) / (this.points[j].Y - this.points[i].Y) * (this.points[j].X - this.points[i].X) < point.X)
                    {
                        isInside = !isInside;
                    }
                }

                j = i;
            }

            return isInside;
        }
    }
}