namespace Begenjov_B.Repository
{
    public abstract class AbstractRepository<T> where T : new()
    {
        public abstract void Add(T item);

        public abstract void Update(T itemUpdeted);

        public abstract void Delete(T item);

        public abstract T Get(int id);
    }
}
