# Repository Pattern Implementation

## Overview

The repository pattern has been implemented following Domain-Driven Design (DDD) principles with a clear separation of concerns:

### Key Components

#### 1. **Base Classes (Domain Layer)**

- **`AggregateRoot<TId>`** - Base class for all aggregate roots
  - Located in: `src/Core/VIAPadelClub.Core.Domain/Common/AggregateRoot.cs`
  - All aggregates that need persistence should inherit from this class
  - Generic parameter `TId` ensures type-safe identifiers

- **`Id<T>`** - Base class for strongly-typed identifiers
  - Located in: `src/Core/VIAPadelClub.Core.Domain/Common/Id.cs`
  - Provides equality comparison based on underlying value
  - Prevents accidental mixing of different ID types

#### 2. **Generic Repository Interface (Domain Layer)**

- **`IGenericRepository<TAggr, TId>`** - Generic async repository interface
  - Located in: `src/Core/VIAPadelClub.Core.Domain/Repositories/IGenericRepository.cs`
  - Constraints:
    - `TAggr : AggregateRoot<TId>` - Only aggregates
    - `TId : Id<TId>` - Only strongly-typed IDs
  - Methods:
    - `Task<TAggr?> GetAsync(TId id)` - Retrieve aggregate with related entities
    - `Task AddAsync(TAggr aggregate)` - Add new aggregate
    - `Task RemoveAsync(TId id)` - Remove aggregate

#### 3. **Abstract Base Implementation (Infrastructure Layer)**

- **`RepositoryEfcBase<TAgg, TId>`** - Abstract EFC repository base class
  - Located in: `src/VIAPadelClub.Infrastructure.SqliteDomainPersistence/RepositoryBase/RepositoryEfcBase.cs`
  - Provides default implementations of repository methods
  - Handles DbContext injection
  - Supports eager loading of related entities via method overriding
  - **Important**: Changes are tracked but NOT persisted until `IUnitOfWork.SaveChangesAsync()` is called

#### 4. **Specific Repository Interfaces (Domain Layer)**

- **`ICourtRepository`** - Court aggregate repository
  - Located in: `src/Core/VIAPadelClub.Core.Domain/Repositories/ICourtRepository.cs`
  - Extends: `IGenericRepository<Court, CourtId>`
  - Can add Court-specific query methods

- **`IScheduleRepository`** - Schedule aggregate repository
  - Located in: `src/Core/VIAPadelClub.Core.Domain/Repositories/IScheduleRepository.cs`
  - Extends: `IGenericRepository<Schedule, ScheduleId>`
  - Can add Schedule-specific query methods

#### 5. **Concrete Implementations (Infrastructure Layer)**

- **`CourtRepositoryEfc`** - EFC implementation for Court repository
  - Located in: `src/VIAPadelClub.Infrastructure.SqliteDomainPersistence/Repositories/CourtRepositoryEfc.cs`
  - Extends: `RepositoryEfcBase<Court, CourtId>`
  - Overrides methods to include related entities (e.g., bookings)

- **`ScheduleRepositoryEfc`** - EFC implementation for Schedule repository
  - Located in: `src/VIAPadelClub.Infrastructure.SqliteDomainPersistence/Repositories/ScheduleRepositoryEfc.cs`
  - Extends: `RepositoryEfcBase<Schedule, ScheduleId>`
  - Can include related time intervals and courts

---

## How to Use

### Creating a New Repository

To create a repository for a new aggregate, follow these steps:

#### 1. Create an ID Value Object (if needed)

```csharp
// In: src/Core/VIAPadelClub.Core.Domain/Common/Values/

public sealed record MyAggregateId(Guid Value)
{
    public static MyAggregateId New() => new(Guid.NewGuid());
    public static MyAggregateId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}
```

#### 2. Make the Aggregate Inherit from AggregateRoot

```csharp
// In: src/Core/VIAPadelClub.Core.Domain/Aggregates/

public class MyAggregate : AggregateRoot<MyAggregateId>
{
    // Aggregate implementation
    public MyAggregate(MyAggregateId id)
    {
        Id = id;
    }
}
```

