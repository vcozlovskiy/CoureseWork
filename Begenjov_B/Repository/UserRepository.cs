using Begenjov_B.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Begenjov_B.Repository
{
    public class UserRepository : IDisposable
    {
        private SqlConnection sqlConnection;
        public UserRepository()
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Environment.CurrentDirectory);
            builder.AddJsonFile("appsettings.json");

            var config = builder.Build();

            var connectionString = config.GetConnectionString("Users");
            sqlConnection = new SqlConnection(connectionString);
        }

        public void Add(User user)
        {
            ArgumentNullException.ThrowIfNull(user, "User is null");
            var command = new SqlCommand("AddUser", sqlConnection);

            Guid id;
            if (user.Id != default)
            {
                id = user.Id;
            }
            else
            {
                id = Guid.NewGuid();
            }

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@FirstName", user.FirstName);
            command.Parameters.AddWithValue("@LastName", user.LastName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.CommandType = CommandType.StoredProcedure;

            sqlConnection.Open();

            int errorCode = command.ExecuteNonQuery();

            sqlConnection.Close();
        }

        public void Update(User updetedUser)
        {
            var cmd = sqlConnection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "UserUpdate";
            cmd.Parameters.AddWithValue("@Id", updetedUser.Id);
            cmd.Parameters.AddWithValue("@FirstName", updetedUser.FirstName);
            cmd.Parameters.AddWithValue("@LastName", updetedUser.LastName);
            cmd.Parameters.AddWithValue("@Email", updetedUser.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", updetedUser.PasswordHash);
            cmd.Parameters.AddWithValue("@IsActive", updetedUser.IsActive);

            sqlConnection.Open();
            cmd.ExecuteNonQuery();
            sqlConnection.Close();
        }

        public User Get(Guid guid)
        {
            var cmd = new SqlCommand("GetUser", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", guid);

            sqlConnection.Open();
            var reader = cmd.ExecuteReader();

            var user = CreateUser(reader);

            sqlConnection.Close();

            if (user.FirstName == null && user.LastName == null)
            {
                return null;
            }

            return user;
        }

        public User Get(string email)
        {
            ArgumentNullException.ThrowIfNull(email);

            var cmd = new SqlCommand("GetUserByEmail", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);

            sqlConnection.Open();
            var reader = cmd.ExecuteReader();

            var user = CreateUser(reader);

            sqlConnection.Close();

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public List<User> GetAllUsers()
        {
            var cmd = sqlConnection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAllUsers";

            sqlConnection.Open();
            var reader = cmd.ExecuteReader();
            var userList = new List<User>();
            
            while (reader != null)
            {
                userList.Add(CreateUser(reader));
            }

            sqlConnection.Close();
            return userList;
        }

        private User CreateUser(SqlDataReader reader)
        {
            User user = null;

            if (reader.Read())
            {
                user = new User();

                user.Id = reader.GetGuid(0);
                user.Email = reader.GetString(1);
                user.PasswordHash = reader.GetString(2);
                user.FirstName = reader.GetString(3);
                user.LastName = reader.GetString(4);
                user.IsActive = reader.GetBoolean(5);
            }
            else
            {
                reader = null;
            }

            return user;
        }

        public void Delete(User user)
        {
            var cmd = sqlConnection.CreateCommand();

            cmd.CommandText = "UserDelete";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", user.Id);
            sqlConnection.Open();
            
            cmd.ExecuteNonQuery();

            sqlConnection.Close();
        }

        public void Dispose()
        {
            sqlConnection.Dispose();
        }
    }
}
