namespace THNeonMirage.Data
{
    public class Authorization
    {
        public Role UserRole { get; set; }
        public ConnectionStatus Status { get; }
        public PlayerData PlayerData { get; private set; }

        public Authorization(Role userRole, ConnectionStatus status)
        {
            UserRole = userRole;
            Status = status;
        }

        public Authorization SetData(PlayerData playerData)
        {
            PlayerData = playerData;
            return this;
        }
        
        public Authorization SetData(string userName, int position)
        {
            PlayerData = new PlayerData(userName, position);
            return this;
        }

        public enum Role
        {
            Forbidden,
            User,
            Administrator,
            Proprietor
        }
        
        public enum ConnectionStatus
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