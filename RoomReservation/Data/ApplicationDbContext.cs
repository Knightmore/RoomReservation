#region Copyright © 2022 Patrick Borger - https: //github.com/Knightmore

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Author: Patrick Borger
// GitHub: https://github.com/Knightmore
// Created: 07.10.2022
// Modified: 07.11.2022

#endregion

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RoomReservation.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; }

    public DbSet<Seat> Seats { get; set; }

    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PasswordHasher<AppUser> hasher = new();
        base.OnModelCreating(modelBuilder);
        var adminRole = new IdentityRole { Id = "d83cc7ab-bb04-44aa-bf91-1e17c1b45fcb", Name = "Admin", NormalizedName = "ADMIN" };

        var memberRole = new IdentityRole { Id = "0941aa3e-2d65-4f57-ab42-3aab9678f00e", Name = "Member", NormalizedName = "MEMBER" };

        var admin = new AppUser
                    {
                        Id                   = "2f697355-9779-47ee-b278-c97cbee59020", // primary key
                        UserName             = "Admin, Super",
                        NormalizedUserName   = "ADMIN, SUPER",
                        FirstName            = "Super",
                        LastName             = "Admin",
                        Role                 = adminRole.Id,
                        Email                = "Super@Admin.de",
                        NormalizedEmail      = "SUPER@ADMIN.DE",
                        EmailConfirmed       = true,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled     = false,
                        LockoutEnabled       = false,
                        AccessFailedCount    = 0,
                        ICalGuid             = Guid.NewGuid().ToString()
                    };

        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

        var member = new AppUser
                     {
                         Id                   = "5532c45c-5f99-4e10-a214-8e59abc7c28d", // primary key
                         UserName             = "Member, Default",
                         NormalizedUserName   = "MEMBER, Default",
                         FirstName            = "Default",
                         LastName             = "Member",
                         Role                 = memberRole.Id,
                         Email                = "Default@Member.de",
                         NormalizedEmail      = "DEFAULT@MEMBER.DE",
                         EmailConfirmed       = false,
                         PhoneNumberConfirmed = false,
                         TwoFactorEnabled     = false,
                         LockoutEnabled       = true,
                         AccessFailedCount    = 0,
                         ICalGuid             = Guid.NewGuid().ToString()
                     };

        member.PasswordHash = hasher.HashPassword(member, "Member123!");

        var dummy = new AppUser
                    {
                        Id                   = "0", // primary key
                        UserName             = " ",
                        NormalizedUserName   = " ",
                        FirstName            = "Dummy",
                        LastName             = "User",
                        Email                = "Dummy@Dum.my",
                        Role                 = "0",
                        NormalizedEmail      = "DUMMY@DUM.MY",
                        EmailConfirmed       = false,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled     = false,
                        LockoutEnabled       = false,
                        AccessFailedCount    = 0,
                        ICalGuid             = Guid.NewGuid().ToString()
                    };

        dummy.PasswordHash = hasher.HashPassword(dummy, Guid.NewGuid().ToString());

        IdentityUserRole<string> adminUserRole  = new() { RoleId = adminRole.Id, UserId  = admin.Id };
        IdentityUserRole<string> memberUserRole = new() { RoleId = memberRole.Id, UserId = member.Id };

        modelBuilder.Entity<IdentityRole>().HasData(adminRole);
        modelBuilder.Entity<IdentityRole>().HasData(memberRole);

        modelBuilder.Entity<AppUser>().HasData(admin);
        modelBuilder.Entity<AppUser>().HasData(member);
        modelBuilder.Entity<AppUser>().HasData(dummy);

        modelBuilder.Entity<IdentityUserRole<string>>().HasData(adminUserRole);
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(memberUserRole);

        var room1 = new Room { RoomId = 1, Name = "101", Limit = 1 };
        var room2 = new Room { RoomId = 2, Name = "102", Limit = 2 };

        modelBuilder.Entity<Room>().HasData(room1);
        modelBuilder.Entity<Room>().HasData(room2);

        var seat1 = new Seat { id = "101-1-1", title = "AP 1", RoomId = 1 };
        var seat2 = new Seat { id = "101-1-2", title = "AP 2", RoomId = 1 };

        var seat3 = new Seat { id = "102-2-1", title = "AP 1", RoomId = 2 };
        var seat4 = new Seat { id = "102-2-2", title = "AP 2", RoomId = 2 };
        var seat5 = new Seat { id = "102-2-3", title = "AP 3", RoomId = 2 };

        modelBuilder.Entity<Seat>().HasData(seat1);
        modelBuilder.Entity<Seat>().HasData(seat2);

        modelBuilder.Entity<Seat>().HasData(seat3);
        modelBuilder.Entity<Seat>().HasData(seat4);
        modelBuilder.Entity<Seat>().HasData(seat5);


        modelBuilder.Entity<Reservation>().HasKey(c => new { c.Start, c.ResourceId });
    }
}