namespace CoursePlanner.Data
{
    using System.Collections.Generic;

    public class Course
    {
        public string Id { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public List<Requisite> Requisites { get; set; } = new List<Requisite>();
    }

    public class Requisite
    {
        public string CourseId { get; set; }
        public string Type { get; set; }
        public List<RequisiteDetail> Details { get; set; } = new List<RequisiteDetail>();
    }

    public class RequisiteDetail
    {
        public string RequirementCode { get; set; }
        public string RequiredCourseCode { get; set; }
        public string MinimumGrade { get; set; }
    }
}