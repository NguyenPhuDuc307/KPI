namespace KPISolution.Authorization.Requirements
{
    /// <summary>
    /// Yêu cầu ủy quyền dựa trên quyền sở hữu tài nguyên
    /// </summary>
    public class OwnershipRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Hành động cần ủy quyền
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Khởi tạo yêu cầu ủy quyền
        /// </summary>
        /// <param name="action">Hành động cần ủy quyền (Create, Read, Update, Delete)</param>
        public OwnershipRequirement(string action)
        {
            this.Action = action;
        }
    }
}
