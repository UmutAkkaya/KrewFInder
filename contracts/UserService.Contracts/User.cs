using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseLayer.Models;

namespace UserService.Contracts
{
    public interface IUserService
    {

        Task<User> GetUser(String userId, String targetUser);
        Task<User> CreateUser(String firstName, String lastName, String email, String password);
        Task<bool> DeleteUser(String userId, String targetUserId);

        // Null or empty keeps the field in its previous state
        Task<bool> ChangePersonalInfo(String userId, String firstName, String lastName);
        Task<bool> ChangeEmail(String userId, String email);
        Task<bool> ChangePassword(String userId, String oldPassword, String newPassword);

        Task<bool> AddGlobalRole(String userId, String targetUserId, GlobalRole role);
        Task<bool> RemoveGlobalRole(String userId, String targetUserId, GlobalRole role);

        Task<bool> LeaveGroup(String userId, String groupId);
        Task<List<Group>> GetGroups(String userId);

        Task<bool> JoinCourse(String userId, String courseId);
        Task<bool> LeaveCourse(String userId, String courseId);
        Task<List<Course>> GetCourses(String userId);

        Task<bool> AddGroupInvitation(String userId, String groupId);
        Task<bool> AcceptGroupInvitation(String userId, String groupId);
        Task<bool> RejectGroupInvitation(String userId, String groupId);

    }
}
