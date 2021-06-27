namespace AspCoreTools.Email
{
    public static class EmailTemplates
    {
        private static string _domain = "localhost:5000";
        public static void SetLinkDomain(string domain)
        {
            _domain = domain;
        }
        
        
        public static string CourseCreateSubject(string courseName) => $"You've been invited to {courseName}";

        public static string CourseCreateBody(string courseName, string courseId) =>
            $"Hello\n\nYou've been invited to <a href='http://{_domain}/organizations/{courseId}'>{courseName}</a> on KrewFindr.";

        public static string GroupInviteSubject(string groupName) => $"You've been invited to {groupName}";

        public static string GroupInviteBody(string groupName, string userName, string groupId) =>
            $"Hello {userName},\n\nYou've been invited to <a href='http://{_domain}/groups/{groupId}'>{groupName}</a> on KrewFindr.";

        public static string GroupMergerSubject(string recipGroupName, string otherGroupName) =>
            $"Your group {recipGroupName} is invited to merge with {otherGroupName}";

        public static string GroupMergerBody(string recipGroupName, string otherGroupName, string recipGroupId,
            string otherGroupId, string userName) =>
            $"Hello {userName},\n\nYour group <a href='http://{_domain}/groups/{recipGroupId}'>{recipGroupName}</a> " +
            $"has been invited to merge with " +
            $"<a href='http://{_domain}/groups/{otherGroupId}'>{otherGroupName}</a> on KrewFindr.";
    }
}