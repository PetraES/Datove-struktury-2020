using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    /// <summary>
    /// Třída reprezentující datovou strukturu vrcholu/bodu na mapě.
    /// </summary>
    class DataVrcholu : ISouradnice
    {
        public string NazevVrcholu { get; set; } //property
        public float XSouradniceVrcholu { get; set; } //property
        public float YSouradniceVrcholu { get; set; } //property
        public TypyVrcholu TypVrcholu { get; set; } //property

        public int vratX()
        {
            return (int)XSouradniceVrcholu;
        }

        public int vratY()
        {
            return (int)YSouradniceVrcholu;
        }

        public override string ToString()
        {
            return NazevVrcholu + " (" + XSouradniceVrcholu + ";" + YSouradniceVrcholu + ")";
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
  
    public enum TypyVrcholu
    {
        None = 0,
        odpocivadlo, //je jedna
        zastavka, //je dva
    }
}
