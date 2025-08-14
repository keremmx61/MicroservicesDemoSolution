using UserService.Models;
using UserService.Data;
using System.Collections.Generic;
using System.Linq;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _context;

        public UserService(UserDbContext context)
        {
            _context = context;
        }

        public List<User> GetAll() => _context.Users.ToList();

        public User GetById(int id) => _context.Users.Find(id);

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            var existing = _context.Users.Find(user.Id);
            if (existing != null)
            {
                existing.Name = user.Name;
                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}
