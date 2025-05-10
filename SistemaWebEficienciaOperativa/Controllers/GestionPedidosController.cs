using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SistemaWebEficienciaOperativa.Models;
using SistemaWebEficienciaOperativa.Services;
using SistemaWebEficienciaOperativa.Models.ViewModels;

namespace SistemaWebEficienciaOperativa.Controllers
{
    public class GestionPedidosController : Controller
    {
        PedidoService pedidoService = new PedidoService();
        // GET: GestionPedidos
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Pedidos()
        {
            return View();
        }

        [HttpGet]
        public JsonResult BuscarProductos(string term)
        {
            var productos = pedidoService.BuscarProductos(term);

            return Json(productos, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GuardarPedido(string detallePedido)
        {
            var _dbContext = new DB_BUENISIMOEntities();
            if (string.IsNullOrEmpty(detallePedido))
            {
                return RedirectToAction("Index");  // O lo que consideres adecuado si no hay productos seleccionados
            }

            pedidoService.GuardarPedido(detallePedido);

            return RedirectToAction("Index");
        }
    }
}