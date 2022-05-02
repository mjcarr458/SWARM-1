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
namespace SWARM.Server.Controllers.Instr
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : Controller
    {
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public InstructorController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetInstructors")]
        public async Task<IActionResult> GetInstructors()
        {
            List<Instructor> lstGrades = await _context.Instructors.OrderBy(x => x.InstructorId).ToListAsync();
            return Ok(lstGrades);
        }

        [HttpGet]
        [Route("GetInstructor/{pInsId}/{pSchId}")]
        public async Task<IActionResult> GetInstructor(int pInsId, int pSchId)
        {
            Instructor lstGrades = await _context.Instructors.Where(
                x => x.SchoolId == pSchId
                &&
                x.InstructorId == pInsId).FirstOrDefaultAsync();
            return Ok(lstGrades);
        }

        [HttpDelete]
        [Route("DeleteInstructor/{pInsId}/{pSchID}")]
        public async Task<IActionResult> DeleteInstructor(int pInsId,int pSchID)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                Instructor itmGrd = await _context.Instructors.Where(
                x => x.SchoolId == pSchID
                &&
                x.InstructorId == pInsId).FirstOrDefaultAsync();
                _context.Remove(itmGrd);
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
        [Route("PostInstructor")]
        public async Task<IActionResult> Post([FromBody] InstructorDTO _GradeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grd = await _context.Instructors.Where(
                x => x.SchoolId == _GradeDTO.SchoolId
                &&
                x.InstructorId == _GradeDTO.InstructorId).FirstOrDefaultAsync();


                if (_grd != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _grd = new Instructor();
                _grd.SchoolId = _GradeDTO.SchoolId;
                _grd.InstructorId = _GradeDTO.InstructorId;
                _grd.Salutation = _GradeDTO.Salutation;
                _grd.FirstName = _GradeDTO.FirstName;
                _grd.LastName = _GradeDTO.LastName;
                _grd.StreetAddress = _GradeDTO.StreetAddress;
                _grd.Zip = _GradeDTO.Zip;
                _grd.Phone = _GradeDTO.Phone;
                _grd.CreatedBy = _GradeDTO.CreatedBy;
                _grd.CreatedDate = _GradeDTO.CreatedDate;
                _grd.ModifiedBy = _GradeDTO.ModifiedBy;
                _grd.ModifiedDate = _GradeDTO.ModifiedDate;
                _context.Instructors.Add(_grd);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_GradeDTO.InstructorId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("PutInstructor")]
        public async Task<IActionResult> Put([FromBody] InstructorDTO _GradeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grd = await _context.Instructors.Where(
                x => x.SchoolId == _GradeDTO.SchoolId
                &&
                x.InstructorId == _GradeDTO.InstructorId).FirstOrDefaultAsync();


                if (_grd == null)
                {
                    trans.Commit();
                    await this.Post(_GradeDTO);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _grd.SchoolId = _GradeDTO.SchoolId;
                _grd.InstructorId = _GradeDTO.InstructorId;
                _grd.Salutation = _GradeDTO.Salutation;
                _grd.FirstName = _GradeDTO.FirstName;
                _grd.LastName = _GradeDTO.LastName;
                _grd.StreetAddress = _GradeDTO.StreetAddress;
                _grd.Zip = _GradeDTO.Zip;
                _grd.Phone = _GradeDTO.Phone;
                _grd.CreatedBy = _GradeDTO.CreatedBy;
                _grd.CreatedDate = _GradeDTO.CreatedDate;
                _grd.ModifiedBy = _GradeDTO.ModifiedBy;
                _grd.ModifiedDate = _GradeDTO.ModifiedDate;
                _context.Instructors.Update(_grd);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_GradeDTO.InstructorId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
