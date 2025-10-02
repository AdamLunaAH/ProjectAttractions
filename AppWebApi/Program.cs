using Configuration;
using Configuration.Extensions;
using DbContext.Extensions;
using DbRepos;
using Models;
using Services;

var builder = WebApplication.CreateBuilder(args);

// NOTE: global cors policy needed for JS and React frontends
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

//adding support for several secret sources and database sources
//to use either user secrets or azure key vault depending on UseAzureKeyVault tag in appsettings.json
builder.Configuration.AddSecrets(builder.Environment, "AppWebApi");

//use encryption and multiple Database connections and their respective DbContexts
builder.Services.AddEncryptions(builder.Configuration);
builder.Services.AddDatabaseConnections(builder.Configuration);
builder.Services.AddUserBasedDbContext();

// adding verion info
builder.Services.AddVersionInfo();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Tourist Attractions API",
#if DEBUG
        Version = "v1.0 DEBUG",
#else
        Version = "v1.0",
#endif
        Description = "This is an API used for the Tourist Attractions project."
        + $"<br>DataSet: {builder.Configuration["DatabaseConnections:UseDataSetWithTag"]}"
        + $"<br>DefaultDataUser: {builder.Configuration["DatabaseConnections:DefaultDataUser"]}"
    });

    // Register custom schema filter
    c.SchemaFilter<Swagger.Filters.UserCreateDtoSchemaFilter>();
});

builder.Services.AddSwaggerGenNewtonsoftSupport();


//Add InMemoryLoggerProvider logger
builder.Services.AddInMemoryLogger();

//Inject DbRepos and Services
builder.Services.AddScoped<AdminDbRepos>();
builder.Services.AddScoped<AttractionAddressesDbRepos>();
builder.Services.AddScoped<AttractionsDbRepos>();
builder.Services.AddScoped<CategoriesDbRepos>();
builder.Services.AddScoped<UsersDbRepos>();
builder.Services.AddScoped<ReviewsDbRepos>();



builder.Services.AddScoped<IAdminService, AdminServiceDb>();
builder.Services.AddScoped<IAttractionAddressesService, AttractionAddressesServiceDb>();
builder.Services.AddScoped<IAttractionsService, AttractionsServiceDb>();
builder.Services.AddScoped<ICategoriesService, CategoriesServiceDb>();
builder.Services.AddScoped<IUsersService, UsersServiceDb>();
builder.Services.AddScoped<IReviewsService, ReviewsServiceDb>();



var app = builder.Build();

// Configure the HTTP request pipeline.
// for the purpose of this example, we will use Swagger also in production
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tourist Attractions API v1.0");
    });
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();
app.MapControllers();

app.Run();
