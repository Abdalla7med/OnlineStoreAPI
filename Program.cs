
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.Json.Serialization;

namespace OnlineStoreAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);

      

            /// this's the solution for the problem JsonCycle  in Order.OrderDetails.Order
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // Adjust depth as needed
                });
           

            // Add services to the container. builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            #region Swagger Configuration 
            builder.Services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "OnlineStoreAPI",
                        Description = "A Online Store ASP.net API",
                    });

                    // Define the API Key security scheme
                    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                    {
                            Name = "X-Api-Key",
                            Type = SecuritySchemeType.ApiKey,
                            In = ParameterLocation.Header,
                            Scheme = "ApiKey",
                            Description = "API Key needed to access the endpoints"
                    });

                    // Apply the API Key security requirement to all endpoints
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme 
                            {
                                Reference = new OpenApiReference    
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "ApiKey"
                                }
                            },
                            new List<string>()
                        }
                    });
                 });

            #endregion

            /// Adding connection string 
            builder.Services.AddDbContext<OnlineStoreContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



            #region Jwt Auth
           
            // Configure JWT authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            #endregion


            var app = builder.Build();
            /// Documentation 
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineStoreAPI v1");
                });
            }

            app.UseMiddleware<ApiKeyValidationMiddleware>(); /// To Validate Key 
            app.UseMiddleware<LoggingMiddleware>(); /// to log any exception will happened 
            app.UseMiddleware<ExceptionHandler>(); /// ExceptionHandling 


            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}
