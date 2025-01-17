using BlueModus.Redirector.Middleware;
using BlueModus.Redirector.Web.Jobs;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Caching.Hybrid;
using Quartz;

namespace BlueModus.Redirector.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<IRedirectItemService, RedirectItemService>();
            builder.Services.AddSingleton<IRedirectItemComparer, RedirectItemComparer>();
#pragma warning disable EXTEXP0018
            builder.Services.AddHybridCache(options =>
            {
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(5),
                    LocalCacheExpiration = TimeSpan.FromMinutes(5)
                };
            });
#pragma warning restore EXTEXP0018

            builder.Services.AddQuartz(configure =>
            {
                var jobKey = new JobKey(nameof(RefreshRedirectCacheJob));

                configure
                    .AddJob<RefreshRedirectCacheJob>(jobKey)
                    .AddTrigger(
                        trigger => trigger
                            .ForJob(jobKey)
                            .WithSimpleSchedule(schedule =>
                                schedule
                                    .WithIntervalInSeconds(10)
                                    .RepeatForever()));
            });

            builder.Services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var rewriteOptions = new RewriteOptions()
                .AddRedirectToHttpsPermanent();
            app.UseRewriter(rewriteOptions);

            app.UseRedirectRules();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapGet("/campaigns/{name}/{channel?}", (string name, string? channel) => $"Campaign: {name} > Channel: {channel ?? "None"}");
            app.MapGet("/products/{category?}/{subCategory?}/{item?}", (string? category, string? subCategory, string? item) =>
                $"Products > Category: {category ?? "None"} > SubCategory: {subCategory ?? "None"} > Item: {item ?? "None"}");

            app.Run();
        }
    }
}
