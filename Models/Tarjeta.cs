using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_plataformas_de_desarrollo.Models
{
    public class Tarjeta
    {
        public int id { get; set; }
        public int id_titular { get; set; }
        public int numero { get; set; }
        public int codigoV { get; set; }
        public float limite { get; set; }
        public float consumo { get; set; }
        public Usuario titular { get; set; }

        public Tarjeta()
        {

        }

        public Tarjeta(int Numero, int CodigoV, Usuario Titular, float Limite)
        {
            this.titular = Titular;
            this.numero = Numero;
            this.codigoV = CodigoV;
            this.limite = Limite;
            this.consumo = 0;
        }
        public Tarjeta(int Id_Usuario, int Numero, int CodigoV, float Limite, float Consumo)
        {
            this.id_titular = Id_Usuario;
            this.numero = Numero;
            this.codigoV = CodigoV;
            this.limite = Limite;
            this.consumo = Consumo;
        }
        public override string ToString()
        {
            return string.Format("Número: {0}, Código: {1}, Límite: {2}, Consumo: {3}", this.numero, this.codigoV, this.limite, this.consumo);
        }
    }
}
