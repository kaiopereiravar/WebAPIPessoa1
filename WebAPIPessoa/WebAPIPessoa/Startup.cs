using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPIPessoa.Application.Autenticacao;
using WebAPIPessoa.Application.Eventos;
using WebAPIPessoa.Repository;

namespace WebAPIPessoa
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; //Ele faz referencia ao nosso appsettings.json
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        //USAR O SWAGGER
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PessoasAPI", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });


            //USAR A DOCUMENTA��O
            services.AddAuthentication
                 (JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,

                         ValidIssuer = "var",
                         ValidAudience = "var",
                         IssuerSigningKey = new SymmetricSecurityKey
                       (Encoding.UTF8.GetBytes("8fab334a-5e6f-4d7c-8d8c-c35241836bd6"))
                     };
                 });

            services.AddAuthorization();

            services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();// quando alguem referenciar a interface, ele resolve com a implementa��o do RabbitMQProducer  
            services.AddScoped<IAutenticacaoService, AutenticacaoService>();

            //REFERENCIANDO O BANCO DE DADOS AQUI

            services.AddDbContext<PessoaContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ConexaoPessoa")));//Vai ter um contexto(que � uma representa��o do banco de dados)__o UseSqlServer � o endere�o do seu banco de dados__ nesse caso esse endere�o vai estar la no appsettings.json na ConexaoPessoa
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());

            //Ativa o Swagger
            app.UseSwagger();

            // Ativa o Swagger UI
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoAPI V1");
                opt.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
