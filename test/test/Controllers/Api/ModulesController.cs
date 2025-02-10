using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using test.Services;
using test.Models.Entity;
using test.Models.DTOs;
using test.Models.DTOs.Response.Module;
using test.Models.DTOs.Request.Module;  


namespace test.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ModulesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ModuleRes>>>> GetModules()
        {
            var modules = await _context.Module.ToListAsync();
            var moduleDTOs = _mapper.Map<IEnumerable<ModuleRes>>(modules);

            return Ok(new ApiResponse<IEnumerable<ModuleRes>>
            {
                Success = true,
                Message = "Modules retrieved successfully",
                Data = moduleDTOs
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ModuleRes>>> GetModule(int id)
        {
            var module = await _context.Module.FindAsync(id);
            if (module == null)
            {
                return NotFound(new ApiResponse<ModuleRes>
                {
                    Success = false,
                    Message = "Module not found"
                });
            }

            return Ok(new ApiResponse<ModuleRes>
            {
                Success = true,
                Message = "Module retrieved successfully",
                Data = _mapper.Map<ModuleRes>(module)
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ModuleReq>>> CreateModule([FromBody] ModuleReq moduleReq)
        {
            var module = _mapper.Map<Module>(moduleReq);
            _context.Module.Add(module);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<ModuleReq>
            {
                Success = true,
                Message = "Module created successfully",
                Data = moduleReq
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ModuleReq>>> UpdateModule(int id, ModuleReq moduleReq)
        {
            var module = await _context.Module.FindAsync(id);
            if (module == null)
            {
                return NotFound(new ApiResponse<ModuleReq>
                {
                    Success = false,
                    Message = "Module not found"
                });
            }
            _mapper.Map(moduleReq, module);
            await _context.SaveChangesAsync();  

            return Ok(new ApiResponse<ModuleReq>
            {
                Success = true,
                Message = "Module updated successfully",
                Data = moduleReq
            });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteModule(int id)
        {
            var module = await _context.Module.FindAsync(id);
            if (module == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Module not found"
                });
            }

            _context.Module.Remove(module);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Module deleted successfully"
            });
        }
    }
} 