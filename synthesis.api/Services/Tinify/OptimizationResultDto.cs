namespace synthesis.api.Services.Tinify;

public class OptimizationResultDto
{
    public ImageInfo Input { get; set; }
    public OptimizedImageInfo Output { get; set; }
}

public class ImageInfo
{
    public int Size { get; set; }
    public string Type { get; set; }
}

public class OptimizedImageInfo
{
    public int Size { get; set; }
    public string Type { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double Ratio { get; set; }
    public string Url { get; set; }
}