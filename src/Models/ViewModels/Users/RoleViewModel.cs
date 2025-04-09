using System.ComponentModel.DataAnnotations;

namespace KPISolution.Models.ViewModels.Users
{
    /// <summary>
    /// View model for creating and editing roles.
    /// </summary>
    public class RoleViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role Name is required")]
        [Display(Name = "Role Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true; // Default to active
    }
}