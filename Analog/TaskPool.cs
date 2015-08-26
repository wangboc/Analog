using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Analog
{
    internal class TaskPool
    {
        private static TaskScheduler _mainUiScheduler;

        public static void AddTask(TaskBase task, TaskScheduler scheduler)
        {
            if (_mainUiScheduler == null)
                _mainUiScheduler = scheduler;
            TaskFactory taskFactory = new TaskFactory(_mainUiScheduler);
            Task A = taskFactory.StartNew(task.Run);
            Task B = A.ContinueWith(t => {  }, _mainUiScheduler);
        }
    }
}