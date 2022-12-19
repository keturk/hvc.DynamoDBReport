namespace hvc.DynamoDBReport.Model.Criteria;

public class SimpleCriterion : Criterion
{
    public Condition Condition { get; }
    public String Value { get; }

    public SimpleCriterion(String name, String condition, String value)
        : base(name)
    {
        Condition = condition.ToCondition();
        Value = value;
    }
}