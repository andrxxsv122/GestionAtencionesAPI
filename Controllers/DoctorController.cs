using Microsoft.AspNetCore.Mvc;
using GestionAtencionesAPI.Models;
using GestionAtencionesAPI.Repositories;

namespace GestionAtencionesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorRepository _repo;

    public DoctorController(IDoctorRepository repo)
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
        var doctor = await _repo.GetByIdAsync(id);
        if (doctor == null) return NotFound();
        return Ok(doctor);
    }

    [HttpGet("license/{license}")]
    public async Task<IActionResult> GetByLicense(string license)
    {
        var doctor = await _repo.GetByLicenseAsync(license);
        if (doctor == null) return NotFound();
        return Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Doctor doctor)
    {
        if (string.IsNullOrWhiteSpace(doctor.Doctor_FirstName) || string.IsNullOrWhiteSpace(doctor.Doctor_LastName))
            return BadRequest(new { error = "Nombre y apellido son requeridos" });

        if (string.IsNullOrWhiteSpace(doctor.Doctor_LicenseNumber))
            return BadRequest(new { error = "LicenseNumber es obligatorio" });

        var existing = await _repo.GetByLicenseAsync(doctor.Doctor_LicenseNumber);
        if (existing != null) return Conflict("Ya existe un doctor con ese número de licencia.");

        doctor.Doctor_CreatedBy = "api";
        doctor.Doctor_CreatedAt = DateTime.Now;

        var id = await _repo.CreateAsync(doctor);
        doctor.Doctor_Id = id;
        return CreatedAtAction(nameof(GetById), new { id }, doctor);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Doctor doctor)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        doctor.Doctor_Id = id;
        doctor.Doctor_ModifiedBy = "api";
        doctor.Doctor_ModifiedAt = DateTime.Now;

        var success = await _repo.UpdateAsync(doctor);
        return success ? Ok(doctor) : StatusCode(500);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        var success = await _repo.DeleteAsync(id);
        return success ? NoContent() : StatusCode(500);
    }
}