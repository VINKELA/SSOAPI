using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SSOService.MiddleWares;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Services.General.Implementation;
using SSOService.Services.General.Interfaces;
using SSOService.Services.Repositories.NonRelational.Implementations;
using SSOService.Services.Repositories.NonRelational.Interfaces;
using SSOService.Services.Repositories.Relational.Implementations;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSOService
{
    public class Startup
    {
        private const string APPName = "SSOService";
        private const string Version = "v1";
        private const string DBConnection = "DefaultConnection";
        private const string SwaggerUrl = "/swagger/v1/swagger.json";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SSODbContext>(options =>
             options.UseSqlServer(Configuration.GetConnectionString(DBConnection)));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(Version, new OpenApiInfo { Title = APPName, Version = Version });
            });
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IServiceResponse, ServiceReponse>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFileRepository, FileServer>();
            services.AddScoped<IToken, TokenService>();
            services.AddScoped<IAuth, AuthService>();

            services.AddAuthentication
            (JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration[JWTConstants.Issuer],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[JWTConstants.Key]))
                };
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("DOC v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "SSO API Documentation",
                    Description = "SSO api documentation",
                    TermsOfService = new Uri("https://robotnigeria.com"),
                    License = new OpenApiLicense
                    {
                        Name = "ROBOT",
                        Url = new Uri("https://robotnigeria.com")
                    },
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter JWT with Bearer token into this field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                 {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                 });
            });




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint(SwaggerUrl, APPName));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<JWTMiddleware>();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
