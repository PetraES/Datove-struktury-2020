using Datove_struktury_2020.data_sem_c;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class KoordinatorLesnichDobrodruzstvi
    {
        private readonly RozsahovyStrom<DataVrcholu> rs = new RozsahovyStrom<DataVrcholu>();
        // Abstraktni graf ma K,V,H 
        private AbstraktniGraf<string, DataVrcholu, DataHran> ag = new AbstraktniGraf<string, DataVrcholu, DataHran>();
        // Deklarace Abstraktniho souboru K, Z ale inicializuje se az v konstruktoru, r33
        private AbstraktniSoubor<string, DataVrcholu> abstraktniSoubor;
        private readonly Dijkstra dijkstra;
        private readonly EditaceCSV editorCSV = new EditaceCSV();

        // vrcholy jsou body v mapě
        private readonly string cestaKsouboruObce50 = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Obce2.csv";
        // hrany jsou cesty v lese
        private readonly string cestaKsouboruCesty50 = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Cesty.csv";
        // cesta, kam se bude ukladat abstraktni soubor SEM C
        private readonly string cestaKsouboruSemC = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\SemCDadaAS";
        // cesta ku csv souboru pro SEM C
        private readonly string cestaKsouboruSemCOrigo = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\mestaCZ.csv";

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
            abstraktniSoubor = new AbstraktniSoubor<string, DataVrcholu>(cestaKsouboruSemC);
            NactiVrcholyProSemA();
            NactiHranyZCSV();
            if (File.Exists(cestaKsouboruSemC) == false)
            {
                NactiVrcholyProSemC();
            }
        }

        /// <summary>
        /// Načítání vrcholů pro abstraktní soubor SEM C.
        /// </summary>
        public void NactiVrcholyProSemC()
        {
            List<DataVrcholu> vrcholy5000 = NactiDataVrcholuZCsv(cestaKsouboruSemCOrigo);
            List<KeyValuePair<string, DataVrcholu>> listDvouHodnot = new List<KeyValuePair<string, DataVrcholu>>();
            for (int i=0; i <= vrcholy5000.Count -1; i++)
            {
                DataVrcholu dv = vrcholy5000[i]; //zalozit promennou vrcholu, protoze forem mam jen indexy
                KeyValuePair<string, DataVrcholu> kvp = new KeyValuePair<string, DataVrcholu>(dv.NazevVrcholu,dv);
                listDvouHodnot.Add(kvp);
            }           
            // blokacni faktor nastaven na 5
            abstraktniSoubor.VybudujSoubor(listDvouHodnot,5);
            // todo nacpi do AS
        }

        /// <summary>
        /// Načítání vrcholů pro mapu SEM A.
        /// </summary>
        public void NactiVrcholyProSemA()
        {
            List<DataVrcholu> vrcholy = NactiDataVrcholuZCsv(cestaKsouboruObce50);
            foreach (DataVrcholu dv in vrcholy)
            {
                ag.PridejVrchol(dv.NazevVrcholu, dv);
            }
            // List ma pretizeny konstruktor, ktery je schopny prijmout kolekci typu IEnumerable
            rs.Vybuduj(new List<DataVrcholu>(ag.VratSeznamVrcholu()));

        }

        /// <summary>
        /// Načítání vrcholů ze souboru, vynechání hlavičky souboru. Hlavicka může zůstat.
        /// </summary>
        /// <param name="cesta">Cesta k souboru.</param>
        /// <returns>List Vrcholů.</returns>
        public List<DataVrcholu> NactiDataVrcholuZCsv(String cesta)
        {
            List<DataVrcholu> vysledek = new List<DataVrcholu>();
            List<string[]> objekt = editorCSV.NactiSoubor(cesta);

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
                vysledek.Add(v);
            }
            return vysledek;
        }

        /// <summary>
        /// Vrátí data vrcholu na základě zadaného klíče. Slouží pro nakreslení bodu.
        /// </summary>
        /// <param name="klicVrcholu"></param>
        /// <returns></returns>
        public DataVrcholu NajdiVrcholSemA(string klicVrcholu)
        {
            return ag.VratVrchol(klicVrcholu);
        }

        public DataVrcholu NajdiVrcholSemC(string klic, ZpusobVyhledvani zv)
        {
            return abstraktniSoubor.VyhledejSpecifickyZaznam(klic, zv);
        }

        public DataVrcholu OdeberVrcholSemC(string klic)
        {
            return abstraktniSoubor.OdeberSpecifickyZaznam(klic);
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
            else if(NajdiVrcholSemA(nazevVrcholu) != null)
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
