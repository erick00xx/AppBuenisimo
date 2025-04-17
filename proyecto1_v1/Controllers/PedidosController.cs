using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace proyecto1_v1.Controllers
{
    public class PedidosController : Controller
    {
        // GET: Pedidos
        public ActionResult Index()//interfaz de pedido que tendra el empleado
        {
            return View();
        }
        public ActionResult RegistrarPedido()
        {
            return View();
        }
        public ActionResult ListarPedido()
        {
            return View();
        }
        public ActionResult EditarPedido()
        {
            return View();
        }
        public ActionResult CancelarPedido()
        {
            return View();
        }

        public ActionResult PedidoUsuario()//interfaz de pedido que tendra el cliente
        {
            return View();
        }
    }
}