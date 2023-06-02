using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDBSetup.Kafka;
using MongoDBSetup.Models;
using MongoDBSetup.Services;

namespace MovieBookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _StudentService;
        private readonly IKafkaProducerService _kafkaProducer;
        private const string TOPIC1 = "topic_get_student_data";
        private const string TOPIC2 = "topic_post_student_data";

        public StudentsController(IStudentService studentService, IKafkaProducerService kafkaProducer)
        {
            _StudentService = studentService;
            _kafkaProducer = kafkaProducer;
        }

        // GET: api/<StudentsController>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public ActionResult<List<Student>> Get()
        {
            var students = _StudentService.Get();
            _kafkaProducer.SendToTopicAsync(TOPIC1, $"Students[Count={students.Count}] fetched.").GetAwaiter().GetResult();
            return Ok(students);
        }

        // GET api/<StudentsController>/5
        [HttpGet("{id}")]
        public ActionResult<Student> Get(string id)
        {
            var student =  _StudentService.Get(id);
            if (student == null)
            {
                _kafkaProducer.SendToTopicAsync(TOPIC1, $"One Student['{id}] NOT fetched.").GetAwaiter().GetResult();
                return NotFound($"Student with ID '${id}' not found!");
            }
            _kafkaProducer.SendToTopicAsync(TOPIC1, $"One Student['{student.Id}] fetched.").GetAwaiter().GetResult();
            return student;
        }

        // POST api/<StudentsController>
        [HttpPost]
        public ActionResult<Student> Post([FromBody] Student student)
        {
            _StudentService.Create(student);
            _kafkaProducer.SendToTopicAsync(TOPIC2, $"New Student['{student.Id}'] inserted.").GetAwaiter().GetResult();
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
