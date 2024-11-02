using Microsoft.AspNetCore.Mvc;
using score_api.Models;

namespace score_api.Controllers
{
    [ApiController]
    [Route("score/[action]")]
    public class TutorialController(MyDbContext context) : ControllerBase
    {
        private readonly MyDbContext _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            IQueryable<Score> result = _context.Scores
                .OrderByDescending(x => x.CreatedAt);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult AddScore(Score score)
        {
            Score scoreSchema = new Score()
            {
                Name = score.Name,
                Module = score.Module,
                Mark = score.Mark,
                CreatedAt = DateTime.Now
            };

            if (score.Mark < 0 || score.Mark > 100)
            {
                return BadRequest("Score must be between 0 and 100");
            }

            bool hasDuplicateScore = _context.Scores
                .Any(score1 => score1.Name == score.Name
                               && score1.Module == score.Module
                               && score1.Mark == score.Mark
                );

            if (hasDuplicateScore)
            {
                return BadRequest("Already exists!");
            }

            _context.Scores.Add(scoreSchema);
            _context.SaveChanges();
            return Ok(scoreSchema);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetScoreById(int id)
        {
            Score? scoreSchema = _context.Scores.Find(id);
            if (scoreSchema == null)
            {
                return NotFound("Score with id " + id + " not found");
            }

            return Ok(scoreSchema);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteScoreById(int id)
        {
            Score? scoreSchema = _context.Scores.Find(id);
            if (scoreSchema == null)
            {
                return NotFound("Score with id " + id + " not found");
            }

            _context.Scores.Remove(scoreSchema);
            _context.SaveChanges();
            return Ok(scoreSchema.Id + " has been removed!");
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateScoreById(int id, Score score)
        {
            Score? scoreSchema = _context.Scores.Find(id);
            if (scoreSchema == null)
            {
                return NotFound("Score with id " + id + " not found");
            }
            
            scoreSchema.Name = score.Name;
            scoreSchema.Mark = score.Mark;
            scoreSchema.Module = score.Module;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet]
        [Route("{name}")]
        public IActionResult GetScoreByName(string name)
        {
            IQueryable<Score> result = _context.Scores
                .Where(scoreSchema => scoreSchema.Name.ToLower().Equals(name.ToLower()));
            return Ok(result);
        }

        [HttpGet]
        [Route("{module}")]
        public IActionResult GetGradeAScoreByModule(string module)
        {
            if (string.IsNullOrWhiteSpace(module))
            {
                return BadRequest("Module is required!");
            }

            var result = _context.Scores
                .Where(score => score.Module.Equals(module))
                .Where(score => score.Mark >= 80)
                .Select(score => new { score.Name, score.Mark })
                .OrderByDescending(score => score.Mark);

            if (result.Count() == 0)
            {
                return NotFound("No such a module - " + module + " !");
            }

            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAverageGradeOfEachModule()
        {
            var result = _context.Scores
                .GroupBy(score => score.Module)
                .Select(group => new { Module = group.Key, Mark = group.Average(score => score.Mark) })
                .OrderByDescending(score => score.Mark)
                .ToList();

            return Ok(result);
        }
    }
}