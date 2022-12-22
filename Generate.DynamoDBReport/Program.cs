// MIT License
//
// Copyright (c) 2022 Kamil Ercan Turkarslan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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