using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;

namespace VIAPadelClub.Infrastructure.EfcDomainPersistence;

internal sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                email => email.StringValue,
                s     => ViaEmail.From(s));

        // Email is a computed alias for Id -- ignore to avoid double-mapping
        builder.Ignore(p => p.Email);

        builder.OwnsOne(p => p.Name, name =>
        {
            name.Property(n => n.FirstName);
            name.Property(n => n.LastName);
        });

        builder.OwnsOne(p => p.ProfilePictureUri, img =>
        {
            img.Property(i => i.Value);
        });

        builder.Property(p => p.isBlackListed);

        builder.OwnsOne(p => p.VipStatus, vip =>
        {
            vip.Property(v => v.StartDate);
            vip.Property(v => v.EndDate);
        });

        builder.OwnsOne(p => p.Quarantine, q =>
        {
            q.Property(v => v.StartDate);
            q.Property(v => v.EndDate);
        });
    }
}

