using Blogify_API.Datas;
using Blogify_API.Dtos;
using Blogify_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Blogify_API.Services
{
    public class AuthorService: IAuthorService
    {
        private readonly AppDbContext _context;
        public  AuthorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuthorDto>> GetAuthors()
        {
            var authors= await _context.Authors.ToListAsync();
            var authorDtos = new List<AuthorDto>();
            foreach (var author in authors)
            {
                var user = await _context.Users.FirstOrDefaultAsync(user => user.Id == author.UserId);
                var authorDto = new AuthorDto
                {
                    FullName = user.FullName,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender,
                    Posts = author.Posts,
                    Likes = author.Likes,
                    Created = user.CreatedTime
                };

                authorDtos.Add(authorDto);
            }

            return authorDtos;
        }
    }
}
