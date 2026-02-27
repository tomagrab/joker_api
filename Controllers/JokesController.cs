using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using joker_api.Data.Context;
using joker_api.Models.Entities;

namespace joker_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JokesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JokesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Jokes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JokeEntity>>> GetJokes()
        {
            return await _context.Jokes.ToListAsync();
        }

        // GET: api/Jokes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JokeEntity>> GetJokeEntity(string id)
        {
            var jokeEntity = await _context.Jokes.FindAsync(id);

            if (jokeEntity == null)
            {
                return NotFound();
            }

            return jokeEntity;
        }

        // PUT: api/Jokes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJokeEntity(string id, JokeEntity jokeEntity)
        {
            if (id != jokeEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(jokeEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JokeEntityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Jokes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JokeEntity>> PostJokeEntity(JokeEntity jokeEntity)
        {
            _context.Jokes.Add(jokeEntity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (JokeEntityExists(jokeEntity.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetJokeEntity", new { id = jokeEntity.Id }, jokeEntity);
        }

        // DELETE: api/Jokes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJokeEntity(string id)
        {
            var jokeEntity = await _context.Jokes.FindAsync(id);
            if (jokeEntity == null)
            {
                return NotFound();
            }

            _context.Jokes.Remove(jokeEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JokeEntityExists(string id)
        {
            return _context.Jokes.Any(e => e.Id == id);
        }
    }
}
