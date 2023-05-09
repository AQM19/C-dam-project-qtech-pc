using System;
using System.Collections.Generic;

namespace Entities {

    public partial class Especie
    {
        public long Id { get; set; }

        public string Genero { get; set; }

        public string Sp { get; set; }

        public string NombreComun { get; set; }

        public string Descripcion { get; set; }

        public string Imagen { get; set; }

        public string Ecosistema { get; set; }

        public int? DuracionVida { get; set; }

        public sbyte Hiberna { get; set; }

        public float TemperaturaMinima { get; set; }

        public float TemperaturaMaxima { get; set; }

        public float? TemperaturaHibMinima { get; set; }

        public float? TemperaturaHibMaxima { get; set; }

        public float HumedadMinima { get; set; }

        public float HumedadMaxima { get; set; }

        public int HorasLuz { get; set; }

        public int? HorasLuzHib { get; set; }
        public string Tipo { get; set; }
    }
}