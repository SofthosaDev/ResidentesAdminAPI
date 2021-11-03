using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WsAdminResidentes.Services;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using System.Reflection;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using WsAdminResidentes.Services.Utilidades;
using Microsoft.Extensions.Caching.Distributed;
using WsAdminResidentes.Services.Seguridad;
using WsAdminResidentes.Helpers;
using Microsoft.OpenApi.Models;
using EvaluadorFinancieraWS.Services.Cobranza.Utilidades;
using EvaluadorFinancieraWS.Services.Utilidades;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WsAdminResidentes.Services.Plantillas;

namespace WsAdminResidentes
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            string json_path = "";
            if (env.EnvironmentName == "production")
            {
                json_path = "appsettings.json";
            }
            else
            {
                json_path = "appsettings.Development.json";
            }
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(json_path, optional: false, reloadOnChange: true);


            Configuration = builder.Build();

        }

        public IConfiguration Configuration { get; }

       public void ConfigureServices(IServiceCollection services)
        {
            AgregarAutenticacion(services);
            ConfigurarSwagger(services);
            ConfigurarSqlite(services);
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            services.AddSignalR();
            SetearAppSetting(services);
            AgregarServicios(services);

        }

        public void AgregarAutenticacion(IServiceCollection services)
        {
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("AccesoTemporal",
            //         policy =>
            //         {
            //             policy.RequireAuthenticatedUser();
            //             policy.RequireRole("Registro");
            //         }
            //    );
            //});

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["AppSettings:Secret"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            })
            .AddJwtBearer("Temporal", x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["AppSettings:CredencialTemporal:Secret"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        }
        public void SetearAppSetting(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
        }

        public void AgregarServicios(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDistributedMemoryCache();
            services.AddTransient<WsAdminResidentes.Services.Seguridad.TokenManagerMiddleware>();
            services.AddScoped<IUsuarioService, UsuariosService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IEmailService, EMailService>();
            services.AddScoped<IBaseDatosService, BaseDatosService>();
            services.AddScoped<IArchivosService, ArchivosService>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddScoped<IPlantillasService, PlantillasService>();


        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Residentes -Documentacion");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseTokenManager();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                //endpoints.MapHub<SesionHub>("/sesionHub");
            });

        }

        public void ConfigurarSqlite(IServiceCollection services)
        {
            //services.AddEntityFrameworkSqlite().AddDbContext<SqliteContext>();
        }

        public void ConfigurarSwagger(IServiceCollection services)
        {
            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ResidentesDocumentacion",
                    Version = "v1",
                });

                 c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    In = ParameterLocation.Header, 
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey 
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { 
                    new OpenApiSecurityScheme 
                    { 
                    Reference = new OpenApiReference 
                    { 
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer" 
                    } 
                    },
                    new string[] { } 
                    } 
                });


               //c.IncludeXmlComments(xmlPath);
            });
        }
    }
}


/*
 
        

     */
