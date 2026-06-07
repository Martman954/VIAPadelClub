using System.Reflection;
using VIAPadelClub.Core.Domain.Aggregates.Courts.Entities;
using VIAPadelClub.Core.Domain.Common.Values;
using VIAPadelClub.Core.Tools.OperationResult.Results;

namespace UnitTests.Features.CourtTest;

public class BookingTests
{

    [Fact]
    public void Constructor_ThroughReflection_InitializesSuccessfully()
    {
        // Arrange
        var id = BookingId.New();
        var start = DateTime.Today.AddDays(1).AddHours(10);
        var end = DateTime.Today.AddDays(1).AddHours(12);
        var interval = ((Result<TimeInterval>.Success)TimeInterval.Create(start, end)).Value;
        var scheduleId = Guid.NewGuid();
        var email = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail("123456@via.dk")).Value;

        // Act
        var booking = CreateInternalBookingInstance(id, interval, scheduleId, email);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(id, booking.Id);
        Assert.Equal(interval, booking.TimeInterval);
        Assert.Equal(scheduleId, booking.ScheduleId);
        Assert.False(booking.IsCancelled, "A brand new booking should start as active (not cancelled).");
    }

    [Fact]
    public void Cancel_ThroughReflection_ShouldSetIsCancelledToTrue()
    {
        // Arrange
        var id = BookingId.New();
        var interval = ((Result<TimeInterval>.Success)TimeInterval.Create(DateTime.Today, DateTime.Today.AddHours(1))).Value;
        var email = ((Result<ViaEmail>.Success)ViaEmail.CreateEmail("123456@via.dk")).Value;
        
        var booking = CreateInternalBookingInstance(id, interval, Guid.NewGuid(), email);

        // Act - Invoke the hidden internal Cancel method
        InvokeInternalCancel(booking);

        // Assert
        Assert.True(booking.IsCancelled, "Executing the internal state modifier must successfully flip IsCancelled to true.");
    }
    
    private static Booking CreateInternalBookingInstance(BookingId id, TimeInterval interval, Guid scheduleId, ViaEmail email)
    {
        var constructor = typeof(Booking).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            null,
            new[] { typeof(BookingId), typeof(TimeInterval), typeof(Guid), typeof(ViaEmail) },
            null);

        if (constructor == null)
        {
            throw new MissingMethodException(nameof(Booking), "Constructor not found.");
        }

        return (Booking)constructor.Invoke(new object[] { id, interval, scheduleId, email });
    }

    private static void InvokeInternalCancel(Booking booking)
    {
        var method = typeof(Booking).GetMethod("Cancel", 
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (method == null)
        {
            throw new MissingMethodException(nameof(Booking), "Cancel method not found.");
        }

        method.Invoke(booking, null);
    }

}