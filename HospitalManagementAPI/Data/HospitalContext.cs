using HospitalManagementAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementAPI.Data;

public class HospitalContext(DbContextOptions<HospitalContext> options) : DbContext(options)
{
    public DbSet<Department>Departments=>Set<Department>();
    public DbSet<Appointment>Appointments=>Set<Appointment>();
    public DbSet<Doctor>Doctors=>Set<Doctor>();
    public DbSet<Patient>Patients=>Set<Patient>();
    public DbSet<User> Users => Set<User>();
}