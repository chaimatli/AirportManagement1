using AM.ApplicationCore.Domain;
using AM.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AM.Infrastructure
{
    public class AMContext:DbContext
    {
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Plane> Planes { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Traveller> Travellers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<ReservationTicket> ReservationTickets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\mssqllocaldb;
                       Initial Catalog=4SE;
                       Integrated Security=true;MultipleActiveResultSets=true");
                optionsBuilder.UseLazyLoadingProxies();

            base.OnConfiguring(optionsBuilder);
        }
        // Application de fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //1ère methode de fluent API
            modelBuilder.ApplyConfiguration(new FlightConfiguration());
            modelBuilder.ApplyConfiguration(new PlaneConfiguration());

            modelBuilder.ApplyConfiguration(new ReservationTicketConfiguration());

            //2 ème meth de fluentAPI
            modelBuilder.Entity<Passenger>()
                // config du type complexe / détenu 
                .OwnsOne(p => p.FullName, full =>
            {
                full.Property(f => f.FirstName).HasColumnName("PassFirstName").HasMaxLength(30);
                full.Property(f => f.LastName).HasColumnName("PassLastName").IsRequired();
            });
            

            // config de l'héritage :TPH
            //.HasDiscriminator<int>("PassengerType")
            //.HasValue<Passenger>(0) // je vais affecter la valeur 0 => Passenger
            //.HasValue<Staff>(1)
            //.HasValue<Traveller>(2);

             
            // config de TPT : pour chaque entité => table 
            modelBuilder.Entity<Traveller>().ToTable("Travellers");
            modelBuilder.Entity<Staff>().ToTable("Staffs");
            //fluentapi fk
            //modelBuilder.Entity<ReservationTicket>()
            //    .HasOne(p => p.Passenger)
            //    .WithMany(p => p.Reservations)
            //    .HasForeignKey(p => p.PassengerFk);
            //modelBuilder.Entity<Ticket>()
            //    .HasMany(t => t.Reservations)
            //    .WithOne(t => t.Ticket)
            //    .HasForeignKey(t => t.TicketFk);
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
             .Properties<DateTime>()
                 .HaveColumnType("date");
        }
    }
}
