using MediatR;
using ShareBook.Application.Books.ViewModels;

namespace ShareBook.Application.Books;

public record GetBooksQuery(string Title) : IRequest<IEnumerable<BookVM>>;