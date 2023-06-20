using System;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace SpatialDataLibrary
{
    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = 8000)]
    public class Point : INullable
    {
        public double x;
        public double y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Point()
        {
            this.x = Double.NaN;
            this.y = Double.NaN;
        }

        public bool IsNull
        {
            get { return (Double.IsNaN(this.x) || Double.IsNaN(this.y)); }
        }

        public static Point Null
        {
            get
            {
                return new Point();
            }
        }

        public bool isNull
        {
            get { return isNull; }
        }

        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }


        public double Distance(Point p)
        {
            if (this.IsNull || p.IsNull)
            {
                throw new NullReferenceException("One of the points is null.");
            }

            return Math.Sqrt(Math.Pow(this.x - p.x, 2) + Math.Pow(this.y - p.y, 2));
        }

        public static Point Parse(SqlString s)
        {
            if (s.IsNull)
                return Point.Null;

            string[] coordinates = s.Value.Split(',');
            if (coordinates.Length != 2)
            {
                throw new ArgumentException("Invalid point format. Expected format: 'x,y'");
            }

            double x = double.Parse(coordinates[0].Trim());
            double y = double.Parse(coordinates[1].Trim());

            return new Point(x, y);
        }

        public override string ToString()
        {
            if (this.IsNull)
            {
                return "NULL";
            }

            return "[" + this.x + ", " + this.y + "]";
        }
    }
}