using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopProject
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
            services.AddDbContext<ShopDbContext>(options=>options.UseSqlServer(Configuration.GetConnectionString("ShopProjectDb")));
           




            services.AddControllersWithViews().AddSessionStateTempDataProvider();
            

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //})
            //        .AddCookie();




            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option => {
                //瀏覽器會限制cookie 只能經由HTTP(S) 協定來存取
                option.Cookie.HttpOnly = true;
                //登入頁，未登入時會自動導到登入頁
                option.LoginPath = new PathString("/Customer/Login");
                //登出網頁(可以省略)
                //option.LogoutPath = new PathString("/Account/Logout");
                //登入有效時間
                option.ExpireTimeSpan = TimeSpan.FromHours(3);

                
            });




            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(3);
                options.Cookie.HttpOnly = true;
            });
            services.AddDistributedMemoryCache();

            services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();




            services.AddHttpContextAccessor();


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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
          
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseCookiePolicy();

            
            app.UseAuthentication();

            app.UseAuthorization();
          

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
