using SQLite;

namespace MauiApiStress.Entity
{
    [SQLite.Table("person")]
    public class Person
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public decimal Weight { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
