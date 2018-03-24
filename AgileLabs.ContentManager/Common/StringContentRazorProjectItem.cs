using RazorLight.Razor;
using System;
using System.IO;

namespace AgileLabs.ContentManager.Common
{
    //public class StringContentRazorProjectItem : RazorLightProjectItem
    //{
    //    public StringContentRazorProjectItem(string key, Func<string, Stream> getContentFunc,
    //        Func<string, bool> existsCheckFunc, Func<string, string> getParentLayoutKeyFunc)
    //    {
    //        Key = key;
    //        this.GetContentFunc = getContentFunc;
    //        this.ExistsCheckFunc = existsCheckFunc;
    //        this.GetParentLayoutKeyFunc = getParentLayoutKeyFunc;
    //    }

    //    public override string Key { get; set; }
    //    public Func<string, Stream> GetContentFunc { get; }
    //    public Func<string, bool> ExistsCheckFunc { get; }
    //    public Func<string, string> GetParentLayoutKeyFunc { get; }

    //    public override bool Exists
    //    {
    //        get
    //        {
    //            return ExistsCheckFunc(Key);
    //        }
    //    }

    //    public override Stream Read()
    //    {
    //        return GetContentFunc(Key);
    //    }

    //    public override string TemplateKey => GetParentLayoutKeyFunc(Key);

    //    public override bool TemplateExists => !string.IsNullOrWhiteSpace(TemplateKey);
    //}
}
