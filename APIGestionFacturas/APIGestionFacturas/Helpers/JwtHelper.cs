using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GestionFacturasModelo.Model.DataModel;

namespace APIGestionFacturas.Helpers
{
    public static class JwtHelpers
    {
        /// <summary>
        /// Generate all claims from the User token data.
        /// </summary>
        /// <param name="userAccounts"> Data from the user token. </param>
        /// <param name="id"> Guid asociated to token. </param>
        /// <returns> IEnumeral with all claims from User token data. </returns>
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

        /// <summary>
        /// Get user claims from User Token data and generate new Guid.
        /// </summary>
        /// <param name="userAccounts"> Data from the user token. </param>
        /// <param name="Id"> Variable to save new Guid. </param>
        /// <returns> IEnumeral with all the configured claims. </returns>
        public static IEnumerable<Claim> GetClaims(this UserToken userAccounts, out Guid Id)
        {
            Id = Guid.NewGuid();
            return GetClaims(userAccounts, Id);
        }


        /// <summary>
        /// Generates a JWT token from a base model with user data and selected
        /// JWT settings.
        /// </summary>
        /// <param name="model"> Base data with user information already added. </param>
        /// <param name="jwtSettings"> Configuration to create JWT token. </param>
        /// <returns> UserToken with all user data and JWT token </returns>
        /// <exception cref="ArgumentNullException"> Base model used is null. </exception>
        /// <exception cref="Exception"> Error happend in token generation. </exception>
        public static UserToken GenTokenKey(UserToken model, JwtSettings jwtSettings)
        {
            try
            {
                var userToken = new UserToken();
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                // Generate secret key
                var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings.IssuerSigningKey);

                Guid Id;
                // Set expiration time to one day
                DateTime expireTime = DateTime.UtcNow.AddDays(1);
                // Set token validity
                userToken.Validity = expireTime.TimeOfDay;

                // Generate JWT
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

                userToken.Id = model.Id;
                userToken.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
                userToken.UserName = model.UserName;
                userToken.UserRol = model.UserRol;
                userToken.EmailId = model.EmailId;
                userToken.GuidId = Id;
                userToken.ExpireTime = expireTime;

                return userToken;
            }
            catch (Exception exception)
            {
                throw new Exception("Error en generación de JWT", exception);
            }
        }
    }
}
