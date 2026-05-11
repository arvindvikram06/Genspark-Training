using Npgsql;
using System.Net.NetworkInformation;

namespace UnderstandingADOApp
{
    
    internal class Program
    {
        string connectionString =
            "Host=localhost;Port=5432;Database=ecom;Username=postgres;Password=Arvind";
        NpgsqlConnection connection;
        public Program()
        {
          connection = new NpgsqlConnection(connectionString);
           
        }
        void GetProductDataFromDatabase()
        {
            string selectQuery = "Select * from Products";
            NpgsqlCommand command = new NpgsqlCommand(selectQuery, connection);
            try
            {
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("Product Id : " + reader[0].ToString());
                    Console.WriteLine("Product Name : " + reader[1].ToString());
                }
                Console.WriteLine("Done reading");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection?.Close();
            }

        }

        void InsertUserInToDatabase()
        {
            User user = GetUserDataFromConsole();
            string insertCmd = $"Insert into Users values('{user.id}','{user.name}','{user.password}')";
            NpgsqlCommand command = new NpgsqlCommand(insertCmd, connection);
            try
            {
                connection.Open();
                int result = command.ExecuteNonQuery();
                if(result>0)
                    Console.WriteLine("User created successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                connection?.Close();
            }
        }

    void UpdatePassword(){
    Console.WriteLine("Enter user id");
    int id;
    int.TryParse(Console.ReadLine(), out id);

    Console.WriteLine("Enter old password");
    string oldPassword = Console.ReadLine() ?? string.Empty;

    string fetchCmd = $"select * from Users where id = {id}";

    try
    {
        connection.Open();

        NpgsqlCommand command =
            new NpgsqlCommand(fetchCmd, connection);

        NpgsqlDataReader reader =
            command.ExecuteReader();

        if(reader.Read())
        {
            string dbPassword =
                reader["password"].ToString() ?? "";

            if(!oldPassword.Equals(dbPassword))
            {
                throw new Exception("Password doesn't match");
            }
        }
        else
        {
            Console.WriteLine("User not found");
            return;
        }

        reader.Close();

        Console.WriteLine("Enter new password");
        string newPass =
            Console.ReadLine() ?? string.Empty;

        string updateCmd =
            $"update Users set password = '{newPass}' where id = {id}";

        NpgsqlCommand upCommand =
            new NpgsqlCommand(updateCmd, connection);

        int rows = upCommand.ExecuteNonQuery();

        if(rows > 0)
        {
            Console.WriteLine("Update successful");
        }
        else
        {
            Console.WriteLine("Update failed");
        }
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        connection.Close();
    }
}
            
        

        private User GetUserDataFromConsole()
        {
            User user = new User();
            int id;
            Console.WriteLine("Please eneter your role");
            int.TryParse(Console.ReadLine(), out id);
            user.id = id;
            Console.WriteLine("Please eneter your preffered username");
            user.name = Console.ReadLine()??"";
            Console.WriteLine("Please eneter teh password");
            user.password = Console.ReadLine()??"";
            return user;

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            // new Program().InsertUserInToDatabase();
            new Program().UpdatePassword();

        }
    }
    public class User
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;

        public string password{get; set;}= string.Empty;
    }
}
