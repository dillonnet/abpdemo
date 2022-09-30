using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Application;
using Application.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Web;

[DependsOn(typeof(AbpAspNetCoreMvcModule), typeof(ApplicationModule), typeof(AbpAutofacModule))]
public class WebModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        ConfigureConventionalControllers();
        ConfigSystem(configuration);
        ConfigureAuthentication(context, configuration);
        context.Services.AddAbpSwaggerGen( options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Server API", Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.CustomSchemaIds(type => type.FullName);
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "Swagger使用JWT来进行授权",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] {}
                }
            });
            options.HideAbpEndpoints();
        });
        
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false;
        });
        Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    private void ConfigSystem(IConfiguration configuration)
    {
        Configure<JwtConfig>(configuration.GetSection("Jwt"));
    }

    /// <summary>
    /// 配置动态代理
    /// </summary>
    private void ConfigureConventionalControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(ApplicationModule).Assembly, co =>
            {
               
            });
        });
    }
    
    /// <summary>
    /// 配置认证
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience =  configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        

        app.UseHttpsRedirection();
        app.UseCorrelationId();
        app.UseRouting();
        app.UseAuthentication();
        app.UseJwtTokenMiddleware();

        app.UseAuthorization();
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints();
    }
}