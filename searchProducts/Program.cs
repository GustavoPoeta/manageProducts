    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using MySql.Data.MySqlClient;

    namespace searchProducts
    {
        internal class Program
        {
            private static readonly Random getrandom = new Random();

            class Product
            {
                public int Id { get; }
                public string Name { get; set; }
                public string Description { get; set; }
                public string Category { get; set; }
                public decimal Price { get; set; }

                public Product()
                {
                    Id = getrandom.Next(10000, 80000);
                }
            }

            static void Main(string[] args)
            {

                // connecting to the database

                string connectionString = "Server=localhost;Port=3306;Database=products;Uid=root;Pwd=;";
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                        connection.Open();

                        int option = 4;

                        List<Product> products = new List<Product>();

                        while (option != 0)
                        {
                            if (option == 4)
                            {
                                Console.WriteLine("Welcome!");
                            }

                            Console.WriteLine("If you want to ADD a product, type 1.");
                            Console.WriteLine("If you want to REMOVE a product, type 2.");
                            Console.WriteLine("If you want to LIST all products, type 3.");
                            Console.WriteLine("If you want to exit type 0.");
                            Console.WriteLine("So the options you have are 1, 2, 3, 0");

                            // Read and validate the option
                            if (int.TryParse(Console.ReadLine(), out option))
                            {
                                switch (option)
                                {
                                    case 1:
                                        {
                                            Product product = new Product();

                                            Console.WriteLine("Insert the name of your product:");
                                            string name = Console.ReadLine();
                                            while (string.IsNullOrEmpty(name))
                                            {
                                                Console.WriteLine("Name cannot be empty. Please insert the name:");
                                                name = Console.ReadLine();
                                            }
                                            product.Name = name;

                                            Console.WriteLine("Insert the description of your product:");
                                            string description = Console.ReadLine();
                                            while (string.IsNullOrEmpty(description))
                                            {
                                                Console.WriteLine("Description cannot be empty. Please insert the description:");
                                                description = Console.ReadLine();
                                            }
                                            product.Description = description;

                                            Console.WriteLine("Insert the category of your product:");
                                            string category = Console.ReadLine();
                                            while (string.IsNullOrEmpty(category))
                                            {
                                                Console.WriteLine("Category cannot be empty. Please insert the category:");
                                                category = Console.ReadLine();
                                            }
                                            product.Category = category;

                                            Console.WriteLine("Insert the price of your product:");
                                            decimal price;
                                            while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
                                            {
                                                Console.WriteLine("Invalid input. Please insert a valid price (greater than 0):");
                                            }
                                            product.Price = price;

                                            products.Add(product);


                                            // insert into database

                                            try
                                            {

                                                string query = "insert into product values (@id, @name, @description, @category, @price)";

                                                using (var cmd = new MySqlCommand(query, connection))
                                                {
                                                    cmd.Parameters.AddWithValue("@id", product.Id);
                                                    cmd.Parameters.AddWithValue("@name", product.Name);
                                                    cmd.Parameters.AddWithValue("@description", product.Description);
                                                    cmd.Parameters.AddWithValue("@category", product.Category);
                                                    cmd.Parameters.AddWithValue("@price", product.Price);
                                                    int rowsAffected = cmd.ExecuteNonQuery();

                                                    Console.ForegroundColor = ConsoleColor.Green;
                                                    Console.WriteLine($"The product was successfully added to the database.");
                                                    Console.ResetColor();
                                                }

                                            }

                                            catch (MySqlException ex)
                                            {
                                                Console.WriteLine("Error: " + ex.Message);
                                            }
                                        }
                                        break;

                                    case 2:
                                        {
                                            Console.WriteLine("Write the id of the product you want to remove: ");
                                            Console.ForegroundColor = ConsoleColor.Cyan;
                                            int idToRemove = Convert.ToInt32(Console.ReadLine());
                                            Console.ResetColor();

                                            try
                                            {

                                                string query = "delete from product where id = @idToRemove";

                                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                                {
                                                    cmd.Parameters.AddWithValue("@idToRemove", idToRemove);

                                                    int rowsAffected = cmd.ExecuteNonQuery();
                                                    if (rowsAffected > 0)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Green;
                                                        Console.WriteLine("The product was successfully removed from database");
                                                        Console.ResetColor();
                                                    }
                                                }
                                            }
                                            catch (MySqlException e)
                                            {
                                                Console.WriteLine("error: " + e.Message);
                                            }

                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine($"{idToRemove} successfully removed!\n");
                                        }

                                        Console.ResetColor();

                                        break;

                                    case 3:
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("\nListing all products:");

                                            try
                                            {

                                                string querySelect = "SELECT * FROM product";

                                                using (MySqlCommand cmd = new MySqlCommand(querySelect, connection))
                                                {
                                                    using (MySqlDataReader reader = cmd.ExecuteReader())
                                                    {
                                                        if (reader.HasRows)
                                                        {
                                                            while (reader.Read())
                                                            {
                                                                int id = reader.GetInt32("id");
                                                                string name = reader.GetString("name");
                                                                string description = reader.GetString("description");


                                                                Console.ForegroundColor = ConsoleColor.Yellow;
                                                                Console.WriteLine($"ID: {id}, NAME: {name}, DESCRIPTION: {description}");
                                                                Console.ResetColor();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Console.ForegroundColor = ConsoleColor.Red;
                                                            Console.WriteLine("No rows found.");
                                                            Console.ResetColor();
                                                        }
                                                    }
                                                }
                                            }
                                            catch (MySqlException ex)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("Error: " + ex.Message);
                                                Console.ResetColor();
                                            }


                                            Console.ResetColor();
                                        }
                                        break;

                                    case 0:
                                        Console.WriteLine("Exiting...");
                                        break;

                                    default:
                                        Console.WriteLine("Invalid option. Please try again.");
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Invalid input. Please enter a number.");
                                option = 5; // Reset option to show menu again
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: " + ex.Message);
                    Console.ResetColor();
                }
            }
        }

    }
