using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Models;
using TestTask.Services.Interfaces;

namespace TestTask.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _db;
        private static readonly DateTime CarolusRexAlbumReleaseDate = new DateTime(2012, 5, 25);
        private static readonly string BookTitleFilterWord  = "Red";
        public BookService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Book> GetBook()
        {
            return await _db.Books
                            .OrderByDescending(b => b.Price * b.QuantityPublished)
                            .FirstOrDefaultAsync();
        }

        public async Task<List<Book>> GetBooks()
        {
            return await _db.Books
                            .Where(b => 
                                    EF.Functions.Like(b.Title, $"%{BookTitleFilterWord}%") &&
                                    b.PublishDate > CarolusRexAlbumReleaseDate)
                            .ToListAsync();
        }
    }
}