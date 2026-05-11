using Npgsql;
namespace NotifyDALLibrary.Repositories
{
    public abstract class AbstractRepository
    {
        protected readonly string _connectionString = 
            "Host=localhost;Port=5432;Database=DummyDb;Username=postgres;password=Arvind";

        protected NpgsqlConnection GetDBConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}