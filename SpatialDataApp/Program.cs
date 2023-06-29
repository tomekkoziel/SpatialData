using System;
using System.Data;
using System.Data.SqlClient;
using SpatialData;

namespace SpatialDataApp
{
    class Program
    {
        // Database connection string
        static string connectionString = "Data Source=WINSERV01;Initial Catalog=SpatialDataDB;Integrated Security=True";

        static void Main(string[] args)
        {
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                Console.WriteLine("Connected to the database.");

                while (true)
                {
                    Console.WriteLine("Choose an operation:");
                    Console.WriteLine("1. Add Point");
                    Console.WriteLine("2. Delete Point");
                    Console.WriteLine("3. Create Empty Figure");
                    Console.WriteLine("4. Delete Figure");
                    Console.WriteLine("5. Add Points to Figure");
                    Console.WriteLine("6. Display Points");
                    Console.WriteLine("7. Display Figures");
                    Console.WriteLine("8. Calculate Distance");
                    Console.WriteLine("9. Calculate Area of Figure");
                    Console.WriteLine("10. Check if Point is Inside Figure");
                    Console.WriteLine("11. Exit");

                    Console.Write("Enter your choice: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Console.Clear();
                            AddPoint(connection);
                            break;
                        case "2":
                            Console.Clear();
                            DisplayPoints(connection);
                            DeletePoint(connection);
                            break;
                        case "3":
                            Console.Clear();
                            CreateEmptyFigure(connection);
                            break;
                        case "4":
                            Console.Clear();
                            DisplayFigures(connection);
                            DeleteFigure(connection);
                            break;
                        case "5":
                            Console.Clear();
                            DisplayPoints(connection);
                            Console.WriteLine("");
                            DisplayFigures(connection);
                            AddPointsToFigure(connection);
                            break;
                        case "6":
                            Console.Clear();
                            DisplayPoints(connection);
                            break;
                        case "7":
                            Console.Clear();
                            DisplayFigures(connection);
                            break;
                        case "8":
                            Console.Clear();
                            DisplayPoints(connection);
                            CalculateDistance(connection);
                            break;
                        case "9":
                            Console.Clear();
                            DisplayFigures(connection);
                            CalculateArea(connection);
                            break;
                        case "10":
                            Console.Clear();
                            DisplayPoints(connection);
                            Console.WriteLine("");
                            DisplayFigures(connection);
                            CheckPointInsideFigure(connection);
                            break;
                        case "11":
                            Console.Clear();
                            Console.WriteLine("Exiting the program.");
                            return;
                        default:
                            Console.Clear();
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }

                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Disconnected from the database.");
                }
            }
        }

        static void AddPoint(SqlConnection connection)
        {
            Console.Write("Enter X coordinate: ");
            double x = double.Parse(Console.ReadLine());

            Console.Write("Enter Y coordinate: ");
            double y = double.Parse(Console.ReadLine());

            try
            {
                SqlCommand command = new SqlCommand("INSERT INTO Points (point) VALUES (@point)", connection);
                command.Parameters.AddWithValue("@point", $"{x},{y}");
                command.ExecuteNonQuery();
                Console.WriteLine("Point added successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error adding point: " + e.Message);
            }
        }

        static void DeletePoint(SqlConnection connection)
        {
            Console.Write("Enter Point ID: ");
            int pointId = int.Parse(Console.ReadLine());

            try
            {
                SqlCommand command = new SqlCommand("DELETE FROM Points WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", pointId);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine("Point deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Point not found.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error deleting point: " + e.Message);
            }
        }

