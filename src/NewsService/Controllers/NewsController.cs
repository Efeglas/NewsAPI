using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsService.Data;
using NewsService.DTOs;
using NewsService.Models;
using System.Security.Claims;

namespace NewsService.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly NewsDbContext _context;

        public NewsController(NewsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<GetNewsDTO>> GetNews([FromQuery] SearchParams searchParams) {

            int count = await _context.News.CountAsync();
            int pageSize = searchParams.PageSize;
            int pageCount = count / pageSize + (count % pageSize == 0 ? 0 : 1);
            int pageNumber = searchParams.PageNumber;

            if (pageCount < 1)
            {
                pageCount = 1;
            }

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageNumber > pageCount)
            {
                pageNumber = pageCount;
            }

            List<News> news = null;       

            if (searchParams.SearchTerm != string.Empty)
            {
                string searchStr = searchParams.SearchTerm.ToLower();
                news = await _context.News
                    .Where(n => n.Author.ToLower().Contains(searchStr) || n.Title.ToLower().Contains(searchStr) || n.Description.ToLower().Contains(searchStr)).OrderBy(x => x.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else 
            {
                news = await _context.News
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }

            return new GetNewsDTO(news, pageCount, count);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<News>> GetNewsById(Guid id)
        {
            News news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);

            if (news == null)
            {
                return NotFound();
            }
            
            return Ok(news);
        }     

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<News>> CreateNews(CreateNewsDTO createDTO)
        {

            Claim nameClaim = HttpContext.User.FindFirst(ClaimTypes.Name);

            News news;
            if (createDTO != null)
            {
                news = new News(createDTO.Title, createDTO.Description, createDTO.ImageUrl, nameClaim.Value);
                _context.News.Add(news);
                bool result = await _context.SaveChangesAsync() > 0;

                if (!result)
                {
                    return BadRequest("Could not save changes to the database");
                }

                return CreatedAtAction(nameof(GetNewsById), new { news.Id }, news);
            }

            return BadRequest("Could not save changes to the database");
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult> UpdateNews(Guid id, UpdateNewsDTO updateDTO)
        {
            Claim nameClaim = HttpContext.User.FindFirst(ClaimTypes.Name);
            News news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            if (news.Author != nameClaim.Value)
            {
                return Unauthorized();
            }

            news.Title = updateDTO.Title ?? news.Title;
            news.Description = updateDTO.Description ?? news.Description;
            news.ImageUrl = updateDTO.ImageUrl ?? news.ImageUrl;
            news.UpdatedAt = DateTime.Now.ToUniversalTime();

            bool result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                return BadRequest("Problem saving changes");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteNews(Guid id)
        {
            Claim nameClaim = HttpContext.User.FindFirst(ClaimTypes.Name);
            News news = await _context.News.FirstOrDefaultAsync(n => n.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            if (news.Author != nameClaim.Value)
            {
                return Unauthorized();
            }

            _context.News.Remove(news);

            bool result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                return BadRequest("Could not delete from database");
            }

            return Ok();
        }
    }
}
