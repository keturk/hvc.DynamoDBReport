using hvc.DataStructures;
using hvc.DataStructures.Node;
using JetBrains.Annotations;

namespace hvc.DynamoDBReport.Model;

public class OutputColumn : ModelNodeBase, IModelNodeLeaf
{
    public OutputColumn(ModelNodeBase parent, String name)
        : base(parent, new ObjectNameSingle($"OutputColumn({name})"))
    {
        ColumnName.Set(name);
    }

    #region Node Attributes

    public NodeAttribute<String> ColumnName { get; } = new();

    [NodeAttributeGroup("SortOrder")]
    [UsedImplicitly]
    public NodeAttribute<Boolean> Ascending { get; } = new();

    [NodeAttributeGroup("SortOrder")]
    [UsedImplicitly]
    public NodeAttribute<Boolean> Descending { get; } = new();

    #endregion
}