using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyRpg
{
    [RequireComponent(typeof(NavMesh))]
    public class Character : MonoBehaviour
    {
        private TaskExecutor taskExecutor;
        public NavMeshAgent navMeshAgent
        {
            private set;
            get;
        }
        public byte netId = 0;
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            taskExecutor = new TaskExecutor();
        }
        public void MoveTo(Vector3 position)
        {
            taskExecutor.Execute(new TaskMoveTo(this, position));
        }
        void Update()
        {
            taskExecutor.Update();
        }
    }
}