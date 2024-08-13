namespace THNeonMirage.Data
{
    public class Authorization
    {
        public enum Role
        {
            Forbidden,
            User,
            Administrator,
            Proprietor
        }
        
        public enum Status
        {
            RegisterSuccess,
            LoginSuccess,
            DuplicateUser,
            UserNonExist,
            PasswordError,
            ConnectionError,
            Failed
        }
    }
}