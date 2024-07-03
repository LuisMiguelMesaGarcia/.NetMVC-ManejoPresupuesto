using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Servicios
{
    public interface IServicioReportes
    {
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemanal(int usuarioId, int mes, int año, dynamic ViewBag);
        Task<ReporteTransaccionesDetallas> ObtenerReporteTransaccionesDetalladas(int usuarioId, int mes, int año, dynamic ViewBag);
        Task<ReporteTransaccionesDetallas> ObtenerReporteTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId, int mes, int año, dynamic ViewBag);
    }
}
