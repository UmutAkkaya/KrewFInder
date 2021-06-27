using System;
using System.Threading.Tasks;
using DatabaseLayer.Models;
using Newtonsoft.Json.Linq;

namespace CourseService.Contracts
{
    public interface ICourseService
    {
        Task<Course> GetCourse(String courseId);
        Task<Course> CreateCourse(string[] email, string name, string description, string startDate, string endDate, int? minGroupSize, int? maxGroupSize, JToken[] skills);

        // These can be performed by instructors
        Task<bool> ModifyGroupSize(String courseId, int? minSize, int? maxSize);
        Task<bool> ModifyStartEndDates(String courseId, String startDate, String endDate);
        Task<bool> ModifyDescription(String courseId, String description);

        // chown can be done by owner only
        Task<bool> ChangeOwner(String courseId, String newOwnerId);
        Task<bool> AddInstructor(String courseId, String instructorId);
        Task<bool> RemoveInstructor(String courseId, String instructorId);

        Task<Group> AddGroup(String courseId, String name, String bio);

    }
}
