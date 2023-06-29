using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Server;

namespace SpatialData
{
    [Serializable]
    [SqlUserDefinedType(Format.UserDefined, MaxByteSize = 8000, IsByteOrdered = true)]
    public class Figure : INullable, IBinarySerialize
    {
        public List<Point> points;

        public Figure()
        {
            this.points = new List<Point>();
        }

        public bool IsNull
        {
            get { return false; }
        }

        public static Figure Null
        {
            get
            {
                Figure f = new Figure();
                return f;
            }
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

        public Figure CloseFigure
        {
            set { this.points.Add(this.points[0]); }
        }

        public static Figure Parse(SqlString s)
        {
            return new Figure();
        }

        public override string ToString()
        {
            string pointsStr = "";

            foreach(Point p in this.points)
            {
                pointsStr += p.ToString();
                pointsStr += "\n";
            }

            return pointsStr;
        }

        public int PointCount
        {
            get { return this.points.Count; }
        }

        #region IBinarySerialize Members

        public void Read(System.IO.BinaryReader r)
        {
            int j = r.ReadInt32();
            for (int i = 0; i < j; i++)
            {
                Point tmp = new Point();
                tmp.Read(r);
                this.points.Add(tmp);
            }
        }

        public void Write(System.IO.BinaryWriter writer)
        {
            writer.Write(this.points.Count);
            foreach (Point i in this.points)
            {
                i.Write(writer);
            }
        }
        #endregion

        public double CalculateArea()
        {
            int quantity = PointCount;

            if (quantity < 3)
            {
                throw new InvalidOperationException("Area calculation requires at least 3 points.");
            }

            double sum1 = 0.0;
            double sum2 = 0.0;

            for (int i = 0; i < quantity - 1; i++)
            {
                sum1 += points[i].X * points[i + 1].Y;
                sum2 += points[i].Y * points[i + 1].X;
            }

            sum1 += points[quantity - 1].X * points[0].Y;
            sum2 += points[0].X * points[quantity - 1].Y;

            double area = Math.Abs(sum1 - sum2) / 2;
            return area;
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