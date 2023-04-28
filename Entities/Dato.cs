using System;

namespace Entities
{

    public partial class Dato
    {
        public long Id { get; set; }

        public long Idterrario { get; set; }

        public DateTime Fecha { get; set; }

        public float Temperatura { get; set; }

        public float Humedad { get; set; }

        public int Luz { get; set; }
    }
}