using MauiApiStress.Entity;
using MauiApiStress.Database;

namespace MauiApiStress.Database
{
    public class PersonDB : BaseDB
    {
        public async Task<List<Person>> GetPeople()
        {
            var dbConnection = await GetDatabaseConnection<Person>();

            return await AttemptAndRetry(() => Task.FromResult(dbConnection.Table<Person>().ToList())).ConfigureAwait(false);
        }

        public async Task<int> DeletePeople()
        {
            var dbConnection = await GetDatabaseConnection<Person>();

            return dbConnection.DeleteAll<Person>();
        }

        public async Task<int> GetPeopleAmount()
        {
            var dbConnection = await GetDatabaseConnection<Person>();
            return dbConnection.Table<Person>().ToList().Count;
        }

        public async Task<int> AddPerson(Person person)
        {
            var dbConnection = await GetDatabaseConnection<Person>();
            return dbConnection.Insert(person);
        }

        public async Task<int> AddOrReplacePerson(Person person)
        {
            var dbConnection = await GetDatabaseConnection<Person>();
            return dbConnection.InsertOrReplace(person);
        }

        public async Task<int> AddPeopleBulk(List<Person> person)
        {
            var dbConnection = await GetDatabaseConnection<Person>();
            return dbConnection.InsertAll(person);
        }
    }
}
