using Begenjov_B.Controllers;
using Begenjov_B.Models;
using Begenjov_B.Models.History;
using Begenjov_B.Models.News;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Begenjov_B.Repository
{
    public class HistoryRepository : AbstractRepository<HistoryModel>
    {
        private readonly SqlConnection sqlConnection;
        public HistoryRepository()
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Environment.CurrentDirectory);
            builder.AddJsonFile("appsettings.json");

            var config = builder.Build();

            var connectionString = config.GetConnectionString("Users");
            sqlConnection = new SqlConnection(connectionString);
        }

        public override void Add(HistoryModel item)
        {
            ArgumentNullException.ThrowIfNull(item);
            var cmd = new SqlCommand("AddHistory", sqlConnection);
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

        public override void Delete(HistoryModel item)
        {
            var cmd = new SqlCommand("DeleteHistory", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", item.Id);

            sqlConnection.Open();

            cmd.ExecuteNonQuery();

            sqlConnection.Close();
        }

        public override HistoryModel Get(int id)
        {
            HistoryModel hisotory;
            var cmd = new SqlCommand("GetHistory", sqlConnection);
            using var userRepo = new UserRepository();

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);

            if (IsExist(id))
            {
                sqlConnection.Open();

                var reader = cmd.ExecuteReader();
                reader.Read();
                hisotory = new HistoryModel()
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

        public List<HistoryModel> GetHistories()
        {
            var cmd = new SqlCommand("GetHistories", sqlConnection);
            using var userRepo = new UserRepository();
            var list = new List<HistoryModel>();

            sqlConnection.Open();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new HistoryModel()
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

        public override void Update(HistoryModel itemUpdeted)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(int id)
        {
            var cmd = new SqlCommand("HistoryExist", sqlConnection);
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
            var cmd = new SqlCommand("Select COALESCE(MAX(Histories.Id), 0) From Histories", sqlConnection);

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
