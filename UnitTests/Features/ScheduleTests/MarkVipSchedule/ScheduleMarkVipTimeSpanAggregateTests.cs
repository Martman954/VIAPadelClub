using System.Reflection;
using VIAPadelClub.Core.Domain.Aggregates.Players;
using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Domain.Contracts.Players;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.ScheduleTests.MarkVipSchedule;

file class EmailAvailableChecker : IEmailInUseChecker
{
    public bool IsEmailInUse(ViaEmail email) => false;
}

public class PlayerElevateToVipAggregateTests
{
    private static readonly DateTime Today = DateTime.Today;

    private static Player CreateTestPlayer()
    {
        var email = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail("123456@via.dk")).Value;
        var name = ((Result<Name>.Success)Name.CreateName("Alex", "Andersen")).Value;
        var image = ((Result<ImageUrl>.Success)ImageUrl.CreateImageUrl("https://via.dk/pic.png")).Value;
        var result = Player.Register(email, name, image, new EmailAvailableChecker());
        return ((Result<Player>.Success)result).Value;
    }

    private static void InvokeQuarantine(Player player, DateTime date)
    {
        typeof(Player)
            .GetMethod("QuarantinePlayer", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(player, [date]);
    }

    private static void InvokeBlacklist(Player player)
    {
        typeof(Player)
            .GetMethod("Blacklist", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(player, null);
    }

    [Fact]
    public void ElevateToVip_NormalPlayer_ReturnsSuccess()
    {
        var player = CreateTestPlayer();

        var result = player.ElevateToVip(Today);

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void ElevateToVip_NormalPlayer_VipStatusIsNotNull()
    {
        var player = CreateTestPlayer();

        player.ElevateToVip(Today);

        Assert.NotNull(player.VipStatus);
    }

    [Fact]
    public void ElevateToVip_NormalPlayer_VipStartDateIsToday()
    {
        var player = CreateTestPlayer();

        player.ElevateToVip(Today);

        Assert.Equal(Today, player.VipStatus!.StartDate);
    }

    [Fact]
    public void ElevateToVip_NormalPlayer_VipEndDateIsOneMonthLater()
    {
        var player = CreateTestPlayer();

        player.ElevateToVip(Today);

        Assert.Equal(Today.AddMonths(1), player.VipStatus!.EndDate);
    }

    [Fact]
    public void ElevateToVip_AlreadyActiveVip_ReturnsSuccess()
    {
        var player = CreateTestPlayer();
        player.ElevateToVip(Today);
        var endDateBefore = player.VipStatus!.EndDate;

        var result = player.ElevateToVip(Today);

        Assert.IsType<Result<None>.Success>(result);
    }

    [Fact]
    public void ElevateToVip_AlreadyActiveVip_ExtendsEndDateByThirtyDays()
    {
        var player = CreateTestPlayer();
        player.ElevateToVip(Today);
        var endDateBefore = player.VipStatus!.EndDate;

        player.ElevateToVip(Today);

        Assert.Equal(endDateBefore.AddDays(30), player.VipStatus!.EndDate);
    }

    [Fact]
    public void ElevateToVip_BlacklistedPlayer_ReturnsFailure()
    {
        var player = CreateTestPlayer();
        InvokeBlacklist(player);

        var result = player.ElevateToVip(Today);

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void ElevateToVip_BlacklistedPlayer_ErrorMentionsBlacklisted()
    {
        var player = CreateTestPlayer();
        InvokeBlacklist(player);

        var result = player.ElevateToVip(Today);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("blacklisted", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ElevateToVip_QuarantinedPlayer_ReturnsFailure()
    {
        var player = CreateTestPlayer();
        InvokeQuarantine(player, Today);

        var result = player.ElevateToVip(Today);

        Assert.IsType<Result<None>.Failure>(result);
    }

    [Fact]
    public void ElevateToVip_QuarantinedPlayer_ErrorMentionsQuarantined()
    {
        var player = CreateTestPlayer();
        InvokeQuarantine(player, Today);

        var result = player.ElevateToVip(Today);

        var failure = Assert.IsType<Result<None>.Failure>(result);
        Assert.Contains(failure.Errors, e => e.Message.Contains("quarantined", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ElevateToVip_ExpiredVip_CreatesNewVipStatus()
    {
        var player = CreateTestPlayer();
        player.ElevateToVip(Today.AddMonths(-2));

        player.ElevateToVip(Today);

        Assert.Equal(Today, player.VipStatus!.StartDate);
    }
}