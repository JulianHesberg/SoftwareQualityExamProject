namespace SoftwareQualityExamProject.Application.Exceptions;

public class JsonErrorException : Exception
{
    public  JsonErrorException(string message) : base(message){}
    
    public JsonErrorException(string message, Exception innerException) : base(message, innerException) { }
}