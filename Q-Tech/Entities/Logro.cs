﻿using System;
using System.Collections.Generic;

namespace Entities
{

    public partial class Logro
    {
        public long Id { get; set; }

        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public string Icono { get; set; }

        public sbyte Disponible { get; set; }
    }
}