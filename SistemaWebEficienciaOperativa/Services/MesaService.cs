using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SistemaWebEficienciaOperativa.Models;

namespace SistemaWebEficienciaOperativa.Services
{
    public class MesaService
    {
        private readonly DB_BUENISIMOEntities _dbContext;

        public MesaService()
        {
            _dbContext = new DB_BUENISIMOEntities();
        }

        public List<tbMesas> ListarMesas()
        {
            return _dbContext.tbMesas.OrderBy(m => m.numeroMesa).ToList();
        }

        public tbMesas ObtenerMesaPorId(int idMesa)
        {
            return _dbContext.tbMesas.Find(idMesa);
        }

        public void ActualizarEstadoMesa(int idMesa, string nuevoEstado)
        {
            var mesa = _dbContext.tbMesas.Find(idMesa);
            if (mesa != null)
            {
                mesa.estado = nuevoEstado; // Asumiendo que tienes una columna 'estado' en tbMesas
                _dbContext.Entry(mesa).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            // Considerar loguear si la mesa no se encuentra
        }

        /// <summary>
        /// Obtiene el estado de la mesa basado en si tiene pedidos activos.
        /// Un pedido activo es aquel que no está "Entregado" ni "Cancelado".
        /// </summary>
        public string ObtenerEstadoCalculadoMesa(int idMesa)
        {
            bool tienePedidoActivo = _dbContext.tbPedidos
                .Any(p => p.idMesa == idMesa &&
                            p.tbEstadosPedidos.estado != "Entregado" &&
                            p.tbEstadosPedidos.estado != "Cancelado"); // Asumimos un estado "Cancelado"

            return tienePedidoActivo ? "Ocupada" : "Disponible";
        }

        /// <summary>
        /// Lista las mesas y calcula su estado actual basado en los pedidos.
        /// Actualiza el campo 'estado' del objeto mesa en memoria para la vista.
        /// NO persiste este estado calculado en la DB aquí, eso se hace en ActualizarEstadoMesa.
        /// </summary>
        public List<tbMesas> ListarMesasConEstadoCalculado()
        {
            var mesas = _dbContext.tbMesas.OrderBy(m => m.numeroMesa).ToList();
            foreach (var mesa in mesas)
            {
                mesa.estado = ObtenerEstadoCalculadoMesa(mesa.idMesa);
            }
            return mesas;
        }
    }
}