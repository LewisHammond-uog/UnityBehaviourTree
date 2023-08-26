using System.Collections.Generic;
using BT.AI.BehaviourTree.BaseTypes.Nodes;

namespace BT.AI.BehaviourTree.BaseTypes
{
    public interface IHasChildren
    {
        public void AddChild(Node child);
        public void RemoveChild(Node child);
        public IEnumerable<Node> GetChildren();
    }
}
