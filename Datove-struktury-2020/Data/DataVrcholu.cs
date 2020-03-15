using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class DataVrcholu
    {
        public string NazevVrcholu { get; set; } //property
        public float XSouradniceVrcholu { get; set; } //property
        public float YSouradniceVrcholu { get; set; } //property
        public TypyVrcholu TypVrcholu { get; set; } //property
        public List<DataHrany> ListHran { get; set; } //property proto je to velkym pismenem

        public DataVrcholu()
        {
            ListHran = new List<DataHrany>();
        }

        public override string ToString()
        {
            return NazevVrcholu + " (" + XSouradniceVrcholu + ";" + YSouradniceVrcholu + ")";
        }
    }

    
    public enum TypyVrcholu
    {
        None = 0,
        odpocivadlo, //je jedna
        zastavka, //je dva
    }
}
