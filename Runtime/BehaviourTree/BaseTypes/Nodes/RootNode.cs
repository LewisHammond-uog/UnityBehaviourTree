using UnityEngine;

namespace BT.AI.BehaviourTree.BaseTypes.Nodes
{
    public class RootNode : Node, IHasChild
    {
        [SerializeField] private Node child;
        
        protected override NodeStatus Update_Internal()
        {
            return child.Update();
        }

        public Node GetChild()
        {
            return child;
        }

        public void SetChild(Node newChild)
        {
            child = newChild;
        }

        /// <summary>
        /// Get a clone of this node and children
        /// </summary>
        /// <returns></returns>
        public override Node Clone()
        {
            RootNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}