﻿using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public class ServicioReportes:IServicioReportes
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly HttpContext httpContext;

        public ServicioReportes(IRepositorioTransacciones repositorioTransacciones,
                                IHttpContextAccessor httpContextAccessor)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.httpContext = httpContextAccessor.HttpContext;

        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemanal(int usuarioId, int mes, int año, dynamic ViewBag) 
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);
            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
            };
            AsignarValoresAlViewBaG(ViewBag, fechaInicio);
            var modelo = await repositorioTransacciones.ObtenerPorSemana(parametro);
            return modelo;
        }

        public async Task<ReporteTransaccionesDetallas> ObtenerReporteTransaccionesDetalladas(int usuarioId, int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);

            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
            };
            var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(parametro);
            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);
            AsignarValoresAlViewBaG(ViewBag, fechaInicio);
            return modelo;
        }

        public async Task<ReporteTransaccionesDetallas> ObtenerReporteTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId, int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);

            var obtenerTransaccionesPorCuenta = new ObtenerTransaccionesPorCuenta()
            {
                CuentaId = cuentaId,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await repositorioTransacciones.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);

            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);
            AsignarValoresAlViewBaG(ViewBag, fechaInicio);

            return modelo;
        }

        private void AsignarValoresAlViewBaG(dynamic ViewBag, DateTime fechaInicio)
        {
            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        private static ReporteTransaccionesDetallas GenerarReporteTransaccionesDetalladas(DateTime fechaInicio, DateTime fechaFin, IEnumerable<Transaccion> transacciones)
        {
            var modelo = new ReporteTransaccionesDetallas();


            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
                .GroupBy(x => x.FechaTransaccion)
                .Select(grupo => new ReporteTransaccionesDetallas.TransaccionesPorFecha()
                {
                    FechaTransaccion = grupo.Key,
                    Transacciones = grupo.AsEnumerable(),
                });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;
            return modelo;
        }

        private (DateTime fechaInicio, DateTime fechaFin) GenerarFechaInicioYFin(int mes, int año) 
        {
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes <= 0 || mes > 12 || año <= 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(año, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }
    }
}
