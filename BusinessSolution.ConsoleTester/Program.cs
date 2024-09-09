using System;
using System.Linq;

namespace DataAccessConsoleTester
{
    class Program
    {
        static void Main(string[] args)
        {
            IAccountDAO dataAccess = new AccountDAO(@"Data Source=.; Initial Catalog=Company; Integrated Security=true;");
            TestInsertAccount(dataAccess);
            TestRetrieveAccount(dataAccess);
            TestUpdateAccount(dataAccess);
            TestDeleteAccount(dataAccess);
            TestInsertSourceAccount(dataAccess);
            TestInsertDestinationAccount(dataAccess);
            TestMoveFunds(dataAccess);
            TestFindAccounts(dataAccess);
            TestDeleteAllAccounts(dataAccess);
        }

        static void TestInsertAccount(IAccountDAO dataAccess)
        {
            Account newAccount = new Account() { Name = "My account", Balance = 42 };
            Console.WriteLine("Inserting new account");
            dataAccess.Insert(newAccount);
            Console.WriteLine("New account inserted. New id was: " + newAccount.Id);
        }

        static void TestRetrieveAccount(IAccountDAO dataAccess)
        {
            Console.WriteLine();
            Console.WriteLine("Retrieving account");
            Account newAccount = new Account() { Name = "My account", Balance = 42 };
            dataAccess.Insert(newAccount);
            Account retrievedAccount = dataAccess.Get(newAccount.Id);
            Console.WriteLine($"Newly created account retrieved again: {retrievedAccount}");
        }

        static void TestUpdateAccount(IAccountDAO dataAccess)
        {
            Console.WriteLine();
            Console.WriteLine("Updating new account");
            Account newAccount = new Account() { Name = "My account", Balance = 42 };
            dataAccess.Insert(newAccount);
            newAccount.Name = "New, improved name";
            dataAccess.Update(newAccount);
            Account updatedAccount = dataAccess.Get(newAccount.Id);
            Console.WriteLine($"Updated account from database: {updatedAccount}");
        }

        static void TestDeleteAccount(IAccountDAO dataAccess)
        {
            Console.WriteLine();
            Console.WriteLine("Deleting new account");
            Account newAccount = new Account() { Name = "My account", Balance = 42 };
            dataAccess.Insert(newAccount);
            dataAccess.Delete(newAccount.Id);
            Account refoundAccount = dataAccess.Get(newAccount.Id);
            if (refoundAccount == null)
            {
                Console.WriteLine($"Account was deleted from database.");
            }
        }

        static void TestInsertSourceAccount(IAccountDAO dataAccess)
        {
            Console.WriteLine();
            Account sourceAccount = new Account() { Name = "Source account", Balance = 100 };
            Console.WriteLine($"Inserting source account: {sourceAccount}.");
            dataAccess.Insert(sourceAccount);
            Console.WriteLine("Source account inserted. New id was: " + sourceAccount.Id);
        }

        static void TestInsertDestinationAccount(IAccountDAO dataAccess)
        {
            Console.WriteLine();
            Account destinationAccount = new Account() { Name = "Destination account", Balance = 100 };
            Console.WriteLine($"Inserting destination account: {destinationAccount}.");
            dataAccess.Insert(destinationAccount);
            Console.WriteLine("Destination account inserted. New id was: " + destinationAccount.Id);
        }

        static void TestMoveFunds(IAccountDAO dataAccess)
        {
            Console.WriteLine();
            Account sourceAccount = new Account() { Name = "Source account", Balance = 100 };
            Account destinationAccount = new Account() { Name = "Destination account", Balance = 100 };
            dataAccess.Insert(sourceAccount);
            dataAccess.Insert(destinationAccount);
            int amountToMove = 50;
            Console.WriteLine($"Moving {amountToMove} from {sourceAccount.Name} to {destinationAccount.Name}");
            dataAccess.TransferFunds(sourceAccount.Id, destinationAccount.Id, amountToMove);
        }

        static void TestFindAccounts(IAccountDAO dataAccess)
        {
            Console.WriteLine();
            Console.WriteLine("Finding accounts:");
            var accounts = dataAccess.FindAccountsFromPartOfName("account");
            foreach (var account in accounts)
            {
                Console.WriteLine(account);
            }
        }

        static void TestDeleteAllAccounts(IAccountDAO dataAccess)
        {
            Console.WriteLine();
            Console.WriteLine("Deleting all accounts");
            var accountsToDelete = dataAccess.GetAll();
            foreach (var account in accountsToDelete)
            {
                dataAccess.Delete(account.Id);
            }
            int numberOfAccountsLeft = dataAccess.GetAll().Count();
            if (numberOfAccountsLeft == 0)
            {
                Console.WriteLine($"All accounts were deleted from database.");
            }
        }
    }
}