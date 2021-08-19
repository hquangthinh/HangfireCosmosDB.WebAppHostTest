using Hangfire.Common;
using Hangfire.Logging;
using Hangfire.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireCosmosDB.WebAppHostTest.Job
{
    public class LogInfoJobFilterAttribute : JobFilterAttribute, IServerFilter
    {
        private readonly ILog logger = LogProvider.For<LogInfoJobFilterAttribute>();

        public void OnPerformed(PerformedContext filterContext)
        {

        }

        public void OnPerforming(PerformingContext filterContext)
        {
            var json = JsonConvert.SerializeObject(filterContext.Items);
            logger.Info(json);
        }
    }
}
