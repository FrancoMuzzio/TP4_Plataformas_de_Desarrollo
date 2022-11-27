using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_plataformas_de_desarrollo.Models
{
    public class CajaDeAhorro
    {
        public int id { get; set; }
        public int cbu { get; set; }
        public float saldo { get; set; }
        public List<Movimiento> movimientos { get; set; }
        public ICollection<Usuario> titulares { get; } = new List<Usuario>();
        public List<UsuarioCaja> usuarioCajas { get; set; }

        public CajaDeAhorro()
        {
            titulares = new List<Usuario>();
            movimientos = new List<Movimiento>();
        }
        public CajaDeAhorro(int id, int cbu, float saldo = 0)
        {
            this.id = id;
            this.cbu = cbu;
            this.saldo = saldo;
            titulares = new List<Usuario>();
            movimientos = new List<Movimiento>();
        }

        public CajaDeAhorro(int Cbu, Usuario Titular)//Constructor alternativo
        {
            this.cbu = Cbu;
            this.saldo = 0;
            this.titulares.Add(Titular);
            movimientos = new List<Movimiento>();
        }

        public override string ToString()
        {
            return string.Format("CBU: {0}, Saldo: {1}", this.cbu, this.saldo);
        }
    }
}
