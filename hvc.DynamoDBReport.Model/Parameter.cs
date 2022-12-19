using hvc.DataStructures;
using hvc.DataStructures.Node;

namespace hvc.DynamoDBReport.Model
{
    public enum ParameterType
    {
        String,
        Integer
    }

    public class Parameter : ModelNodeBase, IModelNodeLeaf
    {
        public ParameterType Type { get; }

        public Parameter(ModelNodeBase parent, String name, ParameterType type) 
            : base(parent, new ObjectNameSingle(name))
        {
            Type = type;
        }
    }
}
