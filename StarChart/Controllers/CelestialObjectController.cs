using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var obj = _context.CelestialObjects.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.Satellites = _context.CelestialObjects.Where(x => x.OrbitedObjectId == id).ToList();
            return Ok(obj);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var objs = _context.CelestialObjects.Where(x => x.Name == name);
            if (!objs.Any())
            {
                return NotFound();
            }

            foreach (var celestialObject in objs)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(objs);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var objs = _context.CelestialObjects.ToList();

            foreach (var celestialObject in objs)
            {
                celestialObject.Satellites = _context.CelestialObjects
                    .Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();
            }

            return Ok(objs);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new {id = celestialObject.Id}, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var obj = _context.CelestialObjects.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            obj.Name = celestialObject.Name;
            obj.OrbitalPeriod = celestialObject.OrbitalPeriod;
            obj.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var obj = _context.CelestialObjects.Find(id);
            if (obj== null)
            {
                return NotFound();
            }

            obj.Name = name;
            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objs = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id);
            if (!objs.Any())
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(objs);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
