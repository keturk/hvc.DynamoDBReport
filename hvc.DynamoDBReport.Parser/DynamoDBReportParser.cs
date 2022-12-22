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

using Antlr4.Runtime;
using hvc.DataStructures.Node;
using hvc.Extensions;
using hvc.Parser;

namespace hvc.DynamoDBReport.Parser;

[ParserExtension("DynaQ")]
public class DynamoDBReportParser : hvc.Parser.Parser
{
    public DynamoDBReportParser(Filename filename)
        : base(filename)
    {
    }

    public static void Initialize()
    {
    }

    public override void Parse(Stack<ModelNodeBase> modelNodes)
    {
        var inputStream = new AntlrInputStream(File.ReadAllText(Filename.Full));

        var lexer = new DynamoDBReportAntlrLexer(inputStream);

        var parser = new DynamoDBReportAntlrParser(new CommonTokenStream(lexer));

        parser.RemoveErrorListeners();
        parser.AddErrorListener(new ErrorListener(Filename.Full));

        var visitor = new DynamoDBReportVisitor(Filename, modelNodes);
        visitor.Visit(parser.start_rule());
    }
}