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