using ApiCatalogo.Context;
using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Extensions;
using ApiCatalogo.Filters;
using ApiCatalogo.Logging;
using ApiCatalogo.Repository;
using ApiCatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiCatalogo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors();

            // Conexão com o BD
            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => 
                options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // JWT
            // Adiciona  o manipulador de autenticação e define o
            // esquema de autenticação usado: Bearer
            // Valida o emissor, a audiencia e a chave
            // Usando a chave secreta valida a assinatura
            services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = Configuration["TokenConfiguration:Audience"],
                    ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                });


            // Repositório utilizando o padrão Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // DTO - Criando o mapeamento de entidades utilizando AutoMapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            // Serviço para injeção de dependencia no response da API (Produtos -> [HttpGet("saudacao/{nome}")] -> [FromServices] IMeuServico meuservico)
            services.AddTransient<IMeuServico, MeuServico>();
            // Transient - vai ser criada cada vez que for solicitada, sempre um objeto novo.
            // Scoped - vai ser criado uma vez dentro da requisição, ao final ele é destruído.
            // Singleton - vai ser criado uma unica instancia geral quando se inicia a api, sempre mesmo objeto.

            // Versionamento da API
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(option =>
            {
                option.GroupNameFormat = "'v'VVV"; // "'v'VVV" = v+MajorVersion+MinorVersion+Patch
                option.SubstituteApiVersionInUrl = true; // Exibir a versão na URL
            });

            // Filtro Personalizado
            services.AddScoped<ApiLoggingFilter>();

            services.AddControllers().AddNewtonsoftJson(options => {
                // Evitar erro de referência cíclica
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            //Swagger
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "APICatalogo",
                    Description = "Catálogo de Produtos e Categorias",
                    TermsOfService = new Uri("https://wedoostudio.com.br/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Magno Dourado",
                        Email = "luiz.ti@live.com",
                        Url = new Uri("https://wedoostudio.com.br")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Usar sobre LICX",
                        Url = new Uri("https://wedoostudio.com.br/license")
                    }
                });

                // Adicinar os comentários na documentação do swagger
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                // Para habilitar o uso do JwtToken no swagger
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme."
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement{
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
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));

            // Adicionando o middleware de tratamento de erros
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

            //app.UseCors(option => option.WithOrigins("http://apirequest.io", "https://apirequest.io", "apirequest.io"));

            app.UseCors(option => option.AllowAnyOrigin());

            //Swagger
            app.UseSwagger();

            //Swagger UI
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "Catálogo de Produtos e Categorias");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
