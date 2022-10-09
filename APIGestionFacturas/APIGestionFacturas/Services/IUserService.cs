using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Function to get all the enterprises the user is allowed to read.
        /// </summary>
        /// <param name="enterprises"> The data of the enterprises to search in. </param>
        /// <param name="id"> Id of the user making the call. </param>
        /// <returns> IEnumerable with all the enterprises the user is allowed to read. </returns>
        IEnumerable<Enterprise> GetUserEnterprises(IQueryable<Enterprise> enterprises, int id);
        /// <summary>
        /// Function to control user login, if succesfull obtain the data of a logued user.
        /// </summary>
        /// <param name="users"> The data of the users to search in. </param>
        /// <param name="userLogin"></param>
        /// <returns> User obtained from the login data, if something fails return null. </returns>
        User? GetUserLogin(IQueryable<User> users, UserAuthorization userLogin);
        /// <summary>
        /// Function to check if a user exists.
        /// </summary>
        /// <param name="users"> The data of the users to search in. </param>
        /// <param name="userLogin"> Data of the user to look for. </param>
        /// <returns> Boolean that indicates if the user exists. </returns>
        Boolean UserExists(IQueryable<User> users, UserAuthorization userLogin);
        /// <summary>
        /// Function to check if a user exists.
        /// </summary>
        /// <param name="users"> The data of the users to search in. </param>
        /// <param name="user"> Data of the user to look for. </param>
        /// <returns> Boolean that indicates if the user exists. </returns>
        Boolean UserExists(IQueryable<User> users, User user);

        /// <summary>
        /// Function to create a User class inside a database.
        /// </summary>
        /// <param name="_context"> Stores the invoices and it's related data. </param>
        /// <param name="userClaims"> The asociated data of the user making the call.</param>
        /// <param name="userData"> Data to create a new user. </param>
        /// <returns> A Task returns the created user. </returns>
        Task<User> CreateUser(GestionFacturasContext _context,
                                    ClaimsPrincipal userClaims,
                                    UserEditable userData);
        /// <summary>
        /// Function to edit the data from a User class inside a database.
        /// </summary>
        /// <param name="_context"> Stores the invoices and it's related data. </param>
        /// <param name="userClaims"> The asociated data of the user making the call.</param>
        /// <param name="userData"> Data to edit the selected user. </param>
        /// <param name="userId"> Id asociated to the selected user. </param>
        /// <returns> A Task returns the updated user. </returns>
        Task<User> EditUser(GestionFacturasContext _context,
                                  ClaimsPrincipal userClaims,
                                  UserEditable userData,
                                  int userId);
        /// <summary>
        /// Function to delete a User class inside a database.
        /// </summary>
        /// <param name="_context"> Stores the invoices and it's related data. </param>
        /// <param name="userClaims"> The asociated data of the user making the call.</param>
        /// <param name="userId"> Id asociated to the selected user. </param>
        /// <returns> A Task returns the deleted user. </returns>
        Task<User> DeleteUser(GestionFacturasContext _context,
                                    ClaimsPrincipal userClaims,
                                    int userId);
    }
}
