using System.IO;
using System.Text;
using Cw3.Handlers;
using Cw3.Middlewares;
using Cw3.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Cw3
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
           // services.AddAuthentication("AuthenticationBasic")
             //   .AddScheme<AuthenticationSchemeOptions, AuthHandler>("AuthenticationBasic", null);

             services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {

                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidIssuer = "Gakko",
                         ValidAudience = "Students",
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]))
                     };
                 });
                     

            services.AddScoped<IStudentsDbService, SqlServerDbService>();
            services.AddSingleton<IDbService, MockDbservice>();
            services.AddControllers().AddXmlSerializerFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IStudentsDbService dbService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //
          // app.UseHttpsRedirection();
          //sprawdzanie/obrabianie otrzymanych danych
            app.UseMiddleware<LoggingMiddleware>();


            app.Use(async (context, next)=>
            {
                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    
                    //za pomoca streamera odczytac body using(var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 102, true)) { await  }
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Nie podano indeksu w naglowku");
                    return;
                }
                else
                {//Sprawdzanie czy w bazie istnieje student o takim indeksie
                    var index = context.Request.Headers["Index"].ToString();
                    if (!dbService.CzyInstnieje(index))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("NIe istnieje student o danym numerze indeksu");
                        return;
                    }

                }
                await next();

            });
           
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
