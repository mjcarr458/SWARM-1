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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;

namespace SWARM.Server.Controllers.Grds
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : Controller
    {
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public GradeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetGrades")]
        public async Task<IActionResult> GetGrades()
        {
            List<Grade> lstGrades = await _context.Grades.OrderBy(x => x.StudentId).ToListAsync();
            return Ok(lstGrades);
        }

        [HttpGet]
        [Route("GetGrade/{pStuID}/{pSecID}/{pSchID}/{pGrdT}/{pGrdC}")]
        public async Task<IActionResult> GetGrade(int pStuID, int pSecID, int pSchID,byte pGrdT, string pGrdC)
        {
            Grade itmGrd = await _context.Grades.Where(
                x => x.StudentId == pStuID
                &&
                x.SectionId == pSecID
                &&
                x.SchoolId == pSchID
                &&
                x.GradeTypeCode == pGrdC
                &&
                x.GradeCodeOccurrence == pGrdT).FirstOrDefaultAsync();
            return Ok(itmGrd);
        }

        [HttpDelete]
        [Route("DeleteGrade/{pStuID}/{pSecID}/{pSchID}/{pGrdT}/{pGrdC}")]
        public async Task<IActionResult> DeleteGrade(int pStuID, int pSecID, int pSchID, byte pGrdT, string pGrdC)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                Grade itmGrd = await _context.Grades.Where(
                x => x.StudentId == pStuID
                &&
                x.SectionId == pSecID
                &&
                x.SchoolId == pSchID
                &&
                x.GradeTypeCode == pGrdC
                &&
                x.GradeCodeOccurrence == pGrdT).FirstOrDefaultAsync();
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
        [Route("PostGrade")]
        public async Task<IActionResult> Post([FromBody] GradeDTO _GradeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grd = await _context.Grades.Where(
                x => x.StudentId == _GradeDTO.StudentId
                &&
                x.SectionId == _GradeDTO.SectionId
                &&
                x.SchoolId == _GradeDTO.SchoolId
                &&
                x.GradeTypeCode == _GradeDTO.GradeTypeCode
                &&
                x.GradeCodeOccurrence == _GradeDTO.GradeCodeOccurrence).FirstOrDefaultAsync();


                if (_grd != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _grd = new Grade();
                _grd.StudentId = _GradeDTO.StudentId;
                _grd.SectionId = _GradeDTO.SectionId;
                _grd.SchoolId = _GradeDTO.SchoolId;
                _grd.GradeTypeCode = _GradeDTO.GradeTypeCode;
                _grd.GradeCodeOccurrence = _GradeDTO.GradeCodeOccurrence;
                _grd.NumericGrade = _GradeDTO.NumericGrade;
                _grd.Comments = _GradeDTO.Comments;
                _grd.CreatedBy = _GradeDTO.CreatedBy;
                _grd.CreatedDate = _GradeDTO.CreatedDate;
                _grd.ModifiedBy = _GradeDTO.ModifiedBy;
                _grd.ModifiedDate = _GradeDTO.ModifiedDate;
                _context.Grades.Add(_grd);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_GradeDTO.StudentId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("PutGrade")]
        public async Task<IActionResult> Put([FromBody] GradeDTO _GradeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grd = await _context.Grades.Where(
                x => x.StudentId == _GradeDTO.StudentId
                &&
                x.SectionId == _GradeDTO.SectionId
                &&
                x.SchoolId == _GradeDTO.SchoolId
                &&
                x.GradeTypeCode == _GradeDTO.GradeTypeCode
                &&
                x.GradeCodeOccurrence == _GradeDTO.GradeCodeOccurrence).FirstOrDefaultAsync();


                if (_grd == null)
                {
                    trans.Commit();
                    await this.Post(_GradeDTO);
                    return Ok();
                }
                _grd.StudentId = _GradeDTO.StudentId;
                _grd.SectionId = _GradeDTO.SectionId;
                _grd.SchoolId = _GradeDTO.SchoolId;
                _grd.GradeTypeCode = _GradeDTO.GradeTypeCode;
                _grd.GradeCodeOccurrence = _GradeDTO.GradeCodeOccurrence;
                _grd.NumericGrade = _GradeDTO.NumericGrade;
                _grd.Comments = _GradeDTO.Comments;
                _grd.CreatedBy = _GradeDTO.CreatedBy;
                _grd.CreatedDate = _GradeDTO.CreatedDate;
                _grd.ModifiedBy = _GradeDTO.ModifiedBy;
                _grd.ModifiedDate = _GradeDTO.ModifiedDate;

                _context.Update(_grd);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_GradeDTO.StudentId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
