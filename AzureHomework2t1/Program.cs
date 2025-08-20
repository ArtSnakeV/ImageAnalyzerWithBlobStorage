using AzureHomework2t1.Services.Abstract;
using AzureHomework2t1.Services.Implementation;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Design;
using AzureHomework2t1.Controllers;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

// Register the connection string and container name
builder.Services.AddSingleton<BlobServiceClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration["AzureStorageConnectionString"]; // Correctly retrieve the string
    return new BlobServiceClient(connectionString);
});


//builder.Services.AddSingleton<string>(containerName);

// Register our BlobService with DI
builder.Services.AddTransient<IBlobService, BlobService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLComputerVisionImagesDb")));

// Other adjustments
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles(); // Added to use css styles

//app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Images}/{action=Index}/{id?}");

app.Run();
