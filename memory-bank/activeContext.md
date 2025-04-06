# KPI Management System - Active Context

## Current Focus

Dựa trên yêu cầu mới về cấu trúc phân cấp đầy đủ và thay đổi quan trọng về việc gộp các entity, chúng ta cần phải điều chỉnh tập trung phát triển. Hiện tại, trọng tâm phát triển là:

1. **Cập nhật mô hình dữ liệu** để bao gồm cấu trúc phân cấp đầy đủ với 3 entity chính:

   - Gộp SF và CSF vào một entity SuccessFactor với cờ IsCritical
   - Gộp RI và KRI vào một entity ResultIndicator với cờ IsKey
   - Gộp PI và KPI vào một entity PerformanceIndicator với cờ IsKey
   - Định nghĩa lại các mối quan hệ giữa các thực thể
   - Cập nhật migration để áp dụng thay đổi vào cơ sở dữ liệu

2. **Phát triển các Service Layers cho cấu trúc mới**:

   - Thiết kế và triển khai ObjectiveService
   - Thiết kế và triển khai SuccessFactorService (hỗ trợ cả SF và CSF)
   - Triển khai ResultIndicatorService (hỗ trợ cả RI và KRI)
   - Triển khai PerformanceIndicatorService (hỗ trợ cả PI và KPI)

3. **Phát triển giao diện người dùng**:
   - Thiết kế và xây dựng giao diện quản lý Objectives
   - Xây dựng giao diện quản lý SF với tab riêng cho SF và CSF
   - Xây dựng giao diện quản lý RI với tab riêng cho RI và KRI
   - Xây dựng giao diện quản lý PI với tab riêng cho PI và KPI
   - **Chuẩn hóa tất cả các views theo tiếng Anh** và đảm bảo tính nhất quán trong UX
   - **Hoàn thiện chức năng quản lý giá trị đo lường** và đảm bảo hoạt động đúng trên tất cả các loại chỉ số

## Recent Changes

### Infrastructure

- Set up the base project structure with ASP.NET Core MVC
- Implemented authentication and authorization with ASP.NET Identity
- Established repository and unit of work patterns for data access
- Configured logging with Serilog
- Added email service infrastructure
- **Triển khai tính năng điều chỉnh tỷ lệ UI để giải quyết vấn đề giao diện quá lớn**

### Quy tắc đặt tên mới và Chuẩn hóa

- **Đã thực hiện chuẩn hóa quy tắc đặt tên trong toàn dự án**:

  - Sử dụng tên đầy đủ thay vì viết tắt (SuccessFactor thay vì SF)
  - Đổi KPI thành Performance Indicator (PI)
  - Đổi KRI thành Result Indicator (RI)
  - Đổi CSF thành Success Factor (SF)
  - Cập nhật tất cả các enum, ViewModel, và tài liệu theo quy tắc mới
  - **Thay đổi "Csf"/"CSF" thành "SuccessFactor" trong toàn bộ hệ thống, bao gồm tên class, handler, controller và authorization policy**
  - **Đổi "Kpi"/"KPI" thành "Indicator" hoặc "PerformanceIndicator" tùy ngữ cảnh**

- **Cập nhật ViewModels để phản ánh quy tắc đặt tên mới**:

  - Tạo mới PerformanceIndicatorViewModel thay thế KpiViewModel
  - Tạo mới ResultIndicatorViewModel cho RI (Result Indicator)
  - Cập nhật SuccessFactorViewModel từ CSFViewModel
  - Đổi tên CSF namespace thành SuccessFactor
  - Cập nhật tất cả tài liệu và hiển thị

- **Cập nhật các enum để phản ánh quy tắc đặt tên mới**:
  - Đổi KpiType thành IndicatorType
  - Đổi KpiStatus thành IndicatorStatus
  - Đổi CSFStatus thành SuccessFactorStatus
  - Đổi CSFCategory thành SuccessFactorCategory
  - Cập nhật tất cả các giá trị enum và hiển thị

### Cập nhật Handler và Authorization

- **Cập nhật các Authorization Handler**:

  - Đổi tên `CsfAuthorizationHandler` thành `SuccessFactorAuthorizationHandler`
  - Đổi tên `CsfResourceAuthorizationHandler` thành `SuccessFactorResourceAuthorizationHandler`
  - Cập nhật các tham chiếu trong `Program.cs`

