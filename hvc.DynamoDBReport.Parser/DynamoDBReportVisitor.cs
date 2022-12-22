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

using hvc.DataStructures.Node;
using hvc.DynamoDBReport.Model;
using hvc.DynamoDBReport.Model.Criteria;
using hvc.Extensions;

namespace hvc.DynamoDBReport.Parser;

public class DynamoDBReportVisitor : DynamoDBReportAntlrParserBaseVisitor<Object>
{
    private readonly Stack<ModelNodeBase> _modelNodes;

    public DynamoDBReportVisitor(Filename filename, Stack<ModelNodeBase>? modelNodes = null)
    {
        Filename = filename;

        _modelNodes = modelNodes ?? new Stack<ModelNodeBase>();
    }

    private ModelNodeBase ParentNode => _modelNodes.Peek();

    public Filename Filename { get; }

    public Model.DynamoDBReport ParentReport =>
        ParentNode as Model.DynamoDBReport
        ?? ParentNode.GetParentByType(typeof(Model.DynamoDBReport)) as Model.DynamoDBReport
        ?? throw new InvalidOperationException("A parent of type DynamoDBReport is expected!");

    public override Object VisitDynamo_query(DynamoDBReportAntlrParser.Dynamo_queryContext context)
    {
        try
        {
            var report = new Model.DynamoDBReport((context.report_name()?.GetText()).Mandatory());
            
            // Set Table attribute for report
            report.SetAttribute(nameof(Model.DynamoDBReport.Table), (context.table_name()?.GetText()).Mandatory());

            _modelNodes.Push(DynamoDBReportCatalog.Reports.Add(report));

            return base.VisitDynamo_query(context);
        }
        finally
        {
            _modelNodes.Pop();
        }
    }

    public override Object VisitPartition_statement(DynamoDBReportAntlrParser.Partition_statementContext context)
    {
        ParentReport.SetKeyStatement((context.partition_key_name()?.GetText()).Mandatory(),
            (context.partition_key_value()?.GetText()).Mandatory());

        return base.VisitPartition_statement(context);
    }

    public override Object VisitIndex_statement(DynamoDBReportAntlrParser.Index_statementContext context)
    {
        // Set Index attribute
        ParentReport.SetAttribute(nameof(Model.DynamoDBReport.Index), (context.index_name()?.GetText()).Mandatory());

        return base.VisitIndex_statement(context);
    }

    public override Object VisitKey_statement(DynamoDBReportAntlrParser.Key_statementContext context)
    {
        ParentReport.SetKeyStatement((context.key_name()?.GetText()).Mandatory(),
            (context.key_statement_value()?.GetText()).Mandatory());

        return base.VisitKey_statement(context);
    }


    public override Object VisitBetween_sortkey_statement(
        DynamoDBReportAntlrParser.Between_sortkey_statementContext context)
    {
        ParentReport.SetSortKeyStatement(new BetweenCriterion((context.sortkey_name()?.GetText()).Mandatory(),
            (context.first_value()?.GetText()).Mandatory(), (context.second_value()?.GetText()).Mandatory()));

        return base.VisitBetween_sortkey_statement(context);
    }

    public override Object VisitBeginswith_sortkey_statement(
        DynamoDBReportAntlrParser.Beginswith_sortkey_statementContext context)
    {
        ParentReport.SetSortKeyStatement(new BeginsWithCriterion((context.sortkey_name()?.GetText()).Mandatory(),
            (context.value()?.GetText()).Mandatory()));

        return base.VisitBeginswith_sortkey_statement(context);
    }

    public override Object VisitSimple_sortkey_statement(
        DynamoDBReportAntlrParser.Simple_sortkey_statementContext context)
    {
        ParentReport.SetSortKeyStatement(new SimpleCriterion((context.sortkey_name()?.GetText()).Mandatory(),
            (context.sortkey_condition()?.GetText()).Mandatory(), (context.value()?.GetText()).Mandatory()));

        return base.VisitSimple_sortkey_statement(context);
    }

