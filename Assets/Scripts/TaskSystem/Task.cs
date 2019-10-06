using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyRpg
{
    public abstract class Task
    {
        public bool isNeedUpdate
        {
            protected set;
            get;
        }
        public bool isBusy
        {
            private set;
            get;
        }
        private System.Action callback;
        //private TaskExecutor executer;
        //public Task()
        //{
        //    //this.executer = executer;
        //}
        public void Run(System.Action endCallback)
        {
            callback = endCallback;
            isBusy = true;
            Runned();
        }
        public void Interupt()
        {
            Interupted();
            callback();
        }
        protected void End()
        {
            isBusy = false;
            callback();
        }
        protected abstract void Runned();
        protected abstract void Interupted();
        protected abstract void UpdateTick();

    }
}