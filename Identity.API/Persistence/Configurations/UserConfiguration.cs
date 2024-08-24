using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.ValueObjects;
using Identity.API.Domain.Entities;
using Persistence.Converters;

namespace Identity.API.Persistence.Configurations;

/// <summary>
/// Represents the configuration for the <see cref="User"/> entity.
/// </summary>
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasIndex(x => x.Id)
            .HasDatabaseName("IdUserIndex");

        builder.HasMany<Role>()
            .WithMany();

        UlidToStringConverter converter = new();

        builder.HasKey(user => user.Id);

        builder
            .Property(u => u.Id)
            .HasConversion(
                v => v.ToString(),
                v => Ulid.Parse(v))
            .ValueGeneratedOnAdd();
        
        builder.OwnsOne(user => user.FirstName, firstNameBuilder =>
        {
            firstNameBuilder.WithOwner();

            firstNameBuilder.Property(firstName => firstName.Value)
                .HasColumnName(nameof(User.FirstName))
                .HasMaxLength(FirstName.MaxLength)
                .IsRequired();
            
            firstNameBuilder.Property(firstName => firstName.UserId)
                .HasConversion(converter)
                .HasColumnName(nameof(User.Id))
                .IsRequired();
        });
        
        builder.OwnsOne(user => user.LastName, lastNameBuilder =>
        {
            lastNameBuilder.WithOwner();

            lastNameBuilder.Property(lastName => lastName.Value)
                .HasColumnName(nameof(User.LastName))
                .HasMaxLength(LastName.MaxLength)
                .IsRequired();
            
            lastNameBuilder.Property(firstName => firstName.UserId)
                .HasConversion(converter)
                .HasColumnName(nameof(User.Id))
                .IsRequired();
        });

        builder.OwnsOne(user => user.EmailAddress, emailBuilder =>
        {
            emailBuilder.WithOwner();

            emailBuilder.Property(email => email.Value)
                .HasColumnName(nameof(User.EmailAddress))
                .HasMaxLength(EmailAddress.MaxLength)
                .IsRequired();
            
            emailBuilder.Property(firstName => firstName.UserId)
                .HasConversion(converter)
                .HasColumnName(nameof(User.Id))
                .IsRequired();
        });

        builder.Property(user => user.CreatedOnUtc).IsRequired();

        builder.Property(user => user.ModifiedOnUtc);

        builder.Property(user => user.DeletedOnUtc);

        builder.Property(user => user.Deleted).HasDefaultValue(false);

        builder.HasQueryFilter(user => !user.Deleted);

        builder.Ignore(user => user.FullName);
    }
}