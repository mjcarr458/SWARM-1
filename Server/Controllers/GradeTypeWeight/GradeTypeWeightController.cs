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
namespace SWARM.Server.Controllers.GrdTypWght
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeTypeWeightController : Controller
    {
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public GradeTypeWeightController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetGradeTypeWeights")]
        public async Task<IActionResult> GetGradeTypeWeights()
        {
            List<GradeTypeWeight> lstGrades = await _context.GradeTypeWeights.OrderBy(x => x.SectionId).ToListAsync();
            return Ok(lstGrades);
        }

        [HttpGet]
        [Route("GetGradeTypeWeight/{pStuID}/{pSecID}/{pSchID}/{pGrdT}/{pGrdC}")]
        public async Task<IActionResult> GetGradeTypeWeight( int pSecID, int pSchID, string pGrdC)
        {
            GradeTypeWeight itmGrd = await _context.GradeTypeWeights.Where(
                x => x.SectionId == pSecID
                &&
                x.SchoolId == pSchID
                &&
                x.GradeTypeCode == pGrdC).FirstOrDefaultAsync();
            return Ok(itmGrd);
        }

        [HttpDelete]
        [Route("DeleteGradeTypeWeight/{pStuID}/{pSecID}/{pSchID}/{pGrdT}/{pGrdC}")]
        public async Task<IActionResult> DeleteInstructor(int pSecID, int pSchID, string pGrdC)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                GradeTypeWeight itmGrd = await _context.GradeTypeWeights.Where(
                x => x.SectionId == pSecID
                &&
                x.SchoolId == pSchID
                &&
                x.GradeTypeCode == pGrdC).FirstOrDefaultAsync();
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
        [Route("PostGradeTypeWeight")]
        public async Task<IActionResult> Post([FromBody] GradeTypeWeightDTO _GradeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grd = await _context.GradeTypeWeights.Where(
                x => x.SectionId == _GradeDTO.SectionId
                &&
                x.SchoolId == _GradeDTO.SchoolId
                &&
                x.GradeTypeCode == _GradeDTO.GradeTypeCode).FirstOrDefaultAsync();


                if (_grd != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _grd = new GradeTypeWeight();
                _grd.SectionId = _GradeDTO.SectionId;
                _grd.SchoolId = _GradeDTO.SchoolId;
                _grd.GradeTypeCode = _GradeDTO.GradeTypeCode;
                _grd.NumberPerSection = _GradeDTO.NumberPerSection;
                _grd.PercentOfFinalGrade = _GradeDTO.PercentOfFinalGrade;
                _grd.DropLowest = _GradeDTO.DropLowest;
                _grd.CreatedBy = _GradeDTO.CreatedBy;
                _grd.CreatedDate = _GradeDTO.CreatedDate;
                _grd.ModifiedBy = _GradeDTO.ModifiedBy;
                _grd.ModifiedDate = _GradeDTO.ModifiedDate;
                _context.GradeTypeWeights.Add(_grd);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_GradeDTO.SectionId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("PutGradeTypeWeight")]
        public async Task<IActionResult> Put([FromBody] GradeTypeWeightDTO _GradeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grd = await _context.GradeTypeWeights.Where(
                x => x.SectionId == _GradeDTO.SectionId
                &&
                x.SchoolId == _GradeDTO.SchoolId
                &&
                x.GradeTypeCode == _GradeDTO.GradeTypeCode).FirstOrDefaultAsync();


                if (_grd == null)
                {
                    trans.Commit();
                    await this.Post(_GradeDTO);
                    return Ok();
                }
                _grd.SectionId = _GradeDTO.SectionId;
                _grd.SchoolId = _GradeDTO.SchoolId;
                _grd.GradeTypeCode = _GradeDTO.GradeTypeCode;
                _grd.NumberPerSection = _GradeDTO.NumberPerSection;
                _grd.PercentOfFinalGrade = _GradeDTO.PercentOfFinalGrade;
                _grd.DropLowest = _GradeDTO.DropLowest;
                _grd.CreatedBy = _GradeDTO.CreatedBy;
                _grd.CreatedDate = _GradeDTO.CreatedDate;
                _grd.ModifiedBy = _GradeDTO.ModifiedBy;
                _grd.ModifiedDate = _GradeDTO.ModifiedDate;
                _context.GradeTypeWeights.Update(_grd);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_GradeDTO.SectionId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
