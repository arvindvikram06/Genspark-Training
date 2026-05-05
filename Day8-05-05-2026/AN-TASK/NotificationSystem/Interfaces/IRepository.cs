// Generic interface for Repo
namespace NotificationSystem.Interfaces
{
    public interface IRepository<K,T> where T : class
    {
        public T Create(T obj);

        public T? Update(K id,T obj);

        public T? GetById(K id);

        public List<T>? GetAll();

        public T? Delete(K key);
    }
}