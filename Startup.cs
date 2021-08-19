using Hangfire;
using Hangfire.Azure;
using HangfireCosmosDB.WebAppHostTest.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireCosmosDB.WebAppHostTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHangfireServer(optionsAction =>
            {
                optionsAction.WorkerCount = 25;
            });

            services.AddHangfireServer(optionsAction =>
            {
                optionsAction.ServerName = "Server2";
                optionsAction.WorkerCount = 25;
                //optionsAction.Queues = new string[] { "server2" };
            });

            services.AddHangfireServer(optionsAction =>
            {
                optionsAction.ServerName = "Server3";
                optionsAction.WorkerCount = 25;
            });

            services.AddScoped<ToDoService>();

            // Init hangfire
            var storageOptions = new CosmosDbStorageOptions
            {
                ExpirationCheckInterval = TimeSpan.FromMinutes(2),
                CountersAggregateInterval = TimeSpan.FromMinutes(2),
                QueuePollInterval = TimeSpan.FromSeconds(15)
            };

            var cosmoClientOptions = new CosmosClientOptions
            {
                ApplicationName = "hangfire",
                RequestTimeout = TimeSpan.FromSeconds(60),
                ConnectionMode = ConnectionMode.Direct,
                MaxRetryAttemptsOnRateLimitedRequests = 1,
                // wait 30s before retry n times. n is set in MaxRetryAttemptsOnRateLimitedRequests which is only 1 in this case
                // Default value is 9 which can cause high usage in RU for cosmos
                MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(30)
            };

            // use cosmos emulator or free comos plan from azure
            string url = "https://localhost:8081"
                , secretkey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
                , database = "hangfire"
                , collection = "hangfire-test";

            services.AddHangfire(o => {
                o.UseAzureCosmosDbStorage(url, secretkey, database, collection, cosmoClientOptions, storageOptions);
            });

            JobStorage.Current = new CosmosDbStorage(url, secretkey, database, collection, cosmoClientOptions, storageOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseHangfireDashboard();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            RecurringJob.AddOrUpdate("TO_DO_TASK_JOB"
                , () => serviceProvider.GetRequiredService<ToDoService>().DoTask()
                , Cron.Minutely()
            );
        }
    }
}
