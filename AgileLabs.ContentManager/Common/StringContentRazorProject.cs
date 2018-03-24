using AgileLabs.ContentManager.Entities;
using AgileLabs.ContentManager.Repositories;
using RazorLight.Razor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.Common
{
    //public class StringContentRazorProject : RazorLightProject
    //{
    //    public StringContentRazorProject(Func<string, Stream> getContentFunc, 
    //        Func<string, bool> existsCheckFunc,
    //        Func<string, string> getParentLayoutKeyFunc)
    //    {
    //        this.GetContentFunc = getContentFunc;
    //        this.ExistsCheckFunc = existsCheckFunc;
    //        this.GetParentLayoutKeyFunc = getParentLayoutKeyFunc;
    //    }

    //    public Func<string, Stream> GetContentFunc { get; }
    //    public Func<string, bool> ExistsCheckFunc { get; }
    //    public Func<string, string> GetParentLayoutKeyFunc { get; }

    //    public override Task<RazorLightProjectItem> GetItemAsync(string templateKey)
    //    {
    //        var item = new StringContentRazorProjectItem(templateKey, getContentFunc: GetContentFunc,
    //            existsCheckFunc: ExistsCheckFunc,
    //          getParentLayoutKeyFunc: GetParentLayoutKeyFunc);

    //        return Task.FromResult((RazorLightProjectItem)item);
    //    }

    //    public override Task<IEnumerable<RazorLightProjectItem>> GetImportsAsync(string templateKey)
    //    {
    //        return Task.FromResult(Enumerable.Empty<RazorLightProjectItem>());
    //    }

    //    public override async Task<string> GetItemParentLayoutKeyAsync(string templateKey)
    //    {
    //        var item = await GetItemAsync(templateKey);
    //        return item.TemplateKey ?? string.Empty;
    //    }
    //}
}
