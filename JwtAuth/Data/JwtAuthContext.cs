using JwtAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Data;

public class JwtAuthContext(DbContextOptions<JwtAuthContext> options) : DbContext(options)
{
    public DbSet<User>Users=>Set<User>();
}