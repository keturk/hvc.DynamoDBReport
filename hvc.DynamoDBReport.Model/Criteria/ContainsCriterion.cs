namespace hvc.DynamoDBReport.Model.Criteria;

public class ContainsCriterion : Criterion
{
    public Boolean IsPositive { get; }
    public String Value { get; }

    public ContainsCriterion(String name, String value, Boolean isPositive)
        : base(name)
    {
        Value = value;
        IsPositive = isPositive;
    }
}