- **Cập nhật các Authorization Policy**:
  - Thay đổi `CanViewCsfs` thành `CanViewSuccessFactors`
  - Thay đổi `CanManageCsfs` thành `CanManageSuccessFactors`
  - Thay đổi `CanDeleteCsfs` thành `CanDeleteSuccessFactors`

### Cập nhật Mapping và Controller

- **Cập nhật AutoMapper Profiles**:

  - Đổi tên `CsfMappingProfile` thành `SuccessFactorMappingProfile`
  - Cập nhật các tham chiếu trong `Program.cs`

- **Cập nhật URL Routes**:
  - Thay đổi đường dẫn URL từ `Csf/` thành `SuccessFactor/`
  - Cập nhật controller references từ `"Csf"` thành `"SuccessFactor"`

### Cấu trúc và tổ chức Enums

- **Gộp và chuẩn hóa các file enum thành một cấu trúc thống nhất**:

  - Chuyển đổi từ KpiEnums sang IndicatorEnums để thống nhất hệ thống tên
  - Gộp CSFCategory.cs và SuccessFactorEnums.cs để tránh trùng lặp
  - Tạo file MeasurementStatusEnum.cs cho trạng thái đo lường
  - Tạo file IndicatorPropertyEnums.cs cho các thuộc tính của indicator
  - Tạo file BasicIndicatorEnums.cs với các enum cơ bản cho indicator
  - Loại bỏ các enum trùng lặp giữa các file
  - Cập nhật RiskLevel enum để phù hợp với cả SuccessFactor và IndicatorProperty
  - **Tạo file RelationshipEnums.cs để định nghĩa enum RelationshipStrength và RelationshipType**
  - **Tạo file MeasurementEnums.cs để tập trung các enum liên quan đến đo lường**

- **Cập nhật namespace cho Enums**:
  - Tổ chức các enum thành các namespace riêng biệt: Indicator, Organization, Business, Relationship
  - Sửa references trong viewmodels để sử dụng namespace đầy đủ và tránh ambiguous references
  - Cập nhật các `using` statements trong tất cả các file để phản ánh cấu trúc namespace mới
  - Sử dụng fully qualified names cho các enum trong các ViewModel để tránh xung đột

### Organization Management

- Created department and unit entity models
- Implemented repository pattern for organizational entities
- Added views for department and unit management
- Created employee and position management

### KPI Management và Cấu trúc phân cấp

- Đã định nghĩa mô hình thực thể KPI, nhưng cần cập nhật theo cấu trúc mới
- Đã tạo repository implementations cho KPI, nhưng cần mở rộng cho các thực thể mới
- Đã bắt đầu triển khai KPI controllers, cần điều chỉnh theo hierarchy mới
- Đã bắt đầu làm việc với KPI view models và AutoMapper profiles, nhưng cần mở rộng
- **Đã triển khai Views cho SuccessFactor (SF)**, bao gồm Index, Details, Create, Edit và Delete
- **Đã cập nhật SFController để sử dụng các ViewModel mới** (SuccessFactorEditViewModel và SuccessFactorDetailsViewModel)
- **Đã thử nghiệm build dự án và sửa lỗi liên quan đến các views mới**
- **Đã đảm bảo các view của SF không sử dụng các thuộc tính không tồn tại như Category và RiskLevel**
- **Đã sửa lỗi trong KPI/Details.cshtml để nút "Thêm giá trị mới" hoạt động chính xác**
- **Đã làm rõ luồng làm việc với giá trị KPI thông qua MeasurementController**
- **Tạo BusinessObjective entity để thừa kế từ Objective, bổ sung các thuộc tính chuyên biệt**
- **Cập nhật các ViewModel (BusinessObjectiveDetailsViewModel, ObjectiveViewModel, ObjectiveEditViewModel) để sử dụng enum references chính xác**

### Cải thiện Measurement System

- **Đã refactor MeasurementController để sử dụng IUnitOfWork thay vì ApplicationDbContext**
- **Đã cập nhật các ViewModels liên quan đến Measurement để tuân theo quy tắc đặt tên mới (IndicatorMeasurementFilterViewModel, IndicatorValueViewModel)**
- **Đã tạo và triển khai MeasurementListViewModel để hiển thị các giá trị đo lường**
- **Đã nâng cấp chức năng lọc để làm việc tốt hơn với UnitOfWork pattern**
- **Đã cải thiện cách xây dựng truy vấn trong MeasurementController để tăng hiệu suất**
- **Đã cập nhật MeasurementStatus enum để sử dụng Actual thay vì Entered**
- **Đã cập nhật \_ViewImports.cshtml để bao gồm tất cả các tham chiếu namespace cần thiết**

