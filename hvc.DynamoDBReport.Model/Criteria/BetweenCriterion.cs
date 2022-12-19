namespace hvc.DynamoDBReport.Model.Criteria;

public class BetweenCriterion : Criterion
{
    public String FirstValue { get; }
    public String SecondValue { get; }

    public BetweenCriterion(String name, String firstValue, String secondValue)
        : base(name)
    {
        FirstValue = firstValue;
        SecondValue = secondValue;
    }
}