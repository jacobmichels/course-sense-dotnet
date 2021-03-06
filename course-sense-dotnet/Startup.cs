using course_sense_dotnet.AlertSystem;
using course_sense_dotnet.DataAccessLayer;
using course_sense_dotnet.NotificationManager;
using course_sense_dotnet.Utility;
using course_sense_dotnet.WebAdvisor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;

namespace course_sense_dotnet
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
            services.AddSingleton<IDataAccess, DataAccess>();
            services.AddTransient<IEmailClient, EmailClient>();
            services.AddTransient<IAlertContact, AlertContact>();
            services.AddTransient<ITwilioClientWrapper, TwilioClientWrapper>();
            services.AddTransient<IContactValidation, ContactValidation>();
            services.AddTransient<IRequestsHelper, RequestsHelper>();
            services.AddTransient<IRequests, Requests>();
            services.AddTransient<INotificationManager, NotificationManager.NotificationManager>();
            services.AddSingleton<SynchronizedCollection<NotificationRequest>>();
            services.AddTransient<IList<Task>, List<Task>>();

            services.AddHostedService<PollingLoop>();

            services.AddLogging();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "course_sense_dotnet", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "course_sense_dotnet v1"));
            }

            loggerFactory.AddFile(app.ApplicationServices.GetService<IConfiguration>()["Logging:FilePath"]);

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
