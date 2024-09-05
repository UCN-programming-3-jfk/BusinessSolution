namespace BusinessSolution.DAL.Model;
public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public override string ToString()
    {
        return $"Account {{ Id={Id}, Name='{Name}', Balance={Balance} }}";
    }
}