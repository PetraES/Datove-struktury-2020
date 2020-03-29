using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class KoordinatorLesnichDobrodruzstvi
    {
        private readonly RozsahovyStrom<DataVrcholu> rs = new RozsahovyStrom<DataVrcholu>();
        // Abstraktni graf ma K,V,H 
        private AbstraktniGraf<string, DataVrcholu, DataHran> ag = new AbstraktniGraf<string, DataVrcholu, DataHran>();
        private readonly Dijkstra dijkstra;
        private readonly EditaceCSV editorCSV = new EditaceCSV();

        // vrcholy jsou body v mapě
        private readonly string cestaKsouboruObce50 = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Obce.csv";
        //hrany jsou cesty v lese
        private readonly string cestaKsouboruCesty50 = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Cesty.csv";

        public List<DataVrcholu> ZpracujInterval(ISouradnice a, ISouradnice b)
        {
            return rs.NajdiInterval(a,b);
        }

        /// <summary>
        /// Konstruktor. Při konstrukci třídy načte data ze souborů.
        /// </summary>
        public KoordinatorLesnichDobrodruzstvi()
        {
            dijkstra = new Dijkstra(ag);
            NactiVrcholyZCSV();
            NactiHranyZCSV();
        }

        /// <summary>
        /// Načítání vrcholů ze souboru, vynechání hlavičky souboru.
        /// </summary>
        public void NactiVrcholyZCSV()
        {
            List<string[]> objekt = editorCSV.NactiSoubor(cestaKsouboruObce50);

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
                ag.PridejVrchol(v.NazevVrcholu, v);
            }
            // List ma pretizeny konstruktor, ktery je schopny prijmout kolekci typu IEnumerable
            rs.Vybuduj(new List<DataVrcholu>(ag.VratSeznamVrcholu()));
        }

        /// <summary>
        /// Vrátí data vrcholu na základě zadaného klíče. Slouží pro nakreslení bodu.
        /// </summary>
        /// <param name="klicVrcholu"></param>
        /// <returns></returns>
        public DataVrcholu NajdiVrchol(string klicVrcholu)
        {
            return ag.VratVrchol(klicVrcholu);
        }

        /// <summary>
        /// Načítání hran/cest ze souboru csv.
        /// </summary>
        public void NactiHranyZCSV()
        {
            List<string[]> objekt = editorCSV.NactiSoubor(cestaKsouboruCesty50);
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
                ag.PridejHranu(radek[0], radek[1], novaHrana);
            }
        }

        /// <summary>
        /// Zpřístupní kolekci hran pro vykreslení.
        /// </summary>
        /// <returns>Vrací to iterátor hran.</returns>
        public IEnumerable<DataHran> VratHrany()
        {
            return ag.VratSeznamHran();
        }

        public IEnumerable<DataHran> VratIncedencniHrany(string klicVrcholu)
        {
            return ag.VratIncidentniHrany(klicVrcholu);
        }

        /// <summary>
        /// Zpřístupní kolekci vrcholů pro vykreslení.
        /// </summary>
        /// <returns>Vrací to iterátor vrcholů.</returns>
        public IEnumerable<DataVrcholu> GetVrcholy()
        {
            return ag.VratSeznamVrcholu();
        }

        /// <summary>
        /// Vkládání vrcholu do mapy.
        /// </summary>
        /// <param name="x">x-ová souřadnice vrcholu</param>
        /// <param name="y">z-souřadnice vrcholu</param>
        /// <param name="typyVrcholu"> typ vrcholů z výčtu</param>
        /// <param name="nazevVrcholu">název vrcholu, řetězec</param>
        /// <returns>vrací přidáváný vrchol</returns>
        public DataVrcholu VlozVrchol(int x, int y, TypyVrcholu typyVrcholu, string nazevVrcholu)
        {
            if (nazevVrcholu == "")
            {
                throw new Exception("Neplatný název bodu.");
            }
            else if(NajdiVrchol(nazevVrcholu) != null)
            {
                throw new Exception("Bod již exitsuje. Prosím zvolte jiný.");
            }

            DataVrcholu v = new DataVrcholu();
            v.XSouradniceVrcholu = x;
            v.YSouradniceVrcholu = y;
            v.TypVrcholu = typyVrcholu;
            v.NazevVrcholu = nazevVrcholu;
            ag.PridejVrchol(v.NazevVrcholu, v);
            // po pridani vrcholu znovu vybudujeme RozsahovyStrom
            // List ma pretizeny konstruktor, ktery je schopny prijmout kolekci typu IEnumerable
            rs.Vybuduj(new List<DataVrcholu>(ag.VratSeznamVrcholu()));
            return v;
        }

        /// <summary>
        /// Ukládá nové nastavení mapy do souboru.
        /// </summary>
        public void UlozMapu()
        {
            string vrcholy = "nazevVrcholu;Xsouradnice;Ysouradnice\n";
            foreach (DataVrcholu v in ag.VratSeznamVrcholu())
            {
                vrcholy += string.Format("{0};{1};{2};{3}\n",
                    v.NazevVrcholu,
                    v.XSouradniceVrcholu,
                    v.YSouradniceVrcholu,
                    (int)v.TypVrcholu);
            }
            editorCSV.ZapisDoCSV(cestaKsouboruObce50, vrcholy);

            string hrany = "klicPocatek;klicKonec;delkaCesty\n";
            foreach (DataHran h in ag.VratSeznamHran())
            {
                hrany += string.Format("{0};{1};{2}\n",
                    h.PocatekHrany,
                    h.KonecHrany,
                    h.DelkaHrany);
            }
            editorCSV.ZapisDoCSV(cestaKsouboruCesty50, hrany);
        }

        /// <summary>
        /// Hledá nejkratší turistovu cestu.
        /// </summary>
        /// <param name="pocatek">Počáteční bod, na němž se nachází turista.</param>
        /// <param name="konec">Cílový bod, kam chce turista dojít.</param>
        /// <returns>Vrací výsledek algoritmu Dijkstra pro hledání nejkratší cesty.</returns>
        public Cesta NajdiCestu(string pocatek, string konec)
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
        public DataHran VytvorHranu(string klicPocatecnihoVrcholu, string klicKonecnehoVrcholu, short delkaHrany)
        {
            DataHran novaHrana = new DataHran(); //vytvorime novou instanci hrany
            novaHrana.PocatekHrany = klicPocatecnihoVrcholu;
            novaHrana.KonecHrany = klicKonecnehoVrcholu;
            novaHrana.DelkaHrany = delkaHrany;
            ag.PridejHranu(klicPocatecnihoVrcholu, klicKonecnehoVrcholu, novaHrana);
            return novaHrana;
        }
    }
}
