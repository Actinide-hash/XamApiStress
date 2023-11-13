using Polly;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MauiApiStress.Database.Interface;
using MauiApiStress.Entity;

namespace MauiApiStress.Database
{
    public class BaseDB : IBaseDB
    {
        private static Lazy<SQLiteConnection> LazyDatabaseConnection = new Lazy<SQLiteConnection>(CreateDatabaseConnection);
        static readonly string _databasePath = Path.Combine(FileSystem.AppDataDirectory, "MauiSqliteDatabase.db3");

        static SQLiteConnection DBConnection => LazyDatabaseConnection.Value;
        private static SQLiteConnection CreateDatabaseConnection()
        {
            var conn = new SQLiteConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
            conn.EnableWriteAheadLogging();
            conn.Trace = false;
            return conn;
        }
        public Task<SQLiteConnection> GetDatabaseConnection<T>()
        {
            
            if (!DBConnection.TableMappings.Any(x => x.MappedType == typeof(T)))
            {
                DBConnection.EnableWriteAheadLogging();
                DBConnection.CreateTables(CreateFlags.None, typeof(T));
            }

            return Task.FromResult(DBConnection);
        }

        private static TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
        public Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 10)
        {
            return Policy.Handle<SQLite.SQLiteException>().WaitAndRetry(numRetries, pollyRetryAttempt).Execute(action);
        }
    }
}
