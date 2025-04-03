using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KPISolution.Models.ViewModels.KPI
{
    /// <summary>
    /// ViewModel cho trang lịch sử đo lường
    /// </summary>
    public class MeasurementHistoryViewModel
    {
        /// <summary>
        /// Danh sách các đo lường
        /// </summary>
        public List<KpiValueViewModel> Measurements { get; set; } = new List<KpiValueViewModel>();

        /// <summary>
        /// Danh sách KPI cho dropdown
        /// </summary>
        public required SelectList KpiList { get; set; }

        /// <summary>
        /// ID của KPI được chọn để lọc
        /// </summary>
        public Guid? SelectedKpiId { get; set; }

        /// <summary>
        /// Ngày bắt đầu lọc
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc lọc
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Kích thước trang
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Kiểm tra có trang trước không
        /// </summary>
        public bool HasPreviousPage => CurrentPage > 1;

        /// <summary>
        /// Kiểm tra có trang sau không
        /// </summary>
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}