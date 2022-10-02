namespace GestionFacturasModelo.Model.DataModel
{
    public class JwtSettings
    {
        public bool ValidateIssuerSigninKey { get; set; }
        public String IssuerSigningKey { get; set; } = String.Empty;
        public bool ValidateIssuer { get; set; }
        public String? ValidIssuer { get; set; }

        public bool ValidateAudience { get; set; }
        public String? ValidAudience { get; set; }

        public bool RequireExpirationTime { get; set; }
        public bool ValidateLifetime { get; set; } = true;
    }
}
