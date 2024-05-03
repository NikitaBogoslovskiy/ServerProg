using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorPages.Data;
using RazorPages.Models;

namespace RazorPages.Controllers.Api
{
    [Route("api/persons")]
    [ApiController]
    public class PersonsControllerApi : ControllerBase
    {
        private readonly MoviesContext _context;

        public PersonsControllerApi(MoviesContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public async Task<IActionResult> Index([FromQuery] int count = 100, [FromQuery] int offset = 0)
        {
            var persons = await _context.Persons.ToListAsync();
            return Ok(persons.Skip(offset).Take(count).Select(m => new Person
            {
                Id = m.Id,
                Name = m.Name
            }));
        }

        //[Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.Include("Movies")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return Ok(person);
        }

        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(Person person)
        {
            if (ModelState.IsValid)
            {
                person.Id = _context.Persons.Select(m => m.Id).Max() + 1;
                _context.Add(person);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpPut("update/{id}")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id, Person person)
        {
            person.Id = id.Value;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
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
            if (_context.Persons == null)
            {
                return NotFound();
            }
            var person = await _context.Persons.FindAsync(id);
            if (person != null)
            {
                var mps = _context.MoviePersons.Where(x => x.PersonId == person.Id);
                if (mps.Any())
                {
                    _context.MoviePersons.RemoveRange(mps);
                    await _context.SaveChangesAsync();
                }
                _context.Persons.Remove(person);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

        private bool PersonExists(int id)
        {
            return (_context.Persons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
