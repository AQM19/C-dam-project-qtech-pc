using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class Usuario
    {
        public long Id { get; set; }

        public string NombreUsuario { get; set; }

        public string Contrasena { get; set; }

        public string Nombre { get; set; }

        public string Apellido1 { get; set; }

        public string Apellido2 { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public string Email { get; set; }

        public string Telefono { get; set; }

        public string FotoPerfil { get; set; }

        public byte[] Salt { get; set; }

        public sbyte Borrado { get; set; }

        public string Perfil { get; set; }
    }
}