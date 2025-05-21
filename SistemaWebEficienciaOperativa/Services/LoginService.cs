using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                    var usuario = context.tbUsuarios.FirstOrDefault(u => u.dni == dni);
                    Debug.WriteLine("Usuario encontrado: " + usuario.dni);
                    Debug.WriteLine("Contraseña en DB: " + usuario.contrasena);
                    Debug.WriteLine("Longitud: " + usuario.contrasena.Length);
                    if (usuario != null)
                    {
                        // Si no está hasheada (ejemplo para transición)
                        if (usuario.contrasena.Length < 40)
                        {
                            if (usuario.contrasena == contrasena)
                            {
                                // Actualiza a contraseña hasheada
                                usuario.contrasena = PasswordHasher.HashPassword(contrasena);
                                context.SaveChanges();
                                Debug.WriteLine("Contraseña coincide, actualizando hash");
                            }
                            else
                            {
                                Debug.WriteLine("Contraseña NO coincide");
                                return null;
                            }
                        }
                        else if (!PasswordHasher.VerifyPassword(contrasena, usuario.contrasena))
                        {
                            Debug.WriteLine("Contraseña NO coincide con PasswordHasher");
                            return null;
                        }
                        Debug.WriteLine("Contraseña hasheada, verificando con PasswordHasher");
                        return usuario;
                    }
                    Debug.WriteLine("Usuario NO encontrado");
                    return null;

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}