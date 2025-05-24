using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Services;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class VentasController : Controller
    {
        private readonly VentasService _ventasService = new VentasService();
        private readonly EmpleadoService _empleadosService = new EmpleadoService();
        private readonly HorarioService _horarioService = new HorarioService();
        // GET: Ventas
        public ActionResult Index()
        {
            return View(_ventasService.ListarVentas());
        }

        public ActionResult Ver(int idPedido)
        {
            return View(_ventasService.Obtener(idPedido));
        }

        public ActionResult AgregarEditar(int idPedido = 0)
        {
            ViewBag.tbUsuarios = _horarioService.ObtenerTodosLosUsuariosParaGestion();
            //ViewBag.tbMesas = _empleadosService.ListarMesas();
            //ViewBag.tbSucursales = _empleadosService.ListarSucursales();

            return View(
                idPedido == 0 ? new tbPedidos()
                : _ventasService.Obtener(idPedido)
                );
        }

        public ActionResult Guardar(tbPedidos pedido)
        {
            if (ModelState.IsValid)
            {
                _ventasService.Guardar(pedido);
                return Redirect("~/Ventas"); //retornar a listar
            }
            else
            {
                return View("~/Views/Ventas/AgregarEditar.cshtml");
            }
        }



        public ActionResult ReporteVentas()
        {
            return View();
        }
    }
}