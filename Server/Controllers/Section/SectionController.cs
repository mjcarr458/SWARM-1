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
namespace SWARM.Server.Controllers.Sect
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : Controller
    {
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public SectionController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetSections")]
        public async Task<IActionResult> GetSections()
        {
            List<Section> lstSection = await _context.Sections.OrderBy(x => x.SectionId).ToListAsync();
            return Ok(lstSection);
        }

        [HttpGet]
        [Route("GetSection/{pSecId}/{pSchId}")]
        public async Task<IActionResult> GetSchool(int pSecId, int pSchId)
        {
            Section lstSection = await _context.Sections.Where(
                x => x.SchoolId == pSchId && x.SectionId == pSecId).FirstOrDefaultAsync();
            return Ok(lstSection);
        }

        [HttpDelete]
        [Route("DeleteSection/{pSecId}/{pSchId}")]
        public async Task<IActionResult> DeleteSchool(int pSecId, int pSchId)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                Section lstSection = await _context.Sections.Where(
                x => x.SchoolId == pSchId && x.SectionId == pSecId).FirstOrDefaultAsync();
                _context.Remove(lstSection);
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
        [Route("PostSection")]
        public async Task<IActionResult> Post([FromBody] SectionDTO _SectionDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _sec = await _context.Sections.Where(
                x => x.SchoolId == _SectionDTO.SchoolId
                &&
                x.SectionId == _SectionDTO.SectionId).FirstOrDefaultAsync();


                if (_sec != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _sec = new Section();
                _sec.SectionId = _SectionDTO.SectionId;
                _sec.CourseNo = _SectionDTO.CourseNo;
                _sec.SectionNo = _SectionDTO.SectionNo;
                _sec.StartDateTime = _SectionDTO.StartDateTime;
                _sec.Location = _SectionDTO.Location;
                _sec.InstructorId = _SectionDTO.InstructorId;
                _sec.Capacity = _SectionDTO.Capacity;
                _sec.CreatedBy = _SectionDTO.CreatedBy;
                _sec.CreatedDate = _SectionDTO.CreatedDate;
                _sec.ModifiedBy = _SectionDTO.ModifiedBy;
                _sec.ModifiedDate = _SectionDTO.ModifiedDate;
                _context.Sections.Add(_sec);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_SectionDTO.SectionId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("PutSection")]
        public async Task<IActionResult> Put([FromBody] SectionDTO _SectionDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _sec = await _context.Sections.Where(
                x => x.SchoolId == _SectionDTO.SchoolId
                &&
                x.SectionId == _SectionDTO.SectionId).FirstOrDefaultAsync();


                if (_sec == null)
                {
                    trans.Commit();
                    await this.Post(_SectionDTO);
                    return Ok();
                }
                _sec.SectionId = _SectionDTO.SectionId;
                _sec.CourseNo = _SectionDTO.CourseNo;
                _sec.SectionNo = _SectionDTO.SectionNo;
                _sec.StartDateTime = _SectionDTO.StartDateTime;
                _sec.Location = _SectionDTO.Location;
                _sec.InstructorId = _SectionDTO.InstructorId;
                _sec.Capacity = _SectionDTO.Capacity;
                _sec.CreatedBy = _SectionDTO.CreatedBy;
                _sec.CreatedDate = _SectionDTO.CreatedDate;
                _sec.ModifiedBy = _SectionDTO.ModifiedBy;
                _sec.ModifiedDate = _SectionDTO.ModifiedDate;

                _context.Sections.Update(_sec);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_SectionDTO.SectionId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);


            }
        }
    }
}
