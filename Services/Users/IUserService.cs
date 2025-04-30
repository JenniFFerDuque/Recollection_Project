using Data.Models;
using Services.Recolections.cs.DTOs;


namespace Services.Users
{
    public interface IUserService
    {
        Task<User?> CreateUser(User user);
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> UpdateUser(User existingUser, User updatedUser); // Updated method signature  
        Task<bool> DeleteUser(int id);
    }
}
