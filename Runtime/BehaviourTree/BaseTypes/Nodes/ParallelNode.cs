namespace AI.BehaviourTree.BaseTypes.Nodes
{
    /// <summary>
    /// Parallel Node - executes all of it's children in the same frame
    /// </summary>
    public class ParallelNode : CompositeNode
    {
        protected override NodeStatus Update_Internal()
        {
            //Execute all of the nodes in this frame
            NodeStatus returnStatus = NodeStatus.Success;
            foreach (Node node in children)
            {
                NodeStatus status = node.Update();
            
                //If the status is running then update the status as that is what we will return
                //if the status is fail then update and abort the loop
                if (status == NodeStatus.Running)
                {
                    returnStatus = NodeStatus.Running;
                }
                else if(status == NodeStatus.Fail)
                {
                    returnStatus = NodeStatus.Fail;
                    break;
                }
            }

            return returnStatus;
        }
    }
}
