using AspNetCore.Identity.CosmosDb;
using GRC.Core.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GRC.Infrastructure.Data;

public class IdentityCosmosDbContext : CosmosIdentityDbContext<User, IdentityRole, string>
{
    public IdentityCosmosDbContext(DbContextOptions<IdentityCosmosDbContext> options)
        : base(options)
    {
    }
}