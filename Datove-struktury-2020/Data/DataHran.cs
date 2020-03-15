using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class DataHran
    {
        public string PocatekHrany { get; set; }
        public string KonecHrany { get; set; }
        public bool JeFunkcniCesta { get; set; }
        public short DelkaHrany { get; set; }
        public bool OznaceniHrany { get; set; }

        public DataHran()
        {
            JeFunkcniCesta = true;
            OznaceniHrany = false;
        }

    }
}
