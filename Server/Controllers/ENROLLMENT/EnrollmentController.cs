using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SWARM.EF.Data;
using SWARM.EF.Models;
using SWARM.Server.Models;
using SWARM.Shared;
using SWARM.Shared.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;
namespace SWARM.Server.Controllers.ENROLLMENT
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : Controller
    {
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public EnrollmentController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetEnrollment")]
        public async Task<IActionResult> GetEnrollment()
        {
            List<Enrollment> lstEnrollment = await _context.Enrollments.OrderBy(x => x.SectionId).ToListAsync();
            return Ok(lstEnrollment);
        }

        [HttpGet]
        [Route("GetEnrollment/{pStuID}/{pSecID}/{pSchID}")]
        public async Task<IActionResult> GetEnrollment(int pStuID, int pSecID, int pSchID)
        {
            Enrollment itmEnroll = await _context.Enrollments.Where(
                x => x.StudentId == pStuID
                &&
                x.SectionId == pSecID
                &&
                x.SchoolId == pSchID).FirstOrDefaultAsync();
            return Ok(itmEnroll);
        }

        [HttpDelete]
        [Route("DeleteEnrollment/{pStuID}/{pSecID}/{pSchID}")]
        public async Task<IActionResult> DeleteEnrollment(int pStuID, int pSecID, int pSchID)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                Enrollment itmEnroll = await _context.Enrollments.Where(
                x => x.StudentId == pStuID
                &&
                x.SectionId == pSecID
                &&
                x.SchoolId == pSchID).FirstOrDefaultAsync();
                _context.Remove(itmEnroll);
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
        [Route("PostEnrollment")]
        public async Task<IActionResult> Post([FromBody] EnrollmentDTO _EnrollDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _enroll = await _context.Enrollments.Where(
                    x => x.StudentId == _EnrollDTO.StudentId
                &&
                x.SectionId == _EnrollDTO.SecId
                &&
                x.SchoolId == _EnrollDTO.SchoolId).FirstOrDefaultAsync();


                if (_enroll != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _enroll = new Enrollment();
                _enroll.StudentId = _EnrollDTO.StudentId;
                _enroll.SectionId = _EnrollDTO.SecId;
                _enroll.EnrollDate = _EnrollDTO.EnrollDate;
                _enroll.FinalGrade = _EnrollDTO.FinalGrade;
                _enroll.CreatedBy = _EnrollDTO.CreatedBy;
                _enroll.CreatedDate = _EnrollDTO.CreatedDate;
                _enroll.ModifiedBy = _EnrollDTO.ModifiedBy;
                _enroll.ModifiedDate = _EnrollDTO.ModifiedDate;
                _enroll.SchoolId = _EnrollDTO.SchoolId;
                _context.Enrollments.Add(_enroll);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_EnrollDTO.StudentId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("PutEnrollment")]
        public async Task<IActionResult> Put([FromBody] EnrollmentDTO _EnrollDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _enroll = await _context.Enrollments.Where(
                    x => x.StudentId == _EnrollDTO.StudentId
                &&
                x.SectionId == _EnrollDTO.SecId
                &&
                x.SchoolId == _EnrollDTO.SchoolId).FirstOrDefaultAsync();


                if (_enroll == null)
                {
                    trans.Commit();
                    await this.Post(_EnrollDTO);
                    return Ok();
                }
                _enroll = new Enrollment();
                _enroll.StudentId = _EnrollDTO.StudentId;
                _enroll.SectionId = _EnrollDTO.SecId;
                _enroll.EnrollDate = _EnrollDTO.EnrollDate;
                _enroll.FinalGrade = _EnrollDTO.FinalGrade;
                _enroll.CreatedBy = _EnrollDTO.CreatedBy;
                _enroll.CreatedDate = _EnrollDTO.CreatedDate;
                _enroll.ModifiedBy = _EnrollDTO.ModifiedBy;
                _enroll.ModifiedDate = _EnrollDTO.ModifiedDate;
                _enroll.SchoolId = _EnrollDTO.SchoolId;

                _context.Update(_enroll);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_EnrollDTO.StudentId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



    }
}
