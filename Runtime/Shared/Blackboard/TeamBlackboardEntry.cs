using UnityEngine;

namespace AI.Shared.Blackboard
{
    public partial class TeamBlackboard
    {
        /// <summary>
        /// Entry within the blackboard
        /// </summary>
        public class Entry
        {
            /// <summary>
            /// Create an entry
            /// </summary>
            /// <param name="key">Entry Key</param>
            /// <param name="value">Entry Value</param>
            /// <param name="updateAgent">Agent that is updating this entry</param>
            /// <param name="confidence">Confidence in value</param>
            public Entry(string key, Vector3 value, BaseAgent updateAgent, float confidence)
            {
                this.Key = key;
                this.Value = value;
                this.LastUpdateAgent = updateAgent;
                this.Confidence = confidence;

                this.LastUpdateTime = Time.realtimeSinceStartup;
            }

            /// <summary>
            /// Update this entry
            /// </summary>
            /// <param name="value">Value to update to</param>
            /// <param name="updateAgent">Agent that is updating this entry</param>
            /// <param name="confidence">Confidence in value</param>
            public void Update(Vector3 value, BaseAgent updateAgent, float confidence)
            {
                this.Value = value;
                this.LastUpdateAgent = updateAgent;
                this.Confidence = confidence;
                
                this.LastUpdateTime = Time.realtimeSinceStartup;
            }

            /// <summary>
            /// Gets the age of this entry
            /// </summary>
            public float GetAge()
            {
                return Time.realtimeSinceStartup - LastUpdateTime;
            }

            //Key of the entry
            public string Key { get; }

            //Value of the entry
            public Vector3 Value { get; private set; }

            //Agent that last updates this entry
            public BaseAgent LastUpdateAgent { get; private set; }
            
            //Time that this entry was last updated
            public float LastUpdateTime { get; private set; }

            //Confidence in the value
            public float Confidence { get; private set; }
        }
    }
}