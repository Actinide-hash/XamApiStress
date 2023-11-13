using System.ComponentModel;
using System.Diagnostics;
using MauiApiStress.Database;
using MauiApiStress.Entity;

namespace MauiApiStress.Model
{
    public class MainModel : INotifyPropertyChanged
    {
        private PersonDB db = new PersonDB();

        public MainModel()
        {
            Task.Run(async () =>
            {
                await DeleteAllData();
            });
            //DeleteAllData()

        }

        private List<Person> list;
        public List<Person> List
        {
            get { return list; }
            set
            {
                list = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(List)));
            }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set
            {
                count = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
        }

        private string insertMilisecs = "0";
        public string InsertMilisecs
        {
            get => insertMilisecs;
            set
            {
                insertMilisecs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InsertMilisecs)));
            }
        }

        private string insertOrReplaceMilisecs = "0";
        public string InsertOrReplaceMilisecs
        {
            get => insertOrReplaceMilisecs;
            set
            {
                insertOrReplaceMilisecs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InsertOrReplaceMilisecs)));
            }
        }

        private string bulkInsertMilisecs = "0";
        public string BulkInsertMilisecs
        {
            get => bulkInsertMilisecs;
            set
            {
                bulkInsertMilisecs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BulkInsertMilisecs)));
            }
        }

        private string listLoadMilisecs = "0";
        public string ListLoadMilisecs
        {
            get => listLoadMilisecs;
            set
            {
                listLoadMilisecs = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListLoadMilisecs)));
            }
        }

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set
            {
                isRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public async Task RunInsertTask()
        {
            isRunning = true;
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            try
            {
                for (int i = 0; i < 50000; i++)
                {
                    Person data = GeneratePerson(i);
                    //var data = new Person { ID = i + 1, Name = $"Person_{i}" };
                    await db.AttemptAndRetry(() => db.AddPerson(data));
                }

                await LoadList();
                Count = await db.GetPeopleAmount();
                stopwatch.Stop();
                InsertMilisecs = stopwatch.ElapsedMilliseconds.ToString();
                await App.Current.MainPage.DisplayAlert("Data Insertion", "Data inserted successfully in " + InsertMilisecs + "ms", "OK"); ;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            isRunning = false;
        }


        public async Task RunInsertOrReplaceTask()
        {
            isRunning = true;
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            try
            {
                for (int i = 0; i < 50000; i++)
                {
                    Person data = GeneratePerson(i);
                    //var data = new Person { ID = i + 1, Name = $"Person_{i}" };
                    await db.AttemptAndRetry(() => db.AddOrReplacePerson(data));
                }

                await LoadList();
                Count = await db.GetPeopleAmount();
                stopwatch.Stop();
                InsertOrReplaceMilisecs = stopwatch.ElapsedMilliseconds.ToString();
                await App.Current.MainPage.DisplayAlert("Data Insertion", "Data inserted successfully in " + InsertOrReplaceMilisecs + "ms", "OK"); ;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            isRunning = false;
        }

        public async Task RunBulkInsertTask()
        {
            isRunning = true;
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            try
            {
                //Person data;
                List<Person> list = new List<Person>();
                for (int i = 0; i < 50000; i++)
                {
                    //data = new Person { ID = i + 1, Name = $"Person_{i}" };
                    Person data = GeneratePerson(i);
                    list.Add(data);
                }

                await db.AttemptAndRetry(() => db.AddPeopleBulk(list));

                await LoadList();
                Count = await db.GetPeopleAmount();
                stopwatch.Stop();
                BulkInsertMilisecs = stopwatch.ElapsedMilliseconds.ToString();
                await App.Current.MainPage.DisplayAlert("Data Insertion", "Data inserted successfully in " + BulkInsertMilisecs + "ms", "OK"); ;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            isRunning = false;
        }

        public async Task DeleteAllData()
        {
            var db = new PersonDB();

            await db.DeletePeople();
            await LoadList();
            Count = await db.GetPeopleAmount();

            await App.Current.MainPage.DisplayAlert("Data Deleted", "Data deleted successfully!", "OK");
        }

        private async Task LoadList()
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                var res = db.GetDatabaseConnection<Person>().Result.Query<Person>("SELECT * FROM person");
                //List = db.GetPeople().Result.ToList();
                stopwatch.Stop();
                ListLoadMilisecs = stopwatch.ElapsedMilliseconds.ToString();
                List = res.ToList();
                Count = await db.GetPeopleAmount();
            }
            catch
            {
                Debug.WriteLine("Problem with the person list addition");
            }
        }

        private static Person GeneratePerson(int Id)
        {
            Person person = new Person();
            // ID
            person.ID = Id;

            // Name
            Random rnd = new Random();
            int nameLen = rnd.Next(2, 7);
            person.Name = GenerateName(nameLen);

            // Salary
            person.Salary = (decimal)rnd.Next(1000, 10000);

            // Weight
            person.Weight = (decimal)rnd.Next(40, 120);

            // Date Of Birth
            DateTime start = new DateTime(1950, 1, 1);
            int range = (DateTime.Today - start).Days;
            person.DateOfBirth = start.AddDays(rnd.Next(range));

            return person;
        }

        private static string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }
    }
}


