using Moq;
using ShareBook.Domain.Shared;
using ShareBook.Domain.Shared.ValueObjects;
using ShareBook.Domain.Users;

namespace ShareBook.UnitTests.Users;

[TestFixture]
public class UserTests
{
    private Mock<Email> _email;
    private Mock<Password> _password;
    private Mock<IHashingProvider> _hashingProvider;

    [SetUp]
    public void SetUp() {
        _email = new();
        _password = new();
        _hashingProvider = new();
    }
    
    [Test]
    public void Validation_ThrowsArgumentException_IfGuidIsEmpty() {
        // Arrange
        var email = _email.Object;
        var password = _password.Object;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new User(Guid.Empty, email, password));
    }

    [Test]
    public void Validation_ThrowsArgumentNullException_IfEmailIsNull()
    {
        // Arrange
        var password = _password.Object;

        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => new User(Guid.NewGuid(), null, password));
    }

    [Test]
    public void Validation_ThrowsArgumentNullException_IfPasswordIsNull()
    {
        // Arrange
        var email = _email.Object;

        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => new User(Guid.NewGuid(), email, null));
    }

    [Test]
    public void Constructor_CreateNewUser_IfValidInputs() {
        // Arrange
        _email.Setup(e => e.Value).Returns("valid_email");
        var guid = Guid.NewGuid();
        var email = _email.Object;
        var password = _password.Object;

        // Act
        var user = new User(guid, email, password);

        // Assert
        Assert.That(user.Email.Value, Is.EqualTo("valid_email"));
    }
}