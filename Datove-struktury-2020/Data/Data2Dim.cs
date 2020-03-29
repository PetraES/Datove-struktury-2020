using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class Data2Dim : ISouradnice
    {
        private int prvniKlicovaHodnota;
        private int druhaKlicovaHodnota;

        public Data2Dim (int prvni, int druha)
        {
            prvniKlicovaHodnota = prvni;
            this.druhaKlicovaHodnota = druha;
        }

        public int CompareTo(object obj)
        {
            //kontrola, jestli vkladany objekt je spravneho/stejneho datoveho typu
            if (obj != null && obj is ISouradnice)
            {
                //vrcholy jsou stejne
                if (prvniKlicovaHodnota == ((ISouradnice)obj).vratX() && druhaKlicovaHodnota == ((ISouradnice)obj).vratY())
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

        public int vratX()
        {
            return prvniKlicovaHodnota;
        }

        public int vratY()
        {
            return druhaKlicovaHodnota;
        }

        public override string ToString()
        {
            return String.Format("({0};{1})", vratX(), vratY());
        }
    }
}
