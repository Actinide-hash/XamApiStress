using SQLite;

namespace MauiApiStress.Database.Interface
{
    public interface IBaseDB
    {
        Task<SQLiteConnection> GetDatabaseConnection<T>();
        Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries);
    }
}
