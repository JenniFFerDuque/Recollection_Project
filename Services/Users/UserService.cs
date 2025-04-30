using System.Text.RegularExpressions;
using Data.DBConext;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Services.Recolections.cs.DTOs;

namespace Services.Users
{
    public class UserService : IUserService
    {
        private readonly RecollectionProjectContext _context;
        private const int PasswordMinLenght = 8;
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled
);

        public UserService(RecollectionProjectContext context)
        {
            _context = context;
        }

        //CREATE
        public async Task<User?> CreateUser(User user)
        {
            //Validaciones básicas
            if (!ValidateUserData(user))
            {
                return null;
            }

            //Validaciones de bases de datos
            if (!ValidateUserUniqueness(user).Result)
            {
                return null;
            }

            //Hash de contraseña
            user.Password = HashPassword(user.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return await GetUserById(user.UserId);
        }

        //READ
        public async Task<User> GetUserById(int id)
        {
            return await _context.Users
                .Include(u => u.Recolections)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<IEnumerable<User>> GetAllUsers() //Obtener todos los usuarios
        {
            return await _context.Users.
                Include(u => u.Recolections)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<User?> UpdateUser(User existingUser, User updatedUser)
        {
            // Validate existing user
            var userInDb = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == existingUser.UserId);

            if (userInDb == null)
            {
                return null;
            }

            // Validate and update email
            if (!string.IsNullOrWhiteSpace(updatedUser.Email))
            {
                bool isEmailTaken = await _context.Users
                    .AnyAsync(u => u.Email == updatedUser.Email && u.UserId != userInDb.UserId);

                if (isEmailTaken)
                {
                    return null;
                }
                userInDb.Email = updatedUser.Email;
            }

            // Validate and update password
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                if (!ValidatePassword(updatedUser.Password))
                {
                    return null;
                }
                userInDb.Password = HashPassword(updatedUser.Password);
            }

            // Save changes if there are modifications
            if (_context.Entry(userInDb).State == EntityState.Modified)
            {
                await _context.SaveChangesAsync();
                return userInDb;
            }

            return null;
        }

        //DELETE
        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Recolections)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return false;
            }

            if (user.Recolections?.Any() == true)
            {
                return false; // No se puede eliminar el usuario si tiene recolecciones asociadas
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private const int PasswordMinLength = 8;

        //Método para hacer las validaciones básicas
        private bool ValidateUserData(User user)
        {
            if (user == null)
            {
                return false;
            }
                

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return false; 
            }

            if (user.Password.Length < PasswordMinLength)
            {
                return false;
            }


            if (string.IsNullOrWhiteSpace(user.Email) || !EmailRegex.IsMatch(user.Email))
            {
                return false;
            }

            if (user.Email.Length > 255)
            {
                return false;
            }

            if (user.Password.Length > 255)
            {
                return false;
            }

            return true;
        }

        //Método para verificar datos en DB
        private async Task<bool> ValidateUserUniqueness(User user)
        {
            // Validar unicidad del email si está presente
            if (!string.IsNullOrWhiteSpace(user.Email) &&
                await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return false;
            }
            return true;
            
        }

        //Método para encriptar la contraseña
        private string HashPassword(string password)
        {
            // Implementación básica - en producción usaría BCrypt o similar
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }


        //Método para validar la contraseña
        private bool ValidatePassword(string password)
        {
            // Ejemplo: Mínimo 8 caracteres, al menos 1 mayúscula y 1 número
            var passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d).{8,}$");
            return passwordRegex.IsMatch(password);
        }
    }

}
