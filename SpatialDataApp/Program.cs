using System;
using SpatialDataLibrary;

namespace SpatialDataApp
{
    class Program
    {
        static Figure figure = new Figure();

        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                DisplayMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddPoint();
                        break;
                    case "2":
                        DisplayPoints();
                        break;
                    case "3":
                        CheckPointInside();
                        break;
                    case "4":
                        CalculateFigureArea();
                        break;
                    case "5":
                        CalculateDistance();
                        break;
                    case "6":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine();
            }
        }

        static void DisplayMenu()
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Add a point");
            Console.WriteLine("2. Display points");
            Console.WriteLine("3. Check if a point is inside the figure");
            Console.WriteLine("4. Calculate figure area");
            Console.WriteLine("5. Calculate distance between two points");
            Console.WriteLine("6. Exit");
            Console.Write("Enter your choice: ");
        }

        static void AddPoint()
        {
            Console.Write("Enter X coordinate: ");
            double x = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Y coordinate: ");
            double y = Convert.ToDouble(Console.ReadLine());

            Point point = new Point(x, y);
            figure.AddPoint(point);

            Console.WriteLine("Point added successfully.");
        }

        static void DisplayPoints()
        {
            Console.WriteLine("Points:");
            foreach (Point point in figure.points)
            {
                Console.WriteLine(point);
            }
        }

        static void CheckPointInside()
        {
            Console.Write("Enter X coordinate: ");
            double x = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Y coordinate: ");
            double y = Convert.ToDouble(Console.ReadLine());

            Point point = new Point(x, y);
            bool isInside = figure.IsInside(point);

            Console.WriteLine("Is point inside the figure? " + isInside);
        }

        static void CalculateFigureArea()
        {
            double area = figure.CalculateArea();
            Console.WriteLine("Figure area: " + area);
        }

        static void CalculateDistance()
        {
            Console.WriteLine("Points:");
            for (int i = 0; i < figure.points.Count; i++)
            {
                Console.WriteLine("Point " + i + ": " + figure.points[i]);
            }

            Console.WriteLine();
            Console.WriteLine("Enter the indices of the two points to calculate distance (starting from 0):");

            Console.Write("Index of the first point: ");
            int index1 = Convert.ToInt32(Console.ReadLine());

            Console.Write("Index of the second point: ");
            int index2 = Convert.ToInt32(Console.ReadLine());

            if (index1 >= 0 && index1 < figure.points.Count && index2 >= 0 && index2 < figure.points.Count)
            {
                Point point1 = figure.points[index1];
                Point point2 = figure.points[index2];

                double distance = point1.Distance(point2);
                Console.WriteLine("Distance between point " + index1 + " and point " + index2 + ": " + distance);
            }
            else
            {
                Console.WriteLine("Invalid point indices.");
            }
        }
    }
}