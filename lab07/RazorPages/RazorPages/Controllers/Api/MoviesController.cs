using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RazorPages.Data;
using RazorPages.Models;

namespace RazorPages.Controllers.Api
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesControllerApi : ControllerBase
    {
        private readonly MoviesContext _context;
        public List<Movie> Movies;

        public MoviesControllerApi(MoviesContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> Index([FromQuery] string? search = null, [FromQuery] int count = 100, [FromQuery] int offset = 0)
        {
            IQueryable<Movie> allQuery = _context.Movies.OrderBy(x => x.Id);
            if (!string.IsNullOrEmpty(search))
            {
                allQuery = allQuery.Where(x => x.Title.ToLower().Contains(search.ToLower()));
            }
            var portionQuery = allQuery.Skip(offset).Take(count);
            var movies = await portionQuery.ToListAsync();
            return Ok(movies.Select(m => new Movie
            {
                Id = m.Id,
                Title = m.Title,
                VoteAverage = m.VoteAverage,
                VoteCount = m.VoteCount
            }));
        }

        [Authorize(Roles = "User", AuthenticationSchemes = "Bearer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.Include("Persons")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }


            return Ok(movie);
        }

        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                movie.Id = _context.Movies.Select(m => m.Id).Max() + 1;
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Edit(int? id, Movie movie)
        {
            movie.Id = id.Value;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return StatusCode(500, "Internal Database Error");
                    }
                }
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (_context.Movies == null)
            {
                return NotFound();
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                var mps = _context.MoviePersons.Where(x => x.MovieId == movie.Id);
                if (mps.Any())
                {
                    _context.MoviePersons.RemoveRange(mps);
                    await _context.SaveChangesAsync();
                }
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

        private bool MovieExists(int? id)
        {
            return (_context.Movies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
