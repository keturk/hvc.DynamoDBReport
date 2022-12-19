namespace hvc.DynamoDBReport.Model.Criteria;

public class BeginsWithCriterion : Criterion
{
    public BeginsWithCriterion(String name, String value)
        : base(name)
    {
        Value = value;
    }

    public String Value { get; }
}