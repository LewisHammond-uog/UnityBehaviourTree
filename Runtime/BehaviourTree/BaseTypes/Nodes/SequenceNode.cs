namespace BT.AI.BehaviourTree.BaseTypes.Nodes
{
    public class SequenceNode : CompositeNode
    {
        private Node currentNode;
        private int currentNodeIndex;
        
        protected override void OnEnterNode()
        {
            currentNodeIndex = 0;
            currentNode = children[currentNodeIndex];
            
            //Update if we have any always check nodes
            CollectAlwaysCheckNodes();
        }

        protected override NodeStatus Update_Internal()
        {
            //Check the always check nodes - if one fails then we should abort this selector
            if (CheckAlwaysCheckNodes() == NodeStatus.Fail)
            {
                return NodeStatus.Fail;
            }
            
            //Execute Child report back it's status
            NodeStatus nodeResult = currentNode.Update();
            switch (nodeResult)
            {
                case NodeStatus.Running:
                    return NodeStatus.Running;
                case NodeStatus.Success:
                    //Move to the next child for the next update loop
                    ++currentNodeIndex;
                    currentNode = currentNodeIndex < children.Count ? children[currentNodeIndex] : null;
                    break;
                case NodeStatus.Fail:
                    //Fall through
                default:
                    return NodeStatus.Fail;
            }
            
            //If next node is null then we are at the end of the list - success! otherwise keep running
            return currentNode == null ? NodeStatus.Success : NodeStatus.Running;
        }
    }
}