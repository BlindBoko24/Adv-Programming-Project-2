namespace CoursePlannerCore
{
    using System.Collections.Generic;

    public class Course
    {
        public int Id { get; set; }
        public string CourseCode { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        // List of requisites associated with the course, one Course can have multiple Requisites
        public List<Requisite> Requisites { get; set; } = new List<Requisite>();
    }

    public class Requisite
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Type { get; set; }

        // List of requisite details associated with the requisite, one Requisite can have multiple RequisiteDetails
        public List<RequisiteDetail> Details { get; set; } = new List<RequisiteDetail>();
    }

    public class RequisiteDetail
    {
        public int Id { get; set; }
        public int RequisiteId { get; set; }
        public string RequiredCourseCode { get; set; }
        public string MinimumGrade { get; set; }
    }
}
