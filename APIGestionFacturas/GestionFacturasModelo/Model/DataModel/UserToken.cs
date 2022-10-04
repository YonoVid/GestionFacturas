namespace GestionFacturasModelo.Model.DataModel
{
    public class UserToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = String.Empty;
        public string UserName { get; set; } = String.Empty;
        public UserRol UserRol{ get; set; } = UserRol.USER;
        public TimeSpan? Validity { get; set; }
        public String RefreshToken { get; set; } = String.Empty;
        public String EmailId { get; set; } = String.Empty;
        public Guid GuidId { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
