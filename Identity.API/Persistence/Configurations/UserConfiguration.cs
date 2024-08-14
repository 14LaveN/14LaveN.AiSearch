using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Common.ValueObjects;
using Domain.Entities;
using Domain.ValueObjects;
using Identity.API.Domain.Entities;

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

        //TODO builder.HasData(new User(
        //TODO     "sdfsdf",
        //TODO     FirstName.Create("dfsdf").Value,
        //TODO     LastName.Create("fdfsdfsf").Value,
        //TODO     new EmailAddress("dfsdfs@mail.ru")
        //TODO     {
        //TODO         UserId  = Guid.NewGuid()
        //TODO     },
        //TODO     "Sdfdsf_2008")
        //TODO {
        //TODO     Id = Guid.NewGuid()
        //TODO });

        builder.HasKey(user => user.Id);

        builder
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();
        
        builder.OwnsOne(user => user.FirstName, firstNameBuilder =>
        {
            firstNameBuilder.WithOwner();

            firstNameBuilder.Property(firstName => firstName.Value)
                .HasColumnName(nameof(User.FirstName))
                .HasMaxLength(FirstName.MaxLength)
                .IsRequired();
        });
        
        builder.OwnsOne(user => user.LastName, lastNameBuilder =>
        {
            lastNameBuilder.WithOwner();

            lastNameBuilder.Property(lastName => lastName.Value)
                .HasColumnName(nameof(User.LastName))
                .HasMaxLength(LastName.MaxLength)
                .IsRequired();
        });

        builder.OwnsOne(user => user.EmailAddress, emailBuilder =>
        {
            emailBuilder.WithOwner();

            emailBuilder.Property(email => email.Value)
                .HasColumnName(nameof(User.EmailAddress))
                .HasMaxLength(EmailAddress.MaxLength)
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