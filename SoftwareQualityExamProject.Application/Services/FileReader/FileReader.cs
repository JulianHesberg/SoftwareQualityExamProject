namespace SoftwareQualityExamProject.Application.Services.FileReader;

public class FileReader : IFileReader
{
    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }
}