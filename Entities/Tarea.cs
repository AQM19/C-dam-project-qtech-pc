using System;
using System.Collections.Generic;

namespace Entities
{

    public partial class Tarea
    {
        public long Id { get; set; }

        public long Idterrario { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaResolucion { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string Estado { get; set; }

    }
}