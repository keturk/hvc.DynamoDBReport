using hvc.DynamoDBReport.Model;
using hvc.DynamoDBReport.Parser;
using hvc.DynamoDBReport.Generator;
using JetBrains.Annotations;
using CommandLine;
using hvc.DataStructures.Node;
using hvc.Extensions;

namespace Generate.DynamoDBReport;

[UsedImplicitly]
internal class Program
{
    public class Options
    {
        [Option('o', "out", Required = false, HelpText = "Output folder for generated file(s).")]
        [UsedImplicitly]
        public String OutputFolder { get; set; } = String.Empty;

        [Option('i', "in", Required = true, HelpText = "List of report files. e.g. `c:\\folder\\*.dynaq`")]
        [UsedImplicitly]
        public String Input { get; set; } = String.Empty;

        [Option('m', "modular", Required = false, HelpText="Generates common module if specified.")]
        [UsedImplicitly]
        public Boolean Modular { get; set; }
    }

    private static void Main(String[] args)
    {
        var outputFolder = String.Empty;
        var input = String.Empty;
        var isModular = false;

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                outputFolder = o.OutputFolder;
                input = o.Input;
                isModular = o.Modular;
            });

        DynamoDBReportParser.Initialize();

        // First try to enumerate files assuming inputFolder contains a path with wildcards
        var files = input.Files();

        // If input is not a path with wildcards, we will check whether provided input is a path to a file
        if (files.Length == 0)
        {
            var filePath = input.Unquote().FullPath();

            if (!filePath.IsExistingFile())
                throw new ArgumentException($"'{input}' is not a full filename or a valid wildcard!");

            files = filePath.ToStringArray();
        }

        // Parse all dynaq files
        foreach (var file in files)
        {
            var dynamoDBReport = hvc.Parser.Parser.Get(file, true) ??
                                 throw new InvalidOperationException("Expecting an AWS DynamoDB report file!");

            dynamoDBReport.Parse(new Stack<ModelNodeBase>());
        }

        // Generate Python scripts for all parsed reports
        foreach (var dynamoDBReport in DynamoDBReportCatalog.Reports.AllItems)
            DynamoDBReportGenerator.GenerateCode(String.IsNullOrWhiteSpace(outputFolder) ? "." : outputFolder.Unquote(), dynamoDBReport, isModular);
    }
}