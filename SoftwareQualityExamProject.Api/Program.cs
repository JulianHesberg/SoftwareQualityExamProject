using SoftwareQualityExamProject.Application.Services.Coupon.Implementations;
using SoftwareQualityExamProject.Application.Services.Coupon.Interfaces;
using SoftwareQualityExamProject.Application.Services.FileReader;
using SoftwareQualityExamProject.Application.Services.Pricing.Implementations;
using SoftwareQualityExamProject.Application.Services.Pricing.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFileReader, FileReader>();
builder.Services.AddScoped<ICouponService, CouponServiceImpl>();
builder.Services.AddScoped<IPricingService, PricingServiceImpl>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program { }