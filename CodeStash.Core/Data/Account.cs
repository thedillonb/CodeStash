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

        [MaxLength(256)]
        public string AvatarUrl { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(Account))
                return false;
            Account other = (Account)obj;
            return Id == other.Id && Domain == other.Domain && Username == other.Username && Password == other.Password && AvatarUrl == other.AvatarUrl;
        }
        

        public override int GetHashCode()
        {
            unchecked
            {
                return Id.GetHashCode() ^ 
                    (Domain != null ? Domain.GetHashCode() : 0) ^ 
                    (Username != null ? Username.GetHashCode() : 0) ^ 
                    (Password != null ? Password.GetHashCode() : 0) ^ 
                    (AvatarUrl != null ? AvatarUrl.GetHashCode() : 0);
            }
        }
    }
}