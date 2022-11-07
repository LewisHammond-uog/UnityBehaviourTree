using System.Collections.Generic;
using AI.BehaviourTree.BaseTypes.Nodes;

namespace AI.BehaviourTree.BaseTypes
{
    public interface IHasChildren
    {
        public void AddChild(Node child);
        public void RemoveChild(Node child);
        public IEnumerable<Node> GetChildren();
    }
}
