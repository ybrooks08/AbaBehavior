﻿using System;
using System.Text;
using AbaBackend.Auxiliary;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure;
using AbaBackend.Infrastructure.Collection;
using AbaBackend.Infrastructure.Reporting.Sessions;
using AbaBackend.Infrastructure.Security;
using AbaBackend.Infrastructure.StoProcess;
using AbaBackend.Infrastructure.Utils;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AbaBackend
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
      //_logger = logger;
    }

    public IConfiguration Configuration { get; }
    //private readonly ILogger _logger;

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddHangfire(c => c.UseMemoryStorage());
      services.AddCors(options =>
      {
        options.AddPolicy("CorsPolicy",
          builder => builder
            .AllowAnyOrigin()
            .SetIsOriginAllowed(isOriginAllowed: _ => true)
            .AllowAnyMethod()
            .AllowAnyHeader());
        //.AllowCredentials());
      });

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"])),
            ClockSkew = TimeSpan.Zero
          });
      services.AddScoped<IPasswordHasher, PasswordHasher>();
      services.AddScoped<IUtils, Utils>();
      services.AddScoped<ICollection, Collection>();
      services.AddScoped<IStoProcess, StoProcess>();
      services.AddScoped<TellusManager, TellusManager>();
      services.AddScoped<ICompetencyCheckReport, CompetencyCheckReport>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddDbContext<AbaDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AbaDbConnectionString")));
      //_logger.LogWarning(Configuration.GetConnectionString("AbaDbConnectionString"));
      services.AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
        .AddJsonOptions(o =>
        {
          o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
          o.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
        });
    }

    // public void Configure(IApplicationBuilder app, IHostingEnvironment env, AbaDbContext context, IUtils utils, IStoProcess stoProcess)
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, AbaDbContext context, IUtils utils)
    {
      app.UseDefaultFiles();
      app.UseStaticFiles();

      // loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      // loggerFactory.AddDebug();
      // loggerFactory.AddAzureWebAppDiagnostics();

      if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
      else app.UseHsts();

      //app.UseHttpsRedirection();
      app.UseHangfireServer();
      app.UseHangfireDashboard("/hangfire", new DashboardOptions
      {
        Authorization = new[] { new MyAuthorizationFilter() }
      });
      app.UseCors("CorsPolicy");
      app.UseMvc();

      context.Database.Migrate();

      DbInitializer.CustomSeed(env, context);

      RecurringJob.AddOrUpdate(() => utils.SendEmailsAsync(true), Cron.Hourly);
      RecurringJob.AddOrUpdate(() => utils.MidNightProcess(), Cron.Daily);
      // RecurringJob.AddOrUpdate(() => stoProcess.ProcessStos(), Cron.Daily);
    }

    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
      public bool Authorize(DashboardContext context)
      {
        //var httpContext = context.GetHttpContext();
        return true;//httpContext.User.Identity.IsAuthenticated;
      }
    }
  }
}