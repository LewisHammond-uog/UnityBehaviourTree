using AI.BehaviourTree.BaseTypes.Nodes;

namespace AI.BehaviourTree.BaseTypes
{
    public interface IHasChild
    {
        public Node GetChild();
        public void SetChild(Node child);
    }
}
