using System;
using System.Collections.Generic;
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
        public JsonResult GenerateText(string input, int num)
        {
            TextGenerationModel textGeneration = new TextGenerationModel
            {
                Output = CallBackend(input, num),
                DateTime = DateTime.Now.ToString()
            };

            return Json(textGeneration);
        }


        private string CallBackend(string InputText, int tokenNumToGenerate)
        {
            string[] lines = InputText.Split("\n");
            List<string> outputLines = new List<string>();

            foreach (var line in lines)
            {
                string outputText = Seq2SeqInstance.Call(line.ToLower(), tokenNumToGenerate);

                outputLines.Add(outputText);

                Logger.WriteLine($"'{line}' --> '{outputText}'");
            }

            return String.Join("<br />", outputLines);
        }

    }
}
