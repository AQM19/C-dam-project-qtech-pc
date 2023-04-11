using System;
using System.Collections.Generic;

namespace Entities {

    public partial class Terrario
    {
        public long Id { get; set; }

        public long Idusuario { get; set; }

        public string Nombre { get; set; }

        public sbyte Privado { get; set; }

        public string Contrasena { get; set; }

        public sbyte Hibernacion { get; set; }

        public string Sustrato { get; set; }

        public string Descripcion { get; set; }

        public float? Tamano { get; set; }

        public string Ecosistema { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaUltimaModificacion { get; set; }

        public string Foto { get; set; }

        public float? PuntuacionMedia { get; set; }

        public string Etiquetas { get; set; }

        public float? TemperaturaMinima { get; set; }

        public float? TemperaturaMaxima { get; set; }

        public float? TemperaturaMedia { get; set; }

        public float? TemperaturaMinimaHiber { get; set; }

        public float? TemperaturaMaximaHiber { get; set; }

        public float? TemperaturaMediaHiber { get; set; }

        public float? HumedadMinima { get; set; }

        public float? HumedadMedia { get; set; }

        public float? HumedadMaxima { get; set; }

        public int? HorasLuz { get; set; }

        public int? HorasLuzHiber { get; set; }
    }
}