#### 3. Create a Specific Repository Interface

```csharp
// In: src/Core/VIAPadelClub.Core.Domain/Repositories/

public interface IMyAggregateRepository : IGenericRepository<MyAggregate, MyAggregateId>
{
    // Add aggregate-specific query methods here if needed
    Task<List<MyAggregate>> GetBySpecialCriteriaAsync(string criteria);
}
```

#### 4. Create the EFC Implementation

```csharp
// In: src/VIAPadelClub.Infrastructure.SqliteDomainPersistence/Repositories/

public class MyAggregateRepositoryEfc : RepositoryEfcBase<MyAggregate, MyAggregateId>, IMyAggregateRepository
{
    public MyAggregateRepositoryEfc(DomainModelContext context) : base(context)
    {
    }

    public override async Task<MyAggregate?> GetAsync(MyAggregateId id)
    {
        return await Context.Set<MyAggregate>()
            .Include(a => a.RelatedEntities) // Include related data
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<MyAggregate>> GetBySpecialCriteriaAsync(string criteria)
    {
        return await Context.Set<MyAggregate>()
            .Where(a => a.SomeProperty == criteria)
            .ToListAsync();
    }
}
```

#### 5. Register the Repository in Dependency Injection

In your application startup/DI configuration:

```csharp
services.AddScoped<IMyAggregateRepository, MyAggregateRepositoryEfc>();
```

---

## Important Notes

### Unit of Work Pattern

- Repository methods track changes in the DbContext but **do not persist them**
- All changes are saved through the `IUnitOfWork` interface
- This ensures atomic operations across multiple repositories

```csharp
// Usage in application service:
await _myAggregateRepository.AddAsync(newAggregate);
// Changes are NOT yet saved!

await _unitOfWork.SaveChangesAsync();
// Changes are NOW persisted to database
```

### Lazy Loading vs. Eager Loading

- The base repository uses `DbContext.Set<T>().FindAsync()` which doesn't automatically load related entities
- Override `GetAsync()` in concrete implementations to use `Include()` and `ThenInclude()` for eager loading when needed
- This prevents N+1 query problems

```csharp
// Bad: N+1 query problem
var court = await _courtRepository.GetAsync(courtId);
var bookings = court.Bookings; // Requires separate query

// Good: Eager loading in override
public override async Task<Court?> GetAsync(CourtId id)
{
    return await Context.Set<Court>()
        .Include(c => c.Bookings)
        .FirstOrDefaultAsync(c => c.Id == id);
}
```

---

## Architecture Diagram

```
┌─────────────────────────────────────────────┐
│         Application Layer                    │
│  (Commands, Queries, Event Handlers)        │
└─────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────┐
│         Domain Layer                         │
│  ┌──────────────────────────────────────┐   │
│  │ IGenericRepository<TAgg, TId>        │   │
│  │ ICourtRepository                     │   │
│  │ IScheduleRepository                  │   │
│  │ (other specific repo interfaces)     │   │
│  └──────────────────────────────────────┘   │
│  ┌──────────────────────────────────────┐   │
│  │ AggregateRoot<TId>                   │   │
│  │ Id<T>                                │   │
│  │ Aggregates (Court, Schedule, etc.)   │   │
│  └──────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────┐
│      Infrastructure Layer (EFC)              │
│  ┌──────────────────────────────────────┐   │
│  │ RepositoryEfcBase<TAgg, TId>         │   │
│  │ CourtRepositoryEfc                   │   │
│  │ ScheduleRepositoryEfc                │   │
│  │ (other concrete repo implementations)│   │
│  └──────────────────────────────────────┘   │
│  ┌──────────────────────────────────────┐   │
│  │ DomainModelContext (DbContext)       │   │
│  │ IUnitOfWork                          │   │
│  └──────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
```

---

## Next Steps

1. Update existing aggregates to inherit from `AggregateRoot<TId>`
2. Create ID value objects for aggregates using raw Guid or string IDs
3. Replace old specific repository interfaces with new generic-based ones
4. Migrate implementations to use `RepositoryEfcBase`
5. Update unit tests to use the new generic repository interfaces


