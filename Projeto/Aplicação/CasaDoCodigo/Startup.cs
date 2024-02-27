using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDoCodigo
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
            services.AddMvc();

            //adicionando sessao na pagina e cache
            services.AddDistributedMemoryCache();
            services.AddSession();

            //Primeiro criar uma classe para atribuir o contexto para criação das tabelas no banco
            //Utilizar DbContextOtions e OnModelCrieting e ModelBuilder
            //Criar no appSetting.json a string de conexão, foi criada com nome (Default)
            //depois ir na Startup e aplicar o services.AddDbContext

            //Entrar no Package Manager Console para aplicar os comandos no prompt para geração de pacotes necessários(migration)
            //Add-Migration Inicial --> Vai criar uma pasta de Migrations
            //Update-Database -verbose --> Vai gerar as tabelas no banco
            //
            string connectionString = Configuration.GetConnectionString("Default");
            services.AddDbContext<AplicationContext>(options =>
            options.UseSqlServer(connectionString)
            );

            //Necessário incluir as interfaces na startup
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<IProdutoRepository, ProdutoRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();
            services.AddTransient<ICadastroRepository, CadastroRepository>();
            services.AddTransient<IItemPedidoRepository, ItemPedidoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //adicionar servico de sessao
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Pedido}/{action=Carrossel}/{codigo?}");
            });
            //CONFIGURACAO PARA GARANTIR QUE O BANCO FOI CRIADO, CRIA AUTOMATICO
            //SE NAO HOUVER BANCO ATRAVÉS DO serviceProvider.GetService
            serviceProvider.GetService<IDataService>().InicializaDB();
        }
    }
}
