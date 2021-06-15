using Castle.Core.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pannotation.Controllers.API;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.ResponseModels;
using Pannotation.ResourceLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pannotation.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : _BaseApiController
    {
        private IStringLocalizer<ErrorsResource> _localizer;
        private UserManager<ApplicationUser> _userManager;
        private IConfiguration _configuration;

        public HomeController(UserManager<ApplicationUser> userManager, IStringLocalizer<ErrorsResource> localizer, IConfiguration configuration)
             : base(localizer)
        {
            _userManager = userManager;
            _localizer = localizer;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            return Redirect("swagger");
        }

        [HttpGet("confirm-email")]
        public IActionResult ConfirmEmail(string email, string token)
        {
            return Redirect($"{_configuration["Frontend:HostName"]}/confirm-email?email={HttpUtility.UrlEncode(email)}&token={HttpUtility.UrlEncode(token)}");
        }

        [HttpGet("ShowLogsDirectory")]
        public IActionResult ShowLogsDirectory(string dir)
        {
            var path = "Logs/" + dir;
            Dictionary<string, string> directories = new Dictionary<string, string>();
            Dictionary<string, string> files = new Dictionary<string, string>();

            if (Directory.Exists(path))
            {
                directories = Directory.GetDirectories(path).Select(t => new KeyValuePair<string, string>(t.Substring(5), "")).ToDictionary(k => k.Key, v => v.Value);
                files = Directory.GetFiles(path).Select(t => new KeyValuePair<string, string>(t.Substring(5), ConvertFileSize(t))).ToDictionary(k => k.Key, v => v.Value);
            }

            if (!dir.IsNullOrEmpty())
            {
                while (path.LastIndexOf("/") == path.Length - 1 || path.LastIndexOf("\\") == path.Length - 1)
                {
                    path = path.Substring(0, path.Length - 1);
                }

                var directoryName = Path.GetFileName(path);
                var prev = path.Substring(0, path.Length - directoryName.Length).Substring(5);
                ViewBag.Prev = prev;
            }
            else
                ViewBag.Prev = null;

            return View(new List<Dictionary<string, string>> { directories, files });
        }

        [HttpGet("ShowLog")]
        public IActionResult ShowLog(string path)
        {
            var text = System.IO.File.ReadAllText(Path.Combine("Logs/", path));

            text = string.Join("\r\n", text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Reverse());

            return Content(text, "application/json");
        }

        [HttpGet("ClearLogs")]
        public IActionResult ClearLogs()
        {
            DirectoryInfo di = new DirectoryInfo("Logs");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            return Content("OK");
        }

        // Handle api errors 
        [Route("ApiError/{id}")]
        public IActionResult ApiError(int? id = null)
        {
            if (!id.HasValue)
                id = Response.StatusCode;

            return StatusCodeHanler(id.Value);
        }

        // Handle ui page errors 
        [Route("Error")]
        public IActionResult Error()
        {
            int statusCode = Response.StatusCode;

            // TODO: change staus code handler logic for ui pages
            return StatusCodeHanler(statusCode);
        }

        // Use this method to return json with error data
        private IActionResult StatusCodeHanler(int statusCode)
        {
            string message = "";
            string stackTrace = "";

            List<ErrorKeyValue> errors = new List<ErrorKeyValue>();

            switch (statusCode)
            {
                case 400:
                    message = "Bad Request";
                    break;
                case 401:
                    message = "Unauthorized";
                    errors.Add(new ErrorKeyValue("token", "Token invalid"));
                    break;
                case 404:
                    message = "Not found";
                    break;
                case 403:
                    message = "Forbidden";
                    break;
                case 500:
                    message = "Internal Server Error";
                    break;
            }

            return new ContentResult()
            {
                Content = JsonConvert.SerializeObject(new ErrorResponseModel(_localizer)
                {
                    Code = message,
                    StackTrace = stackTrace,
                    Errors = errors
                }, new JsonSerializerSettings { Formatting = Formatting.Indented }),
                StatusCode = statusCode,
                ContentType = "application/json"
            };
        }

        private string ConvertFileSize(string filename)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = new FileInfo(filename).Length;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}