using AutoMapper;
using JwtWithIdentiyAuthenticatoin.Data;
using JwtWithIdentiyAuthenticatoin.helper;
using JwtWithIdentiyAuthenticatoin.Mapper;
using JwtWithIdentiyAuthenticatoin.Models.authModels;
using JwtWithIdentiyAuthenticatoin.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var constr = builder.Configuration.GetConnectionString("DefaultString");
builder.Services.Configure< jwtOptions>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IMovieServices, MovieServices>();

// mapper configuration
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfiling());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(constr)
);
builder.Services.AddAuthentication(x=> 
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["jwt:Audienc"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwt:SigninKey"])),
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
