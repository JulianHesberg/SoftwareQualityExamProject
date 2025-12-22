namespace SoftwareQualityExamProject.Application.Exceptions;

public class ExpiredCouponException : Exception
{
    public  ExpiredCouponException(string message) : base(message)
    {
    }
    
    public  ExpiredCouponException(string message, Exception innerException) : base(message, innerException)
    {
    }
}