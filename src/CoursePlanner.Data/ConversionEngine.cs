using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Dapper;

namespace CoursePlanner.Data
{
    public class ConversionEngine
    {
        private const string ConnectionString = "Data Source=CourseInformation.sqlite;";

        /// <p>Loads all courses and their requisites from SQLite into a searchable dictionary for the team.</p>
        public Dictionary<string, Course> LoadCoursesIntoDictionary()
        {
            // SQL query joining Courses with CourseRequisites and CourseRequisiteDetails based on SQLite schema
            string sql = @"
                SELECT 
                    c.Id, c.SubjectCode, c.Number, c.MinimumCredits, c.Title, c.Description,
                    cr.CourseId, cr.SortOrder, cr.RequirementCode, cr.IsRequired, cr.CompletionOrder,
                    crd.RequirementCode, crd.Title, crd.Enforcement, crd.Timing, crd.PrintableText, crd.RequirementSyntax
                FROM Courses c
                LEFT JOIN CourseRequisites cr ON c.Id = cr.CourseId
                LEFT JOIN CourseRequisiteDetails crd ON cr.RequirementCode = crd.RequirementCode;";

            var courseDict = new Dictionary<string, Course>();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                connection.Query<CourseRawDto, CourseRequisiteRawDto, CourseRequisiteDetailRawDto, object>(
                    sql,
                    (courseDto, reqDto, detailDto) =>
                    {
                        string courseKey = courseDto.Id;

                        // Group unique courses
                        if (!courseDict.TryGetValue(courseKey, out var course))
                        {
                            course = new Course
                            {
                                Id = courseDto.Id,
                                CourseCode = $"{courseDto.SubjectCode} {courseDto.Number}",
                                Title = courseDto.Title,
                                Credits = (int)(courseDto.MinimumCredits ?? 0),
                                Requisites = new List<Requisite>()
                            };
                            courseDict.Add(courseKey, course);
                        }

                        // Group unique requisites
                        if (reqDto != null && !string.IsNullOrEmpty(reqDto.RequirementCode))
                        {
                            var requisite = course.Requisites.FirstOrDefault(r => r.Type == reqDto.RequirementCode);
                            if (requisite == null)
                            {
                                requisite = new Requisite
                                {
                                    CourseId = reqDto.CourseId,
                                    Type = reqDto.RequirementCode,
                                    Details = new List<RequisiteDetail>()
                                };
                                course.Requisites.Add(requisite);
                            }

                            // Attach Requisite Details
                            if (detailDto != null && detailDto.RequirementCode == reqDto.RequirementCode)
                            {
                                bool detailExists = requisite.Details.Any(d => d.RequiredCourseCode == detailDto.Title && d.MinimumGrade == detailDto.Enforcement);
                                if (!detailExists)
                                {
                                    requisite.Details.Add(new RequisiteDetail
                                    {
                                        RequirementCode = detailDto.RequirementCode,
                                        RequiredCourseCode = detailDto.Title,
                                        MinimumGrade = detailDto.Enforcement
                                    });
                                }
                            }
                        }

                        return null;
                    },
                    splitOn: "CourseId,RequirementCode"
                );
            }

            return courseDict;
        }
    }

    // --- Internal DTOs matching SQLite Column Types ---
    internal class CourseRawDto
    {
        public string Id { get; set; }
        public string SubjectCode { get; set; }
        public string Number { get; set; }
        public decimal? MinimumCredits { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    internal class CourseRequisiteRawDto
    {
        public string CourseId { get; set; }
        public int SortOrder { get; set; }
        public string RequirementCode { get; set; }
        public decimal? IsRequired { get; set; }
        public string CompletionOrder { get; set; }
    }

    internal class CourseRequisiteDetailRawDto
    {
        public string RequirementCode { get; set; }
        public string Title { get; set; }
        public string Enforcement { get; set; }
        public string Timing { get; set; }
        public string PrintableText { get; set; }
        public string RequirementSyntax { get; set; }
    }
}