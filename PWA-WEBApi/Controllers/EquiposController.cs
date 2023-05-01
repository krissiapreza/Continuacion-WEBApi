﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PWA_WEBApi.Data;
using PWA_WEBApi.Models;

namespace PWA_WEBApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquiposController : ControllerBase
    {
       
        private readonly EquipoContext _equipoContext;

        public EquiposController(EquipoContext equipoContext)
        {
            _equipoContext = equipoContext;
        }

        #region GET_ALL - GET
        [HttpGet]
        [Route("GetAll")]
        public ActionResult Get() {
            var listadoEquipos = (from equipo in _equipoContext.Equipos
                                  join estado in _equipoContext.estados_equipo on equipo.estado_equipo_id equals estado.id_estados_equipo
                                  join tipoEquipo in _equipoContext.tipo_equipo on equipo.tipo_equipo_id equals tipoEquipo.id_tipo_equipo
                                  join marca in _equipoContext.Marcas on equipo.marca_id equals marca.id_marcas
                                  select new
                                  {
                                      equipo.nombre,
                                      equipo.descripcion,
                                      descripcionTipo = tipoEquipo.descripcion,
                                      marca.nombre_marca,
                                      equipo.modelo,
                                      equipo.anio_compra,
                                      equipo.costo,
                                      equipo.vida_util,
                                      estadoEquipo = estado.descripcion
                                  }).ToList();

            if (listadoEquipos.Count == 0)
            {
                return NotFound();
            }

            return Ok(listadoEquipos);
        }

        #endregion

        #region GET_BY_ID - GET
        [HttpGet]
        [Route("GetById")]
        public ActionResult GetById(int id)
        {
            Equipo? equipo = _equipoContext.Equipos.Find(id);

            if(equipo == null) return NotFound();

            return Ok(equipo);
        }
        #endregion

        #region FIND - GET
        [HttpGet]
        [Route("Find")]
        public ActionResult Find(string filtro)
        {
            List<Equipo>? equiposList = _equipoContext.Equipos
                                       .Where( (x => (x.nombre.Contains(filtro) || x.descripcion.Contains(filtro)) && x.estado == "A") )
                                       .ToList();

            if (equiposList.Any())
            {
                return Ok(equiposList);

            }
                
            return NotFound();
        }
        #endregion

        #region AGREGAR - POST
        [HttpPost]
        [Route("add")]
        public IActionResult crear([FromBody] Equipo equipo)
        {

            try
            {
                _equipoContext.Equipos.Add(equipo);
                _equipoContext.SaveChanges();

                return Ok(equipo);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region ACTUALIZAR - POST

        [HttpPut]
        [Route("Actualizar/{id}")]
        public IActionResult actualizar(int id, [FromBody] Equipo equipo)
        {
            Equipo? equipoExistente = _equipoContext.Equipos.Find(id);

            if (equipoExistente == null)
            {
                return NotFound();
            }

            equipoExistente.nombre = equipo.nombre;
            equipoExistente.descripcion = equipo.descripcion;

            _equipoContext.Entry(equipoExistente).State = EntityState.Modified;
            _equipoContext.SaveChanges();

            return Ok(equipoExistente);

        }

        #endregion

        #region ELIMINAR - DELETE 
        [HttpDelete]
        [Route("detelet/{id}")]
        public IActionResult eliminarEquipo(int id)
        {
            Equipo? equipoExiste = _equipoContext.Equipos.Find(id);

            if (equipoExiste == null) return NotFound();

            equipoExiste.estado = "C";

            _equipoContext.Entry(equipoExiste).State = EntityState.Modified;
            _equipoContext.SaveChanges();

            return Ok(equipoExiste);

        }
        #endregion

    }
}
