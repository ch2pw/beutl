namespace Beutl.NodeTree.Nodes;

public partial class OutputNode : Node
{
    public OutputNode()
    {
        InputSocket = AddInput<object>("Output");
    }

    public InputSocket<object> InputSocket { get; }
}
