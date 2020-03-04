using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class Hrana
    {
        public Vrchol PocatekHrany { get; set; }
        public Vrchol KonecHrany { get; set; }
        public bool JeFunkcniCesta { get; set; }
        public short DelkaHrany { get; set; }
        public bool OznaceniHrany { get; set; }

        public Hrana()
        {
            JeFunkcniCesta = true;
        }

    }
}
