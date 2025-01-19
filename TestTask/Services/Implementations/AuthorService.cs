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
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _db;
        private static int BookPublishDateFilter = 2015;
        public AuthorService(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Author> GetAuthor()
        {
            var longestBook = await _db.Books
                                    .OrderByDescending(b => b.Title.Length)
                                    .ThenBy(b => b.AuthorId)
                                    .FirstOrDefaultAsync(); 

            var author = await _db.Authors
                                .FirstOrDefaultAsync(a => a.Id == longestBook.AuthorId);
            
            author.Books = null;

            return author;
        }

        public async Task<List<Author>> GetAuthors()
        {
            var authorsWithBooksAfter2015 = await _db.Books
                                                    .Where(b => b.PublishDate.Year > BookPublishDateFilter)
                                                    .GroupBy(b => b.AuthorId)
                                                    .Select(g => new
                                                    {
                                                        AuthorId = g.Key,
                                                        BookCount = g.Count()
                                                    })
                                                    .Where(x => x.BookCount % 2 == 0)
                                                    .ToListAsync();

           
            var authorIds = authorsWithBooksAfter2015
                            .Select(x => x.AuthorId)
                            .ToList();

            var authors = _db.Authors
                            .Where(a => authorIds.Contains(a.Id))
                            .ToListAsync();

            foreach (var author in await authors)
            {
                author.Books = null;
            }
                    
            return await authors;
        }
    }
}