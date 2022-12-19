namespace hvc.DynamoDBReport.Model.Criteria;

public class ExistsCriterion : Criterion
{
    public Boolean IsPositive { get; }

    public ExistsCriterion(String name, Boolean isPositive)
        : base(name)
    {
        IsPositive = isPositive;
    }
}