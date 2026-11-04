
using Microsoft.AspNetCore.Mvc;
using MiWebAPI.Data;
using MiWebAPI.Models;
using System.Text.RegularExpressions;

namespace MiWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocenteController : ControllerBase
    {
        private readonly DocenteData _docenteData;

        public DocenteController(DocenteData docenteData)
        {
            _docenteData = docenteData;
        }

        private string? ValidarDocente(Docente objeto)
        {
            if (string.IsNullOrWhiteSpace(objeto.NombreCompleto) ||
                string.IsNullOrWhiteSpace(objeto.Correo) ||
                string.IsNullOrWhiteSpace(objeto.Tipo))
            {
                return "El nombre, correo y tipo son obligatorios.";
            }

            if (Regex.IsMatch(objeto.NombreCompleto, @"\d"))
            {
                return "El nombre no debe contener números.";
            }

            if (!Regex.IsMatch(objeto.Correo, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            {
                return "Ingresa un correo electrónico válido.";
            }

            if (objeto.Tipo != "PA" && objeto.Tipo != "PTC")
            {
                return "El tipo de docente debe ser PA o PTC.";
            }

            if (objeto.HorasContratadas.HasValue &&
                (objeto.HorasContratadas <= 0 || objeto.HorasContratadas > 40))
            {
                return "Las horas deben ser mayores a 0 y no exceder 40.";
            }

            return null;
        }

        [HttpGet("Lista")]
        public async Task<IActionResult> Lista()
        {
            var lista = await _docenteData.Lista();
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            var docente = await _docenteData.Obtener(id);

            if (docente == null)
                return NotFound("No se encontró el docente.");

            return Ok(docente);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Docente objeto)
        {
            var error = ValidarDocente(objeto);

            if (error != null)
                return BadRequest(new { respuesta = error, isSuccess = false });

            var resultado = await _docenteData.Crear(objeto);
            return Ok(resultado);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Docente objeto)
        {
            if (objeto.IdDocente <= 0)
                return BadRequest("El ID del docente no es válido.");

            var error = ValidarDocente(objeto);

            if (error != null)
                return BadRequest(new { respuesta = error, isSuccess = false });

            var resultado = await _docenteData.Editar(objeto);
            return Ok(resultado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            var resultado = await _docenteData.Eliminar(id);
            return Ok(resultado);
        }
    }
}
