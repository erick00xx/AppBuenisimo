using AppBuenisimo.Models;
using AppBuenisimo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Globalization;

public class ReporteVentaService
{
    private readonly DB_BUENISIMOEntities _dbContext = new DB_BUENISIMOEntities();

    // Obtener resumen general de ventas
    public ResumenVentasViewModel ObtenerResumenVentas(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idSucursal = null)
    {
        var query = _dbContext.tbVentas.AsQueryable();

        // Aplicar filtros
        if (fechaInicio.HasValue)
            query = query.Where(v => v.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(v => v.fechaVenta <= fechaFin.Value);

        if (idSucursal.HasValue)
            query = query.Where(v => v.idSucursal == idSucursal.Value);

        var ventas = query.ToList();
        var totalVentas = ventas.Sum(v => v.total);
        var cantidadVentas = ventas.Count;
        var promedioVenta = cantidadVentas > 0 ? totalVentas / cantidadVentas : 0;

        // Comparativa con período anterior
        var diasPeriodo = fechaFin.HasValue && fechaInicio.HasValue ?
            (fechaFin.Value - fechaInicio.Value).Days : 30;

        var fechaInicioAnterior = fechaInicio?.AddDays(-diasPeriodo) ?? DateTime.Now.AddDays(-60);
        var fechaFinAnterior = fechaInicio?.AddDays(-1) ?? DateTime.Now.AddDays(-30);

        var ventasAnterior = _dbContext.tbVentas
            .Where(v => v.fechaVenta >= fechaInicioAnterior && v.fechaVenta <= fechaFinAnterior)
            .Where(v => !idSucursal.HasValue || v.idSucursal == idSucursal.Value)
            .Sum(v => (decimal?)v.total) ?? 0;

        var crecimiento = ventasAnterior > 0 ? ((totalVentas - ventasAnterior) / ventasAnterior) * 100 : 0;

        return new ResumenVentasViewModel
        {
            TotalVentas = totalVentas,
            CantidadVentas = cantidadVentas,
            PromedioVenta = promedioVenta,
            CrecimientoVsAnterior = crecimiento,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin
        };
    }

    // Ventas por hora del día
    public List<VentasPorHoraViewModel> ObtenerVentasPorHora(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idSucursal = null)
    {
        var query = _dbContext.tbVentas.AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(v => v.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(v => v.fechaVenta <= fechaFin.Value);

        if (idSucursal.HasValue)
            query = query.Where(v => v.idSucursal == idSucursal.Value);

        var ventasPorHora = query
            .GroupBy(v => v.fechaVenta.Hour)
            .Select(g => new VentasPorHoraViewModel
            {
                Hora = g.Key,
                TotalVentas = g.Sum(v => v.total),
                CantidadVentas = g.Count()
            })
            .OrderBy(v => v.Hora)
            .ToList();

        // Completar horas faltantes con 0
        var horasCompletas = new List<VentasPorHoraViewModel>();
        for (int i = 0; i < 24; i++)
        {
            var ventaHora = ventasPorHora.FirstOrDefault(v => v.Hora == i);
            horasCompletas.Add(ventaHora ?? new VentasPorHoraViewModel { Hora = i, TotalVentas = 0, CantidadVentas = 0 });
        }

        return horasCompletas;
    }

    // Ventas por día de la semana
    public List<VentasPorDiaViewModel> ObtenerVentasPorDiaSemana(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idSucursal = null)
    {
        var query = _dbContext.tbVentas.AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(v => v.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(v => v.fechaVenta <= fechaFin.Value);

        if (idSucursal.HasValue)
            query = query.Where(v => v.idSucursal == idSucursal.Value);

        // Primero obtenemos los datos y luego procesamos en memoria
        var ventasData = query
            .Select(v => new {
                fechaVenta = v.fechaVenta,
                total = v.total
            })
            .ToList();

        var culture = new CultureInfo("es-ES");

        // Procesamos en memoria para evitar el error de LINQ to Entities
        var ventasPorDia = ventasData
            .GroupBy(v => v.fechaVenta.DayOfWeek)
            .Select(g => new VentasPorDiaViewModel
            {
                DiaSemana = (int)g.Key,
                NombreDia = culture.DateTimeFormat.GetDayName(g.Key),
                TotalVentas = g.Sum(v => v.total),
                CantidadVentas = g.Count()
            })
            .OrderBy(v => v.DiaSemana)
            .ToList();

        // Completar días faltantes con 0
        var diasCompletos = new List<VentasPorDiaViewModel>();
        var nombresDias = new[] { "domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado" };

        for (int i = 0; i < 7; i++)
        {
            var ventaDia = ventasPorDia.FirstOrDefault(v => v.DiaSemana == i);
            diasCompletos.Add(ventaDia ?? new VentasPorDiaViewModel
            {
                DiaSemana = i,
                NombreDia = nombresDias[i],
                TotalVentas = 0,
                CantidadVentas = 0
            });
        }

        return diasCompletos;
    }

    // Top productos más vendidos
    public List<TopProductosViewModel> ObtenerTopProductos(int top = 10, DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idSucursal = null)
    {
        var query = _dbContext.tbDetalleVenta
            .Include(dv => dv.tbVentas)
            .Where(dv => dv.tbVentas != null);

        if (fechaInicio.HasValue)
            query = query.Where(dv => dv.tbVentas.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(dv => dv.tbVentas.fechaVenta <= fechaFin.Value);

        if (idSucursal.HasValue)
            query = query.Where(dv => dv.tbVentas.idSucursal == idSucursal.Value);

        return query
            .GroupBy(dv => new { dv.idProductoOriginal, dv.descripcionProducto })
            .Select(g => new TopProductosViewModel
            {
                IdProducto = g.Key.idProductoOriginal ?? 0,
                NombreProducto = g.Key.descripcionProducto,
                CantidadVendida = g.Sum(dv => dv.cantidad),
                TotalVentas = g.Sum(dv => dv.subtotal),
                PromedioVenta = g.Average(dv => dv.subtotal)
            })
            .OrderByDescending(p => p.CantidadVendida)
            .Take(top)
            .ToList();
    }

    // Top categorías más vendidas
    public List<TopCategoriasViewModel> ObtenerTopCategorias(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idSucursal = null)
    {
        var query = _dbContext.tbDetalleVenta
            .Include(dv => dv.tbVentas)
            .Join(_dbContext.tbProductos, dv => dv.idProductoOriginal, p => p.idProducto, (dv, p) => new { dv, p })
            .Join(_dbContext.tbCategorias, x => x.p.idCategoria, c => c.idCategoria, (x, c) => new { x.dv, x.p, c })
            .Where(x => x.dv.tbVentas != null);

        if (fechaInicio.HasValue)
            query = query.Where(x => x.dv.tbVentas.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(x => x.dv.tbVentas.fechaVenta <= fechaFin.Value);

        if (idSucursal.HasValue)
            query = query.Where(x => x.dv.tbVentas.idSucursal == idSucursal.Value);

        return query
            .GroupBy(x => new { x.c.idCategoria, x.c.nombre })
            .Select(g => new TopCategoriasViewModel
            {
                IdCategoria = g.Key.idCategoria,
                NombreCategoria = g.Key.nombre,
                CantidadVendida = g.Sum(x => x.dv.cantidad),
                TotalVentas = g.Sum(x => x.dv.subtotal)
            })
            .OrderByDescending(c => c.TotalVentas)
            .ToList();
    }

    // Ventas por sucursal
    public List<VentasPorSucursalViewModel> ObtenerVentasPorSucursal(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var query = _dbContext.tbVentas
            .Include(v => v.tbSucursales)
            .AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(v => v.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(v => v.fechaVenta <= fechaFin.Value);

        return query
            .GroupBy(v => new { v.idSucursal, v.tbSucursales.nombre })
            .Select(g => new VentasPorSucursalViewModel
            {
                IdSucursal = g.Key.idSucursal,
                NombreSucursal = g.Key.nombre,
                TotalVentas = g.Sum(v => v.total),
                CantidadVentas = g.Count(),
                PromedioVenta = g.Average(v => v.total)
            })
            .OrderByDescending(s => s.TotalVentas)
            .ToList();
    }

    // Distribución de métodos de pago
    public List<MetodosPagoViewModel> ObtenerDistribucionMetodosPago(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idSucursal = null)
    {
        var query = _dbContext.tbVentas
            .Include(v => v.tbMetodosPago)
            .AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(v => v.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(v => v.fechaVenta <= fechaFin.Value);

        if (idSucursal.HasValue)
            query = query.Where(v => v.idSucursal == idSucursal.Value);

        var totalVentas = query.Sum(v => (decimal?)v.total) ?? 0;

        return query
            .GroupBy(v => new { v.idMetodoPago, v.tbMetodosPago.nombre })
            .Select(g => new MetodosPagoViewModel
            {
                IdMetodoPago = g.Key.idMetodoPago ?? 0,
                NombreMetodo = g.Key.nombre ?? "Sin especificar",
                TotalVentas = g.Sum(v => v.total),
                CantidadVentas = g.Count(),
                Porcentaje = totalVentas > 0 ? (g.Sum(v => v.total) / totalVentas) * 100 : 0
            })
            .OrderByDescending(m => m.TotalVentas)
            .ToList();
    }

    // Análisis de precios por producto
    public List<AnalisisPreciosViewModel> ObtenerAnalisisPrecios(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idSucursal = null)
    {
        var query = _dbContext.tbDetalleVenta
            .Include(dv => dv.tbVentas)
            .Where(dv => dv.tbVentas != null);

        if (fechaInicio.HasValue)
            query = query.Where(dv => dv.tbVentas.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(dv => dv.tbVentas.fechaVenta <= fechaFin.Value);

        if (idSucursal.HasValue)
            query = query.Where(dv => dv.tbVentas.idSucursal == idSucursal.Value);

        return query
            .GroupBy(dv => new { dv.idProductoOriginal, dv.descripcionProducto })
            .Select(g => new AnalisisPreciosViewModel
            {
                IdProducto = g.Key.idProductoOriginal ?? 0,
                NombreProducto = g.Key.descripcionProducto,
                PrecioMinimo = g.Min(dv => dv.precioUnitario),
                PrecioMaximo = g.Max(dv => dv.precioUnitario),
                PrecioPromedio = g.Average(dv => dv.precioUnitario),
                CantidadVendida = g.Sum(dv => dv.cantidad)
            })
            .OrderByDescending(p => p.CantidadVendida)
            .ToList();
    }

    // Obtener listas para filtros
    public List<SucursalViewModel> ObtenerSucursales()
    {
        return _dbContext.tbSucursales
            .Select(s => new SucursalViewModel
            {
                IdSucursal = s.idSucursal,
                Nombre = s.nombre
            })
            .OrderBy(s => s.Nombre)
            .ToList();
    }

    public List<CategoriaViewModel> ObtenerCategorias()
    {
        return _dbContext.tbCategorias
            .Select(c => new CategoriaViewModel
            {
                IdCategoria = c.idCategoria,
                Nombre = c.nombre
            })
            .OrderBy(c => c.Nombre)
            .ToList();
    }

    public List<MetodoPagoViewModel> ObtenerMetodosPago()
    {
        return _dbContext.tbMetodosPago
            .Select(m => new MetodoPagoViewModel
            {
                IdMetodoPago = m.idMetodoPago,
                Nombre = m.nombre
            })
            .OrderBy(m => m.Nombre)
            .ToList();
    }

    // Agregar método para obtener ventas por mes
    public List<VentasPorMesViewModel> ObtenerVentasPorMes(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idSucursal = null)
    {
        var query = _dbContext.tbVentas.AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(v => v.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(v => v.fechaVenta <= fechaFin.Value);

        if (idSucursal.HasValue)
            query = query.Where(v => v.idSucursal == idSucursal.Value);

        var ventasData = query
            .Select(v => new {
                año = v.fechaVenta.Year,
                mes = v.fechaVenta.Month,
                total = v.total
            })
            .ToList();

        var culture = new CultureInfo("es-ES");

        return ventasData
            .GroupBy(v => new { v.año, v.mes })
            .Select(g => new VentasPorMesViewModel
            {
                Año = g.Key.año,
                Mes = g.Key.mes,
                NombreMes = culture.DateTimeFormat.GetMonthName(g.Key.mes),
                TotalVentas = g.Sum(v => v.total),
                CantidadVentas = g.Count()
            })
            .OrderBy(v => v.Año).ThenBy(v => v.Mes)
            .ToList();
    }

    // Agregar método para obtener horarios pico por sucursal
    public List<HorarioPicoSucursalViewModel> ObtenerHorariosPicoPorSucursal(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var query = _dbContext.tbVentas
            .Include(v => v.tbSucursales)
            .AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(v => v.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(v => v.fechaVenta <= fechaFin.Value);

        var ventasData = query
            .Select(v => new {
                idSucursal = v.idSucursal,
                nombreSucursal = v.tbSucursales.nombre,
                hora = v.fechaVenta.Hour,
                total = v.total
            })
            .ToList();

        return ventasData
            .GroupBy(v => new { v.idSucursal, v.nombreSucursal })
            .Select(sucursal => {
                var ventasPorHora = sucursal
                    .GroupBy(v => v.hora)
                    .Select(h => new { hora = h.Key, total = h.Sum(x => x.total) })
                    .OrderByDescending(h => h.total)
                    .First();

                return new HorarioPicoSucursalViewModel
                {
                    IdSucursal = sucursal.Key.idSucursal,
                    NombreSucursal = sucursal.Key.nombreSucursal,
                    HoraPico = ventasPorHora.hora,
                    VentasEnHoraPico = ventasPorHora.total
                };
            })
            .OrderByDescending(s => s.VentasEnHoraPico)
            .ToList();
    }

    // Agregar método para estadísticas avanzadas
    public EstadisticasAvanzadasViewModel ObtenerEstadisticasAvanzadas(DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idSucursal = null)
    {
        var query = _dbContext.tbVentas.AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(v => v.fechaVenta >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(v => v.fechaVenta <= fechaFin.Value);

        if (idSucursal.HasValue)
            query = query.Where(v => v.idSucursal == idSucursal.Value);

        var ventas = query.Select(v => v.total).ToList();

        if (!ventas.Any())
        {
            return new EstadisticasAvanzadasViewModel();
        }

        ventas.Sort();
        var count = ventas.Count;
        var median = count % 2 == 0
            ? (ventas[count / 2 - 1] + ventas[count / 2]) / 2
            : ventas[count / 2];

        return new EstadisticasAvanzadasViewModel
        {
            VentaMinima = ventas.Min(),
            VentaMaxima = ventas.Max(),
            MedianaVenta = median,
            DesviacionEstandar = CalcularDesviacionEstandar(ventas),
            TotalTransacciones = count
        };
    }

    private decimal CalcularDesviacionEstandar(List<decimal> valores)
    {
        if (valores.Count <= 1) return 0;

        var promedio = valores.Average();
        var sumaCuadrados = valores.Sum(v => Math.Pow((double)(v - promedio), 2));
        return (decimal)Math.Sqrt(sumaCuadrados / (valores.Count - 1));
    }
}
