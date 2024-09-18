using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BusinessSolution.DAL.Model;
namespace BusinessSolution.DAL;
public class AccountDAO : IAccountDAO
{
    #region SQL statements
    private readonly string _deleteSql = "DELETE FROM Account WHERE id = @accountId";
    private readonly string _selectSingleSql = "SELECT * FROM Account WHERE id = @accountId";
    private readonly string _selectAllSql = "SELECT * FROM Account";
    private readonly string _insertSql = "INSERT INTO Account (name, balance) VALUES (@name, @balance); SELECT CAST(scope_identity() AS int)";
    private readonly string _updateSql = "UPDATE Account SET name=@name, balance=@balance WHERE id=@id";
    private readonly string _updateSourceAccountSql = "UPDATE Account SET Balance = Balance - @amount WHERE id=@id";
    private readonly string _updateDestinationAccountSql = "UPDATE Account SET Balance = Balance + @amount WHERE id=@id";
    private readonly string _findAccountsFromPartOfNameSql = "SELECT * FROM Account WHERE name LIKE @partOfName";

    #endregion
    public string ConnectionString { get; set; }
    public AccountDAO(string connectionString)
    {
        ConnectionString = connectionString;
    }

    #region Core DAO methods
    public bool Delete(int id)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        try
        {
            connection.Open();
            SqlCommand command = new SqlCommand(_deleteSql, connection);
            command.Parameters.AddWithValue("@accountId", id);
            return command.ExecuteNonQuery() == 1;
        }
        catch (Exception ex)
        {
            throw new Exception($"Exception while trying to delete a row from the Account table using id={id} . The exception was: '{ex.Message}'", ex);
        }
    }

    public Account? Get(int accountId)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        connection.Open();

        SqlCommand command = new SqlCommand(_selectSingleSql, connection);
        command.Parameters.AddWithValue("@accountId", accountId);

        try
        {
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return SingleDataReaderRowToAccount(reader);
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Exception while trying to find account with id '{accountId}'. The exception was: '{ex.Message}'", ex);
        }
    }



    public IEnumerable<Account> GetAll()
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        connection.Open();

        SqlCommand command = new SqlCommand(_selectAllSql, connection);

        try
        {
            SqlDataReader reader = command.ExecuteReader();
            return DataReaderToAccounts(reader);
        }
        catch (Exception ex)
        {
            throw new Exception($"Exception while trying to read all rows from the Account table. The exception was: '{ex.Message}'", ex);
        }
    }



    public int Insert(Account account)
    {

        using SqlConnection connection = new SqlConnection(ConnectionString);
        connection.Open();

        SqlCommand command = new SqlCommand(_insertSql, connection);
        command.Parameters.AddWithValue("@name", account.Name);
        command.Parameters.AddWithValue("@balance", account.Balance);

        try
        {
            int insertedAccountsDbGeneratedKey = (int)command.ExecuteScalar();
            account.Id = insertedAccountsDbGeneratedKey;
            return insertedAccountsDbGeneratedKey;
        }
        catch (Exception ex)
        {
            throw new Exception($"Exception while trying to insert account object. The exception was: '{ex.Message}'", ex);
        }
    }

    public bool Update(Account account)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        connection.Open();

        SqlCommand command = new SqlCommand(_updateSql, connection);
        command.Parameters.AddWithValue("@id", account.Id);
        command.Parameters.AddWithValue("@name", account.Name);
        command.Parameters.AddWithValue("@balance", account.Balance);

        try
        {
            return command.ExecuteNonQuery() == 1;
        }
        catch (Exception ex)
        {
            throw new Exception($"Exception while trying to update account. The exception was: '{ex.Message}'", ex);
        }
    } 
    #endregion

    #region Additional functionality
    public void TransferFunds(int sourceAccountId, int destinationAccountId, decimal amount)
    {
        using SqlConnection connection = new SqlConnection(ConnectionString);
        connection.Open();
        SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand sourceCommand = new SqlCommand(_updateSourceAccountSql, connection);
        sourceCommand.Parameters.AddWithValue("@amount", amount);
        sourceCommand.Parameters.AddWithValue("@id", sourceAccountId);
        sourceCommand.Transaction = transaction;

        SqlCommand destinationCommand = new SqlCommand(_updateDestinationAccountSql, connection);
        destinationCommand.Parameters.AddWithValue("@amount", amount);
        destinationCommand.Parameters.AddWithValue("@id", destinationAccountId);
        destinationCommand.Transaction = transaction;

        try
        {
            sourceCommand.ExecuteNonQuery();
            destinationCommand.ExecuteNonQuery();
            transaction.Commit();
        }
        catch (Exception ex)
        {
            try
            {
                transaction.Rollback();
                throw new Exception($"Error while moving {amount} from account with id {sourceAccountId} to account with id {destinationAccountId}. The error was {ex.Message}. Transaction successfully rolled back.", ex);
            }
            catch (Exception exRollback)
            {
                throw new Exception($"Error while rolling back transaction which occurred while moving {amount} from account with id {sourceAccountId} to account with id {destinationAccountId}. The error was {exRollback.Message}", exRollback);
            }
        }
    }

    public IEnumerable<Account> FindAccountsFromPartOfName(string partOfName)
    {
        
        using SqlConnection connection = new SqlConnection(ConnectionString);
        connection.Open();

        SqlCommand command = new SqlCommand(_findAccountsFromPartOfNameSql, connection);
        command.Parameters.AddWithValue("@partOfName", $"%{partOfName}%");

        try
        {
            SqlDataReader reader = command.ExecuteReader();
            return DataReaderToAccounts(reader);

        }
        catch (Exception ex)
        {
            throw new Exception($"Exception while trying to read rows with '{partOfName}' in their name from the Account table. The exception was: '{ex.Message}'", ex);
        }
    }

    #endregion

    #region Internal helper methods for converting DataReader tuples to objects

    private IEnumerable<Account> DataReaderToAccounts(SqlDataReader reader)
    {
        List<Account> accounts = new List<Account>();
        while (reader.Read())
        {
            accounts.Add(SingleDataReaderRowToAccount(reader));
        }
        return accounts;
    }
    private Account SingleDataReaderRowToAccount(SqlDataReader reader)
    {
        Account account = new Account();
        account.Id = (int)reader["id"];
        account.Name = (string)reader["name"];
        account.Balance = (decimal)reader["balance"];
        return account;
    }
    #endregion
}