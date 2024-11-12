using Microsoft.EntityFrameworkCore;
using SmallBearSchool.Models;

public class AccessDbContext : DbContext
{
    public DbSet<UserAccount> UserAccount { get; set; }
    public DbSet<Subject> Subject { get; set; }
    public DbSet<SubjectAnswer> SubjectAnswer { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Use the path to your Access database file here
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=..\\SmallBearSchool\\SmallBearSchoolDB.accdb;";
        optionsBuilder.UseJet(connectionString);
    }
}
