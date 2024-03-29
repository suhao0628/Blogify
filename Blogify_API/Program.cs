using Blogify_API;
using Blogify_API.Datas;
using Blogify_API.Services.IServices;
using Blogify_API.Services;
using Delivery_API.Services.IServices;
using Delivery_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Delivery_API.Middleware;
using Delivery_API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Connect to DB
builder.Services.AddDbContext<AppDbContext>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Blogify";
});
builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllers().
    AddJsonOptions(options =>
    {
        //serialization of Enum as String
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }); ;



builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<ICommunityService, CommunityService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ExceptionHandleMiddleware>();
builder.Services.AddScoped<JwtMiddleware>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    //Version control
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Blogify_API",
        Version = "v1",
        Description = ""
    }
    );
    //Generate comment
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        Type = SecuritySchemeType.Http,
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

//Authentication
builder.Services.Configure<JwtConfig>(
    builder.Configuration.GetSection("Jwt"));
var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();


builder.Services.AddAuthentication
    (authoption => {
        authoption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        authoption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        authoption.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        authoption.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
.AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtConfig.Issuer,
        ValidAudience = jwtConfig.Audience,
        ValidateIssuer = true,
        ValidateLifetime = jwtConfig.ValidateLifetime,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SigningKey)),
    };
});


var app = builder.Build();

//SeedData
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    Initializer.Initialize(services);
//}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandleMiddleware>();
app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
