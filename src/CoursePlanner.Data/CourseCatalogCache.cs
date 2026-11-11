using System;
using System.Collections.Generic;
using CoursePlanner.Data;

public sealed class CourseCatalogCache
{
    private static readonly Lazy<Dictionary<string, Course>> _lazyCatalog =
        new Lazy<Dictionary<string, Course>>(() =>
        {
            var engine = new ConversionEngine();
            // Built and populated strictly ONCE on first access
            return engine.LoadCoursesIntoDictionary();
        });

    /// <summary>
    /// Gets the globally shared, single instance of the course dictionary.
    /// </summary>
    public static Dictionary<string, Course> Instance => _lazyCatalog.Value;
}