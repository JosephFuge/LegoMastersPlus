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
            if (ViewContext == null || PageAction == null) return;

            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            if (PageModel.TotalPages > 1)
            {
                // First page
                AppendPageLink(result, urlHelper, 1);
                
                // Determine if ellipses are needed based on the total number of pages
                bool useEllipses = PageModel.TotalPages >= 6;

                // Ellipsis after first page if needed
                if (useEllipses && PageModel.CurrentPage > 3)
                {
                    AppendEllipsis(result);
                }

                // Middle pages - current page - 1 to current page + 1 (or as many as available)
                int startPage = Math.Max(2, PageModel.CurrentPage - 1);
                int endPage = Math.Min(PageModel.TotalPages - 1, PageModel.CurrentPage + 1);

                if (!useEllipses)
                {
                    startPage = 2;
                    endPage = PageModel.TotalPages - 1;
                }
                for (int i = startPage; i <= endPage; i++)
                {
                    AppendPageLink(result, urlHelper, i);
                }

                // Ellipsis before last page if needed
                if (useEllipses && PageModel.CurrentPage < PageModel.TotalPages - 2)
                {
                    AppendEllipsis(result);
                }

                // Last page
                AppendPageLink(result, urlHelper, PageModel.TotalPages);
            }

            output.Content.AppendHtml(result.InnerHtml);
        }

        private void AppendPageLink(TagBuilder container, IUrlHelper urlHelper, int pageNumber)
        {
            TagBuilder aTag = new TagBuilder("a");
            aTag.Attributes["href"] = urlHelper.Action(PageAction, PageController, new { pageNum = pageNumber });
            if (PageClassesEnabled)
            {
                aTag.AddCssClass(PageClass);
                aTag.AddCssClass(pageNumber == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
            }
            aTag.InnerHtml.Append(pageNumber.ToString());
            container.InnerHtml.AppendHtml(aTag);
        }

        private static void AppendEllipsis(TagBuilder container)
        {
            TagBuilder spanTag = new TagBuilder("span");
            spanTag.AddCssClass("pagination-ellipsis");
            spanTag.InnerHtml.Append("...");
            container.InnerHtml.AppendHtml(spanTag);
        }

    }
}
