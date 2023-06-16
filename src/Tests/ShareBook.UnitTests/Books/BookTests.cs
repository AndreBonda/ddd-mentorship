using ShareBook.Domain.Books;
using ShareBook.Domain.Books.Events;
using ShareBook.Domain.Books.Exceptions;
using static ShareBook.Domain.Books.LoanRequest;

namespace ShareBook.UnitTests.Books;

[TestFixture]
public class BookTests
{
    private Book _book;

    [SetUp]
    protected void SetUp()
    {
        _book = Book.New(Guid.NewGuid(), "owner_user", "title", "author", 50, true, new[] { "label1" });
    }

    [Test]
    public void New_ThrowsException_IfGuidIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => Book.New(Guid.Empty, "valid_owner", "valid_title", "valid_author", 1, true));
    }

    [TestCase("", "valid_title", "valid_author", 1, true)]
    [TestCase(" ", "valid_title", "valid_author", 1, true)]
    [TestCase(null, "valid_title", "valid_author", 1, true)]
    public void New_ThrowsArgumentNullException_IfOwnerIsNullOrEmptyOrWhiteSpaces(string owner, string title, string author, int pages, bool sharedByUser)
    {
        Assert.Throws<ArgumentNullException>(() => Book.New(Guid.NewGuid(), owner, title, author, pages, sharedByUser));
    }

    [TestCase("valid_owner", "", "valid_author", 1, true)]
    [TestCase("valid_owner", " ", "valid_author", 1, true)]
    [TestCase("valid_owner", null, "valid_author", 1, true)]
    public void New_ThrowsArgumentNullException_IfTitleIsNullOrEmptyOrWhiteSpaces(string owner, string title, string author, int pages, bool sharedByUser)
    {
        Assert.Throws<ArgumentNullException>(() => Book.New(Guid.NewGuid(), owner, title, author, pages, true));
    }

    [TestCase("valid_owner", "valid_title", "", 1, true)]
    [TestCase("valid_owner", "valid_title", " ", 1, true)]
    [TestCase("valid_owner", "valid_title", null, 1, true)]
    public void New_ThrowsArgumentNullException_IfAuthorIsNullOrEmptyOrWhiteSpaces(string owner, string title, string author, int pages, bool sharedByUser)
    {
        Assert.Throws<ArgumentNullException>(() => Book.New(Guid.NewGuid(), owner, title, author, pages, sharedByUser));
    }

    [TestCase("valid_owner", "valid_title", "valid_author", 0, true)]
    [TestCase("valid_owner", "valid_title", "valid_author", -1, true)]
    public void New_ThrowsArgumentOutOfRangeException_IfPagesAreLessThanOne(string owner, string title, string author, int pages, bool sharedByUser)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Book.New(Guid.NewGuid(), owner, title, author, pages, sharedByUser));
    }

    [Test]
    public void New_CreateNewBook_IfValidInputs()
    {
        var emptyGuid = Guid.Empty;

        var book = Book.New(
            Guid.NewGuid(),
            "valid_owner",
            "valid_title",
            "valid_author",
            1,
            true,
            new string[] { "label1", "label2" });

        Assert.That(book, Is.Not.Null);
        Assert.That(book.Id, Is.Not.EqualTo(emptyGuid));
        Assert.That(book.CreatedAt, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
        Assert.That(book.Owner, Is.EqualTo("valid_owner"));
        Assert.That(book.Title, Is.EqualTo("valid_title"));
        Assert.That(book.Author, Is.EqualTo("valid_author"));
        Assert.That(book.Pages, Is.EqualTo(1));
        Assert.IsTrue(book.SharedByOwner);
        Assert.That(book.Labels, Is.EquivalentTo(new string[] { "label1", "label2" }));
        Assert.That(book.RequestStatus(), Is.Null);
    }

    [Test]
    public void New_CreateNewBook_IgnoringDuplicateLabels()
    {
        var book = Book.New(
            Guid.NewGuid(),
            "valid_owner",
            "valid_title",
            "valid_author",
            1,
            true,
            new string[] { "label1", "label1" });

        Assert.That(book.Labels, Is.EquivalentTo(new string[] { "label1" }));
    }

    [Test]
    public void Update_ThrowsUserIsNotBookOwnerException_IfCurrentUserIsNotTheBookOwner()
    {
        var book = Book.New(
            Guid.NewGuid(),
            "owner",
            "title",
            "author",
            1,
            true,
            new string[] { "label1" });

        Assert.Throws<UserIsNotBookOwnerException>(() => book.Update(
            "not_the_owner",
            "title",
            "author",
            1,
            true,
            new string[] { "label1" }
        ));
    }

    [Test]
    public void Update_UpdateFields_IfValidInputs()
    {
        var book = Book.New(
            Guid.NewGuid(),
            "owner",
            "title",
            "author",
            1,
            true,
            new string[] { "label1" });

        book.Update(
           "owner",
           "title_update",
           "author_update",
           2,
           false,
           new string[] { "label2" });

        Assert.That(book.Title, Is.EqualTo("title_update"));
        Assert.That(book.Author, Is.EqualTo("author_update"));
        Assert.That(book.Pages, Is.EqualTo(2));
        Assert.IsFalse(book.SharedByOwner);
        Assert.That(book.Labels, Is.EquivalentTo(new string[] { "label2" }));
    }

    [Test]
    public void Update_ThrowsRemoveSharingWithLoanRequst_IfShareByOwnerIsSetToFalseAndCurrentLoanRequestIsNotNull()
    {
        var book = Book.New(
            Guid.NewGuid(),
            "owner",
            "title",
            "author",
            1,
            true,
            new string[] { "label1" });
    }

    [Test]
    public void Update_ThrowsRemoveSharingWithCurrentLoanRequestException_IfShareByOnwerIsSetToFalseAndCurrentLoanRequestExists()
    {
        _book.RequestNewLoan("requesting_user");

        Assert.Throws<RemoveSharingWithCurrentLoanRequestException>(() =>
        _book.Update("owner_user", "title", "author", 50, false, new string[] { "label1" }));
    }

    [Test]
    public void RequestNewLoan_ThrowsBookNotSharedByOwnerException_IfBookNotSharedByOwner()
    {
        var book = Book.New(
            Guid.NewGuid(),
            "owner_a",
            "title_a",
            "author_a",
            2,
            false,
            new string[] { "label" }
            );

        Assert.Throws<BookNotSharedByOwnerException>(() => book.RequestNewLoan("user_c"));
    }

    [Test]
    public void RequestNewLoan_ThrowsBookOwnerCannotMakeALoanRequest_IfOwnerMakeARequestForHisBook()
    {
        var book = Book.New(
            Guid.NewGuid(),
            "owner_a",
            "title_a",
            "author_a",
            2,
            true,
            new string[] { "label" }
            );

        Assert.Throws<BookOwnerCannotMakeALoanRequest>(() => book.RequestNewLoan("owner_a"));
    }

    [Test]
    public void RequestNewLoan_SetCurrentLoanRequestStatus_IfLoanRequestIsCreated()
    {
        _book.RequestNewLoan("not_owner_user");

        Assert.That(_book.RequestStatus(), Is.EqualTo(LoanRequestStatus.WAITING_FOR_ACCEPTANCE));
    }

    [Test]
    public void RefuseLoanRequest_ThrowsUserIsNotBookOwnerException_IfUserIsNotTheBookOwner()
    {
        Assert.Throws<UserIsNotBookOwnerException>(() => _book.RefuseLoanRequest("not_owner_user"));
    }

    [Test]
    public void RefuseLoanRequest_ThrowsNonExistingLoanRequestException_IfLoanRequestDoesNotExist()
    {
        Assert.Throws<NonExistingLoanRequestException>(() => _book.RefuseLoanRequest("owner_user"));
    }

    [Test]
    public void RefuseLoanRequest_ThrowsLoanRequestAlreadyAcceptedException_IfLoanRequestIdAlreadyAccepted()
    {
        _book.RequestNewLoan("not_owner_user");
        _book.AcceptLoanRequest("owner_user");

        Assert.Throws<LoanRequestAlreadyAcceptedException>(() => _book.AcceptLoanRequest("owner_user"));
    }

    [Test]
    public void RefuseLoanRequest_SetCurrentLoanRequestStatusToNull_IfLoanRequestIsRefused()
    {
        _book.RequestNewLoan("not_owner_user");
        _book.RefuseLoanRequest("owner_user");

        Assert.That(_book.RequestStatus(), Is.Null);
    }

    [Test]
    public void AcceptLoanRequest_ThrowsUserIsNotBookOwnerException_IfUserIsNotTheBookOwner()
    {
        Assert.Throws<UserIsNotBookOwnerException>(() => _book.AcceptLoanRequest("not_owner_user"));
    }

    [Test]
    public void AcceptLoanRequest_ThrowsNonExistingLoanRequestException_IfLoanRequestDoesNotExist()
    {
        Assert.Throws<NonExistingLoanRequestException>(() => _book.AcceptLoanRequest("owner_user"));
    }

    [Test]
    public void AcceptLoanRequest_UpdatesBook_IfLoanRequestIsAccepted()
    {
        _book.RequestNewLoan("not_owner_user");
        _book.AcceptLoanRequest("owner_user");

        var events = _book.ReleaseEvents();

        Assert.That(_book.RequestStatus(), Is.EqualTo(LoanRequestStatus.ACCEPTED));
        Assert.That(events, Has.Exactly(1).TypeOf<LoanRequestAcceptedEvent>());

        var @event = ((LoanRequestAcceptedEvent)events.First());

        Assert.That(@event.BookId, Is.EqualTo(_book.Id));
        Assert.That(@event.DateOcurred, Is.EqualTo(DateTime.UtcNow).Within(1).Minutes);
    }
}