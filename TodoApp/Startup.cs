﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using TodoApp.Common.Config;
using TodoApp.Common.Models;
using TodoApp.Repositories;

namespace TodoApp
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
            services.Configure<MongoDbConfig>(
                options => {
                    options.Database = Configuration.GetSection("MongoDB:Database").Value;
                    options.Host = Configuration.GetSection("MongoDB:Host").Value;
                    options.Port = Int32.Parse(Configuration.GetSection("MongoDB:Port").Value);
                    options.User = Configuration.GetSection("MongoDB:User").Value;
                    options.Password = Configuration.GetSection("MongoDB:Password").Value;
                });
            services.AddSwaggerGen(
                c => {
                    c.SwaggerDoc("v1", new Info {
                        Title = "Todo Api",
                        Version = "v1",
                        Description = "Todo Api using mongodb"
                    });
                });

            services.AddTransient<ITodoContext, TodoContext>();
            services.AddSingleton<ITodoRepository, TodoRepository>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(s => {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
