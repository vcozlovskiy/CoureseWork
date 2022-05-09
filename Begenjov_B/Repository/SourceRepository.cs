using Begenjov_B.Models.Source;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Begenjov_B.Repository
{
    public class SourceRepository : AbstractRepository<Source>, IDisposable
    {
        private SqlConnection sqlConnection;
        public SourceRepository()
        {
            var builder = new ConfigurationBuilder();

            builder.SetBasePath(Environment.CurrentDirectory);
            builder.AddJsonFile("appsettings.json");

            var config = builder.Build();

            var connectionString = config.GetConnectionString("Users");
            sqlConnection = new SqlConnection(connectionString);
        }
        public override void Add(Source item)
        {
            ArgumentNullException.ThrowIfNull(item);
            var cmd = new SqlCommand("AddSource", sqlConnection);
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

        public override void Delete(Source item)
        {
            var cmd = new SqlCommand("DeleteSource", sqlConnection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", item.Id);

            sqlConnection.Open();

            cmd.ExecuteNonQuery();

            sqlConnection.Close();
        }

        public override Source Get(int id)
        {
            Source hisotory;
            var cmd = new SqlCommand("GetSource", sqlConnection);
            using var userRepo = new UserRepository();

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Id", id);

            if (IsExist(id))
            {
                sqlConnection.Open();

                var reader = cmd.ExecuteReader();
                reader.Read();
                hisotory = new Source()
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

        public override void Update(Source itemUpdeted)
        {
            var cmd = new SqlCommand("UpdateSource", sqlConnection);
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
        public List<Source> GetTheories()
        {
            var cmd = new SqlCommand("GetSources", sqlConnection);
            using var userRepo = new UserRepository();
            var list = new List<Source>();

            sqlConnection.Open();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Source()
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

        public bool IsExist(int id)
        {
            var cmd = new SqlCommand("SourceExist", sqlConnection);
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
            var cmd = new SqlCommand("Select COALESCE(MAX(Source.Id), 0) From Source", sqlConnection);

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
