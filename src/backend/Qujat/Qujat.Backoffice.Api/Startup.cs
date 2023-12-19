using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qujat.Core.Data.Entities;
using Qujat.Core.Data;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using Microsoft.EntityFrameworkCore;
using Qujat.Core.Services;
using Qujat.Backoffice.Api.Middlewares;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Net.Http.Headers;
using Serilog;
using Prometheus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;

namespace Qujat.Backoffice.Api
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
                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."

                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                config.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                config.DocInclusionPredicate((name, api) => true);
            });

            var connectionString = _configuration["Backend:Database:ApplicationDbConnectionString"];

            services
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(connectionString));

            services.AddIdentity<ApplicationUserEntity, IdentityRole<long>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 8;
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


            var externalBlobServiceConfig = new ExternalS3BlobServiceConfig()
            {
                S3StorageBaseUri = "https://object.pscloud.io",
                BucketName = "qujat-blob-storage",
                AccessKey = "3S5JZTG0CNJD3VC1R1KM",
                SecretKey = "9fabPIFurnF6GFSIozppoJrOS9esrfg51UWh7K4q"
            };

            //var externalBlobServiceConfig = new ExternalS3BlobServiceConfig()
            //{
            //    S3StorageBaseUri = "http://qujat-temp-minio-api.zonakomforta.kz",
            //    BucketName = "qujat-blob-storage",
            //    AccessKey = "gJAQBRWy9ashY9OFdaxj",
            //    SecretKey = "nMcdwiK4A4yyJa5nUHsGKahL16Jht78jBfTktgHc"
            //};

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

            app.UseCors("defaultCorsPolicy");

            //app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });


            if (!_environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}
