# Project Brief - KPI Management System

## Project Overview
The KPI Management System is a comprehensive web application built with ASP.NET Core 9.0, designed to help organizations manage and track Key Performance Indicators (KPIs) and Critical Success Factors (CSFs). The system provides features for KPI definition, measurement, reporting, and analysis.

## Core Requirements
1. KPI Management
   - Define and track KPIs
   - Set targets and thresholds
   - Monitor progress
   - Link KPIs to CSFs

2. Data Collection
   - Manual data entry
   - Batch import from Excel
   - Automated data collection
   - Data validation

3. Reporting and Analysis
   - Standard reports
   - Custom report builder
   - Trend analysis
   - Performance analytics
   - Export to PDF, Excel, CSV

4. Dashboard
   - Executive dashboard
   - Departmental dashboards
   - Custom dashboard builder
   - Real-time KPI status

5. Notification System
   - Email notifications
   - In-app notifications
   - Alert management
   - Threshold monitoring

## Technical Stack
- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server
- Identity Framework
- AutoMapper
- Chart.js
- EPPlus
- DinkToPdf
- Serilog

## Project Phases
1. Phase 3: KPI Management Features
2. Phase 4: Data Collection and Measurement
3. Phase 5: Reporting and Analysis
4. Phase 6: Notification and Alert System
5. Phase 7: Testing and Quality Assurance
6. Phase 8: Documentation and Deployment

## Success Criteria
1. Complete implementation of all core features
2. Successful deployment to production
3. Comprehensive test coverage
4. Complete documentation in Vietnamese
5. User training and support

## Core Goals

1. Create a centralized system for tracking and managing organizational KPIs
2. Implement CSF management to link strategic goals to measurable KPIs
3. Provide real-time dashboards and visualization of performance metrics
4. Enable data-driven decision making through comprehensive reporting
5. Support hierarchical organizational structure with department-specific KPIs
6. Ensure security and role-based access to sensitive performance data

## Cấu trúc phân cấp chỉ số (Indicator Hierarchy)

Hệ thống triển khai cấu trúc phân cấp đầy đủ theo yêu cầu:

1. **Objectives (Mục tiêu)** - Mục tiêu chiến lược của tổ chức
2. **SF (Success Factors)** - Các yếu tố thành công, hỗ trợ đạt được các Objectives
3. **CSF (Critical Success Factors)** - Các yếu tố thành công quan trọng, là tập con của SF
4. **RI (Result Indicators)** - Các chỉ số đo lường kết quả liên quan đến CSF
5. **PI (Performance Indicators)** - Các chỉ số đo lường hiệu suất liên quan đến CSF
6. **KRI (Key Result Indicators)** - Các chỉ số kết quả chính, là tập con quan trọng của RI
7. **KPI (Key Performance Indicators)** - Các chỉ số hiệu suất chính, là tập con quan trọng của PI

Mỗi cấp trong hệ thống phân cấp có mối quan hệ 1-n với cấp dưới nó, tạo thành một cấu trúc phân cấp hoàn chỉnh để theo dõi và đo lường hiệu suất.

## Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 8.0)
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with role-based and resource-based authorization
- **Frontend**: Bootstrap, jQuery, Chart.js for visualizations
- **Reporting**: Custom PDF and Excel export functionality using DinkToPdf and EPPlus
- **Logging**: Serilog for structured logging
- **Email**: SMTP service with templates for notifications

## Key Features

1. **Quản lý cấu trúc phân cấp**:

   - Quản lý Objectives (mục tiêu)
   - Quản lý SF (Success Factors - yếu tố thành công)
   - Quản lý CSF (Critical Success Factors - yếu tố thành công quan trọng)
   - Quản lý các loại chỉ số (RI, PI, KRI, KPI)

2. **KPI Management**: Creation, tracking, and measurement of performance indicators
3. **CSF Management**: Definition of critical success factors linked to KPIs
4. **Dashboard**: Customizable views for different organizational roles
5. **Data Collection**: Manual and batch import of performance measurements
6. **Reporting & Analysis**: Standard and custom reports with trend analysis
7. **Notification System**: Alerts and reminders for KPI updates and thresholds
8. **User Management**: Role-based access control with detailed permissions
9. **Organizational Structure**: Department and unit hierarchy management

## Project Phases

The project is organized into 10 phases covering the full spectrum of features:

1. Core Infrastructure & Authentication
2. Organization Management
3. Cập nhật và mở rộng KPI Management Features (including full indicator hierarchy)
4. Data Collection and Measurement
5. Reporting and Analysis
6. Notification and Alert System
7. Testing and Quality Assurance
8. Documentation and Deployment
9. Enhanced User Experience
10. System Optimization and Maintenance

## Implementation Standards

1. Clean architecture with separation of concerns
2. Repository pattern for data access
3. Unit of Work pattern for transaction management and coordinated data access
4. Dependency injection for services
5. Comprehensive testing at all levels
6. Detailed documentation (technical and user)
7. Security best practices following OWASP guidelines
8. Responsive design for all interfaces
9. Internationalization support for Vietnamese language
10. Consistent use of design patterns across controllers and repositories

## Current Development Progress

The project is currently in Phase 3, focusing on the implementation of KPI Management Features, with services and views for KPIs, CSFs, and dashboards being the priority items.
