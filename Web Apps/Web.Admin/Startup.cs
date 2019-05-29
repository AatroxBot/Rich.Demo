using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Web.Admin.Common;
using Web.Admin.Infrastructure;
using Web.Admin.Services;
using Web.Admin.ViewModels;

namespace Web.Admin
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddOptions();
            services.Configure<AppSettings>(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            AddCustomAuthentication(services, Configuration);

            services.AddHttpClientServices(Configuration);
        }

       


        public static IServiceCollection AddCustomAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var useLoadTest = configuration.GetValue<bool>("UseLoadTest");
            var identityUrl = configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = configuration.GetValue<string>("CallBackUrl");

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
           .AddCookie(setup => setup.ExpireTimeSpan = TimeSpan.FromHours(2))
           .AddOpenIdConnect(options =>
           {
               options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
               options.Authority = identityUrl.ToString();
               options.SignedOutRedirectUri = callBackUrl.ToString();
               options.ClientId = useLoadTest ? "admintest" : "admin";
               options.ClientSecret = "secret";
               options.ResponseType = useLoadTest ? "code id_token token" : "code id_token";
               options.SaveTokens = true;
               options.GetClaimsFromUserInfoEndpoint = true;
               options.RequireHttpsMetadata = false;
               options.Scope.Add("openid");
               options.Scope.Add("profile");
               options.Scope.Add("article");

           });

            return services;
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
          
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "area",
                   template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }


    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            //add custom application services
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

            services.AddHttpClient<IArticleService, ArticleService>()
              .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
              .AddPolicyHandler(GetRetryPolicy())
              .AddPolicyHandler(GetCircuitBreakerPolicy());
              //.AddDevspacesSupport();

            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
              .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        }

        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }

   
}
