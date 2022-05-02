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


namespace SWARM.Server.Controllers.GradeCon
{ 
    [Route("api/[controller]")]
    [ApiController]
        public class GradeConversionController : Controller
        {
            protected readonly SWARMOracleContext _context;
            protected readonly IHttpContextAccessor _httpContextAccessor;

            public GradeConversionController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
            {
                this._context = context;
                this._httpContextAccessor = httpContextAccessor;
            }

            [HttpGet]
            [Route("GetGradeCon")]
            public async Task<IActionResult> GetGradeConversion()
            {
                List<GradeConversion> lstCourses = await _context.GradeConversions.OrderBy(x => x.LetterGrade).ToListAsync();
                return Ok(lstCourses);
            }

        [HttpGet]
        [Route("GetGradeCon/{pSchId}/{pLetG}")]
        public async Task<IActionResult> GetGradCon(int pSchId, string pLetG)
            {
                GradeConversion itmGradeCon = await _context.GradeConversions.Where(
                    x => x.SchoolId == pSchId
                    &&
                    x.LetterGrade == pLetG).FirstOrDefaultAsync();
                return Ok(itmGradeCon);
            }

            [HttpDelete]
            [Route("DeleteGradeCon/{pSchId}/{pLetG}")]
            public async Task<IActionResult> DeleteGradeCon(int pSchId, string pLetG)
            {
                var trans = _context.Database.BeginTransaction();
                try
                {
                    GradeConversion itmGradeCon = await _context.GradeConversions.Where(
                        x => x.SchoolId == pSchId
                    &&
                        x.LetterGrade == pLetG).FirstOrDefaultAsync();
                    _context.Remove(itmGradeCon);
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
            [Route("PostGradeCon")]
            public async Task<IActionResult> Post([FromBody] GradeConversionDTO _GradeConDTO)
            {
                var trans = _context.Database.BeginTransaction();
                try
                {
                    var _con = await _context.GradeConversions.Where(
                        x => x.SchoolId == _GradeConDTO.SchoolId
                        &&
                        x.LetterGrade == _GradeConDTO.LetterGrade).FirstOrDefaultAsync();


                    if (_con != null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Course already exists");
                    }
                    _con = new GradeConversion();
                    _con.SchoolId = _GradeConDTO.SchoolId;
                    _con.LetterGrade = _GradeConDTO.LetterGrade;
                    _con.GradePoint = _GradeConDTO.GradePoint;
                    _con.MaxGrade = _GradeConDTO.MaxGrade;
                    _con.MinGrade = _GradeConDTO.MinGrade;
                    _con.CreatedBy = _GradeConDTO.CreatedBy;
                    _con.CreatedDate = _GradeConDTO.CreatedDate;
                    _con.ModifiedBy = _GradeConDTO.ModifiedBy;
                    _con.ModifiedDate = _GradeConDTO.ModifiedDate;

                    _context.GradeConversions.Add(_con);
                    await _context.SaveChangesAsync();
                    trans.Commit();

                    return Ok(_GradeConDTO.LetterGrade);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

            [HttpPut]
            [Route("PutGradeCon")]
            public async Task<IActionResult> Put([FromBody] GradeConversionDTO _GradeConDTO)
            {
                var trans = _context.Database.BeginTransaction();
                try
                {
                    var _con = await _context.GradeConversions.Where(
                        x => x.SchoolId == _GradeConDTO.SchoolId
                        &&
                        x.LetterGrade == _GradeConDTO.LetterGrade).FirstOrDefaultAsync();


                    if (_con == null)
                    {
                        trans.Commit();
                        await this.Post(_GradeConDTO);
                        return Ok();
                    }
                    _con = new GradeConversion();
                    _con.SchoolId = _GradeConDTO.SchoolId;
                    _con.LetterGrade = _GradeConDTO.LetterGrade;
                    _con.GradePoint = _GradeConDTO.GradePoint;
                    _con.MaxGrade = _GradeConDTO.MaxGrade;
                    _con.MinGrade = _GradeConDTO.MinGrade;
                    _con.CreatedBy = _GradeConDTO.CreatedBy;
                    _con.CreatedDate = _GradeConDTO.CreatedDate;
                    _con.ModifiedBy = _GradeConDTO.ModifiedBy;
                    _con.ModifiedDate = _GradeConDTO.ModifiedDate;

                    _context.Update(_con);
                    await _context.SaveChangesAsync();
                    trans.Commit();

                    return Ok(_GradeConDTO.LetterGrade);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
        }
    }
