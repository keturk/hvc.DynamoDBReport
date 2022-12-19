namespace hvc.DynamoDBReport.Model.Criteria;

public enum Condition
{
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual
}

public static class ConditionExtensions
{
    public static Condition ToCondition(this String value)
    {
        return value.ToUpper().Trim() switch
        {
            "=="  => Condition.Equal,
            "!=" => Condition.NotEqual,
            "<=" => Condition.LessThanOrEqual,
            "<"  => Condition.LessThan,
            ">=" => Condition.GreaterThanOrEqual,
            ">"  => Condition.GreaterThan,
            _ => throw new NotImplementedException()
        };
    }
}