using Xunit;
   using VIAPadelClub.Core.Domain.Aggregates.Players.ValueObjects;
   
   namespace UnitTests.Common.PlayerAggregateValueObjects;
   
   public class VipStatusTests
   {
       [Fact]
       public void Create_Should_ReturnActiveVipStatus_When_ValidCurrentDateProvided()
       {
           // Arrange
           var baseDate = DateTime.Today;
   
           // Act 
           VipStatus vipStatus = VipStatus.Create(baseDate);
   
           // Assert
           Assert.NotNull(vipStatus);
           Assert.True(vipStatus.IsActive(baseDate), "VIP status should be immediately active upon creation.");
       }
   
       [Fact]
       public void IsActive_Should_ReturnFalse_When_VipPeriodHasExpired()
       {
           // Arrange
           var baseDate = DateTime.Today;
           VipStatus vipStatus = VipStatus.Create(baseDate);
   
           // Act
           // Checking 45 days into the future ensures it falls outside the standard 30-day window
           var futureDate = baseDate.AddDays(45);
           var isActiveInFuture = vipStatus.IsActive(futureDate);
   
           // Assert
           Assert.False(isActiveInFuture, "VIP status should naturally expire after its active privilege window.");
       }
   }