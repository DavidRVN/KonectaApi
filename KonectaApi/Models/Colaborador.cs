using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KonectaApi.Models
{
    public class Colaborador
    {
        public string NumeroIdentificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public decimal Salario { get; set; }
        public int IdArea { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Sexo { get; set; }
    }
}