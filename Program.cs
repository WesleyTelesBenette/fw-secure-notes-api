using fw_secure_notes_api.Conventions;
using fw_secure_notes_api.Data;
using fw_secure_notes_api.Filters;
using fw_secure_notes_api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Database
var connectionString = builder.Configuration.GetConnectionString("DefaultDatabase");
builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<PageRepository>();
builder.Services.AddScoped<FileRepository>();

//JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

//Filter
builder.Services.AddScoped<TokenValidateActionFilter>();

//General
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteConvention());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RouteValidateMiddleware>();

app.MapControllers();

app.Run();
