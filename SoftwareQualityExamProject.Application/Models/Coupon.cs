namespace SoftwareQualityExamProject.Application.Models;

public class Coupon
{
    public required string Id { get; set; }
    public required double DiscountPercent { get; set; }
    public required DateTime ExpiresAt { get; set; }
}