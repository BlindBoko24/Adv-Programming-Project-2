using CoursePlanner.Data;

public class Program
{
    public static void Main(string[] args)
    {
        Dictionary<string, Course> courseCatalogCache = CourseCatalogCache.Instance;

        Console.WriteLine("Courses and their prerequisites:");
        foreach (Course course in courseCatalogCache.Values)
        {
            Console.WriteLine($"Course: {course.Title}, Prerequisites: {string.Join(", ", course.Requisites.Select(r => r.Type))}");
        }
    }
}