using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XamApiStress.Database.Interface
{
    public interface IBaseDB
    {
        Task<SQLiteConnection> GetDatabaseConnection<T>();
        Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries);
    }
}
