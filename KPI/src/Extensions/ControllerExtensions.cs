namespace KPISolution.Extensions
{
    /// <summary>
    /// Extension methods for controllers
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Activates the page template for the current view
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="title">Page title</param>
        /// <param name="subtitle">Page subtitle (optional)</param>
        /// <param name="icon">Bootstrap icon class (optional)</param>
        /// <returns>The controller instance for method chaining</returns>
        public static Controller WithPageTemplate(this Controller controller, string title, string subtitle = "", string icon = "bi-circle")
        {
            controller.ViewData["Title"] = title;
            controller.ViewData["Subtitle"] = subtitle;
            controller.ViewData["Icon"] = icon;
            return controller;
        }

        /// <summary>
        /// Sets the primary action button for the page template
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="text">Button text</param>
        /// <param name="controllerName">Target controller</param>
        /// <param name="action">Target action</param>
        /// <param name="id">Route id (optional)</param>
        /// <param name="icon">Button icon (optional)</param>
        /// <returns>The controller instance for method chaining</returns>
        public static Controller WithPrimaryButton(this Controller controller, string text, string controllerName, string action, string id = "", string icon = "bi-plus-lg")
        {
            controller.ViewData["PrimaryButton"] = (text, controllerName, action, id, icon);
            return controller;
        }

        /// <summary>
        /// Sets the secondary action button for the page template
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="text">Button text</param>
        /// <param name="controllerName">Target controller</param>
        /// <param name="action">Target action</param>
        /// <param name="id">Route id (optional)</param>
        /// <param name="icon">Button icon (optional)</param>
        /// <returns>The controller instance for method chaining</returns>
        public static Controller WithSecondaryButton(this Controller controller, string text, string controllerName, string action, string id = "", string icon = "bi-arrow-left")
        {
            controller.ViewData["SecondaryButton"] = (text, controllerName, action, id, icon);
            return controller;
        }

        /// <summary>
        /// Enables or disables the filter panel in the page template
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="enabled">Whether the filter panel is enabled</param>
        /// <returns>The controller instance for method chaining</returns>
        public static Controller WithFilterPanel(this Controller controller, bool enabled = true)
        {
            controller.ViewData["ShowFilterPanel"] = enabled;
            return controller;
        }

        /// <summary>
        /// Sets up breadcrumb items for the page
        /// </summary>
        /// <param name="controller">The controller instance</param>
        /// <param name="items">List of breadcrumb items (Text, Controller, Action, Id)</param>
        /// <returns>The controller instance for method chaining</returns>
        public static Controller WithBreadcrumb(this Controller controller, List<(string Text, string Controller, string Action, string Id)> items)
        {
            controller.ViewData["BreadcrumbItems"] = items;
            return controller;
        }
    }
}