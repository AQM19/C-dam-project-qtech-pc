using System;
using System.Collections.Generic;

namespace Entities {

    public partial class EspecieTerrario
    {
        public long Idterrario { get; set; }

        public long Idespecie { get; set; }

        public DateTime? FechaInsercion { get; set; }
    }
}