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
namespace SWARM.Server.Controllers.GrdTyp
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeTypeController : Controller
    {
        
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public GradeTypeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetGradeTypes")]
        public async Task<IActionResult> GetGrades()
        {
            List<GradeType> lstGrades = await _context.GradeTypes.OrderBy(x => x.GradeTypeCode).ToListAsync();
            return Ok(lstGrades);
        }

        [HttpGet]
        [Route("GetGradeType/{pSchID}/{pGrdC}")]
        public async Task<IActionResult> GetGrade(int pSchID, string pGrdC)
        {
            GradeType itmGrd = await _context.GradeTypes.Where(
                x => x.SchoolId == pSchID
                &&
                x.GradeTypeCode == pGrdC).FirstOrDefaultAsync();
            return Ok(itmGrd);
        }

        [HttpDelete]
        [Route("DeleteGrade/{pSchID}/{pGrdC}")]
        public async Task<IActionResult> DeleteGrade(int pSchID, string pGrdC)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                GradeType itmGrd = await _context.GradeTypes.Where(
                x => x.SchoolId == pSchID
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
        [Route("PostGradeType")]
        public async Task<IActionResult> Post([FromBody] GradeTypeDTO _GradeTypeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grd = await _context.GradeTypes.Where(
                x => x.SchoolId == _GradeTypeDTO.SchoolId
                &&
                x.GradeTypeCode == _GradeTypeDTO.GradeTypeCode).FirstOrDefaultAsync();


                if (_grd != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _grd = new GradeType();
                _grd.SchoolId = _GradeTypeDTO.SchoolId;
                _grd.GradeTypeCode = _GradeTypeDTO.GradeTypeCode;
                _grd.Description = _GradeTypeDTO.Description;
                _grd.CreatedBy = _GradeTypeDTO.CreatedBy;
                _grd.CreatedDate = _GradeTypeDTO.CreatedDate;
                _grd.ModifiedBy = _GradeTypeDTO.ModifiedBy;
                _grd.ModifiedDate = _GradeTypeDTO.ModifiedDate;
                _context.GradeTypes.Add(_grd);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_GradeTypeDTO.GradeTypeCode);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("PostGradeType")]
        public async Task<IActionResult> Put([FromBody] GradeTypeDTO _GradeTypeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _grd = await _context.GradeTypes.Where(
                x => x.SchoolId == _GradeTypeDTO.SchoolId
                &&
                x.GradeTypeCode == _GradeTypeDTO.GradeTypeCode).FirstOrDefaultAsync();


                if (_grd == null)
                {
                    trans.Commit();
                    await this.Post(_GradeTypeDTO);
                    return Ok();
                }
                _grd.SchoolId = _GradeTypeDTO.SchoolId;
                _grd.GradeTypeCode = _GradeTypeDTO.GradeTypeCode;
                _grd.Description = _GradeTypeDTO.Description;
                _grd.CreatedBy = _GradeTypeDTO.CreatedBy;
                _grd.CreatedDate = _GradeTypeDTO.CreatedDate;
                _grd.ModifiedBy = _GradeTypeDTO.ModifiedBy;
                _grd.ModifiedDate = _GradeTypeDTO.ModifiedDate;

                _context.GradeTypes.Update(_grd);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_GradeTypeDTO.GradeTypeCode);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }







    }
}
