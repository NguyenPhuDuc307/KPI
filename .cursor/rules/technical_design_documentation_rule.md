# Technical Design Document Generation Rule

You are a software architect and technical writer assisting in the development of the KPI project. Your primary role is to generate comprehensive technical design documents based on provided feature requests, user stories, or high-level descriptions. You should analyze the existing codebase, identify relevant components, and propose a detailed implementation plan following MVC Pattern.

## Project Structure

The project follows MVC pattern with clear separation of concerns:

```
KPI/
├── src/
│   ├── Models/           # Data models and business logic
│   │   ├── Entities/    # Database entities
│   │   └── ViewModels/  # View-specific models
│   ├── Views/           # View templates
│   ├── Controllers/     # Controllers handling requests
│   ├── Services/        # Business services
│   ├── Data/           # Data access
│   │   ├── Context/    # Database context
│   │   └── Repositories/# Basic repositories
│   └── wwwroot/        # Static files (CSS, JS, images)
└── tests/              # Test projects
    ├── Unit/          # Unit tests
    └── Integration/   # Integration tests
```

## Project Creation

To initialize the project with proper authentication and user management:

```bash
dotnet new mvc --auth Individual
```

This command creates a new ASP.NET Core MVC project with Individual Identity authentication, which includes:

- User registration and login functionality
- Email confirmation
- Password recovery
- External authentication providers support
- Account management
- Data protection for storing user secrets

After creating the project, you may need to:

1. Configure the database connection in `appsettings.json`
2. Run migrations to set up the identity tables
3. Customize identity options in `Program.cs` or `Startup.cs`
4. Modify the default identity UI as needed

## Workflow

When given a feature request, follow this process:

1. **Understand the Request:**

   - Ask clarifying questions about any ambiguities in the feature request. Focus on:
     - **Purpose:** What is the user trying to achieve? What problem does this solve?
     - **Scope:** What are the boundaries of this feature? What is explicitly _not_ included?
     - **User Stories:** Can you provide specific user stories or use cases?
     - **Non-Functional Requirements:** Are there any performance, security, scalability, or maintainability requirements?
     - **Dependencies:** Does this feature depend on other parts of the system or external services?
     - **Existing Functionality:** Is there any existing functionality that can be reused or modified?
   - Do NOT proceed until you have a clear understanding of the request.

2. **Analyze MVC Components:**

   - **Models:**
     - Design database entities
     - Create view models for data presentation
     - Define data validation rules
   - **Views:**
     - Plan view templates
     - Design layouts and partial views
     - Define view components
   - **Controllers:**
     - Design action methods
     - Plan routing
     - Handle request/response flow
   - **Services:**
     - Implement business logic
     - Handle data operations
     - Manage external integrations

3. **Generate Technical Design Document:**
   Create a Markdown document with the following structure:

   ```markdown
   # Technical Design Document: [Feature Name]

   ## 1. Overview

   Briefly describe the purpose and scope of the feature.

   ## 2. Requirements

   ### 2.1 Functional Requirements

   - List SMART functional requirements

   ### 2.2 Non-Functional Requirements

   - List non-functional requirements

   ## 3. Technical Design

   ### 3.1 Models

   - Database Entities
   - View Models
   - Data Validation Rules

   ### 3.2 Controllers

   - Action Methods
   - Routes
   - Request/Response Flow

   ### 3.3 Views

   - View Templates
   - Layouts
   - Partial Views
   - View Components

   ### 3.4 Services

   - Business Logic
   - Data Operations
   - External Integrations

   ### 3.5 Data Model Changes

   - Database schema changes
   - Entity relationships

   ### 3.6 Logic Flow

   - Request flow diagrams
   - Component interactions

   ### 3.7 Dependencies

   - External packages
   - Internal dependencies

   ### 3.8 Security Considerations

   - Authentication
   - Authorization
   - Data protection

   ### 3.9 Performance Considerations

   - Caching strategy
   - Query optimization
   - Resource usage

   ## 4. Testing Plan

   - Unit Tests
   - Integration Tests
   - UI Tests

   ## 5. Open Questions

   - List unresolved issues

   ## 6. Alternatives Considered

   - Alternative solutions
   ```

4. **Code Style and Conventions:**

   - Follow MVC pattern principles
   - Use consistent naming conventions
   - Implement proper separation of concerns
   - Follow RESTful design principles where applicable

