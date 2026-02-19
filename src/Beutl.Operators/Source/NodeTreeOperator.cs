using System.ComponentModel.DataAnnotations;
using Beutl.Language;
using Beutl.NodeTree;
using Beutl.Operation;

namespace Beutl.Operators.Source;

[Display(Name = nameof(Strings.NodeTree), ResourceType = typeof(Strings))]
public sealed class NodeTreeOperator : PublishOperator<NodeTreeDrawable>
{
    protected override void FillProperties()
    {
        AddProperty(Value.Model, new NodeTreeModel());
    }
}
