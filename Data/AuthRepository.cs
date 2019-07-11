using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList2.API.Models;

namespace TodoList2.API.Data
{
    public class AuthRepository: IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Register(User user, string password)
        {
            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.CreatedAtDateTime = DateTime.Now;
            user.LastActiveAtDateTime = DateTime.Now;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null) { return null; }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) { return null; }
            else if (VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                user.LastActiveAtDateTime = DateTime.Now;
                await _context.SaveChangesAsync();
                return user;
            }
            else { return null; }
        }

        public async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                if (computedHash.Length == passwordHash.Length)
                {
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != passwordHash[i]) { return false; }
                    }
                }
                else { return false; }
            }
            return true;
        }
    }
}
