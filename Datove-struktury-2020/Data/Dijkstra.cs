using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    /// <summary>
    /// Uchovává strukturu cesty. Cesta je složená z několikáti hran, nepř jednoho bodu do druhého.
    /// </summary>
    class Cesta
    {
        public string AktualniVrchol { get; set; }
        public List<DataHran> NavstiveneHrany { get; set; }
        public float CenaCeleCesty { get; set; }

        public Cesta()
        {
            NavstiveneHrany = new List<DataHran>();
        }
    }

    /// <summary>
    /// Algoritmus hledající nejkratší cestu v grafu. Využívá prioritní fronty.
    /// </summary>
    class Dijkstra
    {
        AbstraktniGraf<string, DataVrcholu, DataHran> ag;
        PrioritniFronta<Cesta> prioritniFronta = new PrioritniFronta<Cesta>();
        Cesta NejkratsiNalezenaCesta;
        List<string> VrcholyUzVyresene = new List<string>();
        string klicKonce;

        /// <summary>
        /// Konstruktor třídy Dijkstra.
        /// </summary>
        /// <param name="ag">Instance abstraktního grafu.</param>
        public Dijkstra(AbstraktniGraf<string, DataVrcholu, DataHran> ag)
        {
            this.ag = ag;
        }
        
        /// <summary>
        /// Metoda hledá nejkratší cestu z počátečního ke zvolenému cílovému bodu.
        /// </summary>
        /// <param name="zacatek">Počáteční bod, ze kterého vychází sběrač hub, neboli houbař.</param>
        /// <param name="konec">Cílový bod, který hledač hub hledá.</param>
        public void NajdiNejkratsiCestu(string zacatek, string konec)
        {
            prioritniFronta = new PrioritniFronta<Cesta>();
            NejkratsiNalezenaCesta = null;
            VrcholyUzVyresene.Clear();
            this.klicKonce = konec;

            Cesta c = new Cesta();
            c.AktualniVrchol = zacatek;
            c.CenaCeleCesty = 0;

            prioritniFronta.VlozPrvek(c, 0);
            ProjdiGraf();
        }

        /// <summary>
        /// Vkládá nově vygenerovanou cestu do prioritní fronty.
        /// </summary>
        /// <param name="novyVrchol">Koncový vrchol nové cesty.</param>
        /// <param name="novyUsek">Přidávaná hrana do cesty.</param>
        /// <param name="cena">Ohodnocení cesty.</param>
        /// <param name="dosavadniCesta">Původní cesta bez přidávané hrany.</param>
        private void VlozCestuDoFronty(string novyVrchol, DataHran novyUsek, float cena, Cesta dosavadniCesta = null)
        {
            // jestli objekt vubec existuje a cena doposud nalezene cesty je mensi nez vkladana tak se nic nemeni 
            if (NejkratsiNalezenaCesta != null && NejkratsiNalezenaCesta.CenaCeleCesty <= cena)
            {
                return;
            }

            if (VrcholyUzVyresene.Contains(novyVrchol))
            {
                return;
            }
            else
            {
                Cesta cestaSNovymUsekem = new Cesta();
                if (dosavadniCesta != null)
                {
                    foreach (DataHran dosavadniHrana in dosavadniCesta.NavstiveneHrany)
                    {
                        cestaSNovymUsekem.NavstiveneHrany.Add(dosavadniHrana);
                    }
                }
                cestaSNovymUsekem.NavstiveneHrany.Add(novyUsek);
                cestaSNovymUsekem.AktualniVrchol = novyVrchol;
                cestaSNovymUsekem.CenaCeleCesty = cena;


                // pokud nahrazovana cesta konci v hledané zastávce nastavime jako nejkratsi cestu 
                if (cestaSNovymUsekem.AktualniVrchol == klicKonce)
                {
                    NejkratsiNalezenaCesta = cestaSNovymUsekem;
                }

                foreach (Cesta cestaZGertrudy in prioritniFronta.vratPrvky())
                {
                    //v prioritni fronte hledam cestu, ktera konci v aktualnim vrcholu
                    //jestlize ma aktualne nalezena cesta mensi cenu, nahradi dosavadni
                    if (cestaZGertrudy.AktualniVrchol.Equals(novyVrchol) && cena < cestaZGertrudy.CenaCeleCesty)
                    {
                        bool nahrazeno = prioritniFronta.NahradPrvek(cestaZGertrudy, cestaSNovymUsekem, (int)cestaSNovymUsekem.CenaCeleCesty);
                        if (nahrazeno)
                        {
                            return;
                        } 
                    }
                }
                prioritniFronta.VlozPrvek(cestaSNovymUsekem, (int)cena);
            }
        }

        /// <summary>
        /// Rekurzivní prohledávání haldy (procházení grafu/lesa apod). Cílem je nalezení nejratší cesty v daném grafu. 
        /// </summary>
        //rekurzivni prohledani haldy aby to spojilo cesty s naslednyma,
        //aby to prochazelo prioritni frontu dokad je co prochazet
        public void ProjdiGraf()

        {
            Cesta dosavadniProjitaCesta = prioritniFronta.OdeberPrvek();
            if (dosavadniProjitaCesta == null)
            {
                return;
            }

            //v hranate zavorce je x-ty prvek listu - tady je to ten posledni, posledni projita cesta, tady h
            DataHran h = dosavadniProjitaCesta.NavstiveneHrany.Count == 0 ? null : dosavadniProjitaCesta.NavstiveneHrany[dosavadniProjitaCesta.NavstiveneHrany.Count - 1];
            
            foreach (DataHran obecnaHrana in ag.VratIncidentniHrany(dosavadniProjitaCesta.AktualniVrchol))
            {
                // preskoci cestu po ktere jsme prisli
                if (obecnaHrana == h)
                {
                    continue;
                }

                // TODO: pokud bude potreba, mohou se vynechat cesty polomu viz kod nize.
                // Je potreba dodat tlacitka pripadne
                // if (obecnaHrana.JeFunkcniCesta == false)
                // {
                //     continue;
                // }

                // Informace je nositel informace, ktera je prioritizovana, tady treba cesta
                // hledam, ktery vrchol se pouzije jako naslednik 
                if (dosavadniProjitaCesta.AktualniVrchol == obecnaHrana.PocatekHrany)
                {
                    VlozCestuDoFronty(
                        obecnaHrana.KonecHrany,
                        obecnaHrana,
                        dosavadniProjitaCesta.CenaCeleCesty + obecnaHrana.DelkaHrany,
                        dosavadniProjitaCesta);
                }
                else if (dosavadniProjitaCesta.AktualniVrchol == obecnaHrana.KonecHrany)
                {
                    VlozCestuDoFronty(
                        obecnaHrana.PocatekHrany,
                        obecnaHrana,
                        dosavadniProjitaCesta.CenaCeleCesty + obecnaHrana.DelkaHrany,
                        dosavadniProjitaCesta);
                }
            }

            //pridam do listu vyresenych vrcholu
            VrcholyUzVyresene.Add(dosavadniProjitaCesta.AktualniVrchol);
            ProjdiGraf();
        }

        /// <summary>
        /// Vrací nejkratší cestu.
        /// </summary>
        /// <returns>Vrací nejkratší cestu.</returns>
        // mozna metoda, kera vrati nejkratsi cestu? 
        public Cesta vratNejkratsiCestu()
        {
            return NejkratsiNalezenaCesta;
        }
    }
}
