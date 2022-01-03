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
        public JsonResult GenerateText(string srcInput, string tgtInput, int num, bool random, float distancePenalty, float repeatPenalty)
        {
            Logger.WriteLine($"Receive request source string '{srcInput}' and target string '{tgtInput}'");
            TextGenerationModel textGeneration = new TextGenerationModel
            {
                Output = CallBackend(srcInput, tgtInput, num, random, distancePenalty, repeatPenalty),
                DateTime = DateTime.Now.ToString()
            };

            return Json(textGeneration);
        }


        private string CallBackend(string srcInputText, string tgtInputText, int tokenNumToGenerate, bool random, float distancePenalty, float repeatPenalty)
        {
            string[] srcLines = srcInputText.Split("\n");
            string[] tgtLines = tgtInputText.Split("\n");

            List<string> outputLines = new List<string>();

            for (int i = 0; i < srcLines.Length;i++)        
            {
                string srcLine = srcLines[i].ToLower();
                string tgtLine = tgtLines[i].ToLower();

                Stopwatch stopwatch = Stopwatch.StartNew();

                //if (ll.EndsWith("。") == false && ll.EndsWith("？") == false && ll.EndsWith("！") == false)
                //{
                //    ll = ll + "。";
                //}


                string outputText = Seq2SeqInstance.Call(srcLine, tgtLine, tokenNumToGenerate, random, distancePenalty, repeatPenalty);

                outputLines.Add(outputText);

                stopwatch.Stop();

                Logger.WriteLine($"'{srcLine}' and '{tgtLine}' --> '{outputText}', took: {stopwatch.Elapsed}");
            }

            return String.Join("<br />", outputLines);
        }

    }
}
