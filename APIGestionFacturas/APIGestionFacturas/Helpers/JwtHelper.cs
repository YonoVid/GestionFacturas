using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.Helpers
{
    public static class JwtHelpers
    {
        public static IEnumerable<Claim> GetClaims(this UserToken userAccounts, Guid id)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("Id", userAccounts.Id.ToString()),
                new Claim(ClaimTypes.Name, userAccounts.UserName),
                new Claim(ClaimTypes.Email, userAccounts.EmailId),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddDays(1).ToString("MMM ddd dd yyyy HH:mm:ss tt"))

            };

            if (userAccounts.UserRol == UserRol.ADMINISTRATOR)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
                claims.Add(new Claim("UserOnly", "Admin"));
            }
            else if (userAccounts.UserRol == UserRol.USER)
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
                claims.Add(new Claim("UserOnly", "User"));

            }

            return claims;
        }

        public static IEnumerable<Claim> GetClaims(this UserToken userAccounts, out Guid Id)
        {
            Id = new Guid();
            return GetClaims(userAccounts, Id);
        }

        public static UserToken GenTokenKey(UserToken model, JwtSettings jwtSettings)
        {
            try
            {
                var userToken = new UserToken();
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                //Generación de clave secreta
                var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);

                Guid Id;
                //Expira en un día
                DateTime expireTime = DateTime.UtcNow.AddDays(1);
                //Indicar validéz del Token
                userToken.Validity = expireTime.TimeOfDay;

                //Generar JWT
                var jwtToken = new JwtSecurityToken(
                        issuer: jwtSettings.ValidIssuer,
                        audience: jwtSettings.ValidAudience,
                        claims: GetClaims(model, out Id),
                        notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                        expires: new DateTimeOffset(expireTime).DateTime,
                        signingCredentials: new SigningCredentials(
                                new SymmetricSecurityKey(key),
                                SecurityAlgorithms.HmacSha256
                            )
                    );

                userToken.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                userToken.UserName = model.UserName;
                userToken.Id = model.Id;
                userToken.UserRol = model.UserRol;
                userToken.GuidId = Id;

                return userToken;
            }
            catch (Exception exception)
            {
                throw new Exception("Error en generación de JWT", exception);
            }
        }
    }
}
