using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
builder.Services.AddDbContext<SaloonWebApi.Data.SaloonDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("model")));

// CORS: read allowed origins from configuration, fallback to AllowAnyOrigin for dev
//var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
          .AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

var app = builder.Build();


// Enable CORS (must run before Authorization / Authentication middleware)
app.UseCors("DefaultCorsPolicy");


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();



app.UseAuthorization();

app.MapControllers();

app.Run();
