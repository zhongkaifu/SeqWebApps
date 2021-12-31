using AdvUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Seq2SeqWebApps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seq2SeqSharp;
using Seq2SeqSharp._SentencePiece;

namespace SeqWebApps
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            if (String.IsNullOrEmpty(Configuration["Seq2Seq:ModelFilePath"]) == false)
            {
                Logger.WriteLine($"Loading Seq2Seq model '{Configuration["Seq2Seq:ModelFilePath"]}'");

                var modelFilePath = Configuration["Seq2Seq:ModelFilePath"];
                var maxTestSrcSentLength = int.Parse(Configuration["Seq2Seq:MaxSrcTokenSize"]);
                var maxTestTgtSentLength = int.Parse(Configuration["Seq2Seq:MaxTgtTokenSize"]);
                var processorType = Configuration["Seq2Seq:ProcessorType"];
                var deviceIds = Configuration["Seq2Seq:DeviceIds"];
                var tokenGenerationStrategy = Configuration["Seq2Seq:TokenGenerationStrategy"];
                var distancePenalty = float.Parse(Configuration["Seq2Seq:DistancePenalty"]);
                var repeatPenalty = float.Parse(Configuration["Seq2Seq:RepeatPenalty"]);
                var topPSampling = float.Parse(Configuration["Seq2Seq:TopPSampling"]);


                SentencePiece srcSpm = new SentencePiece(Configuration["SourceSpm:ModelFilePath"]);
                SentencePiece tgtSpm = new SentencePiece(Configuration["TargetSpm:ModelFilePath"]);


                Seq2SeqSharp.Utils.DecodingStrategyEnums decodingStrategyEnum = (Seq2SeqSharp.Utils.DecodingStrategyEnums)Enum.Parse(typeof(Seq2SeqSharp.Utils.DecodingStrategyEnums), tokenGenerationStrategy);

                Seq2SeqInstance.Initialization(modelFilePath, maxTestSrcSentLength, maxTestTgtSentLength, deviceIds, srcSpm, tgtSpm, decodingStrategyEnum, topPSampling, distancePenalty, repeatPenalty);
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
       //     services.AddMvc();

            services.AddControllersWithViews().
        AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.IncludeFields = true;
        });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
            });

        }
    }
}
