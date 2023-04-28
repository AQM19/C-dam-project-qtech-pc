using System;

namespace Entities
{
    public partial class Visita
    {
        public long Id { get; set; }

        public long Idusuario { get; set; }

        public long Idterrario { get; set; }

        public DateTime Fecha { get; set; }

        public string Comentario { get; set; }

        public float Puntuacion { get; set; }
    }

}

