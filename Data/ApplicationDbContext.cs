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

        // Users

        // Users

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);

            entity.Property(u => u.FullName)
                  .HasColumnName("Name")
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(u => u.Email)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(u => u.PasswordHash)
                  .IsRequired();

            entity.Property(u => u.Phone)
                  .HasMaxLength(15);

            entity.Property(u => u.Role)
                  .HasMaxLength(20)
                  .IsRequired();

            entity.HasIndex(u => u.Email).IsUnique();
        });


        // Movies
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(m => m.MovieId);
            entity.Property(m => m.Title).IsRequired().HasMaxLength(150);
            entity.Property(m => m.Description).HasMaxLength(500);
            entity.Property(m => m.Language).IsRequired().HasMaxLength(50);
            entity.Property(m => m.Genre).IsRequired().HasMaxLength(50);
            entity.ToTable(t => t.HasCheckConstraint("CK_Movie_DurationMinutes", "[DurationMinutes] > 0"));
        });

        // Theatres
        modelBuilder.Entity<Theatre>(entity =>
        {
            entity.HasKey(t => t.TheatreId);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(150);
            entity.Property(t => t.Location).IsRequired().HasMaxLength(200);
        });

        // Screens
        modelBuilder.Entity<Screen>(entity =>
        {
            entity.HasKey(s => s.ScreenId);
            entity.Property(s => s.ScreenName).IsRequired().HasMaxLength(50);
            entity.HasIndex(s => new { s.TheatreId, s.ScreenName }).IsUnique();

            entity.HasOne(s => s.Theatre)
                  .WithMany(t => t.Screens)
                  .HasForeignKey(s => s.TheatreId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seats
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(s => s.SeatId);
            entity.Property(s => s.RowName).IsRequired().HasMaxLength(5);
            entity.Property(s => s.SeatNumber).IsRequired();
            entity.Property(s => s.SeatType).IsRequired().HasMaxLength(20);
            entity.HasIndex(s => new { s.ScreenId, s.RowName, s.SeatNumber }).IsUnique();

            entity.HasOne(s => s.Screen)
                  .WithMany(sc => sc.Seats)
                  .HasForeignKey(s => s.ScreenId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Shows
        modelBuilder.Entity<Show>(entity =>
        {
            entity.HasKey(s => s.ShowId);
            entity.Property(s => s.TicketPrice).HasColumnType("decimal(10,2)");
            entity.ToTable(t => t.HasCheckConstraint("CK_Show_TicketPrice", "[TicketPrice] > 0"));

            entity.HasOne(s => s.Movie)
                  .WithMany(m => m.Shows)
                  .HasForeignKey(s => s.MovieId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Screen)
                  .WithMany(sc => sc.Shows)
                  .HasForeignKey(s => s.ScreenId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ShowSeats
        modelBuilder.Entity<ShowSeat>(entity =>
        {
            entity.HasKey(ss => ss.ShowSeatId);
            entity.Property(ss => ss.Status).IsRequired().HasMaxLength(20);
            entity.HasIndex(ss => new { ss.ShowId, ss.SeatId }).IsUnique();

            entity.HasOne(ss => ss.Show)
                  .WithMany(s => s.ShowSeats)
                  .HasForeignKey(ss => ss.ShowId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ss => ss.Seat)
                  .WithMany(s => s.ShowSeats)
                  .HasForeignKey(ss => ss.SeatId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Bookings

        // Bookings
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(b => b.BookingId);

            entity.Property(b => b.BookingReference)
                  .IsRequired()
                  .HasMaxLength(30);

            entity.HasIndex(b => b.BookingReference).IsUnique();

            entity.Property(b => b.CustomerName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(b => b.CustomerEmail)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(b => b.CustomerPhone)
                  .IsRequired()
                  .HasMaxLength(15);

            entity.Property(b => b.TotalAmount)
                  .HasColumnType("decimal(10,2)");

            entity.Property(b => b.Status)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Booking_TotalAmount",
                    "[TotalAmount] >= 0"));

            entity.HasOne(b => b.Show)
                  .WithMany(s => s.Bookings)
                  .HasForeignKey(b => b.ShowId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(b => b.User)
                  .WithMany()
                  .HasForeignKey(b => b.UserId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);
        });


        // BookingSeats
        modelBuilder.Entity<BookingSeat>(entity =>
        {
            entity.HasKey(bs => bs.BookingSeatId);
            entity.Property(bs => bs.Price).HasColumnType("decimal(10,2)");
            entity.ToTable(t => t.HasCheckConstraint("CK_BookingSeat_Price", "[Price] >= 0"));

            entity.HasOne(bs => bs.Booking)
                  .WithMany(b => b.BookingSeats)
                  .HasForeignKey(bs => bs.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(bs => bs.Seat)
                  .WithMany(s => s.BookingSeats)
                  .HasForeignKey(bs => bs.SeatId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
