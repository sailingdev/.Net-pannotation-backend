using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pannotation.Domain.Entities;
using Pannotation.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pannotation.DAL.Migrations
{
    public static class DbInitializer
    {
        public static void Initialize(DataContext context, IConfiguration Configuration, IServiceProvider serviceProvider)
        {
            context.Database.EnsureCreated();

            #region Roles

            new List<ApplicationRole>
            {
                new ApplicationRole { Name = Role.User, NormalizedName = Role.User },
                new ApplicationRole { Name = Role.Admin, NormalizedName = Role.Admin }
            }
            .ForEach(i =>
            {
                if (!context.Roles.Any(x => x.Name == i.Name))
                    context.Roles.Add(i);
            });

            context.SaveChanges();

            #endregion

            #region Creating Super Admins

            var admins = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    UserName = "angel@sugarislands.com",
                    Email = "angel@sugarislands.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Profile = new Profile()
                },
                new ApplicationUser
                {
                    UserName = "pannotationmusic@gmail.com",
                    Email = "pannotationmusic@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Profile = new Profile()
                }
            };

            var password = "Partydiscoyachts123";

            foreach (var user in admins)
            {
                if (!context.Users.Any(u => u.UserName == user.UserName))
                    SeedAdmin(user, password);
            }

            void SeedAdmin(ApplicationUser user, string passwordString)
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                IdentityResult result = userManager.CreateAsync(user, passwordString).Result;

                if (result.Succeeded)
                    userManager.AddToRoleAsync(user, Role.Admin).Wait();
            }

            #endregion

            #region Creating Genres

            var genres = new List<Genre>
            {
                new Genre
                {
                    Name = "Alternative"
                },
                new Genre
                {
                    Name = "Blues"
                },
                new Genre
                {
                    Name = "Broadway"
                },
                new Genre
                {
                    Name = "Calypso"
                },
                new Genre
                {
                    Name = "Chutney"
                },
                new Genre
                {
                    Name = "Classical"
                },
                new Genre
                {
                    Name = "Country"
                },
                new Genre
                {
                    Name = "Dance"
                },
                new Genre
                {
                    Name = "Easy"
                },
                new Genre
                {
                    Name = "Listening"
                },
                new Genre
                {
                    Name = "Electronic"
                },
                new Genre
                {
                    Name = "Folk"
                },
                new Genre
                {
                    Name = "Hip Hop/Rap"
                },
                new Genre
                {
                    Name = "Indian Classical"
                },
                new Genre
                {
                    Name = "Indie"
                },
                new Genre
                {
                    Name = "Industrial"
                },
                new Genre
                {
                    Name = "Jazz"
                },
                new Genre
                {
                    Name = "Latin"
                },
                new Genre
                {
                    Name = "New Age"
                },
                new Genre
                {
                    Name = "Parang"
                },
                new Genre
                {
                    Name = "Pop"
                },
                new Genre
                {
                    Name = "Rapso"
                },
                new Genre
                {
                    Name = "Reggae"
                },
                new Genre
                {
                    Name = "Religious"
                },
                new Genre
                {
                    Name = "Rock"
                },
                new Genre
                {
                    Name = "RnB"
                },
                new Genre
                {
                    Name = "Soca"
                },
                new Genre
                {
                    Name = "Soul"
                },
                new Genre
                {
                    Name = "World"
                },
                new Genre
                {
                    Name = "Film"
                },
                new Genre
                {
                    Name = "Panorama"
                },
            };

            foreach (var genre in genres)
                if (!context.Genres.Any(x => x.Name == genre.Name))
                    context.Genres.Add(genre);

            #endregion

            #region Creating Instruments

            var instruments = new List<Instrument>
            {
                new Instrument
                {
                    Name = "tenor"
                },
                new Instrument
                {
                    Name = "double tenor"
                },
                new Instrument
                {
                    Name = "double seconds"
                },
                new Instrument
                {
                    Name = "quadraphonics"
                },
                new Instrument
                {
                    Name = "guitar"
                },
                new Instrument
                {
                    Name = "double guitar"
                },
                new Instrument
                {
                    Name = "triple guitar"
                },
                new Instrument
                {
                    Name = "cello"
                },
                new Instrument
                {
                    Name = "3-cello"
                },
                new Instrument
                {
                    Name = "4-cello"
                },
                new Instrument
                {
                    Name = "tenor bass"
                },
                new Instrument
                {
                    Name = "bass"
                },
                new Instrument
                {
                    Name = "drum set"
                },
                new Instrument
                {
                    Name = "cowbell"
                },
                new Instrument
                {
                    Name = "congas"
                },
                new Instrument
                {
                    Name = "iron"
                },
                new Instrument
                {
                    Name = "scratcher"
                },
                new Instrument
                {
                    Name = "jam block"
                },
                new Instrument
                {
                    Name = "any"
                }
            };

            foreach (var instrument in instruments)
                if (!context.Instruments.Any(x => x.Name == instrument.Name))
                    context.Instruments.Add(instrument);

            #endregion

            context.SaveChanges();
        }
    }
}