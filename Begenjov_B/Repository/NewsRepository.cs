using Begenjov_B.Models;
using Begenjov_B.Models.News;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Begenjov_B.Repository
{
    public class NewsRepository : AbstractRepository<New>, IDisposable
    {
        private readonly SqlConnection sqlConnection;
        public NewsRepository()
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Environment.CurrentDirectory);
            builder.AddJsonFile("appsettings.json");

            var config = builder.Build();

            var connectionString = config.GetConnectionString("Users");
            sqlConnection = new SqlConnection(connectionString);
        }

        public override void Add(New item)
        {
            ArgumentNullException.ThrowIfNull(item);
            var cmd = new SqlCommand("AddNew", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", item.Id);
            cmd.Parameters.AddWithValue("@Title", item.Title);
            cmd.Parameters.AddWithValue("@Text", item.Text);
            cmd.Parameters.AddWithValue("@Date", DateTime.Now);
            cmd.Parameters.AddWithValue("@OvnerId", item.Ovner.Id);

            sqlConnection.Open();
            
            cmd.ExecuteNonQuery();

            sqlConnection.Close();
        }

        public override void Delete(New item)
        {
            var cmd = new SqlCommand("DeleteNew", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", item.Id);

            sqlConnection.Open();

            cmd.ExecuteNonQuery();

            sqlConnection.Close();
        }

        public override New Get(int id)
        {
            New @new;
            var cmd = new SqlCommand("GetNew", sqlConnection);
            using var userRepo = new UserRepository();

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);

            if (IsExist(id))
            {
                sqlConnection.Open();

                var reader = cmd.ExecuteReader();
                @new = new New()
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Text = reader.GetString(2),
                    PublesherDate = reader.GetDateTime(3),
                    Ovner = userRepo.Get(reader.GetGuid(4))
                }; 

                sqlConnection.Close();
            }
            else
            {
                @new = null;
            }

            return @new;
        }

        public List<New> GetNews()
        {
            var cmd = new SqlCommand("GetNews", sqlConnection);
            using var userRepo = new UserRepository();
            var list = new List<New>();

            sqlConnection.Open();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new New()
                {
                    Title = reader.GetString(1),
                    Text = reader.GetString(2),
                    Ovner = userRepo.Get(reader.GetGuid(3)),
                    PublesherDate = reader.GetDateTime(4),
                });
            }

            sqlConnection.Close();
            
            return list;
        }

        public override void Update(New itemUpdeted)
        {
            var cmd = new SqlCommand("UpdateNew", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Title", itemUpdeted.Title);
            cmd.Parameters.AddWithValue("@Text", itemUpdeted.Text);
            cmd.Parameters.AddWithValue("@Data", itemUpdeted.PublesherDate);
            cmd.Parameters.AddWithValue("@OvnerId", itemUpdeted.Ovner);

            sqlConnection.Open();

            var reader = cmd.ExecuteReader();

            var recExist = reader.VisibleFieldCount > 0;

            sqlConnection.Close();
        }

        public bool IsExist(int id)
        {
            var cmd = new SqlCommand("NewExist", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);

            sqlConnection.Open();

            var reader = cmd.ExecuteReader();

            sqlConnection.Open();

            var recExist = reader.VisibleFieldCount > 0;

            sqlConnection.Close();

            return recExist;
        }

        public int GetMaxId()
        {
            var cmd = new SqlCommand("Select Select COALESCE(MAX(News.Id),0) From News", sqlConnection);

            sqlConnection.Open();

            var reder = cmd.ExecuteReader();
            reder.Read();

            var maxId = reder.GetInt32(0);

            sqlConnection.Close();

            return maxId;
        }

        public void Dispose()
        {
            sqlConnection.Dispose();
        }
    }
}
