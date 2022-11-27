using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication_plataformas_de_desarrollo.Models
{
    public class Pago
    {
        public Usuario usuario { get; set; }
        public int id { get; set; }
        public int id_usuario { get; set; }
        public string nombre { get; set; }
        public float monto { get; set; }
        public bool pagado { get; set; }
        public string metodo { get; set; }

        public Pago(int Id_usuario, string Nombre, float Monto, bool Pagado, string Metodo)
        {
            this.id_usuario = Id_usuario;
            this.nombre = Nombre;
            this.monto = Monto;
            this.pagado = Pagado;
            this.metodo = Metodo;
        }

        public Pago()    {    }

        public Pago(Usuario User, string Nombre, float Monto)
        {
            this.usuario = User;
            this.id_usuario = User.id;
            this.nombre = Nombre;
            this.monto = Monto;
            this.pagado = false;
        }
        public override string ToString()
        {
            return string.Format("Nombre: {0}, Monto: {1}, Pagado: {2}, Método: {3}, Id: {4}", this.nombre, this.monto, this.pagado, this.metodo, this.id);
        }
    }
}
