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

        // podle List<T>.Sort Method, Comparison<T> Delegate
        static int porovnejPodleX(T a, T b)
        {
            return porovnejCisla(a.vratX(), b.vratX());
        }
        static int porovnejPodleY(T a, T b)
        {
            return porovnejCisla(a.vratY(), b.vratY());
        }

        static int porovnejCisla(int hodnotaA, int hodnotaB)
        {
            if (hodnotaA == hodnotaB)
            {
                return 0;
            }
            else if (hodnotaA > hodnotaB)
            {
                return 1;
            }
            else
            {
                return -1;
            }
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
                // nastaveni intervalu pro navigacni prvek (ten ktery rika jake prvky jsou pod nim)
                if (dimenzeX == true)
                {

                    // sort vraci void nikoli serazenou kolekci, tzn ze zmeni zdrojova data
                    seznamPrvku.Sort(porovnejPodleX);
                    // utridit seznam vrcholu, vzit prvni vrchol a jeho x ovou souradnici 
                    // pouzit jako (zacatek intervalu) navigacniho vrcholu a jeho dalsi souradnici 
                    // pouzit jako (konec intervalu) navigacniho vrcholu, tim vytvorit navigacni vrchol
                    int zacatekIntervalu = seznamPrvku[0].vratX();
                    int konecIntervalu = seznamPrvku[seznamPrvku.Count - 1].vratX();
                    Data2Dim soruradniceNavigacnihoVrcholu = new Data2Dim(zacatekIntervalu, konecIntervalu);
                    // vytvorit instanci prvku pomocneho rozsahoveho stromu do ktereho prijde interval a false
                    // je to vlastne navigacni vrchol - soude podle hodnoty false pri volani konstruktoru
                    pomocny = new PrvekRozsahovehoStromu(soruradniceNavigacnihoVrcholu, false);
                }
                else
                {
                    seznamPrvku.Sort(porovnejPodleY);
                    int zacatekIntervalu = seznamPrvku[0].vratY();
                    int konecIntervalu = seznamPrvku[seznamPrvku.Count - 1].vratY();
                    Data2Dim soruradniceNavigacnihoVrcholu = new Data2Dim(zacatekIntervalu, konecIntervalu);
                    pomocny = new PrvekRozsahovehoStromu(soruradniceNavigacnihoVrcholu, false);
                }
                //vytvoreni seznamů prvků pro levý a pravý podstrom, do nichž se prvky rozdělí
                
               
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
            return NajdiInterval(levyHorniBod, pravyDolniBod, koren, true);
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

            // proměnná platný určuje, zda se jedná o plnohodnotný nebo navigační vrchol
            // je-li prvek platný (true), nese finální data 
            // je-li prvek platný (false), nenese
            public bool platny = false;

            public ISouradnice data;

            // pro prvek prvek rozsahoveho stromu - "list"
            // vynucuje znalost dat(souradnic) pri vytvareni instance
            public PrvekRozsahovehoStromu(ISouradnice s, bool platny)
            {
                this.platny = platny;
                data = s;
            }
        }

    }
}