        static void CreateEmptyFigure(SqlConnection connection)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Figures (figure) VALUES (@figure)";
                SqlParameter parameter = new SqlParameter("@figure", SqlDbType.Udt);
                parameter.UdtTypeName = "dbo.Figure";
                parameter.Value = Figure.Null;
                command.Parameters.Add(parameter);
                command.ExecuteNonQuery();
            }

            Console.WriteLine("Empty figure created successfully.");
        }

        static void DeleteFigure(SqlConnection connection)
        {
            Console.Write("Enter Figure ID: ");
            int figureId = int.Parse(Console.ReadLine());

            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Figures WHERE id = @figureId";
                command.Parameters.AddWithValue("@figureId", figureId);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Figure deleted successfully.");
                }
                else
                {
                    Console.WriteLine("Figure not found.");
                }
            }
        }

        static void AddPointsToFigure(SqlConnection connection)
        {
            Console.Write("Enter Figure ID: ");
            int figureId = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Point IDs (comma-separated): ");
            string[] pointIds = Console.ReadLine().Split(',');

            try
            {
                SqlCommand command = new SqlCommand("UPDATE Figures SET figure.ImportPoints = (SELECT point FROM Points WHERE id = @pointId) WHERE id = @figureId", connection);
                command.Parameters.AddWithValue("@figureId", figureId);
                SqlParameter pointIdParameter = command.Parameters.AddWithValue("@pointId", SqlDbType.Int);

                foreach (string pointId in pointIds)
                {
                    pointIdParameter.Value = int.Parse(pointId);
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Points added to the figure successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error adding points to the figure: " + e.Message);
            }
        }

        static void DisplayPoints(SqlConnection connection)
        {
            Console.WriteLine("Displaying points:");

            using (SqlCommand command = new SqlCommand("SELECT id, point FROM Points", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        Point point = (Point)reader.GetValue(1); // Cast the value to the Point type

                        Console.WriteLine($"ID: {id}, Point: {point}");
                    }
                }
            }
        }

        static void DisplayFigures(SqlConnection connection)
        {
            try
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Figures", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        Figure figure = (Figure)reader.GetValue(1); // Cast the value to the Point type

                        Console.WriteLine($"ID: {id}, Figure: {figure}");
                    }
                }
                else
                {
                    Console.WriteLine("No figures found.");
                }

                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error displaying figures: " + e.Message);
            }
        }

        static void CalculateDistance(SqlConnection connection)
        {
            Console.Write("Enter Point 1 ID: ");
            int point1Id = int.Parse(Console.ReadLine());

            Console.Write("Enter Point 2 ID: ");
            int point2Id = int.Parse(Console.ReadLine());

            try
            {
                SqlCommand command = new SqlCommand("SELECT point.Distance((SELECT point FROM Points WHERE id = @point1Id)) AS distance FROM Points WHERE id = @point2Id", connection);
                command.Parameters.AddWithValue("@point1Id", point1Id);
                command.Parameters.AddWithValue("@point2Id", point2Id);
                SqlDataReader reader = command.ExecuteReader();

                double distance = 0;

                if (reader.HasRows)
                {
                    reader.Read();
                    distance = reader.GetDouble(0);
                }

                reader.Close();

                Console.WriteLine($"Distance between Point {point1Id} and Point {point2Id}: {distance}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error calculating distance: " + e.Message);
            }
        }

        static void CalculateArea(SqlConnection connection)
        {
            Console.Write("Enter Figure ID: ");
            int figureId = int.Parse(Console.ReadLine());

            try
            {
                SqlCommand command = new SqlCommand("SELECT figure.CalculateArea() AS area FROM Figures WHERE id = @figureId", connection);
                command.Parameters.AddWithValue("@figureId", figureId);
                SqlDataReader reader = command.ExecuteReader();

                double area = 0;

                if (reader.HasRows)
                {
                    reader.Read();
                    area = reader.GetDouble(0);
                }

                reader.Close();

                Console.WriteLine($"Area of Figure {figureId}: {area}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error calculating area: " + e.Message);
            }
        }

        static void CheckPointInsideFigure(SqlConnection connection)
        {
            Console.Write("Enter Point ID: ");
            int pointId = int.Parse(Console.ReadLine());

            Console.Write("Enter Figure ID: ");
            int figureId = int.Parse(Console.ReadLine());

            try
            {
                SqlCommand command = new SqlCommand("SELECT figure.IsInside((SELECT point FROM Points WHERE id = @pointId)) AS result FROM Figures WHERE id = @figureId", connection);
                command.Parameters.AddWithValue("@pointId", pointId);
                command.Parameters.AddWithValue("@figureId", figureId);
                SqlDataReader reader = command.ExecuteReader();

                bool isInside = false;

                if (reader.HasRows)
                {
                    reader.Read();
                    isInside = reader.GetBoolean(0);
                }

                reader.Close();

                if (isInside)
                {
                    Console.WriteLine($"Point {pointId} is inside Figure {figureId}.");
                }
                else
                {
                    Console.WriteLine($"Point {pointId} is not inside Figure {figureId}.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error checking point inside figure: " + e.Message);
            }
        }
    }
}