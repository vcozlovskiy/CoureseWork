using Begenjov_B.Models.Account;
using Begenjov_B.Models.Theory;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Begenjov_B.Repository
{
    public class TheoryRepository : AbstractRepository<Theory>, IDisposable
    {
        private readonly SqlConnection sqlConnection;
        public TheoryRepository()
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Environment.CurrentDirectory);
            builder.AddJsonFile("appsettings.json");

            var config = builder.Build();

            var connectionString = config.GetConnectionString("Users");
            sqlConnection = new SqlConnection(connectionString);
        }

        public override void Add(Theory item)
        {
            ArgumentNullException.ThrowIfNull(item);
            var cmd = new SqlCommand("AddTheory", sqlConnection);
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

        public override void Delete(Theory item)
        {
            var cmd = new SqlCommand("DeleteTheory", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", item.Id);

            sqlConnection.Open();

            cmd.ExecuteNonQuery();

            sqlConnection.Close();
        }

        public override Theory Get(int id)
        {
            Theory hisotory;
            var cmd = new SqlCommand("GetTheory", sqlConnection);
            using var userRepo = new UserRepository();

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);

            if (IsExist(id))
            {
                sqlConnection.Open();

                var reader = cmd.ExecuteReader();
                reader.Read();
                hisotory = new Theory()
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Text = reader.GetString(2),
                    Ovner = userRepo.Get(reader.GetGuid(3)),
                    PublesherDate = reader.GetDateTime(4)
                };

                sqlConnection.Close();
            }
            else
            {
                hisotory = null;
            }

            return hisotory;
        }

        public List<Theory> GetTheories()
        {
            var cmd = new SqlCommand("GetTheories", sqlConnection);
            using var userRepo = new UserRepository();
            var list = new List<Theory>();

            sqlConnection.Open();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Theory()
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Text = reader.GetString(2),
                    Ovner = userRepo.Get(reader.GetGuid(3)),
                    PublesherDate = reader.GetDateTime(4),
                });
            }

            sqlConnection.Close();

            return list;
        }

        public override void Update(Theory itemUpdeted)
        {
            var cmd = new SqlCommand("UpdateTheory", sqlConnection);
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
            var cmd = new SqlCommand("TheoryExist", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);

            sqlConnection.Open();

            var reader = cmd.ExecuteReader();

            var recExist = reader.VisibleFieldCount > 0;

            sqlConnection.Close();

            return recExist;
        }

        public int GetMaxId()
        {
            var cmd = new SqlCommand("Select COALESCE(MAX(Theory.Id), 0) From Theory", sqlConnection);

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
