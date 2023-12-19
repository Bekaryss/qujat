using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MimeMapping;
using Qujat.Core.Data;
using Qujat.Core.Data.Entities;
using Qujat.Core.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Qujat.Core.Extensions
{
    public static class ApplicationDbExtensions
    {
        public static void ExecuteMigrations(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string connectionString = configuration["Backend:Database:ApplicationDbConnectionString"];

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString, config => config
                    .MigrationsAssembly("Qujat.Core"));

            var database = new ApplicationDbContext(optionsBuilder.Options);

            database.Database.Migrate();
        }


        public static async Task SeedDefaultIcons(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            string connectionString = configuration["Backend:Database:ApplicationDbConnectionString"];

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(connectionString, config => config
                    .MigrationsAssembly("Qujat.Core"));

            var database = new ApplicationDbContext(optionsBuilder.Options);

            var blobService = scope.ServiceProvider.GetRequiredService<ExternalS3BlobService>();

            var iconsFromDb = await database.Icons.ToArrayAsync();
            var iconsFromSeed = new DirectoryInfo(@"Seed\Icons").EnumerateFiles();

            foreach (var item in iconsFromSeed)
            {
                var iconFromDb = iconsFromDb.SingleOrDefault(p => p.FileName ==  item.Name);
                if(iconFromDb == null)
                {
                    using var stream = item.OpenRead();
                    var uploadResult = await blobService.UploadBlob(stream, item.Name, CancellationToken.None);

                    var iconEntityToAdd = new IconBlobEntity
                    {
                        Uri = uploadResult.ToString(),
                        Content = null,
                        Extension = item.Extension,
                        FileName = item.Name,
                        MimeType = MimeUtility.GetMimeMapping(item.Name)
                    };

                    database.Icons.Add(iconEntityToAdd);
                }
            }

            await database.SaveChangesAsync();
        }
    }
}
