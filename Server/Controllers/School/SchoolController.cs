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

namespace SWARM.Server.Controllers.Schl
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolController : Controller
    {
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public SchoolController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetSchools")]
        public async Task<IActionResult> GetSchools()
        {
            List<School> lstGrades = await _context.Schools.OrderBy(x => x.SchoolId).ToListAsync();
            return Ok(lstGrades);
        }

        [HttpGet]
        [Route("GetSchool/{pSchId}")]
        public async Task<IActionResult> GetSchool(int pSchId)
        {
            School lstGrades = await _context.Schools.Where(
                x => x.SchoolId == pSchId).FirstOrDefaultAsync();
            return Ok(lstGrades);
        }


        [HttpDelete]
        [Route("DeleteSchool/{pSchID}")]
        public async Task<IActionResult> DeleteSchool(int pSchID, string pGrdC)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                School itmGrd = await _context.Schools.Where(
                x => x.SchoolId == pSchID).FirstOrDefaultAsync();
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
        [Route("PostSchool")]
        public async Task<IActionResult> Post([FromBody] SchoolDTO _SchoolDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _sch = await _context.Schools.Where(
                x => x.SchoolId == _SchoolDTO.SchoolId).FirstOrDefaultAsync();


                if (_sch != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _sch = new School();
                _sch.SchoolId = _SchoolDTO.SchoolId;
                _sch.SchoolName = _SchoolDTO.SchoolName;
                _sch.CreatedBy = _SchoolDTO.CreatedBy;
                _sch.CreatedDate = _SchoolDTO.CreatedDate;
                _sch.ModifiedBy = _SchoolDTO.ModifiedBy;
                _sch.ModifiedDate = _SchoolDTO.ModifiedDate;
                _context.Schools.Add(_sch);

                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok(_SchoolDTO.SchoolId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("PostSchool")]
        public async Task<IActionResult> Put([FromBody] SchoolDTO _SchoolDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _sch = await _context.Schools.Where(
                x => x.SchoolId == _SchoolDTO.SchoolId).FirstOrDefaultAsync();


                if (_sch == null)
                {
                    trans.Commit();
                    await this.Post(_SchoolDTO);
                    return Ok();
                }
                _sch.SchoolId = _SchoolDTO.SchoolId;
                _sch.SchoolName = _SchoolDTO.SchoolName;
                _sch.CreatedBy = _SchoolDTO.CreatedBy;
                _sch.CreatedDate = _SchoolDTO.CreatedDate;
                _sch.ModifiedBy = _SchoolDTO.ModifiedBy;
                _sch.ModifiedDate = _SchoolDTO.ModifiedDate;

                _context.Schools.Update(_sch);
                await _context.SaveChangesAsync();
                trans.Commit();
                return Ok(_SchoolDTO.SchoolId);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }






    }
}