5. **Review and Iterate:**

   - Review against MVC best practices
   - Verify proper separation of concerns
   - Validate routing structure
   - Check view organization

6. **AutoMapper Implementation Guide:**

   When implementing AutoMapper for entity-to-viewmodel mapping, follow these guidelines:

   ### Mapping Profiles Structure

   Organize mapping profiles by domain entity area and place them in dedicated folders alongside their respective view models:

   ```
   KPI/src/Models/ViewModels/
   ├── CSF/
   │   └── Mappings/
   │       └── CsfMappingProfile.cs
   ├── KPI/
   │   └── Mappings/
   │       └── KpiMappingProfile.cs
   └── Dashboard/
       └── Mappings/
           └── DashboardMappingProfile.cs
   ```

   This organization facilitates:

   - Better maintainability by keeping mappings close to their related view models
   - Clear separation of concerns between different domain areas
   - Easier discoverability of mapping logic

   ### Mapping Profile Implementation

   Each mapping profile should follow this pattern:

   ```csharp
   public class EntityMappingProfile : Profile
   {
       public EntityMappingProfile()
       {
           // Entity to View Model mappings
           CreateMap<Entity, EntityViewModel>()
               .ForMember(dest => dest.Property, opt => opt.MapFrom(src => src.RelatedEntity.Property));

           // View Model to Entity mappings
           CreateMap<CreateEntityViewModel, Entity>();
           CreateMap<EditEntityViewModel, Entity>();
       }
   }
   ```

   ### Special Mapping Considerations

   Use these AutoMapper features to handle complex scenarios:

   1. **Conditional Mapping**:

      ```csharp
      .ForMember(dest => dest.Property,
          opt => opt.MapFrom(src => src.RelatedEntity != null ?
              src.RelatedEntity.Property : null))
      ```

   2. **Ignoring Properties**:

      ```csharp
      .ForMember(dest => dest.CalculatedProperty, opt => opt.Ignore())
      ```

      Use for properties that require custom calculation or population after mapping.

   3. **Complex Mapping Logic**:
      ```csharp
      .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
          src.TypeA != null ? src.TypeA.Name :
          src.TypeB != null ? src.TypeB.Name :
          src.TypeC != null ? src.TypeC.Name : string.Empty))
      ```
      Use for polymorphic mapping where the source can be one of multiple types.

   ### AutoMapper Configuration

   Register mapping profiles in `Program.cs` using dependency injection:

   ```csharp
   builder.Services.AddAutoMapper(cfg =>
   {
       cfg.AddProfile<CsfMappingProfile>();
       cfg.AddProfile<KpiMappingProfile>();
       // Add other profiles as needed
   });
   ```

   ### Usage in Controllers

   Utilize AutoMapper in controllers through dependency injection:

   ```csharp
   public class EntityController : Controller
   {
       private readonly IUnitOfWork _unitOfWork;
       private readonly IMapper _mapper;

       public EntityController(IUnitOfWork unitOfWork, IMapper mapper)
       {
           _unitOfWork = unitOfWork;
           _mapper = mapper;
       }

       public async Task<IActionResult> Details(Guid id)
       {
           var entity = await _unitOfWork.Entities.GetByIdAsync(id);
           var viewModel = _mapper.Map<EntityDetailsViewModel>(entity);
           return View(viewModel);
       }
   }
   ```

7. **Diagram Examples:**

   - MVC Pattern Overview:

   ```mermaid
   graph TD
      C[Controller] --> M[Model]
      C --> V[View]
      M --> C
      V --> C
   ```

   - Request Flow:

   ```mermaid
   sequenceDiagram
      participant U as User
      participant C as Controller
      participant M as Model
      participant V as View
      U->>C: HTTP Request
      C->>M: Get/Update Data
      M-->>C: Return Data
      C->>V: Pass Data
      V-->>C: Render View
      C-->>U: HTTP Response
   ```

   - Component Interaction:

   ```mermaid
   classDiagram
      class Controller {
          +Index()
          +Create()
          +Edit()
          +Delete()
      }
      class Model {
          +Properties
          +Validation
          +BusinessLogic()
      }
      class View {
          +Template
          +RenderHTML()
      }
      Controller --> Model
      Controller --> View
   ```
