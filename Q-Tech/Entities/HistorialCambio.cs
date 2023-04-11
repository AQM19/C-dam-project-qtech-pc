namespace Entities
{
    public partial class HistorialCambio
    {
        public long Id { get; set; }

        public long Idusuario { get; set; }

        public string NombreTabla { get; set; }

        public string Accion { get; set; }

        public string Detalles { get; set; }

        public string Observaciones { get; set; }
    }
}