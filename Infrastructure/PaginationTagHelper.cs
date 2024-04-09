using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using LegoMastersPlus.Models.ViewModels;

namespace LegoMastersPlus.Infrastructure
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("div", Attributes="page-model")]
    public class PaginationTagHelper(IUrlHelperFactory tempUrlHelperFactory) : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory = tempUrlHelperFactory;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }

        public string? PageController {  get; set; }
        public string? PageAction { get; set; }

        public PaginationInfo PageModel { get; set; }

        public bool PageClassesEnabled { get; set; } = false;
        public string PageClass { get; set; } = String.Empty;
        public string PageClassNormal { get; set; } = String.Empty;
        public string PageClassSelected { get; set; } = String.Empty;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext != null && PageAction != null)
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);

                TagBuilder result = new TagBuilder("div");

                // Don't add pagination if there's only one page
                if (PageModel.TotalPages > 1)
                {
                    // Build out a link for each page
                    for (int i = 1; i <= PageModel.TotalPages; i++)
                    {
                        TagBuilder aTag = new TagBuilder("a");

                        aTag.Attributes["href"] = urlHelper.ActionLink(PageAction, PageController, new { pageNum = i });

                        // Add class styling if applicable
                        if (PageClassesEnabled)
                        {
                            aTag.AddCssClass(PageClass);
                            aTag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                        }

                        aTag.InnerHtml.Append(i.ToString());

                        result.InnerHtml.AppendHtml(aTag);
                    }
                }
                // Put the resulting built HTML tag into the containing div that has HTML attributes
                output.Content.AppendHtml(result.InnerHtml);
            }
        }
    }
}
