using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWARM.EF.Data;
using SWARM.EF.Models;
using SWARM.Server.Models;
using SWARM.Shared;
using SWARM.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;
namespace SWARM.Server.Controllers.Stu
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public StudentController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetStudents")]
        public async Task<IActionResult> GetStudents()
        {
            List<Student> lstStudents = await _context.Students.OrderBy(x => x.StudentId).ToListAsync();
            return Ok(lstStudents);
        }

        [HttpGet]
        [Route("GetStudent/{pStuId}/{pSchId}")]
        public async Task<IActionResult> GetSchool(int pStuId, int pSchId)
        {
            Student lstStudent = await _context.Students.Where(
                x => x.SchoolId == pSchId && x.StudentId == pStuId).FirstOrDefaultAsync();
            return Ok(lstStudent);
        }

        [HttpDelete]
        [Route("DeleteStudent/{pStuId}/{pSchId}")]
        public async Task<IActionResult> DeleteStudent(int pStuId, int pSchId)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                Student lstStudent = await _context.Students.Where(
                x => x.SchoolId == pSchId && x.StudentId == pStuId).FirstOrDefaultAsync();
                _context.Remove(lstStudent);
                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("PostStudent")]
        public async Task<IActionResult> Post([FromBody] StudentDTO _StudentDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _stu = await _context.Students.Where(
                x => x.SchoolId == _StudentDTO.SchoolId
                &&
                x.StudentId == _StudentDTO.StudentId).FirstOrDefaultAsync();


                if (_stu != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _stu = new Student();
                _stu.StudentId = _StudentDTO.StudentId;
                _stu.Salutation = _StudentDTO.Salutation;
                _stu.FirstName = _StudentDTO.FirstName;
                _stu.LastName = _StudentDTO.LastName;
                _stu.StreetAddress = _StudentDTO.StreetAddress;
                _stu.Zip = _StudentDTO.Zip;
                _stu.Phone = _StudentDTO.Phone;
                _stu.Employer = _StudentDTO.Employer;
                _stu.RegistrationDate = _StudentDTO.RegistrationDate;
                _stu.CreatedBy = _StudentDTO.CreatedBy;
                _stu.CreatedDate = _StudentDTO.CreatedDate;
                _stu.ModifiedBy = _StudentDTO.ModifiedBy;
                _stu.ModifiedDate = _StudentDTO.ModifiedDate;
                _context.Students.Add(_stu);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_StudentDTO.StudentId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    [HttpPut]
    [Route("PutStudent")]
    public async Task<IActionResult> Put([FromBody] StudentDTO _StudentDTO)
    {
        var trans = _context.Database.BeginTransaction();
        try
        {
            var _stu = await _context.Students.Where(
            x => x.SchoolId == _StudentDTO.SchoolId
            &&
            x.StudentId == _StudentDTO.StudentId).FirstOrDefaultAsync();


            if (_stu == null)
            {
                trans.Commit();
                    await this.Post(_StudentDTO);
                    return Ok();
            }
            _stu.StudentId = _StudentDTO.StudentId;
            _stu.Salutation = _StudentDTO.Salutation;
            _stu.FirstName = _StudentDTO.FirstName;
            _stu.LastName = _StudentDTO.LastName;
            _stu.StreetAddress = _StudentDTO.StreetAddress;
            _stu.Zip = _StudentDTO.Zip;
            _stu.Phone = _StudentDTO.Phone;
            _stu.Employer = _StudentDTO.Employer;
            _stu.RegistrationDate = _StudentDTO.RegistrationDate;
            _stu.CreatedBy = _StudentDTO.CreatedBy;
            _stu.CreatedDate = _StudentDTO.CreatedDate;
            _stu.ModifiedBy = _StudentDTO.ModifiedBy;
            _stu.ModifiedDate = _StudentDTO.ModifiedDate;

            _context.Students.Update(_stu);
            await _context.SaveChangesAsync();
            trans.Commit();

            return Ok(_StudentDTO.StudentId);
        }
        catch (Exception ex)
        {
            trans.Rollback();
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
}
