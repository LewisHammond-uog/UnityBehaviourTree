using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace BT.AI
{
    /// <summary>
    /// Base class of agent
    /// </summary>
    public class BaseAgent : MonoBehaviour
    {
        //List of all AI spawned
        private static List<BaseAgent> allAgents = new List<BaseAgent>();
        
        protected virtual void Start()
        {
            //Add to the AI List
            allAgents.Add(this);
        }

        protected virtual void OnDestroy()
        {
            //Remove this AI
            allAgents.Remove(this);
        }

        /// <summary>
        /// Get all of the AI Agents
        /// </summary>
        /// <returns>IEnumerable of AI Agents</returns>
        public static IEnumerable<BaseAgent> GetAllAgents()
        {
            return allAgents;
        }

        /// <summary>
        /// Get a random point on the Navmesh
        /// </summary>
        /// <param name="randomPos">[OUT] Random Position on the NavMesh</param>
        /// <param name="wanderRadius">Radius to search</param>
        /// <returns>If position was on the navmesh</returns>
        public bool GetRandomPointOnNavMesh(out Vector3 randomPos, float wanderRadius)
        {
            //Determine Random Point
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
        
            //See if it is on the navmesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                randomPos = hit.position;
                return true;
            }

            randomPos = Vector3.zero;
            return false;
        }

        
    }
}