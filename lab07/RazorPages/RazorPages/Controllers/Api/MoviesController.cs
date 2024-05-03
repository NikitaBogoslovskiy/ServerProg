using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Index([FromQuery] int count = 100, [FromQuery] int offset = 0)
        {
            var movies = await _context.Movies.ToListAsync();
            return Ok(movies.Skip(offset).Take(count).Select(m => new Movie
            {
                Id = m.Id,
                Title = m.Title,
                VoteAverage = m.VoteAverage,
                VoteCount = m.VoteCount
            }));
        }

        //[Authorize(Roles = "User")]
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

        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
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
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
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
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
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
