using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgileLabs.ContentManager.UEditor
{
    /// <summary>
    /// Config 的摘要说明
    /// </summary>
    public class ConfigHandler : Handler
    {
        private IHostingEnvironment _env;

        public ConfigHandler(IHostingEnvironment env)
        {
            this._env = env;
        }
        public async override Task ExecuteAsync()
        {
            //var configFilePath = Path.Combine("config.json");
            //var fileInfo = _env.WebRootFileProvider.GetFileInfo(configFilePath);
            //if (fileInfo.Exists)
            //{
            //    WriteJson(File.ReadAllText(fileInfo.PhysicalPath));
            //}
            //WriteJson(JsonConvert.SerializeObject(config));



            //Response.ContentType = "application/javascript";
            //Output.Write(JsonConvert.SerializeObject(_env));
            //Write((Items);
            //Output.Write(JsonConvert.SerializeObject(Items));
            WriteJson(Items);
            await Task.FromResult(0);
        }

        private JObject BuildItems()
        {
            var configFilePath = Path.Combine("config.json");
            var fileInfo = _env.WebRootFileProvider.GetFileInfo(configFilePath);
            var json = File.ReadAllText(fileInfo.PhysicalPath);
            return JObject.Parse(json);
        }

        private bool noCache = true;
        public JObject Items
        {
            get
            {
                if (noCache || _Items == null)
                {
                    _Items = BuildItems();
                }
                return _Items;
            }
        }
        private JObject _Items;
    }
}