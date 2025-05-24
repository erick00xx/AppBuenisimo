using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Models.ViewModels;

namespace SistemaWebEficienciaOperativa.Services
{
    public class VentasService
    {
        public List<tbPedidos> ListarVentas()
        {
            using (var _dbContext = new DB_BUENISIMOEntities())
            {
                return _dbContext.tbPedidos
                    .Include("tbUsuarios")
                    .Include("tbMesas")
                    .Include("tbSucursales")
                    .Where(p => p.idEstadoPedido == 5)
                    .OrderByDescending(p => p.idPedido)
                    .ToList();
            }
        }

        public tbPedidos Obtener(int idPedido)
        {
            using (var db = new DB_BUENISIMOEntities())
            {
                return db.tbPedidos
                    .AsNoTracking()
                    .Include("tbMesas")
                    .Include("tbUsuarios")
                    .Include("tbSucursales")
                    .FirstOrDefault(x => x.idPedido == idPedido);
            }
        }

        public void Guardar(tbPedidos pedidos)
        {
            try
            {
                using (var db = new DB_BUENISIMOEntities())
                {
                    //Si existe un registro, se modificará
                    if (pedidos.idPedido > 0 )
                    {
                        db.Entry(this).State = EntityState.Modified;
                    }
                    //Agrega un nuevo objeto
                    else
                    {
                        db.Entry(this).State = EntityState.Added;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}