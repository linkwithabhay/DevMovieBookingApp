using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDBSetup.Models;
using MongoDBSetup.Services;

namespace MovieBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _StudentService;

        public StudentsController(IStudentService studentService)
        {
            _StudentService = studentService;
        }

        // GET: api/<StudentsController>
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult<List<Student>> Get()
        {
            return Ok(_StudentService.Get());
        }

        // GET api/<StudentsController>/5
        [HttpGet("{id}")]
        public ActionResult<Student> Get(string id)
        {
            var student =  _StudentService.Get(id);
            if (student == null) return NotFound($"Student with ID '${id}' not found!");
            return student;
        }

        // POST api/<StudentsController>
        [HttpPost]
        public ActionResult<Student> Post([FromBody] Student student)
        {
            _StudentService.Create(student);
            return CreatedAtAction(nameof(Get), new {id = student.Id}, student);
        }

        // PUT api/<StudentsController>/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Student student)
        {
            var existingStudent = _StudentService.Get(id);
            if (existingStudent == null) return NotFound($"Student with ID '${id}' not found!");
            _StudentService.Update(id, student);
            return CreatedAtAction(nameof(Get), new { id = existingStudent.Id }, student);
        }

        // DELETE api/<StudentsController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var existingStudent = _StudentService.Get(id);
            if (existingStudent == null) return NotFound($"Student with ID '${id}' not found!");
            _StudentService.Delete(id);
            return StatusCode(204, $"Student with ID '${id}' deleted.");
        }
    }
}
