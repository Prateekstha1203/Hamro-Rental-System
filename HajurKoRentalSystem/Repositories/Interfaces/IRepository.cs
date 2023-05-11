using System.Linq.Expressions;

namespace HajurKoRentalSystem.Repositories.Interfaces
{
    public interface IRepository <T> where T : class
    {
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);

        T Get(int ID);

        T Retrieve(int? ID);

        T Retrieve(string ID);

        List<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null);

        void Add(T entity);

        void Update(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}
