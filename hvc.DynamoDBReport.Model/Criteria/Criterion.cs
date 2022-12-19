namespace hvc.DynamoDBReport.Model.Criteria;

public abstract class Criterion 
{
    public String Name { get; }

    protected Criterion(String name)
    {
        Name = name;
    }
}