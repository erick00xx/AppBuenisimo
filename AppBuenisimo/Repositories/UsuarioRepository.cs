using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppBuenisimo.Models;

namespace AppBuenisimo.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        // Mantenemos una instancia del contexto, pero no la creamos aquí.
        // Se podría inyectar también, pero para simplificar, lo creamos en cada método.
        private readonly DB_BUENISIMOEntities _context;

        public UsuarioRepository()
        {
            _context = new DB_BUENISIMOEntities();
        }

        // Constructor para poder inyectar un contexto si fuera necesario (buena práctica)
        public UsuarioRepository(DB_BUENISIMOEntities context)
        {
            _context = context;
        }

        public tbUsuarios ObtenerPorDni(string dni)
        {
            // La lógica de búsqueda que antes estaba en el servicio, ahora está aquí.
            return _context.tbUsuarios.FirstOrDefault(u => u.dni == dni);
        }

        public void GuardarCambios()
        {
            // La lógica de guardado que antes estaba en el servicio, ahora está aquí.
            _context.SaveChanges();
        }
    }
}