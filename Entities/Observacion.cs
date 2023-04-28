using System;
using System.Collections.Generic;

namespace Entities
{

    public partial class Observacion
    {
        public long Id { get; set; }

        public long Idterrario { get; set; }

        public DateTime Fecha { get; set; }

        public string Texto { get; set; }
    }
}