### UI và UX Improvements

- **Đã cải thiện bố cục cho BusinessObjective/Details.cshtml**:

  - Đặt các chỉ số (CSF, SF, KPI, PI, KRI, RI) lên phần trên và nhóm chúng một cách hợp lý
  - Sử dụng card với border màu và icon để phân biệt các loại chỉ số
  - Cải thiện bố cục tổng thể để dễ đọc và điều hướng

- **Đã tối ưu hóa hiển thị nút lệnh**:

  - Mở rộng \_PageTitle.cshtml để hỗ trợ nhiều nút lệnh thông qua ViewData["CommandButtons"]
  - Thêm chức năng tự động chọn màu sắc nút dựa trên action (Delete = danger, Index = secondary)
  - Cải thiện UI và UX cho các nút lệnh đảm bảo thống nhất trên toàn hệ thống

- **Cải thiện form nhập liệu**:

  - Đánh dấu các trường bắt buộc bằng dấu sao (\*) trong tất cả các form
  - Thêm placeholder và hướng dẫn trong các trường nhập liệu
  - Tối ưu layout form để dễ sử dụng và thân thiện hơn

- **Chuẩn hóa giao diện người dùng theo tiếng Anh**:

  - Hoàn thành việc chuyển đổi tất cả các views KPI sang tiếng Anh (KPI, KRI, RI, PI)
  - Thêm breadcrumb navigation nhất quán cho tất cả các trang
  - Thêm thông báo thông tin (alert boxes) cho tất cả các views để giải thích mục đích của mỗi loại indicator
  - Đánh dấu tất cả các trường bắt buộc bằng dấu sao (\*) dựa trên thuộc tính [Required] trong ViewModel
  - Đảm bảo tính nhất quán trong các tiêu đề, nút và thông báo trên tất cả các views
  - Cải thiện cách hiển thị phần liên kết CSF trong tất cả các form chỉ số
  - **Sửa lỗi trong nút "Thêm giá trị mới" cho phép người dùng ghi lại các giá trị đo lường mới**

- **Cải thiện kích thước giao diện**:
  - Đã điều chỉnh CSS trong site.css để giảm kích thước tổng thể của giao diện
  - Giảm font-size và padding của các phần tử UI
  - Tạo giao diện nhỏ gọn hơn mà không làm mất đi tính dễ đọc
  - Triển khai tính năng điều chỉnh tỷ lệ (UI Scaling) cho phép người dùng chọn mức độ thu phóng
  - Thêm file CSS ui-scale.css với cơ chế zoom/scale
  - Thêm điều khiển tỷ lệ UI vào layout cho phép điều chỉnh 75%, 80%, 90%, 100%
  - Lưu trữ tùy chọn tỷ lệ trong localStorage để duy trì giữa các phiên

### KPI Measurement và Tracking

- **Đã hoàn thiện hệ thống đo lường và theo dõi KPI**:
  - Sửa lỗi trong nút "Thêm giá trị mới" trên trang KPI Details để trỏ đến đúng controller action
  - Triển khai đầy đủ MeasurementController với các action Create, History và Details
  - Xây dựng các model như KpiValueCreateViewModel và AddMeasurementViewModel để quản lý giá trị KPI
  - Tạo giao diện người dùng để thêm và xem lịch sử các giá trị KPI
  - Hiển thị trạng thái KPI dựa trên giá trị mới nhất và mục tiêu
  - Hiển thị xu hướng giá trị KPI qua thời gian bằng biểu đồ

## Next Steps

### Immediate Tasks

1. **Cập nhật mô hình dữ liệu gộp**

   - Triển khai mô hình SuccessFactor mới (gộp SF và CSF)
   - Triển khai mô hình ResultIndicator mới (gộp RI và KRI)
   - Triển khai mô hình PerformanceIndicator mới (gộp PI và KPI)
   - Thiết kế bảng Measurement để hỗ trợ cả hai loại indicator
   - Cập nhật DbContext và tạo migrations

