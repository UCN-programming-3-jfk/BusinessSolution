using System;
using System.Linq;
namespace DataAccessConsoleTester;
class Program
{
    static void Main(string[] args)
    {
        IAccountDAO dataAccess =
            new AccountDAO(@"Data Source=.; Initial Catalog=Company; Integrated Security=true;");

        //Insert account
        Account newAccount = new Account() { Name = "My account", Balance = 42 };
        Console.WriteLine("Inserting new account");            
        dataAccess.Insert(newAccount);
        Console.WriteLine("New account inserted. New id was: " + newAccount.Id);

        //Retrieving account
        Console.WriteLine();
        Console.WriteLine("Retrieving account");
        Account retrievedAccount = dataAccess.Get(newAccount.Id);
        Console.WriteLine($"Newly created account retrieved again: {retrievedAccount}");

        //Update account
        Console.WriteLine();
        Console.WriteLine("Updating new account");
        newAccount.Name = "New, improved name";
        dataAccess.Update(newAccount);
        Account updatedAccount = dataAccess.Get(newAccount.Id);
        Console.WriteLine($"Updated account from database: {updatedAccount}");

        //Delete account
        Console.WriteLine();
        Console.WriteLine("Deleting new account");
        dataAccess.Delete(newAccount.Id);
        Account refoundAccount = dataAccess.Get(newAccount.Id);
        if (refoundAccount == null) 
        {
            Console.WriteLine($"Account was deleted from database.");
        }

        //Insert source account
        Console.WriteLine();
        Account sourceAccount = new Account() { Name = "Source account", Balance = 100 };
        Console.WriteLine($"Inserting source account: {sourceAccount}.");
        dataAccess.Insert(sourceAccount);
        Console.WriteLine("Source account inserted. New id was: " + sourceAccount.Id);

        //Insert destination account
        Console.WriteLine();
        Account destinationAccount = new Account() { Name = "Destination account", Balance = 100 };
        Console.WriteLine($"Inserting destination account: {destinationAccount}.");
        dataAccess.Insert(destinationAccount);
        Console.WriteLine("Destination account inserted. New id was: " + destinationAccount.Id);

        //Move funds between accounts 
        Console.WriteLine();
        int amountToMove = 50;
        Console.WriteLine($"Moving {amountToMove} from {destinationAccount.Name} to {destinationAccount.Name}");
        dataAccess.TransferFunds(sourceAccount.Id, destinationAccount.Id, amountToMove);

        //Find accounts
        Console.WriteLine();
        Console.WriteLine("Finding accounts:");
        var accounts = dataAccess.FindAccountsFromPartOfName("account");
        foreach (var account in accounts)
        {
            Console.WriteLine(account);
        }

        //Delete all accounts
        Console.WriteLine();
        Console.WriteLine("Deleting all accounts");
        var accountsToDelete = dataAccess.GetAll();
        foreach (var account in accountsToDelete)
        {
            dataAccess.Delete(account.Id);
        }
        //add "using System.Linq;" to your code
        //to enable the .Count() method on IEnumerable
        int numberOfAccountsLeft = dataAccess.GetAll().Count();
        if (numberOfAccountsLeft == 0)
        {
            Console.WriteLine($"All accounts were deleted from database.");
        }
    }
}