    public override Object VisitSimple_filter_statement(
        DynamoDBReportAntlrParser.Simple_filter_statementContext context)
    {
        try
        {
            _modelNodes.Push(new FilterCriterion(ParentNode,
                new SimpleCriterion((context.filter_name()?.GetText()).Mandatory(),
                    (context.filter_condition()?.GetText()).Mandatory(), (context.value()?.GetText()).Mandatory())));
            return base.VisitSimple_filter_statement(context);
        }
        finally
        {
            _modelNodes.Pop();
        }
    }

    public override Object VisitBetween_filter_statement(
        DynamoDBReportAntlrParser.Between_filter_statementContext context)
    {
        try
        {
            _modelNodes.Push(new FilterCriterion(ParentNode,
                new BetweenCriterion((context.filter_name()?.GetText()).Mandatory(),
                    (context.first_value()?.GetText()).Mandatory(), (context.second_value()?.GetText()).Mandatory())));
            return base.VisitBetween_filter_statement(context);
        }
        finally
        {
            _modelNodes.Pop();
        }
    }

    public override Object VisitExists_filter_statement(
        DynamoDBReportAntlrParser.Exists_filter_statementContext context)
    {
        try
        {
            _modelNodes.Push(new FilterCriterion(ParentNode,
                new ExistsCriterion((context.filter_name()?.GetText()).Mandatory(),
                    context.EXISTS()?.GetText() != null)));

            return base.VisitExists_filter_statement(context);
        }
        finally
        {
            _modelNodes.Pop();
        }
    }

    public override Object VisitContains_filter_statement(
        DynamoDBReportAntlrParser.Contains_filter_statementContext context)
    {
        try
        {
            _modelNodes.Push(new FilterCriterion(ParentNode,
                new ContainsCriterion(
                    (context.filter_name()?.GetText()).Mandatory(),
                    (context.value()?.GetText()).Mandatory(),
                    context.CONTAINS()?.GetText() != null)));

            return base.VisitContains_filter_statement(context);
        }
        finally
        {
            _modelNodes.Pop();
        }
    }

    public override Object VisitBegins_with_filter_statement(
        DynamoDBReportAntlrParser.Begins_with_filter_statementContext context)
    {
        try
        {
            _modelNodes.Push(new FilterCriterion(ParentNode,
                new BeginsWithCriterion(
                    (context.filter_name()?.GetText()).Mandatory(),
                    (context.value()?.GetText()).Mandatory())));
            return base.VisitBegins_with_filter_statement(context);
        }
        finally
        {
            _modelNodes.Pop();
        }
    }

    public override Object VisitOutput_column(DynamoDBReportAntlrParser.Output_columnContext context)
    {
        try
        {
            var ascending = context.ASCENDING()?.GetText();
            var descending = context.DESCENDING()?.GetText();

            var outputColumn = new OutputColumn(ParentNode, (context.IDENTIFIER()?.GetText()).Mandatory());

            if (ascending != null)
                outputColumn.SetAttribute(ascending);

            if (descending != null)
                outputColumn.SetAttribute(descending);

            _modelNodes.Push(outputColumn);
            return base.VisitOutput_column(context);
        }
        finally
        {
            _modelNodes.Pop();
        }
    }

    public override Object VisitParameter(DynamoDBReportAntlrParser.ParameterContext context)
    {
        ParentReport.ExtractNumericParameter((context.IDENTIFIER()?.GetText()).Mandatory());

        return base.VisitParameter(context);
    }

    public override Object VisitString_with_quotes(DynamoDBReportAntlrParser.String_with_quotesContext context)
    {
        ParentReport.ExtractParametersFromString((context.STRING_WITH_QUOTES()?.GetText()).Mandatory());

        return base.VisitString_with_quotes(context);
    }
}