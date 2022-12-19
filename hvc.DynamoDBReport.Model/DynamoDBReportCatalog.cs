using hvc.DataStructures;

namespace hvc.DynamoDBReport.Model;

public static class DynamoDBReportCatalog
{
    public static Items<DynamoDBReport> Reports { get; } =
        new(StringComparer.InvariantCultureIgnoreCase);
}