using Microsoft.Data.Sqlite;
using Dapper;

namespace CoursePlannerCore
{
    public class CourseRepository
    {
        private const string ConnectionString = "Data Source=courses.sqlite;";

        public IEnumerable<Course> GetAllCoursesWithRequisites()
        {
            // Flattened SQL query fetching everything in one trip
            string sql = @"
            SELECT 
                c.Id, c.CourseCode, c.Title, c.Credits,
                r.Id, r.CourseId, r.Type,
                rd.Id, rd.RequisiteId, rd.RequiredCourseCode, rd.MinimumGrade
            FROM Courses c
            LEFT JOIN Requisites r ON c.Id = r.CourseId
            LEFT JOIN RequisiteDetails rd ON r.Id = rd.RequisiteId;";

            // Dictionary to track unique courses and requisites during mapping
            var courseDict = new Dictionary<int, Course>();
            var requisiteDict = new Dictionary<int, Requisite>();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                // Dapper's Multi-Mapping maps a single row to multiple objects
                connection.Query<Course, Requisite, RequisiteDetail, Course>(
                    sql,
                    (course, requisite, detail) =>
                    {
                        // Group / Find Unique Courses
                        if (!courseDict.TryGetValue(course.Id, out var currentCourse))
                        {
                            currentCourse = course;
                            courseDict.Add(currentCourse.Id, currentCourse);
                        }

                        // Group / Find Unique Requisites
                        if (requisite != null)
                        {
                            if (!requisiteDict.TryGetValue(requisite.Id, out var currentRequisite))
                            {
                                currentRequisite = requisite;
                                requisiteDict.Add(currentRequisite.Id, currentRequisite);
                                currentCourse.Requisites.Add(currentRequisite);
                            }

                            // Attach Details
                            if (detail != null)
                            {
                                currentRequisite.Details.Add(detail);
                            }
                        }

                        return currentCourse;
                    },
                    splitOn: "Id,Id" // Tells Dapper where the next object's properties begin in the SELECT statement
                );
            }

            return courseDict.Values;
        }
    }

}
