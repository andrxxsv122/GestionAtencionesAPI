using Microsoft.AspNetCore.Mvc;
using GestionAtencionesAPI.Models;
using GestionAtencionesAPI.Repositories;

namespace GestionAtencionesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecialityController : ControllerBase
{
    private readonly ISpecialityRepository _repo;

    public SpecialityController(ISpecialityRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _repo.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var speciality = await _repo.GetByIdAsync(id);
        if (speciality == null) return NotFound();
        return Ok(speciality);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Speciality speciality)
    {
        if (string.IsNullOrWhiteSpace(speciality.Speciality_Name))
            return BadRequest(new { error = "Speciality_Name is required" });

        speciality.Speciality_CreatedBy = "api"; 
        speciality.Speciality_CreatedAt = DateTime.Now;

        var newId = await _repo.CreateAsync(speciality);
        speciality.Speciality_Id = newId;
        return CreatedAtAction(nameof(GetById), new { id = newId }, speciality);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Speciality speciality)
    {
        if (id != speciality.Speciality_Id) return BadRequest();

        speciality.Speciality_ModifiedBy = "api";
        speciality.Speciality_ModifiedAt = DateTime.Now;

        var updated = await _repo.UpdateAsync(speciality);
        if (!updated) return NotFound();

        return Ok(speciality);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
