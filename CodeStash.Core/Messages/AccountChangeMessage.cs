using System;
using CodeStash.Core.Data;

namespace CodeStash.Core.Messages
{
    public class AccountChangeMessage
    {
        public Account Account { get; private set; }

        public AccountChangeMessage(Account account)
        {
            Account = account;
        }
    }
}

