using System.Collections.Generic;
using UnityEditor.UIElements;

namespace AI.Shared.Blackboard
{
    /// <summary>
    /// Manager class for blackboards, handles the blackboards for each team
    /// </summary>
    public static class TeamBlackboardManager
    {
        private static TeamBlackboard blackboard;

        /// <summary>
        /// Get the blackboard for a given team
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static TeamBlackboard GetBlackboard()
        {
            return blackboard;
        }

        /// <summary>
        /// Create a Blackboard
        /// </summary>
        /// <param name="team">Team for blackboard</param>
        /// <returns>Team blackboard</returns>
        private static TeamBlackboard CreateBlackboard()
        {
            blackboard = new TeamBlackboard();
            return blackboard;
        }

        /// <summary>
        /// Reset all of the team blackboards
        /// </summary>
        public static void ResetAllBlackboards()
        {
            blackboard.Reset();
        }
    }
}