using System.Collections.Generic;
using BusinessSolution.DAL.Model;

namespace BusinessSolution.DAL;

/// <summary>
/// This DAO interface defines the CRUD methods 
/// and supplementary functionality
/// for the Account table and Account objects
/// </summary>
public interface IAccountDAO
{

    #region Core DAO methods

    /// <summary>
    /// Inserts a new account object
    /// </summary>
    /// <param name="account"></param>
    /// <returns>The new db identity of the inserted account</returns>
    int Insert(Account account);

    /// <summary>
    /// Gets all existing accounts from the Account table
    /// </summary>
    /// <returns>all existing accounts from the Account table</returns>
    IEnumerable<Account> GetAll();

    /// <summary>
    /// Finds an existing account from the primary key: Id
    /// </summary>
    /// <param name="id">The id of the account to find</param>
    /// <returns>The account, if it exists. Otherwise NULL</returns>
    Account Get(int id);

    /// <summary>
    /// Updates an account
    /// </summary>
    /// <param name="account">The account to update (based on the Id)</param>
    /// <returns>Whether the account was found</returns>
    bool Update(Account account);

    /// <summary>
    /// Deletes an account
    /// </summary>
    /// <param name="id">The id of the account to delete</param>
    /// <returns>Whether the corresponding account was found</returns>
    bool Delete(int id);
    #endregion

    #region Additional functionality
    /// <summary>
    /// Finds and retrieves all accounts that match part of the name.
    /// </summary>
    /// <param name="partOfName">The part of the account names to match.</param>
    /// <returns>All accounts with a name matching the part of name provided.</returns>
    IEnumerable<Account> FindAccountsFromPartOfName(string partOfName);

    /// <summary>
    /// Transfers a specified amount from one account to another
    /// </summary>
    /// <param name="sourceAccountId">The id of the account to transfer from</param>
    /// <param name="destinationAccountId">The id of the account to transfer to</param>
    /// <param name="amount">How much to transfer</param>
    void TransferFunds(int sourceAccountId, int destinationAccountId, decimal amount); 
    #endregion

}