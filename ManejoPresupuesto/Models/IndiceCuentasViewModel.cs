namespace ManejoPresupuesto.Models
{
    public class IndiceCuentasViewModel
    {
        public string TipoCuenta { set; get; }
        public IEnumerable<Cuenta> Cuentas { get; set; }
        public decimal Balance => Cuentas.Sum(x => x.Balance);
    }
}
