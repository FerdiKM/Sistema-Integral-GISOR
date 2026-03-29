using System.Linq.Expressions;

namespace GisorSystem.Repositories
{
    // Patrón GOF: Repository (Interfaz Genérica)
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        // Método para buscar con condiciones (filtro)
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}