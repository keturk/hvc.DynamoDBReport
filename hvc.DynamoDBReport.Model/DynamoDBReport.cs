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

using System.Text.RegularExpressions;
using hvc.DataStructures;
using hvc.DataStructures.Node;
using hvc.DynamoDBReport.Model.Criteria;
using hvc.Extensions;
using JetBrains.Annotations;

namespace hvc.DynamoDBReport.Model;

public class DynamoDBReport : ModelNodeBase, IModelNodeRoot
{
    public DynamoDBReport(String name)
        : base(new ObjectNameSingle(name))
    {
        KeyCriterion = null;
        SortKeyCriterion = null;
    }

    public void SetKeyStatement(String keyName, String statementValue)
    {
        if (KeyCriterion != null)
            throw new InvalidOperationException("Key Statement is already defined!");

        KeyCriterion = new SimpleCriterion(keyName, "==", statementValue);
    }

    public void SetSortKeyStatement(Criterion sortKeyCriterion)
    {
        if (SortKeyCriterion != null)
            throw new InvalidOperationException("SortKey Statement is already defined!");

        SortKeyCriterion = sortKeyCriterion switch
        {
            SimpleCriterion => sortKeyCriterion,
            BetweenCriterion => sortKeyCriterion,
            BeginsWithCriterion => sortKeyCriterion,
            _ => throw new InvalidOperationException("Provided criterion is not supported as SortKey criterion!")
        };
    }

    public void ExtractParametersFromString(String value)
    {
        var regex = new Regex(@"\{.*?\}");
        var matches = regex.Matches(value);

        foreach (var match in matches.Select(p => p.Value).ToArray())
        {
            var parameter = match.SafeSubstring(1, match.Length - 2);

            if (!Parameters.ContainsKey(parameter))
                _ = new Parameter(this, parameter, ParameterType.String);
        }
    }

    public void ExtractNumericParameter(String parameter)
    {
        if (!Parameters.ContainsKey(parameter))
            _ = new Parameter(this, parameter, ParameterType.Integer);
    }

    public Boolean IsParameter(String value, out Boolean isString)
    {
        isString = false;

        if (Parameters.ContainsKey(value))
            return true;

        var values = new Regex(@"\{.*?\}").Matches(value).Select(p => p.Value).ToArray();

        if (values.Length != 1 || !Parameters.ContainsKey(values[0].SafeSubstring(1, values[0].Length - 2)))
            return false;

        isString = true; return true;
    }

    public SimpleCriterion? KeyCriterion { get; protected set; }
    public Criterion? SortKeyCriterion { get; protected set; }
    
    #region Node Items

    public Items<FilterCriterion> FilterCriterion { get; } = new(StringComparer.InvariantCultureIgnoreCase);
    public Items<OutputColumn> OutputColumns { get; } = new(StringComparer.InvariantCultureIgnoreCase);
    public Items<Parameter> Parameters { get; } = new (StringComparer.InvariantCultureIgnoreCase);

    #endregion

    #region Node Attributes

    [UsedImplicitly] public NodeAttribute<String> Table { get; } = new();
    [UsedImplicitly] public NodeAttribute<String> Index { get; } = new();

    #endregion
}