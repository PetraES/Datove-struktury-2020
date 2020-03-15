using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class Cesta
    {
        public DataVrcholu AktualniVrchol { get; set; }
        public List<DataHrany> NavstiveneHrany { get; set; }
        public float CenaCeleCesty { get; set; }

        public Cesta()
        {
            NavstiveneHrany = new List<DataHrany>();
        }
    }

    class Dijkstra
    {
        PrioritniFronta<Cesta> D = new PrioritniFronta<Cesta>();
        Cesta NejkratsiNalezenaCesta;

        List<DataVrcholu> VrcholyUzVyresene = new List<DataVrcholu>();

        DataVrcholu konec;

        public void NajdiZastavku(DataVrcholu zacatek, DataVrcholu konec)
        {
            D = new PrioritniFronta<Cesta>();
            NejkratsiNalezenaCesta = null;
            VrcholyUzVyresene.Clear();
            this.konec = konec;

            PrvekHaldy<Cesta> obecnyPrvek = new PrvekHaldy<Cesta>();

            Cesta c = new Cesta();
            c.AktualniVrchol = zacatek;
            c.CenaCeleCesty = 0;
            obecnyPrvek.Informace = c;
            obecnyPrvek.Priorita = 0;

            D.VlozPrvek(obecnyPrvek);
            ProjdiGraf();
        }

        public void VlozCestuDoFronty(DataVrcholu novyVrchol, DataHrany novyUsek, float cena, Cesta dosavadniCesta = null)
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
                    foreach (DataHrany dosavadniHrana in dosavadniCesta.NavstiveneHrany)
                    {
                        cestaSNovymUsekem.NavstiveneHrany.Add(dosavadniHrana);
                    }
                }
                cestaSNovymUsekem.NavstiveneHrany.Add(novyUsek);
                cestaSNovymUsekem.AktualniVrchol = novyVrchol;
                cestaSNovymUsekem.CenaCeleCesty = cena;

                PrvekHaldy<Cesta> obecnyPrvek = new PrvekHaldy<Cesta>();
                obecnyPrvek.Priorita = (int)cena;
                obecnyPrvek.Informace = cestaSNovymUsekem;

                // pokud nahrazovana cesta konci v hledané zastávce nastavime jako nejkratsi cestu 
                if (obecnyPrvek.Informace.AktualniVrchol.NazevVrcholu == konec.NazevVrcholu)
                {
                    NejkratsiNalezenaCesta = obecnyPrvek.Informace;
                }

                //v prioritni fronte hledam cestu, ktera konci v aktualnim vrcholu
                for (int i = 0; i < D.gertruda.Count; i++)
                {
                    if (D.gertruda[i].Informace.AktualniVrchol.Equals(novyVrchol))
                    {
                        //jestlize ma aktualne nalezena cesta mensi cenu, nahradi dosavadni
                        if (D.gertruda[i].Priorita > cena)
                        {
                            D.gertruda[i] = obecnyPrvek;
                            D.PrerovnejHypu();
                            return;
                        }
                    }
                }
                D.VlozPrvek(obecnyPrvek);
            }
        }

        //rekurzivni prohledani haldy aby to spojilo cesty s naslednyma,
        //aby to prochazelo prioritni frontu dokad je co prochazet
        public void ProjdiGraf()

        {
            PrvekHaldy<Cesta> dosavadniProjitaCesta = D.OdeberPrvek();

            if (dosavadniProjitaCesta == null)
            {
                return;
            }

            //v hranate zavorce je x-ty prvek listu - tady je to ten posledni, posledni projita cesta, tady h

            DataHrany h = dosavadniProjitaCesta.Informace.NavstiveneHrany.Count == 0 ? null : dosavadniProjitaCesta.Informace.NavstiveneHrany[dosavadniProjitaCesta.Informace.NavstiveneHrany.Count - 1];
            foreach (DataHrany obecnaHrana in dosavadniProjitaCesta.Informace.AktualniVrchol.ListHran)
            {
                // preskoci cestu po ktere jsme prisli
                if (obecnaHrana == h)
                {
                    continue;
                }

                // Informace je nositel informace, ktera je prioritizovana, tady treba cesta
                // hledam, ktery vrchol se pouzije jako naslednik 
                if (dosavadniProjitaCesta.Informace.AktualniVrchol == obecnaHrana.PocatekHrany)
                {
                    VlozCestuDoFronty(
                        obecnaHrana.KonecHrany,
                        obecnaHrana,
                        dosavadniProjitaCesta.Informace.CenaCeleCesty + obecnaHrana.DelkaHrany,
                        dosavadniProjitaCesta.Informace);
                }
                else if (dosavadniProjitaCesta.Informace.AktualniVrchol == obecnaHrana.KonecHrany)
                {
                    VlozCestuDoFronty(
                        obecnaHrana.PocatekHrany,
                        obecnaHrana,
                        dosavadniProjitaCesta.Informace.CenaCeleCesty + obecnaHrana.DelkaHrany,
                        dosavadniProjitaCesta.Informace);
                }
            }

            //pridam do listu vyresenych vrcholu
            VrcholyUzVyresene.Add(dosavadniProjitaCesta.Informace.AktualniVrchol);
            ProjdiGraf();
        }

        // mozna metoda, kera vrati nejkratsi cestu? 
        public Cesta vratNejkratsiCestu()
        {
            return NejkratsiNalezenaCesta;
        }
    }
}
