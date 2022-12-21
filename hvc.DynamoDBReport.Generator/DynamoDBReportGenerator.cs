using System.Text;
using hvc.DynamoDBReport.Model;
using hvc.DynamoDBReport.Model.Criteria;
using hvc.Extensions;
using hvc.Generator;

namespace hvc.DynamoDBReport.Generator;

public class DynamoDBReportGenerator : CodeGenerator
{
    private readonly Model.DynamoDBReport _dynamoDBReport;
    private readonly String _isModular;
    private readonly String _outputFolder;
    private readonly PythonOutput _python;

    private DynamoDBReportGenerator(String outputFolder, Model.DynamoDBReport dynamoDBReport, Boolean isModular)
    {
        _dynamoDBReport = dynamoDBReport;
        _isModular = isModular ? "True" : String.Empty;
        _outputFolder = outputFolder;
        _python = new PythonOutput();
    }

    public static void GenerateCode(String outputFolder, Model.DynamoDBReport dynamoDBReport, Boolean isModular)
    {
        new DynamoDBReportGenerator(outputFolder, dynamoDBReport, isModular).Generate();
    }

    private static String ConvertCondition(Condition condition)
    {
        return condition switch
        {
            Condition.Equal => "eq",
            Condition.NotEqual => "neq",
            Condition.LessThanOrEqual => "lte",
            Condition.LessThan => "lt",
            Condition.GreaterThanOrEqual => "gte",
            Condition.GreaterThan => "gt",
            _ => throw new InvalidOperationException($"{condition} is not supported!")
        };
    }

    public String GenerateOutput(String fieldType, Criterion criterion)
    {
        switch (criterion)
        {
            case SimpleCriterion simpleCriterion:
                return
                    $"{fieldType}('{criterion.Name}').{ConvertCondition(simpleCriterion.Condition)}({ConvertValue(simpleCriterion.Value)})";
            case BetweenCriterion betweenCriterion:
                return
                    $"{fieldType}('{criterion.Name}').between({ConvertValue(betweenCriterion.FirstValue)}, {ConvertValue(betweenCriterion.SecondValue)})";
            case ExistsCriterion existsCriterion:
                return existsCriterion.IsPositive
                    ? $"{fieldType}('{criterion.Name}').exists()"
                    : $"{fieldType}('{criterion.Name}').not_exists()";
            case ContainsCriterion containsCriterion:
            {
                if (!containsCriterion.IsPositive)
                    throw new InvalidOperationException("Not Contains is not supported!");

                return $"{fieldType}('{criterion.Name}').contains({ConvertValue(containsCriterion.Value)})";
            }
            case BeginsWithCriterion beginsWithCriterion:
                return $"{fieldType}('{criterion.Name}').begins_with({ConvertValue(beginsWithCriterion.Value)})";
            default:
                throw new InvalidOperationException(
                    $"Unsupported filter criterion! {criterion.GetType().Name}");
        }
    }

    private String ConvertValue(String value)
    {
        if (_dynamoDBReport.IsParameter(value, out var isString))
            return isString ? $"f{value.SnakeCase()}" : $"int({value.SnakeCase()})";

        return value;
    }

