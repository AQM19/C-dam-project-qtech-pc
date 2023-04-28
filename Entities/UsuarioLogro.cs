using System;
using System.Collections.Generic;

namespace Entities
{

    public partial class UsuarioLogro
    {
        public long Idusuario { get; set; }

        public long Idlogro { get; set; }

        public DateTime FechaAdquisicion { get; set; }
    }
}