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
        static HashSet<string> setInputSents = new HashSet<string>();
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerateText(string srcInput, string tgtInput, int num, bool random, float distancePenalty, float repeatPenalty, int contextSize)
        {
            TextGenerationModel textGeneration = new TextGenerationModel
            {
                Output = CallBackend(srcInput, tgtInput, num, random, distancePenalty, repeatPenalty, contextSize),
                DateTime = DateTime.Now.ToString()
            };

            return Json(textGeneration);
        }


        private string CallBackend(string srcInputText, string tgtInputText, int tokenNumToGenerate, bool random, float distancePenalty, float repeatPenalty, int tgtContextSize)
        {
            srcInputText = srcInputText.Replace("<br />", "");
            tgtInputText = tgtInputText.Replace("<br />", "");

            string[] srcLines = srcInputText.Split("\n");
            string[] tgtLines = tgtInputText.Split("\n");

            srcInputText = String.Join(" ", srcLines).ToLower();
            tgtInputText = String.Join(" ", tgtLines).ToLower();


            string prefixTgtLine = "";
            if (tgtInputText.Length > tgtContextSize)
            {
                prefixTgtLine = tgtInputText.Substring(0, tgtInputText.Length - tgtContextSize);
                tgtInputText = tgtInputText.Substring(tgtInputText.Length - tgtContextSize);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            if (srcInputText.EndsWith("。") == false && srcInputText.EndsWith("？") == false && srcInputText.EndsWith("！") == false)
            {
                srcInputText = srcInputText + "。";
            }

            if (setInputSents.Contains(srcInputText) == false)
            {
                Logger.WriteLine(srcInputText);
                setInputSents.Add(srcInputText);
            }

            string outputText = Seq2SeqInstance.Call(srcInputText, tgtInputText, tokenNumToGenerate, random, distancePenalty, repeatPenalty);

            stopwatch.Stop();

            outputText = prefixTgtLine + outputText;

            string[] outputSents = outputText.Split("。");

            return String.Join("。<br />", outputSents);

        }

    }
}
