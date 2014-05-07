using Xamarin.Utilities.Core.Persistence;

namespace CodeStash.Core.Data
{
    public class Accounts : DatabaseCollection<Account, int>
    {
        public Accounts(SQLite.SQLiteConnection sqlConnection)
            : base(sqlConnection)
        {
        }
    }
}