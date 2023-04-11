using System;

namespace Entities
{

    public partial class Notificacion
    {
        public long Id { get; set; }

        public long Idterrario { get; set; }

        public DateTime Fecha { get; set; }

        public string Texto { get; set; }

        public sbyte Vista { get; set; }
    }
}