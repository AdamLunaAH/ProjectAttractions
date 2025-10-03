
public class IntFromStringJsonConverter : JsonConverter<int>
{
    public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            // Try parsing string to int
            if (int.TryParse((string)reader.Value, out var result))
            {
                return result;
            }
            throw new JsonSerializationException($"Cannot convert {reader.Value} to int.");
        }
        if (reader.TokenType == JsonToken.Integer)
        {
            return Convert.ToInt32(reader.Value);
        }
        throw new JsonSerializationException($"Unexpected token {reader.TokenType} when parsing int.");
    }

    public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
    {
        writer.WriteValue(value); // normal int serialization
    }
}



public class RoomDbM
{
    [Required]
    [JsonProperty("floor")]
    [JsonConverter(typeof(IntFromStringJsonConverter))]
    public override int Floor { get; public class RoomJsonDto
{
    public string RoomName { get; set; }
    public string Floor { get; set; }
    public int Capacity { get; set; }
    public Guid RoomGuid { get; set; }
    public string StreetAddress { get; set; } // e.g. "Storgatan 10 (City var roomJsonDtos = JsonConvert.DeserializeObject<List<RoomJsonDto>>(json);







    public class RoomJsonDto
{
    public string RoomName { get; set; }
    public string Floor { get; set; }
    public int Capacity { get; set; }
    public Guid RoomGuid { get; set; }
    public string StreetAddress { get; set; } // e.g. "Storgatan 10 (City hall)"
}

var roomJsonDtos = JsonConvert.DeserializeObject<List<RoomJsonDto>>(json);


var buildingDict = new Dictionary<string, BuildingCuDto>();
var roomDtos = new List<RoomCuDto>();

foreach (var json in roomJsonDtos)
{
    // split streetAddress → address + building name
    string address = json.StreetAddress;
    string? buildingName = null;

    if (address.Contains("(") && address.Contains(")"))
    {
        int start = address.IndexOf("(");
        int end = address.IndexOf(")", start);
        buildingName = address.Substring(start + 1, end - start - 1).Trim();
        address = address.Substring(0, start).Trim();
    }

    string buildingKey = $"{address}|{buildingName}";

    if (!buildingDict.TryGetValue(buildingKey, out var buildingDto))
    {
        buildingDto = new BuildingCuDto
        {
            BuildingId = null, // may be set later if found in DB
            BuildingName = buildingName,
            BuildingAddress = address
        };
        buildingDict[buildingKey] = buildingDto;
    }

    var roomDto = new RoomCuDto
    {
        RoomId = null,
        RoomName = json.RoomName,
        Floor = json.Floor,
        Capacity = json.Capacity,
        RoomGuid = json.RoomGuid,
        BuildingId = buildingDto.BuildingId
    };

    roomDtos.Add(roomDto);
    buildingDto.RoomsId.Add(roomDto.RoomId ?? 0); // placeholder until DB IDs assigned
}

// Load all existing buildings into memory keyed by Address+Name
var existingBuildings = _dbContext.Buildings
    .ToList()
    .ToDictionary(
        b => $"{b.BuildingAddress}|{b.BuildingName}",
        b => b);

foreach (var kvp in buildingDict)
{
    var key = kvp.Key;
    var dto = kvp.Value;

    if (existingBuildings.TryGetValue(key, out var existingBuilding))
    {
        // reuse existing building
        dto.BuildingId = existingBuilding.Id;
    }
    else
    {
        // create new
        var building = new BuildingDbM
        {
            BuildingName = dto.BuildingName,
            StreetAddress = dto.BuildingAddress
        };
        _dbContext.Buildings.Add(building);
        dto.BuildingId = building.Id; // will be set after SaveChanges
    }
}

// Save buildings first
_dbContext.SaveChanges();

// Now map roomDtos → RoomDbM with correct BuildingId
var roomDbModels = roomDtos.Select(dto => new RoomDbM
{
    RoomName = dto.RoomName,
    Floor = dto.Floor,
    Capacity = dto.Capacity,
    RoomGuid = dto.RoomGuid,
    BuildingId = dto.BuildingId.Value
}).ToList();

_dbContext.Rooms.AddRange(roomDbModels);
_dbContext.SaveChanges();

public interface IRoomImportService
{
    Task ImportRoomsFromJsonAsync(string json);
}

public class RoomImportService : IRoomImportService
{
    private readonly MyDbContext _dbContext;

    public RoomImportService(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ImportRoomsFromJsonAsync(string json)
    {
        // 1. Deserialize incoming JSON
        var roomJsonDtos = JsonConvert.DeserializeObject<List<RoomJsonDto>>(json)
                           ?? new List<RoomJsonDto>();

        // 2. Prepare DTO maps
        var buildingDict = new Dictionary<string, BuildingCuDto>();
        var roomDtos = new List<RoomCuDto>();

        foreach (var jsonDto in roomJsonDtos)
        {
            // --- Split "streetAddress" into address + name ---
            string address = jsonDto.StreetAddress;
            string? buildingName = null;

            if (address.Contains("(") && address.Contains(")"))
            {
                int start = address.IndexOf("(");
                int end = address.IndexOf(")", start);
                buildingName = address.Substring(start + 1, end - start - 1).Trim();
                address = address.Substring(0, start).Trim();
            }

            string buildingKey = $"{address}|{buildingName}";

            // --- Ensure building entry exists ---
            if (!buildingDict.TryGetValue(buildingKey, out var buildingDto))
            {
                buildingDto = new BuildingCuDto
                {
                    BuildingId = null,
                    BuildingName = buildingName,
                    BuildingAddress = address
                };
                buildingDict[buildingKey] = buildingDto;
            }

            // --- Build Room DTO ---
            var roomDto = new RoomCuDto
            {
                RoomId = null,
                RoomName = jsonDto.RoomName,
                Floor = jsonDto.Floor,
                Capacity = jsonDto.Capacity,
                RoomGuid = jsonDto.RoomGuid,
                BuildingId = buildingDto.BuildingId
            };

            roomDtos.Add(roomDto);
            buildingDto.RoomsId.Add(roomDto.RoomId ?? 0);
        }

        // 3. Check for existing buildings in DB
        var existingBuildings = await _dbContext.Buildings
            .ToListAsync();

        var existingLookup = existingBuildings.ToDictionary(
            b => $"{b.StreetAddress}|{b.BuildingName}",
            b => b);

        foreach (var kvp in buildingDict)
        {
            var key = kvp.Key;
            var dto = kvp.Value;

            if (existingLookup.TryGetValue(key, out var existingBuilding))
            {
                dto.BuildingId = existingBuilding.Id;
            }
            else
            {
                var building = new BuildingDbM
                {
                    BuildingName = dto.BuildingName,
                    StreetAddress = dto.BuildingAddress
                };
                await _dbContext.Buildings.AddAsync(building);
                await _dbContext.SaveChangesAsync(); // Save to get Id

                dto.BuildingId = building.Id;
            }
        }

        // 4. Convert Room DTOs into EF entities
        var roomDbModels = roomDtos.Select(dto => new RoomDbM
        {
            RoomName = dto.RoomName,
            Floor = dto.Floor,
            Capacity = dto.Capacity,
            RoomGuid = dto.RoomGuid,
            BuildingId = dto.BuildingId ?? throw new InvalidOperationException("BuildingId missing")
        }).ToList();

        await _dbContext.Rooms.AddRangeAsync(roomDbModels);
        await _dbContext.SaveChangesAsync();
    }
}

services.AddScoped<IRoomImportService, RoomImportService>();

[HttpPost("import")]
public async Task<IActionResult> ImportRooms([FromBody] string json)
{
    await _roomImportService.ImportRoomsFromJsonAsync(json);
    return Ok("Import completed");
}


# Explanation of `Configuration/Extensions` and `DbContext/Extensions`

This document explains the purpose and structure of the files and classes found in the `Configuration/Extensions` and `DbContext/Extensions` folders. It also describes how these extension methods simplify application setup in `Program.cs` by encapsulating configuration and service registration logic.

---

## 1. `Configuration/Extensions`

This folder contains static extension classes that encapsulate common configuration and service registration patterns. Each class provides methods that extend `IServiceCollection` or `IConfigurationBuilder`, allowing for clean, modular, and reusable setup code in `Program.cs`.

### Main Classes:

- **DatabaseExtensions**
  - Adds and configures database connection options and services.
  - Example method: `AddDatabaseConnections(IServiceCollection, IConfiguration)`
- **EncryptionExtensions**
  - Registers encryption-related options and services.
  - Example method: `AddEncryptions(IServiceCollection, IConfiguration)`
- **LoggerExtensions**
  - Adds a custom in-memory logger provider.
  - Example method: `AddInMemoryLogger(IServiceCollection)`
- **SecretsExtensions**
  - Configures secret storage, supporting both user secrets and Azure Key Vault, based on environment and configuration.
  - Example method: `AddSecrets(IConfigurationBuilder, IHostEnvironment, string)`
- **VersionExtensions**
  - Registers version information from assembly metadata.
  - Example method: `AddVersionInfo(IServiceCollection)`

#### **How it simplifies `Program.cs`**
Instead of cluttering `Program.cs` with detailed configuration and service registration logic, you can simply call these extension methods. This keeps the startup code clean and focused on high-level application flow.

**Example usage in `Program.cs`:**
```csharp
builder.Services
    .AddDatabaseConnections(builder.Configuration)
    .AddEncryptions(builder.Configuration)
    .AddInMemoryLogger()
    .AddVersionInfo();
```

---

## 2. `DbContext/Extensions`

This folder contains extension classes for configuring Entity Framework Core `DbContext` services, both at runtime and design-time (for migrations).

### Main Classes:

- **DbContextExtensions**
  - Registers the application's main `DbContext` with dependency injection, using connection details from configuration and supporting multiple database providers (SQL Server, MySQL, PostgreSQL).
  - Example method: `AddUserBasedDbContext(IServiceCollection)`
- **DbContextDesignTimeExtensions**
  - Provides helpers for configuring the `DbContext` at design-time (e.g., for EF Core migrations), including reading secrets and connection info as in the main app.
  - Example method: `ConfigureForDesignTime(DbContextOptionsBuilder, Func<DbContextOptionsBuilder, string, DbContextOptionsBuilder>)`

#### **How it simplifies `Program.cs` and migrations**
- Keeps all `DbContext` setup logic in one place, making it easy to switch database providers or update connection logic.
- Ensures that both runtime and design-time (migrations) use consistent configuration patterns.

**Example usage in `Program.cs`:**
```csharp
builder.Services.AddUserBasedDbContext();
```

---

## **Benefits of Using Extension Methods for Application Building**
- **Separation of Concerns:** Keeps configuration and registration logic out of `Program.cs`.
- **Reusability:** Extension methods can be reused across multiple projects or services.
- **Maintainability:** Changes to configuration logic are isolated to extension classes.
- **Readability:** `Program.cs` remains concise and easy to understand.

---

**In summary:**
The use of extension methods in `Configuration/Extensions` and `DbContext/Extensions` enables a modular, maintainable, and scalable approach to configuring services and application components, simplifying the application startup process.