2. **Hoàn thành việc gộp và chuẩn hóa các Enum**

   - Kiểm tra và đảm bảo không còn trùng lặp enum trong hệ thống
   - Xóa các file enum cũ không còn sử dụng
   - Kiểm tra xem có cần cập nhật enum nào khác để phù hợp với cấu trúc mới
   - Cập nhật tất cả các references trong code để sử dụng enum mới

3. **Cập nhật Repositories**

   - Cập nhật SuccessFactorRepository để hỗ trợ cả SF và CSF
   - Tạo ResultIndicatorRepository để hỗ trợ cả RI và KRI
   - Tạo PerformanceIndicatorRepository để hỗ trợ cả PI và KPI
   - Tạo các phương thức query chuyên biệt cho từng loại (GetSF, GetCSF, GetKPI, etc.)

4. **Triển khai các Service mới**

   - Thiết kế và triển khai IObjectiveService
   - Cập nhật ISuccessFactorService để hỗ trợ cả SF và CSF
   - Thiết kế và triển khai IResultIndicatorService (cho cả RI và KRI)
   - Thiết kế và triển khai IPerformanceIndicatorService (cho cả PI và KPI)
   - Thêm logic phân loại và xử lý khác nhau cho các loại indicator

5. **Cập nhật Controllers**

   - Tạo ObjectiveController
   - Cập nhật SuccessFactorController để hỗ trợ cả SF và CSF
   - Tạo ResultIndicatorController với action riêng cho RI và KRI
   - Tạo PerformanceIndicatorController với action riêng cho PI và KPI
   - Thêm logic hiển thị và validation phù hợp với từng loại

6. **Cập nhật Views**

   - Tạo views cho Objective
   - Cập nhật views cho SuccessFactor để phù hợp với mô hình mới
   - Tạo views cho ResultIndicator với hiển thị khác nhau cho RI và KRI
   - Tạo views cho PerformanceIndicator với hiển thị khác nhau cho PI và KPI
   - **Mở rộng khả năng quản lý giá trị đo lường để hoạt động với cả RI và PI**

7. **Hoàn thiện chuẩn hóa giao diện**
   - Chuyển đổi tất cả views sang tiếng Anh
   - Đảm bảo tính nhất quán trong cách hiển thị các loại indicator
   - Đảm bảo responsive design trên tất cả các views

### Upcoming Tasks

1. **Cập nhật Dashboard**

   - Điều chỉnh thiết kế dashboard để hiển thị cấu trúc phân cấp mới
   - Tạo các biểu đồ và báo cáo cho tất cả các cấp indicator
   - Thêm chế độ xem theo Objective và SF

2. **Cập nhật Navigation và UX**

   - Điều chỉnh menu navigation để hiển thị cấu trúc mới
   - Cải thiện UX để dễ dàng điều hướng giữa các cấp
   - Thêm các tính năng tìm kiếm và lọc cho cấu trúc phân cấp

3. **Xử lý dữ liệu**
   - Phát triển các công cụ nhập liệu cho cấu trúc phân cấp mới
   - Tạo các tính năng báo cáo và phân tích dữ liệu
   - Triển khai hệ thống thông báo cho các chỉ số quan trọng

## Active Decisions and Considerations

### Technical Decisions

1. **Cách tiếp cận mô hình dữ liệu**

   - Quyết định sử dụng class riêng biệt cho mỗi loại indicator thay vì dùng discriminator
   - Cân nhắc việc quản lý KRI và KPI bằng flag hoặc bằng các entity riêng
   - Xác định cách xử lý các công thức tính toán phức tạp giữa các indicators
   - **Quyết định:** Chuẩn hóa sử dụng entity `Objective` cơ sở thay vì `BusinessObjective` để đồng bộ hệ thống. Các thuộc tính đặc thù (nếu cần) sẽ được xem xét tích hợp vào `Objective` hoặc xử lý theo cách khác.

2. **Hiệu suất của truy vấn phân cấp**

   - Cân nhắc việc sử dụng eager loading hay lazy loading cho cấu trúc phân cấp sâu
   - Đánh giá việc sử dụng caching cho các truy vấn thường xuyên
   - Lên kế hoạch tối ưu hóa database cho cấu trúc phân cấp lớn

