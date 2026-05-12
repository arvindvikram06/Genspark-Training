using Npgsql;
namespace WordGame.Repositories
{
    public abstract class AbstractRepository
    {
        protected readonly string _connectionString = 
            "Host=localhost;Port=5432;Database=WordDb;Username=postgres;password=Arvind";

        protected NpgsqlConnection GetDBConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}