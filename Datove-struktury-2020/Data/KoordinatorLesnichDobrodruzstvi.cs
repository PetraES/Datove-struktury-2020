using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class KoordinatorLesnichDobrodruzstvi
    {
        // Abstraktni graf ma K,V,H 
        public AbstraktniGraf<string, DataVrcholu, DataHran> AG = new AbstraktniGraf<string, DataVrcholu, DataHran>();
        private Dijkstra dijkstra;
        private EditaceCSV editujCSV = new EditaceCSV();

        // vrcholy jsou body v mapě
        private string cestaKsouboruObce50 = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Obce.csv";
        //hrany jsou cesty v lese
        private string cestaKsouboruCesty50 = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Cesty.csv";

        /// <summary>
        /// Konstruktor. Při konstrukci třídy načte data ze souborů.
        /// </summary>
        public KoordinatorLesnichDobrodruzstvi()
        {
            dijkstra = new Dijkstra(AG);
            nactiVrcholyZCSV();
            nactiHranyZCSV();
        }

        /// <summary>
        /// Načítání vrcholů ze souboru, vynechání hlavičky souboru.
        /// </summary>
        public void nactiVrcholyZCSV()
        {
            List<string[]> objekt = editujCSV.NactiSoubor(cestaKsouboruObce50);

            //ukladani poradi radku do int, aby se pak dala vynechat hlavicka souboru Cesty
            int poradiRadku = 0;
            foreach (string[] radek in objekt)
            {
                poradiRadku++;
                if (poradiRadku == 1)
                {
                    continue;
                }
                DataVrcholu v = new DataVrcholu();
                v.NazevVrcholu = radek[0];
                v.XSouradniceVrcholu = float.Parse(radek[1]);
                v.YSouradniceVrcholu = float.Parse(radek[2]);
                v.TypVrcholu = (TypyVrcholu)int.Parse(radek[3]);
                AG.PridejVrchol(v.NazevVrcholu, v);
            }
        }

        /// <summary>
        /// Vrátí data vrcholu na základě zadaného klíče. Slouží pro nakreslení bodu.
        /// </summary>
        /// <param name="klicVrcholu"></param>
        /// <returns></returns>
        public DataVrcholu najdiVrchol(string klicVrcholu)
        {
            return AG.VratVrchol(klicVrcholu);
        }

        /// <summary>
        /// Načítání hran/cest ze souboru csv.
        /// </summary>
        public void nactiHranyZCSV()
        {
            List<string[]> objekt = editujCSV.NactiSoubor(cestaKsouboruCesty50);
            //ukladani poradi radku do int, aby se pak dala vynechat hlavicka souboru Cesty
            int poradiRadku = 0;
            foreach (string[] radek in objekt)
            {
                poradiRadku++;
                if (poradiRadku == 1)
                {
                    continue;
                }
                DataHran novaHrana = new DataHran(); //vytvorime novou instanci hrany
                novaHrana.PocatekHrany = radek[0];
                novaHrana.KonecHrany = radek[1];
                novaHrana.DelkaHrany = short.Parse(radek[2]);
                AG.PridejHranu(radek[0], radek[1], novaHrana);
            }
        }

        /// <summary>
        /// Zpřístupní kolekci hran pro vykreslení.
        /// </summary>
        /// <returns>Vrací to iterátor hran.</returns>
        public IEnumerable<DataHran> VratHrany()
        {
            return AG.VratSeznamHran();
        }

        /// <summary>
        /// Zpřístupní kolekci vrcholů pro vykreslení.
        /// </summary>
        /// <returns>Vrací to iterátor vrcholů.</returns>
        public IEnumerable<DataVrcholu> GetVrcholy()
        {
            return AG.VratSeznamVrcholu();
        }

        /// <summary>
        /// Vkládání vrcholu do mapy.
        /// </summary>
        /// <param name="x">x-ová souřadnice vrcholu</param>
        /// <param name="y">z-souřadnice vrcholu</param>
        /// <param name="typyVrcholu"> typ vrcholů z výčtu</param>
        /// <param name="nazevVrcholu">název vrcholu, řetězec</param>
        /// <returns>vrací přidáváný vrchol</returns>
        public DataVrcholu vlozVrchol(int x, int y, TypyVrcholu typyVrcholu, string nazevVrcholu)
        {
            if (nazevVrcholu == "")
            {
                throw new Exception("Neplatný název bodu.");
            }
            else if(najdiVrchol(nazevVrcholu) != null)
            {
                throw new Exception("Bod již exitsuje. Prosím zvolte jiný.");
            }

            DataVrcholu v = new DataVrcholu();
            v.XSouradniceVrcholu = x;
            v.YSouradniceVrcholu = y;
            v.TypVrcholu = typyVrcholu;
            v.NazevVrcholu = nazevVrcholu;
            AG.PridejVrchol(v.NazevVrcholu, v);
            return v;
        }

        /// <summary>
        /// Ukládá nové nastavení mapy do souboru.
        /// </summary>
        public void ulozMapu()
        {
            string vrcholy = "nazevVrcholu;Xsouradnice;Ysouradnice\n";
            foreach (DataVrcholu v in AG.VratSeznamVrcholu())
            {
                vrcholy += string.Format("{0};{1};{2};{3}\n",
                    v.NazevVrcholu,
                    v.XSouradniceVrcholu,
                    v.YSouradniceVrcholu,
                    (int)v.TypVrcholu);
            }
            editujCSV.ZapisDoCSV(cestaKsouboruObce50, vrcholy);

            string hrany = "klicPocatek;klicKonec;delkaCesty\n";
            foreach (DataHran h in AG.VratSeznamHran())
            {
                hrany += string.Format("{0};{1};{2}\n",
                    h.PocatekHrany,
                    h.KonecHrany,
                    h.DelkaHrany);
            }
            editujCSV.ZapisDoCSV(cestaKsouboruCesty50, hrany);
        }

        /// <summary>
        /// Hledá nejkratší turistovu cestu.
        /// </summary>
        /// <param name="pocatek">Počáteční bod, na němž se nachází turista.</param>
        /// <param name="konec">Cílový bod, kam chce turista dojít.</param>
        /// <returns>Vrací výsledek algoritmu Dijkstra pro hledání nejkratší cesty.</returns>
        public Cesta najdiCestu(string pocatek, string konec)
        {
            dijkstra.NajdiNejkratsiCestu(pocatek, konec);
            return dijkstra.vratNejkratsiCestu();
        }

        /// <summary>
        /// Vytváří novou hranu, dle zadání uživatele a přidává ji do grafu. 
        /// </summary>
        /// <param name="klicPocatecnihoVrcholu">Klíč počátečního vrcholu</param>
        /// <param name="klicKonecnehoVrcholu">Klíč konečného vrcholu</param>
        /// <param name="delkaHrany">Délka hrany</param>
        /// <returns>Novou hranu.</returns>
        public DataHran vytvorHranu(string klicPocatecnihoVrcholu, string klicKonecnehoVrcholu, short delkaHrany)
        {
            DataHran novaHrana = new DataHran(); //vytvorime novou instanci hrany
            novaHrana.PocatekHrany = klicPocatecnihoVrcholu;
            novaHrana.KonecHrany = klicKonecnehoVrcholu;
            novaHrana.DelkaHrany = delkaHrany;
            AG.PridejHranu(klicPocatecnihoVrcholu, klicKonecnehoVrcholu, novaHrana);
            return novaHrana;
        }
    }
}
