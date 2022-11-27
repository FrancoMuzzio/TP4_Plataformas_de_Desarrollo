using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_plataformas_de_desarrollo.Models
{
    public class PlazoFijo
    {
        public int id { get; set; }
        public float monto { get; set; }
        public DateTime fechaIni { get; set; }
        public DateTime fechaFin { get; set; }
        public float tasa { get; set; }
        public bool pagado { get; set; }
        public Usuario titular { get; set; }
        public int id_titular { get; set; }
        public int cbu { get; set; }

        public PlazoFijo() { }

        public PlazoFijo(Usuario Titular, float Monto, DateTime FechaFin, float Tasa, int cbu)
        {
            this.titular = Titular;
            this.id_titular = Titular.id;
            this.monto = Monto;
            this.fechaIni = DateTime.Now;
            this.fechaFin = FechaFin;
            this.cbu = cbu;
            this.tasa = Tasa;
            this.pagado = false;
        }

        public override string ToString()
        {
            return string.Format("Monto: {0}, fechaIni: {1}, fechaFin: {2}, tasa: {3} ,pagado: {4} ,titular{5}", monto, fechaIni, fechaFin, tasa, pagado, titular);
        }

    }
}
