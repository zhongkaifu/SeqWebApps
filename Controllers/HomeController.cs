using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AdvUtils;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Seq2SeqWebApps;

namespace SeqWebApps
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerateText(string input, int num, bool random, float distancePenalty, float repeatPenalty)
        {
            Logger.WriteLine($"Receive request string '{input}'");
            TextGenerationModel textGeneration = new TextGenerationModel
            {
                Output = CallBackend(input, num, random, distancePenalty, repeatPenalty),
                DateTime = DateTime.Now.ToString()
            };

            return Json(textGeneration);
        }


        private string CallBackend(string InputText, int tokenNumToGenerate, bool random, float distancePenalty, float repeatPenalty)
        {
            string[] lines = InputText.Split("\n");
            List<string> outputLines = new List<string>();

            foreach (var line in lines)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                string ll = line.ToLower().Trim();
                if (ll.EndsWith("。") == false && ll.EndsWith("？") == false && ll.EndsWith("！") == false)
                {
                    ll = ll + "。";
                }

                string outputText = Seq2SeqInstance.Call(ll, tokenNumToGenerate, random, distancePenalty, repeatPenalty);

                outputLines.Add(outputText);

                stopwatch.Stop();

                Logger.WriteLine($"'{line}' --> '{outputText}', took: {stopwatch.Elapsed}");
            }

            return String.Join("<br />", outputLines);
        }

    }
}
