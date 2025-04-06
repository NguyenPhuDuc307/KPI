// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Xử lý cảnh báo DataTables
(function () {
    // Ghi đè hàm alert của DataTables để ngăn chặn hiển thị popup cảnh báo
    if (typeof $.fn.dataTable !== 'undefined') {
        // Backup hàm alert gốc
        var oldAlert = $.fn.dataTable.ext.errMode;

        // Ghi đè để xử lý lỗi tốt hơn
        $.fn.dataTable.ext.errMode = function (settings, techNote, message) {
            var tableId = $(settings.nTable).attr('id');
            console.warn('DataTables warning: ' + tableId + ' - ' + message);

            // Kiểm tra nếu là lỗi về số lượng cột
            if (message.indexOf('incorrect column count') > -1) {
                var $table = $('#' + tableId);
                // Thay thế bảng bằng thông báo lỗi thân thiện
                if ($table.length > 0) {
                    var $parent = $table.parent();
                    $parent.html('<div class="alert alert-info mt-3"><i class="bi bi-info-circle me-2"></i>Không có dữ liệu hoặc cấu trúc bảng không chính xác. Vui lòng kiểm tra lại.</div>');
                }
                return;
            }

            // Đối với các lỗi khác, có thể sử dụng hành vi mặc định
            if (oldAlert === 'alert') {
                console.error('DataTables error: ' + message);
            } else if (typeof oldAlert === 'function') {
                oldAlert(settings, techNote, message);
            }
        };
    }
})();
