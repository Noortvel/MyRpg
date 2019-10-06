using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace MyRpg
{
    public class TaskMoveTo : Task
    {
        private Character character;
        public Vector3 destonation
        {
            private set;
            get;
        }
        private bool isMoving = false;
        public TaskMoveTo(Character character, Vector3 position)
        {
            this.character = character;
            destonation = position;
            isNeedUpdate = true;
        }
        private void Stop()
        {
            isMoving = false;
            character.navMeshAgent.isStopped = true;
            character.navMeshAgent.updateRotation = false;
            character.navMeshAgent.SetDestination(character.transform.position);
            destonation = Vector3.zero;
            End();
        }
        protected override void Runned()
        {
            character.navMeshAgent.updateRotation = true;
            character.navMeshAgent.SetDestination(destonation);
            character.navMeshAgent.isStopped = false;
            isMoving = true;
        }
        protected override void Interupted()
        {
            Stop();
        }
        private float eps = 0.1f;
        protected override void UpdateTick()
        {
            if (isMoving && 
                (character.navMeshAgent.destination - character.transform.position).magnitude < eps)
            {
                Stop();
            }
        }
    }
}