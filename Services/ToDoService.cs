using HangfireCosmosDB.WebAppHostTest.Job;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireCosmosDB.WebAppHostTest.Services
{
    public class ToDoService
    {
        private readonly ILogger<ToDoService> _logger;

        public ToDoService(ILogger<ToDoService> logger)
        {
            _logger = logger;
        }

        //[LogInfoJobFilterAttribute]
        //[Hangfire.DisableConcurrentExecution(timeoutInSeconds: 60 * 30)]
        public void DoTask()
        {
            _logger.LogInformation($"Task completed {DateTime.Now} by {Thread.CurrentThread.Name} - {Thread.CurrentThread.ManagedThreadId}");
        }

        public void DoAnotherTask()
        {
            _logger.LogInformation($"DoAnotherTask completed {DateTime.Now} by {Thread.CurrentThread.Name} - {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
