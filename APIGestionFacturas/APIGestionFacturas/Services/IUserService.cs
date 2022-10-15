using APIGestionFacturas.DataAccess;
using GestionFacturasModelo.Model.DataModel;
using GestionFacturasModelo.Model.Templates;
using System.Security.Claims;

namespace APIGestionFacturas.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Get all the users in the database
        /// </summary>
        /// <returns> IEnumerable of all the users </returns>
        public Task<IEnumerable<User>> GetUsers();
        /// <summary>
        /// Get the user selected by id
        /// </summary>
        /// <param name="id"> Id of the user </param>
        /// <returns> Task with a promise of the user if founded</returns>
        public Task<User> GetUser(int id);
        /// <summary>
        /// Function to get all the enterprises the user is allowed to read.
        /// </summary>
        /// <param name="id"> Id of the user making the call. </param>
        /// <returns> IEnumerable with all the enterprises the user is allowed to read. </returns>
        IEnumerable<Enterprise> GetUserEnterprises(int id);
        /// <summary>
        /// Function to control user login, if succesfull obtain the data of a logued user.
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns> User obtained from the login data, if something fails return null. </returns>
        User? GetUserLogin(UserAuthorization userLogin);
        public Task<User> RegisterUser(UserAuthorization userData);
        /// <summary>
        /// Function to check if a user exists.
        /// </summary>
        /// <param name="userLogin"> Data of the user to look for. </param>
        /// <returns> Boolean that indicates if the user exists. </returns>
        Boolean UserExists(UserAuthorization userLogin);
        /// <summary>
        /// Function to check if a user exists.
        /// </summary>
        /// <param name="user"> Data of the user to look for. </param>
        /// <returns> Boolean that indicates if the user exists. </returns>
        Boolean UserExists(User user);

        /// <summary>
        /// Function to create a User class inside a database.
        /// </summary>
        /// <param name="userData"> Data to create a new user. </param>
        /// <returns> A Task returns the created user. </returns>
        Task<User> CreateUser(UserEditable userData);
        /// <summary>
        /// Function to edit the data from a User class inside a database.
        /// </summary>
        /// <param name="userData"> Data to edit the selected user. </param>
        /// <param name="userId"> Id asociated to the selected user. </param>
        /// <returns> A Task returns the updated user. </returns>
        Task<User> EditUser(UserEditable userData, int userId);
        /// <summary>
        /// Function to delete a User class inside a database.
        /// </summary>
        /// <param name="userId"> Id asociated to the selected user. </param>
        /// <returns> A Task returns the deleted user. </returns>
        Task<User> DeleteUser(int userId);
    }
}
