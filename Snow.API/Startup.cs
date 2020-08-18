using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;
using Snow.API.MiddleWare;
using Snow.Utility.Handler;
using Snow.Utility.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace Snow.API
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Snow Message API", Version = "v1" });
            });

            // Inject App DI
            services.AddSingleton<IMessageHandler, MessageHandler>();
            services.AddSingleton<IConnection>(s =>
            {
                Options opts = ConnectionFactory.GetDefaultOptions();
                opts.Url = Defaults.Url;
                return new ConnectionFactory().CreateConnection(opts);
            });

            // Inject Automapper
            services.AddAutoMapper(typeof(MessageMapperProfile));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "Snow Message API");
                c.RoutePrefix = "";
            });

            app.UseMvc();
        }
    }
}
