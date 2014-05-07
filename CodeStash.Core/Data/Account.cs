using SQLite;
using Xamarin.Utilities.Core.Persistence;

namespace CodeStash.Core.Data
{
    public class Account : IDatabaseItem<int>
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [MaxLength(256)]
        public string Domain { get; set; }

        [MaxLength(128)]
        public string Username { get; set; }

        [MaxLength(64)]
        public string Password { get; set; }
    }
}