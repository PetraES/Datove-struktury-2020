using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class RozsahovyStrom<T> where T : ISouradnice
    {

        PrvekRozsahovehoStromu koren;
        int pocetPrvkuVeStrukture;
        int pocetUrovniStromu;
        //delegát porovnani - reseno CompareTo

        static int compX(T a, T b)
        {
            return 0;
        }
       
        public void Vybuduj(List<T> seznamPrvku) 
        {
            if (seznamPrvku == null || seznamPrvku.Count == 0)
            {
                throw new Exception("Strukturu Rozsahový strom nešlo vybudovat, list vrcholů je prázdný nebo neexistuje.");
            }
            else
            {
                pocetPrvkuVeStrukture = seznamPrvku.Count;
                //vybudovani stromu prvni dimenze
                koren = VybudujStrom(seznamPrvku, null, true);
            }
        }
        // dimenzeX - aby bylo zřejmé, jestli se bude budovat od dimenze X nebo Y
        // prvekDruheDimenze - asi aby se odkazal Y interval do Xoveho intervalu
        private PrvekRozsahovehoStromu VybudujStrom(List<T> seznamPrvku, PrvekRozsahovehoStromu prvekDruheDimenze, bool dimenzeX) 
        {
            PrvekRozsahovehoStromu pomocny = VybudujPodstrom(seznamPrvku, null, null, dimenzeX);
            //případné spojení vrcholu podstromu s prvkem opačné dimenze
            if (prvekDruheDimenze != null)
            {
                pomocny.druhaDimenze = prvekDruheDimenze;
            }
            return pomocny;
        }
        private PrvekRozsahovehoStromu VybudujPodstrom(List<T> seznamPrvku, PrvekRozsahovehoStromu predek,
            PrvekRozsahovehoStromu predchoziZListu, bool dimenzeX)
        {  
            PrvekRozsahovehoStromu pomocny;
            if (seznamPrvku.Count >= 2)
            {
                if (dimenzeX == true)
                {
                    //prejmenovat metodu, a implementovat razeni podle X
                    seznamPrvku.Sort(compX);
                }
            }

            //dodelat!
            return predek;
        }
        public T Najdi(ISouradnice obeSouradnice) 
        {

            return default(T); 
        }
        /// <summary>
        /// VErejna implementace pro Intervalove vyhledavani. Vola privatni metodu NajdiInterval, kde je implementovano vyledavani.
        /// </summary>
        /// <param name="levyHorniBod">Levý horní roh v oblasti intervalového vyhledávání.</param>
        /// <param name="pravyDolniBod">Pravý horní roh v oblasti intervalového vyhledávání.</param>
        /// <returns>Vracé seznam vrcholů ve vybraném intervalu. </returns>
        public List<T> NajdiInterval(ISouradnice levyHorniBod, ISouradnice pravyDolniBod) 
        {
            //dle diplomky se pri prvnim volani nastavi vrchol jako koren a dimenzeX na true
            return NajdiInterval(levyHorniBod, pravyDolniBod,koren,true);    
        }

        private List<T> NajdiInterval(ISouradnice levyHorniBod, ISouradnice pravyDolniBod, PrvekRozsahovehoStromu vrchol, bool dimenzeX)
        {

            return new List<T>();
        }

        /// <summary>
        /// Uzel stromu
        /// </summary>
        private class PrvekRozsahovehoStromu
        {
            public PrvekRozsahovehoStromu levyPotomek, pravyPotomek, otec, druhaDimenze;
            public PrvekRozsahovehoStromu predchozi, dalsi;
            public bool platny = false;
            public ISouradnice data;

            public PrvekRozsahovehoStromu()
            {
            }

            public PrvekRozsahovehoStromu(ISouradnice s) 
            {
                data = s;
            }
        }

    }
}
