using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyRpg
{
    public class TaskExecutor
    {
        private Queue<Task> queue;
        private Task current;
        public TaskExecutor()
        {
            queue = new Queue<Task>();
        }
        public void AddTaskToQueue(Task task)
        {
            queue.Enqueue(task);
        }
        public void InteruptCurrent()
        {
            if (current != null)
            {
                current.Interupt();
                current = null;
            }
        }
        public void ClearQueue()
        {
            queue.Clear();
        }
        public void ExecuteQueue()
        {
            InteruptCurrent();
            if (queue.Count > 0)
            {
                current = queue.Dequeue();
                current.Run(EndCurrentTask);
            }
        }
        public void Execute(Task task)
        {
            queue.Clear();
            InteruptCurrent();
            current = task;
            current.Run(EndCurrentTask);
        }
        private void EndCurrentTask()
        {
            if(queue.Count > 0)
            {
                current = queue.Dequeue();
                current.Run(EndCurrentTask);
            }
        }
        public void Update()
        {
            if(current != null && current.isNeedUpdate)
            {

            }
        }
    }
}
