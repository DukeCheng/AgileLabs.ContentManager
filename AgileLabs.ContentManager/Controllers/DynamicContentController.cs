using AgileLabs.ContentManager.Common;
using AgileLabs.ContentManager.Entities;
using AgileLabs.ContentManager.Repositories;
using AgileLabs.ContentManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AgileLabs.ContentManager.Controllers
{

    [AllowAnonymous]
    public class DynamicContentController : Controller
    {
        private IMemoryCache _memoryCache;
        private MongoDbBaseRepository<UrlRecord> _urlRecordRepository;
        private MongoDbBaseRepository<Page> _pageRepository;
        private MongoDbBaseRepository<Template> _templateRepository;
        private MongoDbBaseRepository<Settings> _settingsRepository;
        private UrlRecordServcie _urlRecordServcie;

        public DynamicContentController(IMemoryCache memoryCache,
            MongoDbBaseRepository<UrlRecord> urlRecordRepository,
            MongoDbBaseRepository<Page> pageRepository,
            MongoDbBaseRepository<Template> templateRepository,
            MongoDbBaseRepository<Settings> settingsRepository,
            UrlRecordServcie urlRecordServcie)
        {
            _memoryCache = memoryCache;
            _urlRecordRepository = urlRecordRepository;
            _pageRepository = pageRepository;
            _templateRepository = templateRepository;
            _settingsRepository = settingsRepository;
            _urlRecordServcie = urlRecordServcie;
        }

        // GET: /<controller>/
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> Preview(Guid urlRecordId, string slug)
        {
            //UrlRecord urlRecord = GetUrlRecord(slug);
            UrlRecord urlRecord = await _urlRecordServcie.GetUrlRecord(urlRecordId);
            if (urlRecord == null)
                return NotFound();

            switch (urlRecord.Type)
            {
                case UrlRecordType.Page:
                    return await DynamicRazorPage(Guid.Parse(urlRecord.RefValue));
                case UrlRecordType.Redirect:
                    return Redirect(FormatUrl(urlRecord.RefValue));
                case UrlRecordType.RedirectPermanent:
                    return RedirectPermanent(FormatUrl(urlRecord.RefValue));
                default:
                    return NotFound();
            }
        }

        private static string NormalizeSlug(string slug)
        {
            if (slug != null)
            {
                slug = slug.ToLower();
            }
            else if (string.IsNullOrWhiteSpace(slug))
            {
                slug = null;
            }

            return slug;
        }

        private string FormatUrl(string refValue)
        {
            return refValue;
        }

        public async Task<IActionResult> DynamicRazorPage(Guid? pageId)
        {
            if (!pageId.HasValue)
                return NotFound();

            var page = _pageRepository.GetByIdAsync(pageId.Value).Result;
            if (page == null || !page.TemplateId.HasValue)
                throw new Exception("Page Not exists");

            var engine = new RazorLightEngineBuilder()
              .UseMemoryCachingProvider()
              .Build();

            string template = "Hello, @Model.Name. Welcome to RazorLight repository";
            var model = new { Name = "John Doe" };

            string content = await engine.CompileRenderAsync("templateKey", template, model);


            //var engine = new EngineFactory().Create(new StringContentRazorProject(
            //    getContentFunc: key =>
            //    {
            //        var template = _templateRepository.GetByIdAsync(Guid.Parse(key)).Result;
            //        if (template == null)
            //            throw new Exception("Template is null");

            //        MemoryStream stream = new MemoryStream();
            //        StreamWriter writer = new StreamWriter(stream);
            //        writer.Write(template.BodyContent);
            //        writer.Flush();
            //        stream.Seek(0, SeekOrigin.Begin);
            //        return stream;
            //    },
            //    existsCheckFunc: key =>
            //    {
            //        var template = _templateRepository.GetByIdAsync(Guid.Parse(key)).Result;
            //        return template != null;
            //    },
            //    getParentLayoutKeyFunc: key =>
            //    {
            //        var template = _templateRepository.GetByIdAsync(Guid.Parse(key)).Result;
            //        if (template == null)
            //            return string.Empty;
            //        return template.ParentTemplateId.HasValue ? template.ParentTemplateId.Value.ToString() : string.Empty;
            //    }
            //    ));

            //var templateKey = page.TemplateId.Value.ToString();
            //var templte = await engine.GetTemplateAsync(templateKey);
            //if (page.TemplateId.HasValue)
            //{
            //    var pageTemplate = _templateRepository.GetByIdAsync(page.TemplateId.Value).Result;
            //    if (pageTemplate == null)
            //        throw new Exception("Template is null");

            //    if (pageTemplate.ParentTemplateId.HasValue)
            //        templte.Layout = pageTemplate.ParentTemplateId.Value.ToString();
            //}

            //var viewbag = CreateViewBag();
            //viewbag.Page = page;

            //var model = new
            //{
            //    Name = "DukeCheng",
            //    Page = page
            //};

            //templte.PageContext = new PageContext(viewbag)
            //{
            //    ExecutingPageKey = templateKey,
            //    ModelTypeInfo = new ModelTypeInfo(model.GetType())
            //};
            ////var content = await engine.CompileRenderAsync(templateKey, model, model.GetType(), null);
            //var content = await engine.RenderTemplateAsync(templte, model);
            return Content(content, "text/html", Encoding.UTF8);
        }

        private dynamic CreateViewBag()
        {
            dynamic viewbag = new ExpandoObject();
            var dictViewbag = viewbag as IDictionary<String, object>;
            var settings = _settingsRepository.GetAll();
            foreach (var item in settings)
            {
                dictViewbag.TryAdd(item.Key, item.Value);
            }
            return viewbag;
        }

        //public async Task<IActionResult> DynamicPage(Guid? pageId)
        //{
        //    if (!pageId.HasValue)
        //        return NotFound();

        //    StringBuilder sbHtml = new StringBuilder();
        //    sbHtml.Append("<!DOCTYPE html>");//append doctype
        //    TagBuilder html = new TagBuilder("html");

        //    var page = _pageRepository.GetByIdAsync(pageId.Value).Result;
        //    if (page == null)
        //        return NotFound();

        //    string templateHead = null, templateBody = null;
        //    if (page.TemplateId.HasValue)
        //    {
        //        RecursionBuildTempalte(page.TemplateId.Value, ref templateHead, ref templateBody);
        //    }

        //    BuildHtmlHead(html, page, templateHead);

        //    BuildHtmlBody(html, page, templateBody);

        //    sbHtml.Append(ConvertTagToSring(html));

        //    var compileTemplate = Handlebars.Compile(sbHtml.ToString() ?? string.Empty);
        //    var settings = await _settingsRepository.SearchAsync(Builders<Settings>.Filter.Where(x => x.Category == "GlobalSettings"));

        //    Dictionary<string, string> GlobalContext = new Dictionary<string, string>();
        //    foreach (var item in settings)
        //    {
        //        GlobalContext.Add(item.Key, item.Value);
        //    }

        //    dynamic data = new DynamicDictionary();
        //    data.title = "My new post";
        //    data.body = "this is my first post";
        //    data.user = new DynamicDictionary();
        //    data.user.firstname = "Duke";
        //    data.user.lastname = "cheng";

        //    var globalContext = new DynamicDictionary();
        //    foreach (var item in GlobalContext)
        //    {
        //        globalContext.Set(item.Key, item.Value);
        //    }
        //    data.GLOBALCONTEXT = globalContext;


        //    var pageContext = new DynamicDictionary();
        //    pageContext.Set("Path", page.Url);
        //    pageContext.Set("Title", page.Title);

        //    var pageSeoInfo = new DynamicDictionary();
        //    if (page.SeoInfo == null)
        //    {
        //        page.SeoInfo = new PageSeoInfo();
        //    }
        //    pageSeoInfo.Set(nameof(page.SeoInfo.Keywords), page.SeoInfo.Keywords);
        //    pageSeoInfo.Set(nameof(page.SeoInfo.Description), page.SeoInfo.Description);
        //    data.SEO = pageSeoInfo;

        //    data.PAGECONTEXT = pageContext;

        //    return Content(compileTemplate(data), "text/html", Encoding.UTF8);
        //}

        //const string HEAD_FLAG = "{{HEAD}}";
        //const string BODY_FLAG = "{{BODY}}";
        //public void RecursionBuildTempalte(Guid templateId, ref string headTeamplate, ref string bodyTemplate)
        //{
        //    var template = _templateRepository.GetByIdAsync(templateId).Result;
        //    if (template == null)
        //        return;
        //    if (!template.ParentTemplateId.HasValue)
        //    {
        //        headTeamplate = template.HeadContent ?? string.Empty;
        //        bodyTemplate = template.BodyContent ?? string.Empty + template.FootContent ?? string.Empty;
        //    }
        //    else
        //    {
        //        string parentHeadTemplate = string.Empty;
        //        string parentBodyTemplate = string.Empty;
        //        RecursionBuildTempalte(template.ParentTemplateId.Value, ref parentHeadTemplate, ref parentBodyTemplate);
        //        headTeamplate = parentHeadTemplate.Replace(HEAD_FLAG, template.HeadContent);
        //        bodyTemplate = parentBodyTemplate.Replace(BODY_FLAG, template.BodyContent) + template.FootContent;
        //    }

        //    //RenderWidget(ref headTeamplate);
        //    //RenderWidget(ref bodyTemplate);
        //}

        //private void RenderWidget(ref string content)
        //{
        //    if (string.IsNullOrWhiteSpace(content))
        //        return;
        //    //Regex regex1 = new Regex(@"<widget[^<>]+>");
        //    //Regex regex2 = new Regex(@"\{widget[^\{\}]+\}");
        //    //var widgetsFlag1 = Regex.Matches(content, @"<widget[^<>]+>");
        //    var widgetsFlag2 = Regex.Matches(content, @"\{widget[^\{\}]+\}");
        //    //foreach (Match item in widgetsFlag1)
        //    //{
        //    //    if (item.Success)
        //    //    {

        //    //    }
        //    //}

        //    foreach (Match item in widgetsFlag2)
        //    {
        //        if (item.Success)
        //        {
        //            //1. reslove widget name and 
        //            Regex.Replace(content, @"\{widget[^\{\}]+\}", "widget-replaced result", RegexOptions.IgnoreCase);
        //        }
        //    }
        //}

        //public string ConvertTagToSring(TagBuilder Tag)
        //{
        //    var writer = new StringWriter();
        //    Tag.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
        //    return writer.ToString();
        //}

        //private void BuildHtmlHead(TagBuilder html, Page page, string template = null)
        //{
        //    TagBuilder head = new TagBuilder("head");
        //    html.InnerHtml.AppendHtml(head);
        //    var headerContent = string.IsNullOrWhiteSpace(template) ? page.HeadContent : template.Replace(HEAD_FLAG, page.BodyContent);

        //    RenderWidget(ref headerContent);
        //    //RenderWidget(ref bodyTemplate);
        //    head.InnerHtml.AppendHtml(headerContent);
        //}

        //private void BuildHtmlBody(TagBuilder html, Page page, string template = null)
        //{
        //    TagBuilder body = new TagBuilder("body");
        //    html.InnerHtml.AppendHtml(body);
        //    var bodyContentBuilder = new StringBuilder(string.IsNullOrWhiteSpace(template) ? page.BodyContent : template.Replace(BODY_FLAG, page.BodyContent));
        //    bodyContentBuilder.Append(page.FootContent);
        //    var bodyContent = bodyContentBuilder.ToString();
        //    RenderWidget(ref bodyContent);

        //    body.InnerHtml.AppendHtml(bodyContent);
        //}
    }
}
