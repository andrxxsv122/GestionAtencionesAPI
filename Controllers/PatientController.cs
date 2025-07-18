using Microsoft.AspNetCore.Mvc;
using GestionAtencionesAPI.Models;
using Microsoft.AspNetCore.Http;
using GestionAtencionesAPI.Repositories;

[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientRepository _repository;

    public PatientController(IPatientRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _repository.GetAllAsync();
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _repository.GetByIdAsync(id);
        if (patient == null) return NotFound();
        return Ok(patient);
    }

    [HttpGet("rut/{rut}")]
    public async Task<IActionResult> GetByRut(string rut)
    {
        var patient = await _repository.GetByRutAsync(rut);
        if (patient == null) return NotFound();
        return Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Patient patient)
    {
        if (string.IsNullOrWhiteSpace(patient.Patient_FirstName))
            return BadRequest(new { error = "Patient_FirstName is required" });

        var existing = await _repository.GetByRutAsync(patient.Patient_RUT);
        if (existing != null)
            return Conflict("Ya existe un paciente con ese RUT.");

        patient.Patient_CreatedBy = "api"; // puedes reemplazar con usuario logueado
        patient.Patient_CreatedAt = DateTime.Now;

        var id = await _repository.CreateAsync(patient);
        patient.Patient_Id = id;

        return CreatedAtAction(nameof(GetById), new { id }, patient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Patient patient)
    {
        if (id != patient.Patient_Id)
            return BadRequest();

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        patient.Patient_ModifiedBy = "api";
        patient.Patient_ModifiedAt = DateTime.Now;

        var result = await _repository.UpdateAsync(patient);
        return result ? Ok(patient) : StatusCode(500);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return NotFound();

        var result = await _repository.DeleteAsync(id);
        return result ? NoContent() : StatusCode(500);
    }
}
