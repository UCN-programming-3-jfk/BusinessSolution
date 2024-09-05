namespace TestingDataAccess;

/// <summary>
/// Class for testing the AccountDAO implementation.
/// Note that two developers running the tests simultaneously
/// may give errors, though the code is working fine.
/// To reduce the likelyhood of this, add a developer specific string to test data,
/// E.g.
/// var newAccount = new Account() { Name = "[TESTDATA" + developerName + "] Test Account", Balance = 42 };
/// so cleanup only deletes test data from this developer
/// </summary>
public class Tests
{

    #region Variables
    private List<int> _idsToCleanUp = new(); //ids of test-data accounts to delete after each test
    private IAccountDAO _dataAccess;    
    #endregion

    #region Setup and clean up

    [OneTimeSetUp]
    public void SetupDao()
    {
        _dataAccess = new AccountDAO(@"Data Source=.; Initial Catalog=Company; Integrated Security=true;");
    }

    //this cleanup is performed for each test run
    [TearDown]
    public void CleanUp()
    {
        foreach (int id in _idsToCleanUp)
        {
            _dataAccess.Delete(id);
        }
    }

    /// <summary>
    /// This cleanup is performed at the end of all tests
    /// to remove all data marked [TESTDATA].
    /// This is done, because a failed test 
    /// may have exited before cleaning up after itself.
    /// </summary>
    [OneTimeTearDown]
    public void DeleteAllTestData()
    {
        var testAccounts = _dataAccess.FindAccountsFromPartOfName("[TESTDATA]");
        foreach (var account in testAccounts)
        {
            _dataAccess.Delete(account.Id);
        }
    }
    #endregion

    #region Tests

    [Test]
    public void TestInsertAccount()
    {
        // Arrange: create new account object
        var newAccount = new Account() { Name = "[TESTDATA] Test Account", Balance = 42 };

        //Act: insert it into the database
        _dataAccess.Insert(newAccount);
        _idsToCleanUp.Add(newAccount.Id);

        // Assert: account was inserted with a valid ID
        Assert.That(newAccount.Id, Is.GreaterThan(0), "Account ID should be greater than 0 after insertion.");
    }

    [Test]
    public void TestRetrieveAccount()
    {
        // Arrange
        var newAccount = new Account() { Name = "[TESTDATA] Test Account", Balance = 42 };
        _dataAccess.Insert(newAccount);
        _idsToCleanUp.Add(newAccount.Id);

        // Act
        var retrievedAccount = _dataAccess.Get(newAccount.Id);

        // Assert that the account retrieved matches the inserted one
        Assert.That(retrievedAccount, Is.Not.Null, "Retrieved account should not be null.");
        Assert.That(retrievedAccount.Id, Is.EqualTo(newAccount.Id), "Account ID should match.");
        Assert.That(retrievedAccount.Name, Is.EqualTo(newAccount.Name), "Account Name should match.");
        Assert.That(retrievedAccount.Balance, Is.EqualTo(newAccount.Balance), "Account Balance should match.");
    }

    [Test]
    public void TestUpdateAccount()
    {
        // Insert account
        var newAccount = new Account() { Name = "[TESTDATA] Test Account", Balance = 42 };
        _dataAccess.Insert(newAccount);
        _idsToCleanUp.Add(newAccount.Id);

        // Update account
        newAccount.Name = "Updated Account";
        _dataAccess.Update(newAccount);

        // Retrieve updated account
        var updatedAccount = _dataAccess.Get(newAccount.Id);

        // Assert that the updated account matches the expected values
        Assert.That(updatedAccount.Name, Is.EqualTo("Updated Account"), "Account Name should be updated.");
    }

    [Test]
    public void TestDeleteAccount()
    {
        // Arrange
        var newAccount = new Account() { Name = "[TESTDATA] Test Account", Balance = 42 };
        _dataAccess.Insert(newAccount);
        _idsToCleanUp.Add(newAccount.Id);

        // Act
        _dataAccess.Delete(newAccount.Id);

        // Assert
        var deletedAccount = _dataAccess.Get(newAccount.Id);
        Assert.That(deletedAccount, Is.Null, "Deleted account should not be found in the database.");
    }

    [Test]
    public void TestTransferFunds()
    {
        // Arrange: Insert source account
        var sourceAccount = new Account() { Name = "[TESTDATA] Source Account", Balance = 100 };
        _dataAccess.Insert(sourceAccount);
        _idsToCleanUp.Add(sourceAccount.Id);

        // Arrange: Insert destination account
        var destinationAccount = new Account() { Name = "[TESTDATA] Destination Account", Balance = 100 };
        _dataAccess.Insert(destinationAccount);
        _idsToCleanUp.Add(destinationAccount.Id);

        // Act: Transfer funds
        int amountToTransfer = 50;
        _dataAccess.TransferFunds(sourceAccount.Id, destinationAccount.Id, amountToTransfer);

        // Assert: Retrieve both accounts and assert balances
        var updatedSourceAccount = _dataAccess.Get(sourceAccount.Id);
        var updatedDestinationAccount = _dataAccess.Get(destinationAccount.Id);

        Assert.That(updatedSourceAccount.Balance, Is.EqualTo(sourceAccount.Balance - amountToTransfer), "Source account should have 50 balance after transfer.");
        Assert.That(updatedDestinationAccount.Balance, Is.EqualTo(destinationAccount.Balance + amountToTransfer), "Destination account should have 150 balance after transfer.");
    }

    [Test]
    public void TestFindAccounts()
    {
        // Arrange: Insert multiple accounts
        var account1 = new Account() { Name = "FindMe Account 1", Balance = 100 };
        var account2 = new Account() { Name = "FindMe Account 2", Balance = 100 };
        _dataAccess.Insert(account1);
        _dataAccess.Insert(account2);
        _idsToCleanUp.Add(account1.Id);
        _idsToCleanUp.Add(account2.Id);

        // Act: Find accounts by partial name
        var foundAccounts = _dataAccess.FindAccountsFromPartOfName("FindMe");

        // Assert: the correct accounts were found
        Assert.That(foundAccounts.Count(), Is.EqualTo(2), "Two accounts should be found.");
        Assert.That(foundAccounts.Any(a => a.Id == account1.Id), Is.True, "Account 1 should be found.");
        Assert.That(foundAccounts.Any(a => a.Id == account2.Id), Is.True, "Account 2 should be found.");
    } 
    #endregion

}