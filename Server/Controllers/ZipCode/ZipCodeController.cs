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
namespace SWARM.Server.Controllers.Zip
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZipCodeController : Controller
    {
        protected readonly SWARMOracleContext _context;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public ZipCodeController(SWARMOracleContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("GetZIPs")]
        public async Task<IActionResult> GetZIPs()
        {
            List<Zipcode> lstZips = await _context.Zipcodes.OrderBy(x => x.Zip).ToListAsync();
            return Ok(lstZips);
        }

        [HttpGet]
        [Route("GetZIP/{pZIP}")]
        public async Task<IActionResult> GetSchool(string pZIP)
        {
            Zipcode lstStudent = await _context.Zipcodes.Where(
                x => x.Zip == pZIP).FirstOrDefaultAsync();
            return Ok(lstStudent);
        }

        [HttpDelete]
        [Route("DeleteZIP/{pZIP}")]
        public async Task<IActionResult> DeleteStudent(string pZIP)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                Zipcode lstStudent = await _context.Zipcodes.Where(
                x => x.Zip == pZIP).FirstOrDefaultAsync();
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
        [Route("PostZIP")]
        public async Task<IActionResult> Post([FromBody] ZipcodeDTO _ZipcodeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _zip = await _context.Zipcodes.Where(
                x => x.Zip == _ZipcodeDTO.Zip).FirstOrDefaultAsync();


                if (_zip != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Enrollment already exists");
                }
                _zip = new Zipcode();
                _zip.Zip = _ZipcodeDTO.Zip;
                _zip.City = _ZipcodeDTO.City;
                _zip.State = _ZipcodeDTO.State;
                _zip.CreatedBy = _ZipcodeDTO.CreatedBy;
                _zip.CreatedDate = _ZipcodeDTO.CreatedDate;
                _zip.ModifiedBy = _ZipcodeDTO.ModifiedBy;
                _zip.ModifiedDate = _ZipcodeDTO.ModifiedDate;

                _context.Zipcodes.Add(_zip);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_ZipcodeDTO.Zip);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Route("PutZIP")]
        public async Task<IActionResult> Put([FromBody] ZipcodeDTO _ZipcodeDTO)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                var _zip = await _context.Zipcodes.Where(
                x => x.Zip == _ZipcodeDTO.Zip).FirstOrDefaultAsync();


                if (_zip == null)
                {
                    trans.Commit();
                    await this.Post(_ZipcodeDTO);
                    return Ok();
                }
                _zip.Zip = _ZipcodeDTO.Zip;
                _zip.City = _ZipcodeDTO.City;
                _zip.State = _ZipcodeDTO.State;
                _zip.CreatedBy = _ZipcodeDTO.CreatedBy;
                _zip.CreatedDate = _ZipcodeDTO.CreatedDate;
                _zip.ModifiedBy = _ZipcodeDTO.ModifiedBy;
                _zip.ModifiedDate = _ZipcodeDTO.ModifiedDate;

                _context.Zipcodes.Update(_zip);
                await _context.SaveChangesAsync();
                trans.Commit();

                return Ok(_ZipcodeDTO.Zip);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




    }
}
