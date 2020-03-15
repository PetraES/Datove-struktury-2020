using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class DataHrany
    {
        public DataVrcholu PocatekHrany { get; set; }
        public DataVrcholu KonecHrany { get; set; }
        public bool JeFunkcniCesta { get; set; }
        public short DelkaHrany { get; set; }
        public bool OznaceniHrany { get; set; }

        public DataHrany()
        {
            JeFunkcniCesta = true;
            OznaceniHrany = false;
        }

    }
}
