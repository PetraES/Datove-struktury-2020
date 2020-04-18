using Datove_struktury_2020.data_sem_c;
using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    /// <summary>
    /// Třída reprezentující datovou strukturu vrcholu/bodu na mapě.
    /// </summary>
    [Serializable]
    class DataVrcholu : ISouradnice, IVelikostZaznamu
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

        public int vratVelikostZaznamu()
        {
            return 500;
        }

        public override string ToString()
        {
            return NazevVrcholu + " (" + XSouradniceVrcholu + ";" + YSouradniceVrcholu + ")";
        }

        public int CompareTo(object obj)
        {
            //kontrola, jestli vkladany objekt je spravneho/stejneho datoveho typu
            if (obj != null && obj is ISouradnice)
            {
                //vrcholy jsou stejne
                if (XSouradniceVrcholu == ((ISouradnice)obj).vratX() && YSouradniceVrcholu == ((ISouradnice)obj).vratY())
                {
                    return 0;
                }
                // jestlize je porovnavany vrchol mensi nebo vetsi nez obj nas nezajima, tak neresim
                return -1;
            }
            else
            {
                throw new ArgumentException("Porovnavany objekt neni stejneho datoveho typu!");
            }
        }      
    }
  
    public enum TypyVrcholu
    {
        None = 0,
        odpocivadlo, //je jedna
        zastavka, //je dva
    }
}
