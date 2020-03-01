using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class Vrchol
    {
        public string NazevVrcholu { get; set; } //property
        public float XSouradniceVrcholu { get; set; } //property
        public float YSouradniceVrcholu { get; set; } //property
        public TypyVrcholu TypVrcholu { get; set; } //property
        List<Hrana> ListHran { get; set; } //property proto je to velkym pismenem

        public Vrchol()
        {
            ListHran = new List<Hrana>();
        }
    }

    public enum TypyVrcholu
    {
        None = 0,
        odpocivadlo, //je jedna
        zastavka, //je dva
    }
}
