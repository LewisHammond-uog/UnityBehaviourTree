using BT.AI.BehaviourTree.BaseTypes.Nodes;

namespace BT.AI.BehaviourTree.BaseTypes
{
    public interface IHasChild
    {
        public Node GetChild();
        public void SetChild(Node child);
    }
}
