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

        [Option('i', "in", Required = true, HelpText = "List of report files.")]
        [UsedImplicitly]
        public IEnumerable<String> In { get; set; } = new List<String>();

        [Option('m', "modular", Required = false, HelpText="Generates common module if specified.")]
        [UsedImplicitly]
        public Boolean Modular { get; set; }
    }

    private static void Main(String[] args)
    {
        var outputFolder = String.Empty;
        var inputItems = new List<String>();
        var isModular = false;

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                outputFolder = o.OutputFolder;
                inputItems = o.In.ToList();
                isModular = o.Modular;
            });

        DynamoDBReportParser.Initialize();

        var modelNodes = new Stack<ModelNodeBase>();

        foreach (var item in inputItems)
        {
            var files = item.Files();
            if (files.Length == 0)
            {
                var filePath = item.Unquote().FullPath();

                if (!filePath.IsExistingFile())
                    throw new ArgumentException($"'{item}' is not a full filename or a valid wildcard!");

                files = filePath.ToStringArray();
            }

            foreach (var file in files)
            {
                var dynamoDBReport = hvc.Parser.Parser.Get(file, true) ??
                                     throw new InvalidOperationException("Expecting an AWS DynamoDB report file!");

                dynamoDBReport.Parse(modelNodes);
            }
        }

        foreach (var dynamoDBReport in DynamoDBReportCatalog.Reports.AllItems)
            DynamoDBReportGenerator.GenerateCode(String.IsNullOrWhiteSpace(outputFolder) ? "." : outputFolder.Unquote(), dynamoDBReport, isModular);
    }
}