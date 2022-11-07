using System.Collections.Generic;
using UnityEngine;

namespace AI.Shared.Blackboard
{
    public partial class TeamBlackboard
    {
        //List of all entries
        private List<Entry> entries;
        //Dictonary of keys of entries for fast lookup
        private Dictionary<string, Entry> entriesDictonary;

        /// <summary>
        /// Create a team blackboard for a given team
        /// </summary>
        /// <param name="team"></param>
        public TeamBlackboard()
        {
            entries = new List<Entry>();
            entriesDictonary = new Dictionary<string, Entry>();
        }
        
        /// <summary>
        /// Try and get an entry from the blackboard with a maximum age
        /// </summary>
        /// <param name="key">Key of the entry</param>
        /// <param name="maxAge">Maximum age of the entry</param>
        /// <param name="entry">OUT entry to populate</param>>
        /// <returns></returns>
        public bool TryGetEntry(string key, float maxAge, out Entry entry)
        {
            //If we have an entry then check its time
            if (TryGetEntry(key, out entry))
            {
                if (entry.GetAge() < maxAge)
                {
                    return true;
                }
            }

            entry = null;
            return false;
        }
        
        /// <summary>
        /// Try and get an entry from the blackboard
        /// </summary>
        /// <param name="key">Key of the entry</param>
        /// <param name="entry">OUT entry to populate</param>
        /// <returns></returns>
        public bool TryGetEntry(string key, out Entry entry)
        {
            if (!entriesDictonary.ContainsKey(key))
            {
                entry = null;
                return false;
            }

            entry = entriesDictonary[key];
            return true;
        }

        /// <summary>
        /// Try and update or add an entry to the team blackboard.
        /// Entry will only be updated if it is better than the existing value
        /// </summary>
        /// <param name="key">Key of Entry</param>
        /// <param name="value">Value of Entry</param>
        /// <param name="agent">Agent that is updating this entry</param>
        /// <param name="confidence">Confidence that the agent has in this entry (default = 1)</param>
        /// <returns>If entry was added or updated</returns>
        public bool TryAddOrUpdateEntry(string key, Vector3 value, BaseAgent agent, float confidence = 1f)
        {
            bool hasExistingEntry = entriesDictonary.ContainsKey(key);
            if (hasExistingEntry)
            {
                //If we have a higher confidence or a more recent time then overwrite 
                Entry existingEntry = entriesDictonary[key];
                float confidenceDifference = confidence - existingEntry.Confidence;
                if (confidenceDifference > 0 || (existingEntry.LastUpdateTime < Time.realtimeSinceStartup && confidenceDifference >= 0))
                {
                    AddOrUpdateEntry(key, value, agent, confidence);
                    return true;
                }
            }
            else
            {
                //If we don't have an existing entry then this is the best we have
                AddOrUpdateEntry(key, value, agent, confidence);
                return true;
            }
            
            //We failed to add or update an entry
            return false;
        }

        /// <summary>
        /// Reset the team blackboard
        /// </summary>
        public void Reset()
        {
            entries.Clear();
            entriesDictonary.Clear();
        }

        /// <summary>
        /// Add or update an entry within the blackboard
        /// </summary>  
        /// <param name="key">Key of entry</param>
        /// <param name="value">Value of entry</param>
        /// <param name="agent">Agent </param>
        /// <param name="confidence">Confidence of the entry</param>
        private void AddOrUpdateEntry(string key, Vector3 value, BaseAgent agent, float confidence)
        {
            //Check if we already have entry and update it
            if (entriesDictonary.ContainsKey(key))
            {
                entriesDictonary[key].Update(value, agent, confidence);
                return;
            }
            
            //Create a new entry
            Entry createdEntry = new Entry(key, value, agent, confidence);
            entries.Add(createdEntry);
            entriesDictonary.Add(createdEntry.Key, createdEntry);
        }
    }
}