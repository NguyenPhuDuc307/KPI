// System and Framework usings
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using System.ComponentModel.DataAnnotations;
global using System.Text.Json;
global using System.Text;
global using System.IO;

// ASP.NET Core usings
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Rendering;

// Entity Framework Core usings
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

// AutoMapper usings  
global using AutoMapper;

// Project specific usings - Core infrastructure
global using KPISolution.Authorization;
global using KPISolution.Data;
global using KPISolution.Data.Repositories.Extensions;
global using KPISolution.Data.Repositories.Implementation;
global using KPISolution.Data.Repositories.Interfaces;
global using KPISolution.Extensions;
global using KPISolution.Infrastructure.Routing;

// Entity models
global using KPISolution.Models.Entities.Base;
global using KPISolution.Models.Entities.Identity;
global using KPISolution.Models.Entities.Indicator;
global using KPISolution.Models.Entities.Organization;
global using KPISolution.Models.Entities.Dashboard;
global using KPISolution.Models.Entities.Measurement;
global using KPISolution.Models.Entities.Notification;
global using KPISolution.Models.Entities.Progress;
// Enums - Base namespace
global using KPISolution.Models.Enums.Indicator;
global using KPISolution.Models.Enums.Measurement;
global using KPISolution.Models.Enums.Notification;
global using KPISolution.Models.Enums.Relationship;
global using KPISolution.Models.Enums.SuccessFactor;
global using KPISolution.Models.Enums.Object;
global using KPISolution.Models.Enums.Visualization;

// View models - Base namespace  
global using KPISolution.Models.ViewModels;
global using KPISolution.Models.ViewModels.SuccessFactor;
global using KPISolution.Models.ViewModels.Indicator;
global using KPISolution.Models.ViewModels.Indicator.PerformanceIndicator;
global using KPISolution.Models.ViewModels.Indicator.ResultIndicator;
global using KPISolution.Models.ViewModels.Organization;
global using KPISolution.Models.ViewModels.Dashboard;
global using KPISolution.Models.ViewModels.Department;
global using KPISolution.Models.ViewModels.Objective;
global using KPISolution.Models.ViewModels.Users;
global using KPISolution.Models.ViewModels.Measurement;
// Add using for Widget ViewModels
global using KPISolution.Models.ViewModels.Dashboard.Widgets;

// Services
global using KPISolution.Services.Email;