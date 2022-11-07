namespace AI.BehaviourTree.BaseTypes.Nodes
{
    /// <summary>
    /// A node that tests if a condition or conditions are true
    /// </summary>
    public abstract class ConditionNode : ActionNode
    {
        protected override NodeStatus Update_Internal()
        {
            return CheckCondition() ? NodeStatus.Success : NodeStatus.Fail;
        }

        /// <summary>
        /// Overrideable function to check if a condtion is true
        /// </summary>
        /// <returns>If condition is true</returns>
        protected abstract bool CheckCondition();
    }
}