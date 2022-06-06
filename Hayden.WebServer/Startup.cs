using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Hayden.Consumers.HaydenMysql.DB;
using Hayden.WebServer.DB.Elasticsearch;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Nest;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;

namespace Hayden.WebServer
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		private Config Config { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages();

			services.AddOptions();

			var section = Configuration.GetSection("config");

			Config = section.Get<Config>();
			services.Configure<Config>(section);

			string connectionString = section["DBConnectionString"];

			services.AddDbContext<HaydenDbContext>(x =>
				x.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
					y =>
					{
						y.CommandTimeout(86400);
						y.EnableIndexOptimizedBooleanColumns();
					}));
			
			services.AddSingleton<ElasticClient>(x =>
			{
				var settings = new ConnectionSettings(new Uri(Configuration["Elasticsearch:Url"]))
					.DefaultMappingFor<PostIndex>(map => map.IndexName(PostIndex.IndexName));

				if (bool.Parse(Configuration["Elasticsearch:Debug"]))
				{
					settings.EnableDebugMode();
				}

				return new ElasticClient(settings);
			});

			services.AddMvc(x => { x.EnableEndpointRouting = false; });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			if (env.IsDevelopment())
			{
				app.Use(async (context, next) =>
				{
					context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

					await next();
				});
			}

			app.UseRouting();

			app.UseAuthorization();

			app.UseMvc();
			//app.UseMvc(routes =>
			//{
			//	if (Config.ApiMode)
			//	{
			//		routes.MapRoute("api", "/api/{action=Index}", new { controller = "ArchiveApi" });
			//		routes.MapRoute("frontend-index", "/", new { controller = "ArchiveApi", action = "SvelteFrontend" });
			//		routes.MapRoute("frontend-thread", "{board}/thread/{threadId}", new { controller = "ArchiveApi", action = "SvelteFrontend" });
			//		routes.MapRoute("frontend-privacy", "privacy", new { controller = "ArchiveApi", action = "SvelteFrontend" });
			//	}
			//	else
			//	{
			//		routes.MapRoute("default", "{controller=Archive}/{action=Index}");
			//	}
			//});
		}
	}
}