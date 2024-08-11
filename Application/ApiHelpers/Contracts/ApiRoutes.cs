namespace Application.ApiHelpers.Contracts;

/// <summary>
/// Contains the API endpoint routes.
/// </summary>
public static class ApiRoutes
{
    
    /// <summary>
    /// Contains the product routes.
    /// </summary>
    public static class Product
    {
        public const string Create = "create-product";


        public const string GetAuthorTasksByIsDone = "get-authror_tasks-by-is_done";
        
        public const string GetTaskById = "get-taskEntity/{taskId:guid}";
        
        public const string Update = "update-taskEntity/{taskId:guid}";
    }
    
    /// <summary>
    /// Contains the message routes.
    /// </summary>
    public static class Message
    {
        public const string Create = "create-message";
    }
    
    /// <summary>
    /// Contains the authentication routes.
    /// </summary>
    public static class Authentication
    {
        public const string Login = "login";

        public const string Register = "register";
    }

    /// <summary>
    /// Contains the attendee routes.
    /// </summary>
    public static class Attendees
    {
        public const string Get = "attendees";
    }

    /// <summary>
    /// Contains the group events routes.
    /// </summary>
    public static class GroupEvents
    {
        public const string Get = "group-events";

        public const string GetById = "group-events/{groupEventId:guid}";

        public const string GetMostRecentAttending = "group-events/most-recent-attending";
            
        public const string Create = "group-events";

        public const string Update = "group-events/{groupEventId:guid}";

        public const string Cancel = "group-events/{groupEventId:guid}";

        public const string InviteFriend = "group-events/{groupEventId:guid}/invite";
    }

    /// <summary>
    /// Contains the group invitations routes.
    /// </summary>
    public static class Invitations
    {
        public const string GetById = "invitations/{invitationId:guid}";

        public const string GetPending = "invitations/pending";

        public const string GetSent = "invitations/sent";

        public const string Accept = "invitations/{invitationId:guid}/accept";

        public const string Reject = "invitations/{invitationId:guid}/reject";
    }

    /// <summary>
    /// Contains the personal events routes.
    /// </summary>
    public static class PersonalEvents
    {
        public const string Get = "personal-events";

        public const string GetById = "personal-events/{personalEventId:guid}";

        public const string Create = "personal-events";

        public const string Update = "personal-events/{personalEventId:guid}";

        public const string Cancel = "personal-events/{personalEventId:guid}";
    }

    /// <summary>
    /// Contains the posts routes.
    /// </summary>
    public static class Posts
    {
        public const string CreatePost = "create-post";
        
        public const string UpdatePost = "update-post/{postId:guid}";
    }

    /// <summary>
    /// Contains the users routes.
    /// </summary>
    public static class Users
    {
        public const string Login = "users/login";
        
        public const string Register = "users/register";
        
        public const string GetById = "users/{userId:guid}";

        public const string Update = "users/{userId:guid}";

        public const string ChangePassword = "users/change-password";
        
        public const string ChangeName = "users/change-name";
    }
}