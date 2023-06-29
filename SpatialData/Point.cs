using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text;
using System.Globalization;

namespace SpatialData
{
    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = 8000)]
    public class Point : INullable, IBinarySerialize
    {
        public double x;
        public double y;
        private bool isNull;

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

        public static Point Null
        {
            get
            {
                Point p = new Point();
                p.isNull = true;
                return p;
            }
        }

        public bool IsNull
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


        public SqlDouble Distance(Point p)
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

            return this.x + "," + this.y;
        }

        #region IBinarySerialize Members

        public void Write(System.IO.BinaryWriter writer)
        {
            writer.Write(this.x);
            writer.Write(this.y);
        }

        public void Read(System.IO.BinaryReader reader)
        {
            this.x = reader.ReadDouble();
            this.y = reader.ReadDouble();
        }
        #endregion
    }
}