3. **Granularity của Authorization**

   - Làm thế nào để áp dụng authorization cho cấu trúc phân cấp mới
   - Xác định quyền truy cập ở mức độ Objective, SF, CSF, và các indicators

4. **Tiêu chuẩn hóa UI/UX**
   - Quyết định sử dụng tiếng Anh cho toàn bộ giao diện người dùng
   - Áp dụng cách đánh dấu trường bắt buộc bằng dấu sao (\*) trên tất cả các form
   - Sử dụng breadcrumb navigation và alert boxes nhất quán trên tất cả các trang

### Open Questions

1. Làm thế nào để xử lý việc chuyển đổi dữ liệu hiện có sang mô hình dữ liệu mới?
2. Làm thế nào để hiển thị trực quan cấu trúc phân cấp phức tạp trên giao diện người dùng?
3. Làm thế nào để tối ưu hóa hiệu suất khi làm việc với nhiều lớp phân cấp dữ liệu?
4. Có nên tạo các entity riêng cho KRI và KPI hay chỉ cần sử dụng flag IsKey trên RI và PI?
5. Nên tiếp tục chuyển đổi toàn bộ hệ thống sang tiếng Anh hay duy trì hỗ trợ đa ngôn ngữ?
6. Làm thế nào để tích hợp hiệu quả hệ thống đo lường KPI với dashboard và báo cáo?

## Development Priorities

Ưu tiên phát triển hiện tại được xếp hạng như sau:

1. **Cập nhật mô hình thực thể** - Ưu tiên cao nhất

   - Bổ sung đầy đủ các entities trong cấu trúc phân cấp
   - Cập nhật mối quan hệ giữa các thực thể
   - Thực hiện migration cơ sở dữ liệu

2. **Triển khai Objective và SF** - Ưu tiên cao

   - Hoàn thiện các service, controller và view cho Objective
   - ~~Hoàn thiện các service, controller và view cho SF~~ (Đã hoàn thành các view cơ bản cho SF)

3. **Cập nhật CSF, RI, PI** - Ưu tiên cao

   - Điều chỉnh CSF để kết nối với SF
   - Triển khai đầy đủ RI và PI

4. **Cập nhật KRI và KPI** - Ưu tiên cao

   - Cập nhật KPI để trở thành một phần của PI
   - Triển khai KRI dựa trên RI
   - **Đảm bảo hệ thống đo lường KPI hoạt động đúng với tất cả các loại chỉ số**

5. **Chuẩn hóa UI/UX** - Ưu tiên cao

   - ~~Chuyển đổi tất cả các views KPI sang tiếng Anh~~ (Đã hoàn thành)
   - ~~Thêm breadcrumb navigation và alert boxes nhất quán~~ (Đã hoàn thành)
   - ~~Đánh dấu trường bắt buộc bằng dấu sao (\*)~~ (Đã hoàn thành)
   - Chuyển đổi phần còn lại của ứng dụng sang tiếng Anh

6. **Dashboard hiện thị cấu trúc phân cấp** - Ưu tiên trung bình
   - Cập nhật giao diện dashboard để hiển thị cấu trúc mới
   - Tạo các báo cáo tổng hợp dựa trên cấu trúc phân cấp

## Testing Focus

Trọng tâm kiểm thử hiện tại tập trung vào:

1. Unit tests cho các service methods mới và cập nhật
2. Integration tests cho repository implementations với cấu trúc phân cấp mới
3. Tests cho các entity relationships và data integrity
4. Tests cho việc authorization trên cấu trúc phân cấp mới
5. UI testing cho tính nhất quán của giao diện người dùng
6. **Kiểm thử chức năng đo lường KPI trên tất cả các loại chỉ số**

## Technical Debt

Các khu vực cần chú ý:

1. Cập nhật tài liệu API để phản ánh cấu trúc phân cấp mới
2. Tối ưu hóa truy vấn cơ sở dữ liệu cho cấu trúc phân cấp phức tạp
3. Cập nhật seed data để bao gồm tất cả các loại entity mới
4. Chuẩn hóa quy trình validation cho tất cả các entity trong cấu trúc phân cấp
5. Giải quyết sự không nhất quán về ngôn ngữ trong hệ thống (chuyển đổi hoàn toàn sang tiếng Anh)
6. **Cải thiện UX cho việc quản lý giá trị KPI, bao gồm giao diện biểu đồ và báo cáo**
