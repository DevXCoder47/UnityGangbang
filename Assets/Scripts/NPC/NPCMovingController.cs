using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    public class NPCMovingController : MonoBehaviour
    {
        private Transform target;
        private NavMeshAgent agent;

        public Transform Target {
            get => target;
            set
            {
                target = value;
                if (agent != null && target != null)
                    agent.SetDestination(target.position);
            }
        } 

        public NavMeshAgent Agent { get => agent; }

        
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            agent.avoidancePriority = Random.Range(20, 80); 
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null)
                return;

            // Проверяем, достиг ли агент цели
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    // Цель достигнута
                    target = null;
                }
            }
        }
    }
}
