using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class RozsahovyStrom<T> 
    {
        PrvekRozsahovehoStromu koren;
        
        public void Vybuduj() 
        { 
           
        }
        private void VybudujStrom() { }
        private void VybudujPodstrom() { }
        public T Najdi() 
        {
            return default(T); 
        }
        public List<T> NajdiInterval() 
        {
            return new List<T> ();    
        }

        private class PrvekRozsahovehoStromu
        {
            PrvekRozsahovehoStromu levyPotomek, pravyPotomek, otec, druhaDimenze;
            PrvekRozsahovehoStromu predchozi, dalsi;
            bool platny = false;
            ISouradnice data;

            public PrvekRozsahovehoStromu(T x, T y) 
            {
                
            }
        }

    }
}
