using SistemaWebEficienciaOperativa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SistemaWebEficienciaOperativa.Services
{
    public class LoginService
    {
        public tbUsuarios Autenticar(string dni, string contrasena)
        {
            try
            {
                using (DB_BUENISIMOEntities context = new DB_BUENISIMOEntities())
                {
                    return context.tbUsuarios
                        .FirstOrDefault(u => u.dni == dni && u.contrasena == contrasena);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}