namespace OpenRequest.Configuration.Messages;

public static class ErrorMessages
{
    public static class Generic 
    {
        public static string InvalidPayload = "Invalid payload";
        public static string SomethingWentWrong = "Something went wrong, please try again later";   
    }

    public static class Type
    {
        public static string BadRequest = "Bad Request";
        public static string NotFound = "Not Found";
        public static string UnableToProcess = "Unable to process";
    }

    public static class Profile
    {
        public static string NotFound = "User not found";
    }

    public static class Users
    {
        public static string NotFound = "User not found";
    }

    public static class Category
    {
        public static string NotFound = "Category not found";
    }

    public static class Post
    {
        public static string NotFound = "Post not found";
        public static string InvalidAction = "Post status is not open";
    }
}