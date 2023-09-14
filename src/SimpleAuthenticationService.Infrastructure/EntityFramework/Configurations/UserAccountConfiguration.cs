using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleAuthenticationService.Domain.UserAccounts;

namespace SimpleAuthenticationService.Infrastructure.EntityFramework.Configurations;

internal sealed class UserAccountConfiguration : IEntityTypeConfiguration<UserAccount>
{
    public void Configure(EntityTypeBuilder<UserAccount> builder)
    {
        builder.ToTable("user_accounts");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired()
            .HasConversion(
                userAccountId => userAccountId.Value,
                value => new UserAccountId(value));

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();
        
        builder.Property(x => x.Login)
            .HasColumnName("login")
            .IsRequired()
            .HasMaxLength(128)
            .HasConversion(
                login => login.Value,
                value => new Login(value));
        
        builder.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired()
            .HasMaxLength(512)
            .HasConversion(
                passwordHash => passwordHash.Value,
                value => new PasswordHash(value));

        builder.OwnsOne(x => x.RefreshToken, refreshTokenBuilder =>
        {
            refreshTokenBuilder.Property(rt => rt.Value)
                .HasColumnName("refresh_token_value")
                .HasMaxLength(1024);
            
            refreshTokenBuilder.Property(rt => rt.ExpirationDateUtc)
                .HasColumnName("refresh_token_expiration_date_utc");
            
            refreshTokenBuilder.Property(rt => rt.IsActive)
                .HasColumnName("refresh_token_is_active");
        });

        builder.OwnsMany(typeof(Claim), "_claims", claimsBuilder =>
        {
            claimsBuilder.ToTable("user_account_claims");
            
            claimsBuilder.WithOwner().HasForeignKey("user_account_id");
            
            claimsBuilder.Property<Guid>("id");
            
            claimsBuilder.HasKey("id");

            claimsBuilder.Property("Type")
                .HasColumnName("type")
                .IsRequired()
                .HasMaxLength(128);
            
            claimsBuilder.Property("Value")
                .HasColumnName("value")
                .IsRequired()
                .HasMaxLength(512);
        });
        
        builder.Property<uint>("Version")
            .IsRowVersion();
    }
}