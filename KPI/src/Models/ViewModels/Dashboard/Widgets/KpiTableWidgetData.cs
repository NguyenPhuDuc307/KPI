using System;
using System.Collections.Generic;

namespace KPISolution.Models.ViewModels.Dashboard.Widgets
{
    /// <summary>
    /// Dữ liệu cho widget hiển thị danh sách KPI dạng bảng
    /// </summary>
    public class KpiTableWidgetData : WidgetData
    {
        /// <summary>
        /// Danh sách các KPI hiển thị trong bảng
        /// </summary>
        public List<KpiTableItem> KpiItems { get; set; } = new List<KpiTableItem>();

        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// Số lượng KPI trên mỗi trang
        /// </summary>
        public int PageSize { get; set; } = 5;

        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages { get; set; } = 1;

        /// <summary>
        /// Tổng số KPI
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Có hiển thị phân trang không
        /// </summary>
        public bool ShowPagination { get; set; } = true;

        /// <summary>
        /// Có hiển thị cột xu hướng không
        /// </summary>
        public bool ShowTrend { get; set; } = true;
    }

    /// <summary>
    /// Thông tin của một KPI trong bảng
    /// </summary>
    public class KpiTableItem
    {
        /// <summary>
        /// ID của KPI
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Mã của KPI
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Tên của KPI
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị hiện tại của KPI
        /// </summary>
        public decimal CurrentValue { get; set; }

        /// <summary>
        /// Giá trị mục tiêu của KPI
        /// </summary>
        public decimal TargetValue { get; set; }

        /// <summary>
        /// Đơn vị đo lường
        /// </summary>
        public string MeasurementUnit { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái hiện tại của KPI
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Hướng xu hướng (up, down, neutral)
        /// </summary>
        public string TrendDirection { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị xu hướng
        /// </summary>
        public string TrendValue { get; set; } = string.Empty;
    }
}