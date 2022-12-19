using hvc.DataStructures;
using hvc.DataStructures.Node;

namespace hvc.DynamoDBReport.Model.Criteria
{
    public class FilterCriterion : ModelNodeBase, IModelNodeLeaf
    {
        public Criterion Criterion { get; }

        public FilterCriterion(ModelNodeBase parent, Criterion criterion) 
            : base(parent, new ObjectNameSingle($"Criterion({criterion.Name})"))
        {
            Criterion = criterion;
        }
    }
}
