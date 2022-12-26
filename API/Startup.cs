using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            Configuration = configuration;
        }

        public string GetConnectionString(string name)
        {
            string conStr = Configuration.GetConnectionString(name);
            if (String.IsNullOrEmpty(conStr))
            {
                conStr = Environment.GetEnvironmentVariable($"SQLAZURECONNSTR_{name}", EnvironmentVariableTarget.Process);
            }
            return conStr;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var connectionString = GetConnectionString("TrekordConnectionString");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString,
                options => options.EnableRetryOnFailure()));

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Trekord", Version = "v1" });
            });
            

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trekord v1"));

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

