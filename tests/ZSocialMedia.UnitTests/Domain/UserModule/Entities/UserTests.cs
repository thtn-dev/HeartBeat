using FluentAssertions;
using ZSocialMedia.Domain.UserModule.Entities;
using ZSocialMedia.Domain.UserModule.Events;

namespace ZSocialMedia.UnitTests.Domain.UserModule.Entities;

public class UserTests
{
    [Fact]
    public void User_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var user = CreateValidUser();

        // Assert
        user.IsActive.Should().BeTrue();
        user.IsEmailVerified.Should().BeFalse();
        user.IsSuspended.Should().BeFalse();
        user.IsVerified.Should().BeFalse();
        user.IsTwoFactorEnabled.Should().BeFalse();
        user.IsDeleted.Should().BeFalse();
        user.FailedLoginAttempts.Should().Be(0);
        user.Followers.Should().BeEmpty();
        user.Following.Should().BeEmpty();
        user.BlockedUsers.Should().BeEmpty();
        user.BlockedByUsers.Should().BeEmpty();
        user.Roles.Should().BeEmpty();
    }

    [Fact]
    public void Suspend_ShouldSetSuspensionProperties_WhenCalled()
    {
        // Arrange
        var user = CreateValidUser();
        var reason = "Violation of terms of service";
        var beforeSuspension = DateTime.UtcNow;

        // Act
        user.Suspend(reason);

        // Assert
        user.IsSuspended.Should().BeTrue();
        user.SuspensionReason.Should().Be(reason);
        user.SuspendedAt.Should().NotBeNull();
        user.SuspendedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.SuspendedAt.Should().BeOnOrAfter(beforeSuspension);
    }

    [Fact]
    public void Suspend_ShouldRaiseDomainEvent_WhenCalled()
    {
        // Arrange
        var user = CreateValidUser();
        var reason = "Spam behavior";

        // Act
        user.Suspend(reason);

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First();
        domainEvent.Should().BeOfType<UserSuspendedEvent>();

        var userSuspendedEvent = (UserSuspendedEvent)domainEvent;
        userSuspendedEvent.UserId.Should().Be(user.Id);
        userSuspendedEvent.Reason.Should().Be(reason);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Suspend_ShouldAllowEmptyReason_WhenReasonIsNullOrWhitespace(string reason)
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        user.Suspend(reason);

        // Assert
        user.IsSuspended.Should().BeTrue();
        user.SuspensionReason.Should().Be(reason);
        user.SuspendedAt.Should().NotBeNull();
    }

    [Fact]
    public void Suspend_ShouldOverridePreviousSuspension_WhenCalledMultipleTimes()
    {
        // Arrange
        var user = CreateValidUser();
        var firstReason = "First violation";
        var secondReason = "Second violation";

        // Act
        user.Suspend(firstReason);
        var firstSuspensionTime = user.SuspendedAt;

        Thread.Sleep(10); // Ensure different timestamps
        user.Suspend(secondReason);

        // Assert
        user.IsSuspended.Should().BeTrue();
        user.SuspensionReason.Should().Be(secondReason);
        user.SuspendedAt.Should().BeAfter(firstSuspensionTime!.Value);

        // Should have two domain events
        user.DomainEvents.Should().HaveCount(2);
        user.DomainEvents.Should().AllBeOfType<UserSuspendedEvent>();
    }

    [Fact]
    public void Activate_ShouldClearSuspensionProperties_WhenCalled()
    {
        // Arrange
        var user = CreateValidUser();
        user.Suspend("Test suspension");

        // Act
        user.Activate();

        // Assert
        user.IsSuspended.Should().BeFalse();
        user.SuspendedAt.Should().BeNull();
        user.SuspensionReason.Should().BeNull();
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Activate_ShouldWorkOnNonSuspendedUser_WhenCalled()
    {
        // Arrange
        var user = CreateValidUser();
        user.IsActive = false; // Simulate inactive but not suspended user

        // Act
        user.Activate();

        // Assert
        user.IsSuspended.Should().BeFalse();
        user.SuspendedAt.Should().BeNull();
        user.SuspensionReason.Should().BeNull();
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void VerifyEmail_ShouldSetEmailVerificationProperties_WhenCalled()
    {
        // Arrange
        var user = CreateValidUser();
        var beforeVerification = DateTime.UtcNow;

        // Act
        user.VerifyEmail();

        // Assert
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerifiedAt.Should().NotBeNull();
        user.EmailVerifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.EmailVerifiedAt.Should().BeOnOrAfter(beforeVerification);
    }

    [Fact]
    public void VerifyEmail_ShouldRaiseDomainEvent_WhenCalled()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        user.VerifyEmail();

        // Assert
        user.DomainEvents.Should().ContainSingle();
        var domainEvent = user.DomainEvents.First();
        domainEvent.Should().BeOfType<UserEmailVerifiedEvent>();

        var emailVerifiedEvent = (UserEmailVerifiedEvent)domainEvent;
        emailVerifiedEvent.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void VerifyEmail_ShouldUpdateVerificationTime_WhenCalledMultipleTimes()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        user.VerifyEmail();
        var firstVerificationTime = user.EmailVerifiedAt;

        Thread.Sleep(10); // Ensure different timestamps
        user.VerifyEmail();

        // Assert
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerifiedAt.Should().BeAfter(firstVerificationTime!.Value);

        // Should have two domain events
        user.DomainEvents.Should().HaveCount(2);
        user.DomainEvents.Should().AllBeOfType<UserEmailVerifiedEvent>();
    }

    [Fact]
    public void User_ShouldMaintainStateConsistency_WhenSuspendedAndActivated()
    {
        // Arrange
        var user = CreateValidUser();
        var suspensionReason = "Policy violation";

        // Act & Assert - Suspend
        user.Suspend(suspensionReason);
        user.IsSuspended.Should().BeTrue();
        user.SuspensionReason.Should().Be(suspensionReason);
        user.SuspendedAt.Should().NotBeNull();

        // Act & Assert - Activate
        user.Activate();
        user.IsSuspended.Should().BeFalse();
        user.SuspensionReason.Should().BeNull();
        user.SuspendedAt.Should().BeNull();
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void User_ShouldHandleComplexScenario_WhenMultipleOperationsPerformed()
    {
        // Arrange
        var user = CreateValidUser();

        // Act - Perform multiple operations
        user.VerifyEmail();
        user.Suspend("Test suspension");
        user.Activate();

        // Assert
        // Email verification should remain
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerifiedAt.Should().NotBeNull();

        // Suspension should be cleared
        user.IsSuspended.Should().BeFalse();
        user.SuspensionReason.Should().BeNull();
        user.SuspendedAt.Should().BeNull();
        user.IsActive.Should().BeTrue();

        // Should have 3 domain events
        user.DomainEvents.Should().HaveCount(3);
        user.DomainEvents.Should().Contain(e => e is UserEmailVerifiedEvent);
        user.DomainEvents.Should().Contain(e => e is UserSuspendedEvent);
        user.DomainEvents.Should().Contain(e => e is UserActivatedEvent);
    }

    [Theory]
    [InlineData("user@example.com", "validuser", "hashedpassword123")]
    [InlineData("test.email+tag@domain.co.uk", "test_user", "anotherhash456")]
    public void User_ShouldAcceptValidProperties_WhenCreated(string email, string username, string passwordHash)
    {
        // Act
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            PasswordHash = passwordHash
        };

        // Assert
        user.Email.Should().Be(email);
        user.Username.Should().Be(username);
        user.PasswordHash.Should().Be(passwordHash);
        user.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void User_ShouldTrackSecurityProperties_WhenSet()
    {
        // Arrange
        var user = CreateValidUser();
        var loginTime = DateTime.UtcNow;
        var loginIp = "192.168.1.1";
        var secretKey = "ABCDEFGHIJK123456789";

        // Act
        user.LastLoginAt = loginTime;
        user.LastLoginIp = loginIp;
        user.IsTwoFactorEnabled = true;
        user.TwoFactorSecretKey = secretKey;
        user.FailedLoginAttempts = 3;
        user.LockoutEndAt = DateTime.UtcNow.AddMinutes(30);

        // Assert
        user.LastLoginAt.Should().Be(loginTime);
        user.LastLoginIp.Should().Be(loginIp);
        user.IsTwoFactorEnabled.Should().BeTrue();
        user.TwoFactorSecretKey.Should().Be(secretKey);
        user.FailedLoginAttempts.Should().Be(3);
        user.LockoutEndAt.Should().NotBeNull();
    }

    [Fact]
    public void User_ShouldTrackVerificationStatus_WhenSet()
    {
        // Arrange
        var user = CreateValidUser();
        var verificationTime = DateTime.UtcNow;

        // Act
        user.IsVerified = true;
        user.VerifiedAt = verificationTime;

        // Assert
        user.IsVerified.Should().BeTrue();
        user.VerifiedAt.Should().Be(verificationTime);
    }

    [Fact]
    public void User_ShouldSupportSoftDelete_WhenDeleted()
    {
        // Arrange
        var user = CreateValidUser();
        var deletionTime = DateTime.UtcNow;
        var deletedBy = "admin";

        // Act
        user.IsDeleted = true;
        user.DeletedAt = deletionTime;
        user.DeletedBy = deletedBy;

        // Assert
        user.IsDeleted.Should().BeTrue();
        user.DeletedAt.Should().Be(deletionTime);
        user.DeletedBy.Should().Be(deletedBy);
    }

    // Helper method to create a valid user for testing
    private static User CreateValidUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashed_password_123"
        };
    }
}

//// Test fixtures for edge cases and validation
//public class UserValidationTests
//{
//    [Fact]
//    public void User_ShouldRequireUsername_WhenCreated()
//    {
//        // Act & Assert
//        Assert.Throws<ArgumentNullException>(() => new User
//        {
//            Id = Guid.NewGuid(),
//            Username = null!, // This should cause validation error
//            Email = "test@example.com",
//            PasswordHash = "hash"
//        });
//    }

//    [Fact]
//    public void User_ShouldRequireEmail_WhenCreated()
//    {
//        // Act & Assert
//        Assert.Throws<ArgumentNullException>(() => new User
//        {
//            Id = Guid.NewGuid(),
//            Username = "testuser",
//            Email = null!, // This should cause validation error
//            PasswordHash = "hash"
//        });
//    }

//    [Fact]
//    public void User_ShouldRequirePasswordHash_WhenCreated()
//    {
//        // Act & Assert
//        Assert.Throws<ArgumentNullException>(() => new User
//        {
//            Id = Guid.NewGuid(),
//            Username = "testuser",
//            Email = "test@example.com",
//            PasswordHash = null! // This should cause validation error
//        });
//    }
//}

