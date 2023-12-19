using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qujat.Core.Data.Entities;
using Qujat.Core.Data;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using Microsoft.EntityFrameworkCore;
using Qujat.Core.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Net.Http.Headers;
using Serilog;
using Prometheus;
using Microsoft.Extensions.Hosting;
using Qujat.Api.Middlewares;
using Newtonsoft.Json.Converters;
using Qujat.Api.Services;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Collections.Generic;

namespace Qujat.Api
{
    public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IWebHostEnvironment _environment = environment;

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                })
                .AddNewtonsoftJson(config =>
                {
                    var converter = new StringEnumConverter();
                    config.SerializerSettings.Converters.Add(converter);
                });

            services.AddHttpContextAccessor();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("frontend", new OpenApiInfo { Title = "frontend", Version = "frontend" });
                config.SwaggerDoc("openapi", new OpenApiInfo { Title = "openapi", Version = "openapi" });

                config.DocInclusionPredicate((name, api) =>
                {
                    if (!api.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<ApiExplorerSettingsAttribute>()
                        .Select(attr => attr.GroupName);

                    return versions.Any(v => v == name);
                });
            });

            var connectionString = _configuration["Backend:Database:ApplicationDbConnectionString"];

            services
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));

            services.AddIdentity<ApplicationUserEntity, IdentityRole<long>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 4;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.SignIn.RequireConfirmedEmail = false;
                options.Stores.MaxLengthForKeys = 128;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //services.AddScoped<PhoneNumberValidator>();
            services.AddScoped<VerificationService>();
            services.AddScoped<ISmsNotificationService, TelegramMockSmsNotificationProvider>();


            services.AddScoped<ICurrentUserProvider, HttpContextCurrentUserProvider>();

            services.AddScoped<IAccessTokenGenerator, DefaultJwtAccessTokenGenerator>();
            services.AddScoped<IAccessTokenDecoder, DefaultAccessTokenDecoder>();

            services.AddMemoryCache();
            services.AddScoped<ICacheProvider, DefaultCacheProvider>();

            services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

            services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add(HeaderNames.Accept);
                logging.RequestHeaders.Add(HeaderNames.ContentType);
                logging.RequestHeaders.Add(HeaderNames.ContentDisposition);
                logging.RequestHeaders.Add(HeaderNames.ContentEncoding);
                logging.RequestHeaders.Add(HeaderNames.ContentLength);

                logging.MediaTypeOptions.AddText("application/json");
                logging.MediaTypeOptions.AddText("multipart/form-data");

                logging.RequestBodyLogLimit = 1024;
                logging.ResponseBodyLogLimit = 1024;
            });

            services.AddSingleton<RequestBodyRewindMiddleware>();

            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddAutoMapper(typeof(Startup));
            services.AddHttpClient();
            services.AddScoped<EmailSenderProviderFactory>();

            var externalBlobServiceConfig = new ExternalS3BlobServiceConfig()
            {
                S3StorageBaseUri = "https://object.pscloud.io",
                BucketName = "qujat-blob-storage",
                AccessKey = "3S5JZTG0CNJD3VC1R1KM",
                SecretKey = "9fabPIFurnF6GFSIozppoJrOS9esrfg51UWh7K4q"
            };

            services.AddSingleton(externalBlobServiceConfig);
            services.AddScoped<ExternalS3BlobService>();

            var corsPolicyName = "defaultCorsPolicy";

            services.AddCors(opt =>
                opt.AddPolicy(corsPolicyName, policy =>
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()));

        }


        public void Configure(IApplicationBuilder app, ApplicationDbContext applicationDbContext)
        {
            app.UseMiddleware<RequestBodyRewindMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseHttpLogging();

            app.UseSerilogRequestLogging();

            app.MapProblemDetails();

            app.UseRouting();
            
            app.UseHttpMetrics();

            app.UseCors("defaultCorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });
            

            if (!_environment.IsProduction())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/frontend/swagger.json", "frontend");
                    c.SwaggerEndpoint("/swagger/openapi/swagger.json", "openapi");

                    c.DefaultModelRendering(ModelRendering.Model);
                    c.DisplayRequestDuration();
                    c.DocExpansion(DocExpansion.List);
                    c.ShowExtensions();
                    c.ShowCommonExtensions();
                    c.UseRequestInterceptor("(request) => { return request; }");
                    c.UseResponseInterceptor("(response) => { return response; }");
                });
            }
        }
    }
}
