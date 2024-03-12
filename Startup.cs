using FormsNet.Common;
using FormsNet.DB;
using FormsNet.DB.DBClass;
using FormsNet.DB.Form;
using FormsNet.DB.IDB;
using FormsNet.DB.IDB.Form;
using FormsNet.Middlewares;
using FormsNet.Services;
using FormsNet.Services.Flow;
using FormsNet.Services.Form;
using FormsNet.Services.Game;
using FormsNet.Services.IServices;
using FormsNet.Services.IServices.Form;
using FormsNet.Services.IServices.Game;
using FormsNet.Services.IServices.Menu;
using FormsNet.Services.IServices.Search;
using FormsNet.Services.Menu;
using FormsNet.Services.Search;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FormsNet
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
			services.ConfigureCors();

			services.AddMemoryCache();

			services.AddControllers();

			services.AddSingleton<IDBConnection, DBConnection>();
			services.AddSingleton<IDB_Test, DB_Test>();
			services.AddSingleton<IBackEndV2DB, BackEndV2DB>();
			services.AddSingleton<IBackEndV2DB_File, BackEndV2DB_File>();
			services.AddSingleton<IBackEndV2DB_Flow, BackEndV2DB_Flow>();
			services.AddSingleton<IBackEndV2DB_Flow_Step, BackEndV2DB_Flow_Step>();
			services.AddSingleton<IBackEndV2DB_Sign, BackEndV2DB_Sign>();

			#region DB Form
			services.AddSingleton<IBackEndV2DB_Form, BackEndV2DB_Form>();
			services.AddSingleton<IDB_Form_DEMO, DB_Form_DEMO>();
			services.AddSingleton<IDB_Form_SR, DB_Form_SR>();
			#endregion

			services.AddSingleton<ICacheService, CacheService>();
			services.AddSingleton<IMyService, MyService>();
			services.AddSingleton<ILoginService, LoginService>();
			services.AddSingleton<IAuthService, AuthService>();
			services.AddSingleton<IFileService, FileService>();
			services.AddSingleton<IFlowService, FlowService>();
			services.AddSingleton<ISearchService, SearchService>();
			services.AddSingleton<ISendEmailService, SendEmailService>();


			#region Menu
			services.AddSingleton<IMenuService, MenuService>();
			#endregion

			#region Game
			services.AddSingleton<IGameService, GameService>();
			#endregion

			#region Form
			services.AddSingleton<IFormService, FormService>();
			services.AddSingleton<IDEMOService, DEMOService>();
			services.AddSingleton<ISRService, SRService>();

			#endregion

			services.AddSingleton<IFlowService, FlowService>();

			services.AddSingleton<IOrganizationService, OrganizationService>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseDeveloperExceptionPage();

			//if (env.IsDevelopment())
			//{
			//}
			//else
			//{
			//	app.AddProductionExceptionHandling();
			//}



			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseCors("CorsPolicy");


			app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/Login/Login")
								&& !context.Request.Path.StartsWithSegments("/api/Login/ResetPw")
								&& !context.Request.Path.StartsWithSegments("/api/Test/Test")
			  , appBuilder =>
			   {
				   appBuilder.UseMiddleware<TokenCheckMiddleware>();
			   });


			//app.UseMiddleware<AuthCheckMiddleware>();





			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
