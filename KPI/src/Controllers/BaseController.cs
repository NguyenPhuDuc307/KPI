using Microsoft.AspNetCore.Mvc;

namespace KPISolution.Controllers
{
    /// <summary>
    /// Base controller that provides common functionality for all controllers
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Sets up the page template with common view data
        /// </summary>
        /// <param name="title">Page title</param>
        /// <param name="subtitle">Page subtitle (optional)</param>
        /// <param name="icon">Bootstrap icon class (optional)</param>
        /// <param name="showButtons">Whether to show action buttons (default: true)</param>
        /// <returns>The current controller instance for method chaining</returns>
        protected void SetupPageTemplate(
            string title,
            string subtitle = "",
            string icon = "bi-circle",
            bool showButtons = true)
        {
            ViewData["Title"] = title;
            ViewData["Subtitle"] = subtitle;
            ViewData["Icon"] = icon;
            ViewData["ShowButtons"] = showButtons;
        }

        /// <summary>
        /// Sets the primary action button for the page template
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="controller">Target controller</param>
        /// <param name="action">Target action</param>
        /// <param name="id">Route id (optional)</param>
        /// <param name="icon">Button icon (optional)</param>
        protected void SetPrimaryButton(
            string text,
            string controller,
            string action,
            string id = "",
            string icon = "bi-plus-lg")
        {
            ViewData["PrimaryButton"] = (text, controller, action, id, icon);
        }

        /// <summary>
        /// Sets the secondary action button for the page template
        /// </summary>
        /// <param name="text">Button text</param>
        /// <param name="controller">Target controller</param>
        /// <param name="action">Target action</param>
        /// <param name="id">Route id (optional)</param>
        /// <param name="icon">Button icon (optional)</param>
        protected void SetSecondaryButton(
            string text,
            string controller,
            string action,
            string id = "",
            string icon = "bi-arrow-left")
        {
            ViewData["SecondaryButton"] = (text, controller, action, id, icon);
        }

        /// <summary>
        /// Enables or disables the filter panel in the page template
        /// </summary>
        /// <param name="enabled">Whether the filter panel is enabled</param>
        protected void SetFilterPanel(bool enabled = true)
        {
            ViewData["ShowFilterPanel"] = enabled;
        }

        /// <summary>
        /// Sets up breadcrumb items for the page
        /// </summary>
        /// <param name="items">List of breadcrumb items (Text, Controller, Action, Id)</param>
        protected void SetBreadcrumb(List<(string Text, string Controller, string Action, string Id)> items)
        {
            ViewData["BreadcrumbItems"] = items;
        }

        /// <summary>
        /// Adds a success alert message to be displayed on the next page load
        /// </summary>
        /// <param name="message">The success message</param>
        protected void AddSuccessAlert(string message)
        {
            TempData["SuccessAlert"] = message;
        }

        /// <summary>
        /// Adds an error alert message to be displayed on the next page load
        /// </summary>
        /// <param name="message">The error message</param>
        protected void AddErrorAlert(string message)
        {
            TempData["ErrorAlert"] = message;
        }

        /// <summary>
        /// Adds a warning alert message to be displayed on the next page load
        /// </summary>
        /// <param name="message">The warning message</param>
        protected void AddWarningAlert(string message)
        {
            TempData["WarningAlert"] = message;
        }

        /// <summary>
        /// Adds an info alert message to be displayed on the next page load
        /// </summary>
        /// <param name="message">The info message</param>
        protected void AddInfoAlert(string message)
        {
            TempData["InfoAlert"] = message;
        }
    }
}