    private void Generate()
    {
        if (_dynamoDBReport.KeyCriterion == null)
            throw new InvalidOperationException("Mandatory value keyCriterion not found!");

        var reportName = _dynamoDBReport.Name.Original.SnakeCase();
        var tableName = _dynamoDBReport.Table.Value;
        var indexName = _dynamoDBReport.Index.Value;

        var keyName = _dynamoDBReport.KeyCriterion.Name;
        var keyCriterion = _dynamoDBReport.KeyCriterion.Value;

        var outputColumns = new StringBuilder();

        var sortColumn = String.Empty;
        var reverseSort = String.Empty;


        // Generate outputColumns block for the report
        var oneTimeFlag = new OneTimeFlag();
        _dynamoDBReport.OutputColumns.ForEach(outputColumn =>
        {
            if (!oneTimeFlag.IsFirstTime())
                outputColumns.Append(", ");

            outputColumns.Append($"\"{outputColumn.ColumnName.Value}\"");

            if (!outputColumn.Ascending.IsSet && !outputColumn.Descending.IsSet)
                return;

            if (!String.IsNullOrWhiteSpace(sortColumn))
                throw new InvalidOperationException("Sorting on multiple columns is not supported!");

            sortColumn = outputColumn.ColumnName.Value;
            reverseSort = outputColumn.Descending.IsSet ? "True" : String.Empty;
        });

        // generate import section
        _python
            .Reset()
            .Line(TemplateStack["ImportSection"]
                .Optional("isModular", _isModular)
                .Render())
            .Line(TemplateStack["ClickDefault"].Render());

        // generate custom parameters
        _dynamoDBReport.Parameters.ForEach(parameter =>
        {
            _python
                .Line(TemplateStack["ClickBody"]
                    .Mandatory("param", parameter.Key.SnakeCase())
                    .Mandatory("prompt", parameter.Key.CamelCase())
                    .Render());
        });

        _python
            .Line(TemplateStack["MethodBody01"]
                .Add("reportName", reportName).Render());

        var stripQuotes = new StringBuilder();
        var paramCount = 0;

        _dynamoDBReport.Parameters.ForEach(parameter =>
        {
            var endValue = paramCount++ == _dynamoDBReport.Parameters.Count - 1 ? "):" : ",";
            var paramType = parameter.Type == ParameterType.String ? "str" : "int";
            _python
                .Line($"{parameter.Key.SnakeCase()}: {paramType}{endValue}".Prepend(new String(' ',
                    "def generate_(".Length + reportName.Length)));

            if (parameter.Type == ParameterType.String)
                stripQuotes.Append(
                    TemplateStack["StripQuotes"]
                        .Mandatory("paramName", parameter.Name.Original.SnakeCase())
                        .Optional("isModular", _isModular)
                        .Render());
        });

        var hasFilters = _dynamoDBReport.FilterCriterion.Count > 0 ? "hasFilters" : String.Empty;

        var sortKeyStatement =
            _dynamoDBReport.SortKeyCriterion != null
                ? GenerateOutput("Key", _dynamoDBReport.SortKeyCriterion)
                : String.Empty;

        _python
            .Line(TemplateStack["MethodBody02"]
                .Optional("isModular", _isModular)
                .Mandatory("outputColumns", outputColumns)
                .Optional("sortColumn", sortColumn)
                .Optional("reverseSort", reverseSort)
                .Mandatory("tableName", tableName)
                .Mandatory("keyName", keyName)
                .Mandatory("keyCriterion", ConvertValue(keyCriterion))
                .Optional("sortKeyStatement", sortKeyStatement)
                .Optional("stripQuotes", stripQuotes)
                .Optional("hasFilters", hasFilters)
                .Render());

        var filterCount = 0;
        _dynamoDBReport.FilterCriterion.ForEach(filterCriterion =>
        {
            var isLastCriterion = filterCount++ == _dynamoDBReport.FilterCriterion.AllItems.Count() - 1;
            var criterionOutput = GenerateOutput("Attr", filterCriterion.Criterion);

            _python
                .SetIndentLevel(2)
                .Line(isLastCriterion ? $"{criterionOutput}" : $"{criterionOutput} & \\")
                .SetIndentLevel(0);
        });

        _python
            .Line(TemplateStack["MethodBody03"]
                .Optional("isModular", _isModular)
                .Mandatory("tableName", tableName)
                .Optional("sortColumn", sortColumn)
                .Mandatory("reverseSort", String.IsNullOrWhiteSpace(reverseSort) ? "False" : "True")
                .Mandatory("reportName", reportName)
                .Optional("indexName", indexName)
                .Optional("hasFilters", hasFilters)
                .Render());

        _python.WriteToFile(Path.Combine(_outputFolder, $"{_dynamoDBReport.Name.Original.SnakeCase()}.py"));

        if (!String.IsNullOrWhiteSpace(_isModular))
            GenerateReportCommon();

        GenerateRequirementsTxt();
    }

    private void GenerateReportCommon()
    {
        // Since this method is called from Generate method which is called for each report, we will skip writing this file if it already exists
        _python
            .Reset()
            .Line(TemplateStack["ReportCommon"].Render())
            .WriteToFile(Path.Combine(_outputFolder, "common", "report_common.py"), true);
    }

    private void GenerateRequirementsTxt()
    {
        // Since this method is called from Generate method which is called for each report, we will skip writing this file if it already exists
        _python
            .Reset()
            .Line(TemplateStack["RequirementsTxt"].Render())
            .WriteToFile(Path.Combine(_outputFolder, "requirements.txt"), true);
    }
}