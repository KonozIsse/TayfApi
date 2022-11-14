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
            builder.HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
             new Role
             {
                 Id = 2,
                 Name = "Customer",
                 NormalizedName = "CUSTOMER"
             },
            new Role
            {
                Id = 3,
                 Name = "Store",
                NormalizedName = "STORE"
            }
            );
        }
    }
}
