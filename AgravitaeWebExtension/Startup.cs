using AgravitaeWebExtension.Hooks;
using AgravitaeWebExtension.Services;
using DirectScale.Disco.Extension.Middleware;
using PaymentureEwallet;
using AgravitaeWebExtension.Helper;
using AgravitaeWebExtension.Helper.Interface;
using AgravitaeWebExtension.Merchants;
using AgravitaeWebExtension.Repositories;
using AgravitaeWebExtension.Hooks.Associate;
using AgravitaeWebExtension.Hooks.Autoship;
using AgravitaeWebExtension.Hooks.order;
using AgravitaeExtension.Merchants.Tyga.Interfaces;
using AgravitaeExtension.Merchants.Tyga.Tyga;
using ZiplingoEngagement;
using RPMSEwallet;
using RPMSEwallet.Merchants.EwalletMerchant.Ewallet;

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
            //    services.AddSingleton<ITokenProvider>(x => new AgravitaeWebExtensionTokenProvider
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
            services.AddZiplingoEngagement();
            services.AddRPMSEwallet();
            services.AddDirectScale(options =>
            {
                options.AddHook<SubmitOrderHook>();
                options.AddHook<WriteApplication>();
                options.AddHook<UpdateAssociateHook>();
                options.AddHook<CreateAutoshipHook>();
                options.AddHook<UpdateAutoshipHook>();
                options.AddHook<FinalizeAcceptedOrderHook>();
                options.AddHook<FinalizeNonAcceptedOrder>();
                options.AddHook<FullRefundOrderHook>();
                options.AddHook<LogRealtimeRankAdvanceHook>();
                options.AddHook<MarkPackageShippedHook>();

                // Functionality for 201450 
                options.AddHook<ProcessCouponCodesHook>();
                options.AddHook<GetCouponAdjustedVolumeHook>();

                //options.AddHook<SubmitOrderHook>();
                //options.AddCustomPage(Menu.AssociateDetail, "Hello Associate", "ViewAdministration", "/CustomPage/SecuredHelloWorld");
                //options.AddHook<CreateAutoshipHook>();
                //options.AddMerchant<StripeMoneyIn>();
                //options.AddEventHandler("OrderCreated", "/api/webhooks/Order/CreateOrder");

                options.AddMerchant<PaymentureEwalletMoneyOut>(9960, "EWallet", "Paymenture", "USD");
                options.AddMerchant<PaymentureEwalletMoneyInMerchant>(9961, "EWallet", "Paymenture", "USD");

                options.AddMerchant<EwalletMoneyIn>(9012, "E-Wallet", "Pay with E-Wallet", "USD");
                options.AddMerchant<EwalletMoneyOut>(9013, "E-Wallet", "Pay with E-Wallet", "USD");

                options.AddMerchant<TygaMoneyIn>(9014, "Tyga Wallet", "Pay with crypto", "USD"); //TygaPay

                services.AddControllers();
            });

            //Repositories
            services.AddSingleton<ICustomLogRepository, CustomLogRepository>();
            services.AddSingleton<IGenericReportRepository, GenericReportRepository>();
            services.AddSingleton<IReportSourceRepository, ReportSourceRepository>();
            services.AddSingleton<IDailyRunCustomRepository, DailyRunCustomRepository>();
            services.AddSingleton<IRankAdvancementRepository, RankAdvancementRepository>();
            services.AddSingleton<IAssociateWebRepository, AssociateWebRepository>();


            // services.AddSingleton<IOrdersRepository, OrdersRepository>();

            //Ewallet
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<IClientService, ClientService>();
            

            //Tyga services and repo
            services.AddSingleton<ITygaService, TygaService>();
            services.AddSingleton<ITygaRepository, TygaRepository>();

            //Services
            services.AddSingleton<IAVOrderService, AVOrderService>();
            services.AddSingleton<ICommonService, CommonService>();
            services.AddSingleton<IHttpClientService, HttpClientService>();
            services.AddSingleton<IGenericReportService, GenericReportService>();
            services.AddSingleton<IDailyRunCustomService, DailyRunCustomService>();
            services.AddSingleton<IRankAdvancementService, RankAdvancementService>();
            services.AddSingleton<ICustomLogService, CustomLogService>();
            services.AddSingleton<IAssociateWebService, AssociateWebService>();
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
    internal class AgravitaeWebExtensionTokenProvider : ITokenProvider
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
