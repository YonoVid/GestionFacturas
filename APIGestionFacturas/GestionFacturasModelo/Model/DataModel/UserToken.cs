namespace GestionFacturasModelo.Model.DataModel
{
    public class UserToken
    {
        public int Id { get; set; }                                 // Id of the user
        public string Token { get; set; } = String.Empty;           // JWT token of the user
        public string UserName { get; set; } = String.Empty;        // Name of the user
        public UserRol UserRol{ get; set; } = UserRol.USER;         // User rol (enum)
        public TimeSpan? Validity { get; set; }                     // Time span the token is valid
        public String RefreshToken { get; set; } = String.Empty;    //
        public String EmailId { get; set; } = String.Empty;         // User email
        public Guid GuidId { get; set; }                            // Guid of the token
        public DateTime ExpireTime { get; set; }                    // Time the token expire
    }
}
