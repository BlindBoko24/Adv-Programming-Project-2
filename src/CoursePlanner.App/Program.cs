using CoursePlanner.Data;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Courses and their prerequisites:");
        foreach (Course course in CourseCatalogCache.Instance.Values)
        {
            Console.WriteLine($"Course: {course.Title}, Prerequisites: {string.Join(", ", course.Requisites.Select(r => r.Type))}");
        }
    }
}