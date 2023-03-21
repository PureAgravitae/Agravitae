using AgravitaeWebExtension.Hooks;
using AgravitaeWebExtension.Services;
using DirectScale.Disco.Extension.Middleware;
using PaymentureEwallet;
using WebExtension.Helper;
using WebExtension.Helper.Interface;
using WebExtension.Merchants.CambridgeMerchant.Services;
using WebExtension.Merchants.EwalletMerchant.Ewallet;
using WebExtension.Merchants;
using WebExtension.Repositories;
using WebExtension.Services;

namespace AgravitaeExtension
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region FOR LOCAL DEBUGGING USE
            //
            //
            //
            //Remark This section before upload
            //if (CurrentEnvironment.IsDevelopment())
            //{
            //    services.AddSingleton<ITokenProvider>(x => new WebExtensionTokenProvider
            //    {
            //        DirectScaleUrl = Configuration["configSetting:BaseURL"].Replace("{clientId}", Configuration["configSetting:Client"]).Replace("{environment}", Configuration["configSetting:Environment"]),
            //        DirectScaleSecret = Configuration["configSetting:DirectScaleSecret"],
            //        ExtensionSecrets = new[] { Configuration["configSetting:ExtensionSecrets"] }
            //    });
            //}
            //Remark This section before upload
            //
            //
            //
            #endregion

            string environmentURL = Environment.GetEnvironmentVariable("DirectScaleServiceUrl");

            // services.AddResponseCaching();
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins(environmentURL, environmentURL.Replace("corpadmin", "clientextension"))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin());
            });
            services.AddRazorPages();
            services.AddMvc();
            services.AddDirectScale(options =>
            {
                options.AddHook<SubmitOrderHook>();
                //options.AddHook<SubmitOrderHook>();
                //options.AddCustomPage(Menu.AssociateDetail, "Hello Associate", "ViewAdministration", "/CustomPage/SecuredHelloWorld");
                //options.AddHook<CreateAutoshipHook>();
                //options.AddMerchant<StripeMoneyIn>();
                //options.AddEventHandler("OrderCreated", "/api/webhooks/Order/CreateOrder");

                options.AddMerchant<PaymentureEwalletMoneyOut>(9960, "EWallet", "Paymenture", "USD");
                options.AddMerchant<PaymentureEwalletMoneyInMerchant>(9961, "EWallet", "Paymenture", "USD");
                services.AddControllers();
            });

            //Repositories
            services.AddSingleton<ICustomLogRepository, CustomLogRepository>();
            // services.AddSingleton<IOrdersRepository, OrdersRepository>();

            //Ewallet
            services.AddSingleton<IEwalletRepository, EwalletRepository>();
            services.AddSingleton<ICambridgeRepository, CambridgeRepository>();
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<IEwalletService, EwalletService>();
            services.AddSingleton<ICambridgeService, CambridgeService>();
            services.AddSingleton<ICambridgeSetting, CambridgeSetting>();
            services.AddSingleton<IClientService, ClientService>();

            //Services
            services.AddSingleton<IAVOrderService, AVOrderService>();
            services.AddSingleton<ICommonService, CommonService>();
            services.AddSingleton<IHttpClientService, HttpClientService>();
            services.AddSingleton<INomadEwalletService, NomadEwalletService>();
            services.AddSingleton<ICustomLogService, CustomLogService>();
            services.AddControllersWithViews();

            //Swagger
            services.AddSwaggerGen();
            //Configurations
            //services.Configure<configSetting>(Configuration.GetSection("configSetting"));
            services.AddMvc(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var environmentUrl = Environment.GetEnvironmentVariable("DirectScaleServiceUrl");
            if (environmentUrl != null)
            {
                var serverUrl = environmentUrl.Replace("https://agravitae.corpadmin.", "");
                var appendUrl = @" http://"+ serverUrl + " " + "https://" + serverUrl + " " + "http://*." + serverUrl + " " + "https://*." + serverUrl;

                var csPolicy = "frame-ancestors https://agravitae.corpadmin.directscale.com https://agravitae.corpadmin.directscalestage.com" + appendUrl + ";";
                app.UseRequestLocalization();

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                //Configure Cors

                app.UseCors("CorsPolicy");
                app.UseHttpsRedirection();

                app.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    }
                });

                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthorization();

                //DS
                app.UseDirectScale();

                //Swagger
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V2");
                });

                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Content-Security-Policy", csPolicy);
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    await next();
                });
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });
                app.UseMvc();
            }


            
        }
    }
    internal class WebExtensionTokenProvider : ITokenProvider
    {
        public string DirectScaleUrl { get; set; }
        public string DirectScaleSecret { get; set; }
        public string[] ExtensionSecrets { get; set; }

        public async Task<string> GetDirectScaleSecret()
        {
            return await Task.FromResult(DirectScaleSecret);
        }
        public async Task<string> GetDirectScaleServiceUrl()
        {
            return await Task.FromResult(DirectScaleUrl);
        }
        public async Task<IEnumerable<string>> GetExtensionSecrets()
        {
            return await Task.FromResult(ExtensionSecrets);
        }

    }
}
