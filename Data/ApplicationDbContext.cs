using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Theatre> Theatres => Set<Theatre>();
    public DbSet<Screen> Screens => Set<Screen>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Show> Shows => Set<Show>();
    public DbSet<ShowSeat> ShowSeats => Set<ShowSeat>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(15);
            entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(20);
        });

        // Movie
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Language).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Genre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ReleaseDate).HasColumnType("date");
        });

        // Theatre
        modelBuilder.Entity<Theatre>(entity =>
        {
            entity.HasKey(e => e.TheatreId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
        });

        // Screen
        modelBuilder.Entity<Screen>(entity =>
        {
            entity.HasKey(e => e.ScreenId);
            entity.Property(e => e.ScreenName).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Theatre)
                  .WithMany(t => t.Screens)
                  .HasForeignKey(e => e.TheatreId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TheatreId, e.ScreenName }).IsUnique();
        });

        // Seat
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId);
            entity.Property(e => e.RowName).IsRequired().HasMaxLength(5);
            entity.Property(e => e.SeatType).IsRequired().HasMaxLength(30);
            entity.HasOne(e => e.Screen)
                  .WithMany(s => s.Seats)
                  .HasForeignKey(e => e.ScreenId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.ScreenId, e.RowName, e.SeatNumber }).IsUnique();
        });

        // Show
        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(e => e.ShowId);
            entity.Property(e => e.TicketPrice).HasPrecision(10, 2);
            entity.HasOne(e => e.Movie)
                  .WithMany(m => m.Shows)
                  .HasForeignKey(e => e.MovieId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Screen)
                  .WithMany(s => s.Shows)
                  .HasForeignKey(e => e.ScreenId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ShowSeat
        modelBuilder.Entity<ShowSeat>(entity =>
        {
            entity.HasKey(e => e.ShowSeatId);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(e => e.Show)
                  .WithMany(s => s.ShowSeats)
                  .HasForeignKey(e => e.ShowId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Seat)
                  .WithMany(s => s.ShowSeats)
                  .HasForeignKey(e => e.SeatId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.ShowId, e.SeatId }).IsUnique();
        });

        // Booking
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId);
            entity.Property(e => e.BookingReference).IsRequired().HasMaxLength(30);
            entity.HasIndex(e => e.BookingReference).IsUnique();
            entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(150);
            entity.Property(e => e.CustomerPhone).IsRequired().HasMaxLength(15);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(e => e.Show)
                  .WithMany(s => s.Bookings)
                  .HasForeignKey(e => e.ShowId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // BookingSeat
        modelBuilder.Entity<BookingSeat>(entity =>
        {
            entity.HasKey(e => e.BookingSeatId);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.HasOne(e => e.Booking)
                  .WithMany(b => b.BookingSeats)
                  .HasForeignKey(e => e.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Seat)
                  .WithMany(s => s.BookingSeats)
                  .HasForeignKey(e => e.SeatId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}