#region Using

using System;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections;

#endregion

namespace SmartAdmin.Helpers
{
    public static class HtmlHelperExtensions
    {
        private static string _displayVersion;
        private static Assembly _executingAssembly;
        private static Settings _settings;

        public static Settings Settings => _settings;

        public static void EnsureSettings(this IHtmlHelper helper)
        {
            _settings = helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IOptions<Settings>>().Value;
        }

        /// <summary>
        ///     Retrieves a non-HTML encoded string containing the assembly version as a formatted string.
        ///     <para>If a project name is specified in the application configuration settings it will be prefixed to this value.</para>
        ///     <para>
        ///         e.g.
        ///         <code>1.0 (build 100)</code>
        ///     </para>
        ///     <para>
        ///         e.g.
        ///         <code>ProjectName 1.0 (build 100)</code>
        ///     </para>
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static IHtmlContent AssemblyVersion(this IHtmlHelper helper)
        {
            if (_displayVersion.IsEmpty())
                SetDisplayVersion();

            return helper.Raw(_displayVersion);
        }


        public static string ElapsedTime(this IHtmlHelper helper, DateTime dtEvent)
        {
            TimeSpan TS = DateTime.Now - dtEvent;
            int intYears = DateTime.Now.Year - dtEvent.Year;
            int intMonths = DateTime.Now.Month - dtEvent.Month;
            int intDays = DateTime.Now.Day - dtEvent.Day;
            int intHours = DateTime.Now.Hour - dtEvent.Hour;
            int intMinutes = DateTime.Now.Minute - dtEvent.Minute;
            int intSeconds = DateTime.Now.Second - dtEvent.Second;
            if (intYears > 0) return String.Format("{0} {1} ago", intYears, (intYears == 1) ? "year" : "years");
            else if (intMonths > 0) return String.Format("{0} {1} ago", intMonths, (intMonths == 1) ? "month" : "months");
            else if (intDays > 0) return String.Format("{0} {1} ago", intDays, (intDays == 1) ? "day" : "days");
            else if (intHours > 0) return String.Format("{0} {1} ago", intHours, (intHours == 1) ? "hour" : "hours");
            else if (intMinutes > 0) return String.Format("{0} {1} ago", intMinutes, (intMinutes == 1) ? "minute" : "minutes");
            else if (intSeconds > 0) return String.Format("{0} {1} ago", intSeconds, (intSeconds == 1) ? "second" : "seconds");
            else
            {
                return String.Format("{0} {1} ago", dtEvent.ToString("d"), dtEvent.ToString("d"));
            }
        }

        /// <summary>
        ///     Compares the requested route with the given <paramref name="value" /> value, if a match is found the
        ///     <paramref name="attribute" /> value is returned.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="value">The action value to compare to the requested route action.</param>
        /// <param name="attribute">The attribute value to return in the current action matches the given action value.</param>
        /// <returns>A HtmlString containing the given attribute value; otherwise an empty string.</returns>
        public static IHtmlContent RouteIf(this IHtmlHelper helper, string value, string attribute)
        {
            var currentController = (helper.ViewContext.RouteData.Values["controller"] ?? string.Empty).ToString().UnDash();
            var currentAction = (helper.ViewContext.RouteData.Values["action"] ?? string.Empty).ToString().UnDash();

            var hasController = value.Equals(currentController, StringComparison.OrdinalIgnoreCase);
            var hasAction = value.Equals(currentAction, StringComparison.OrdinalIgnoreCase);
            

            return hasAction || hasController ? new HtmlString(attribute) : new HtmlString(string.Empty);
        }

        public static IHtmlContent RouteIf(this IHtmlHelper helper, string value, string controller, string attribute )
        {
            var currentController = (helper.ViewContext.RouteData.Values["controller"] ?? string.Empty).ToString().UnDash();
            var currentAction = (helper.ViewContext.RouteData.Values["action"] ?? string.Empty).ToString().UnDash();
            var splitValue = value.Split(';');
            var splicController = controller.Split(';');

            var hasController = splicController.Contains(currentController);//controller.Equals(currentController, StringComparison.OrdinalIgnoreCase); //
            var hasAction = splitValue.Contains(currentAction);

            if (string.IsNullOrEmpty(value))
            {
                hasAction = true;
            }

            return hasAction && hasController ? new HtmlString(attribute) : new HtmlString(string.Empty);
        }


        /// <summary>
        ///     Renders the specified partial view with the parent's view data and model if the given setting entry is found and
        ///     represents the equivalent of true.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="partialViewName">The name of the partial view.</param>
        /// <param name="condition">The boolean value that determines if the partial view should be rendered.</param>
        public static void RenderPartialIf(this IHtmlHelper htmlHelper, string partialViewName, bool condition)
        {
            if (!condition)
                return;

            htmlHelper.RenderPartialAsync(partialViewName).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Retrieves a non-HTML encoded string containing the assembly version and the application copyright as a formatted
        ///     string.
        ///     <para>If a company name is specified in the application configuration settings it will be suffixed to this value.</para>
        ///     <para>
        ///         e.g.
        ///         <code>1.0 (build 100) © 2015</code>
        ///     </para>
        ///     <para>
        ///         e.g.
        ///         <code>1.0 (build 100) © 2015 CompanyName</code>
        ///     </para>
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static IHtmlContent Copyright(this IHtmlHelper helper)
        {
            helper.EnsureSettings();

            var copyright = $"{helper.AssemblyVersion()} &copy; {DateTime.Now.Year} {Settings.Company}".Trim();

            return helper.Raw(copyright);
        }

        private static void SetDisplayVersion()
        {
            _executingAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var version = _executingAssembly.GetName().Version;

            _displayVersion =
                string.Format("{4} {0}.{1}.{2} (build {3})", version.Major, version.Minor, version.Build,
                    version.Revision, Settings.Project).Trim();
        }

        /// <summary>
        ///     Returns an unordered list (ul element) of validation messages that utilizes bootstrap markup and styling.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="alertType">The alert type styling rule to apply to the summary element.</param>
        /// <param name="heading">The optional value for the heading of the summary element.</param>
        /// <returns></returns>
        public static IHtmlContent ValidationBootstrap(this IHtmlHelper htmlHelper, string alertType = "danger",
            string heading = "")
        {
            if (htmlHelper.ViewData.ModelState.IsValid)
                return new HtmlString(string.Empty);

            var div = new TagBuilder("div");
            div.AddCssClass($"alert alert-{alertType} alert-block");

            var button = new TagBuilder("button");
            button.AddCssClass("close");
            button.Attributes.Add("data-dismiss", "alert");
            button.Attributes.Add("aria-hidden", "true");
            button.InnerHtml.AppendHtml("&times;");

            div.InnerHtml.AppendHtml(button);

            if (!heading.IsEmpty())
            {
                var h4 = new TagBuilder("h4");
                h4.AddCssClass("alert-heading");
                h4.InnerHtml.Append($"<h4 class=\"alert-heading\">{heading}</h4>");

                div.InnerHtml.AppendHtml(h4);
            }

            var summary = htmlHelper.ValidationSummary();

            div.InnerHtml.AppendHtml(summary);

            return div;
        }
    }
}