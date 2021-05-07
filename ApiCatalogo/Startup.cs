using ApiCatalogo.Context;
using ApiCatalogo.Extensions;
using ApiCatalogo.Filters;
using ApiCatalogo.Logging;
using ApiCatalogo.Repository;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // Conexão com o BD
            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => 
                options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

            //Repositório utilizando o padrão Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Serviço para injeção de dependencia no response da API (Produtos -> [HttpGet("saudacao/{nome}")] -> [FromServices] IMeuServico meuservico)
            services.AddTransient<IMeuServico, MeuServico>();
            // Transient - vai ser criada cada vez que for solicitada, sempre um objeto novo.
            // Scoped - vai ser criado uma vez dentro da requisição, ao final ele é destruído.
            // Singleton - vai ser criado uma unica instancia geral quando se inicia a api, sempre mesmo objeto.

            // Filtro Personalizado
            services.AddScoped<ApiLoggingFilter>();

            services.AddControllers().AddNewtonsoftJson(options => {
                // Evitar erro de referência cíclica
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
