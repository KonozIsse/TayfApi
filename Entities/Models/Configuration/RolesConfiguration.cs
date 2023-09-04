using Entities.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.OwnsOne(x => x.TimeSlot, ts =>
            {
                ts.Property(p => p.CreatedAt).HasColumnType("time").HasColumnName("CreatedAt");
                ts.Property(p => p.UpdatedAt).HasColumnType("time").HasColumnName("UpdatedAt");
            });
            builder.HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN",
                IsVendorLink = false,
                IsStatus = Status.Active
            },
             new Role
             {
                 Id = 2,
                 Name = "Customer",
                 NormalizedName = "CUSTOMER",
                 IsVendorLink = false,
                 IsStatus = Status.Active
             },
            new Role
            {
                Id = 3,
                Name = "Store",
                NormalizedName = "STORE",
                IsVendorLink = true,
                IsStatus = Status.Active
            }
            );
        }
    }
}
