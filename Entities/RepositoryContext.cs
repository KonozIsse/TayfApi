using Entities.Models;
using Entities.Models.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Entities
{
    public class RepositoryContext : IdentityDbContext <User,Role,int>
    {
        public RepositoryContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<ProductAttribut> AttributesProducts { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartAttributeProduct> CartAttributeProducts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CommentNews> CommentNews { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<DeliveryTime> DeliveryTimes { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ImageSetting> ImageSettings { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<MessageTemplate> MessageTemplate { get; set; }
        public DbSet<MailList> MailLists { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<NotificationAction> NotificationAction { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrdersProducts { get; set; }
        public DbSet<OrderAttributProduct> OrderAttributesProducts { get; set; }
        public DbSet<OrderStatus> OrdersStatus { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<PaymentMethodDetail> PaymentMethodDetail { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductOptionValue> ProductOptionValues { get; set; } 
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ProductSales> ProductSales { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ShippingMethods> ShippingMethods { get; set; }
        public DbSet<Sliders> Sliders { get; set; }
        public DbSet<StaticPages> StaticPages { get; set; }
        public DbSet<SpecialProducts> SpecialProducts { get; set; }
        public DbSet<TaxClass> TaxClasses { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }
        public DbSet<Unit> Units { get; set; } 
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        break;
                    case EntityState.Deleted:
                        bool IsDeleted;
                        if (entry.OriginalValues.TryGetValue("IsDeleted", out IsDeleted))
                        {
                            entry.State = EntityState.Modified;
                            entry.CurrentValues["IsDeleted"] = true;
                            entry.CurrentValues["DeletedAt"] = DateTime.Now;
                        }
                        break;
                    case EntityState.Modified:
                        DateTime? UpdatedAt;
                        if (entry.OriginalValues.TryGetValue("UpdatedAt", out UpdatedAt))
                        {
                            entry.CurrentValues["UpdatedAt"] = DateTime.Now;
                        }
                        break;

                }
            }
        }

        private void HandleDependent(EntityEntry entry)
        {
            entry.CurrentValues["IsDeleted"] = true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            ConfigureConfiguration(modelBuilder);
            ConfigureSoftDelete(modelBuilder);
            ConfigureSoftDeleteUser(modelBuilder);
            ConfigureAutoMapToTables(modelBuilder);
            ConfigureDeleteBehavior(modelBuilder);

        

            modelBuilder.Entity<User>().HasMany(address => address.Addresses)
                          .WithOne(user => user.User).HasForeignKey(con => con.UserId);

            modelBuilder.Entity<User>().HasMany(store => store.StoreOrders)
                         .WithOne(order => order.Store).HasForeignKey(con => con.StoreId);

            modelBuilder.Entity<User>().HasMany(customer => customer.CustomerOrders)
                         .WithOne(order => order.Customer).HasForeignKey(con => con.CustomerId);

            modelBuilder.Entity<User>().Property(c => c.UserType)
                .HasConversion(x => x.ToString(), x => (UserType)Enum.Parse(typeof(UserType), x));
        }
        private static void ConfigureDeleteBehavior(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                            .SelectMany(t => t.GetForeignKeys())
                            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
        private static void ConfigureConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryContext).Assembly);
            //modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }
        private static void ConfigureSoftDelete(ModelBuilder modelBuilder)
        {
            Expression<Func<BaseEntity, bool>> filterExpr = bm => !bm.IsDeleted ;
            foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
            {
                // check if current entity type is child of BaseModel
                if (mutableEntityType.ClrType.IsAssignableTo(typeof(BaseEntity)))
                {
                    // modify expression to handle correct child type
                    var parameter = Expression.Parameter(mutableEntityType.ClrType);
                    var body = ReplacingExpressionVisitor.Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
                    var lambdaExpression = Expression.Lambda(body, parameter);

                    // set filter
                    mutableEntityType.SetQueryFilter(lambdaExpression);
                }
            }
        }
        private static void ConfigureSoftDeleteUser(ModelBuilder modelBuilder)
        {
            Expression<Func<User, bool>> filterExpr = bm => !bm.IsDeleted;
            foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
            {
                // check if current entity type is child of BaseModel
                if (mutableEntityType.ClrType.IsAssignableTo(typeof(User)))
                {
                    // modify expression to handle correct child type
                    var parameter = Expression.Parameter(mutableEntityType.ClrType);
                    var body = ReplacingExpressionVisitor.Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
                    var lambdaExpression = Expression.Lambda(body, parameter);

                    // set filter
                    mutableEntityType.SetQueryFilter(lambdaExpression);
                }
            }
        }
        private static void ConfigureAutoMapToTables(ModelBuilder modelBuilder)
        {
            var allDbSets = typeof(RepositoryContext).GetProperties()
                         .Where(p => p.PropertyType.Name.Contains("DbSet") && p.Module.Name.Contains(nameof(RepositoryContext)))
                         .Select(p => new
                         {
                             Type = p.PropertyType.GetGenericArguments()[0],
                             p.Name
                         });
            string schema = null;
            foreach (var property in allDbSets)
            {
                var type = property.Type;
                modelBuilder.Entity(type, b =>
                {
                    b.ToTable(property.Name, schema);
                });
            }
        }
    }

}
