namespace DALLibrary.Interfaces
{
    public interface IGenericRepository<T>
    {
        void Add(T obj);

        void Update(T obj);

        T? GetById(int id);

        IEnumerable<T> GetAll();

        void Delete(T entity);
    }
}