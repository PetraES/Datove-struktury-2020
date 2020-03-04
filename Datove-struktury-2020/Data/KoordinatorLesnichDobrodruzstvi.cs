using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class KoordinatorLesnichDobrodruzstvi
    {
        public List<Vrchol> vsechnyVrcholy;
        public List<Hrana> vsechnyHrany;
        public KoordinatorLesnichDobrodruzstvi()
        {
            vsechnyVrcholy = vytvorVrcholy();
            vsechnyHrany = vytvorHrany();
        }
        public List<Vrchol> vytvorVrcholy()
        {
            NacteniCSV nacteniCSV = new NacteniCSV();
            List<string[]> objekt = nacteniCSV.NactiSoubor(@"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Obce.csv");
            List<Vrchol> vseckyVrcholy = new List<Vrchol>();
            foreach (string[] radek in objekt)
            {
                Vrchol v = new Vrchol();
                v.NazevVrcholu = radek[0];
                v.XSouradniceVrcholu = float.Parse(radek[1]);
                v.YSouradniceVrcholu = float.Parse(radek[2]);
               // v.TypVrcholu = (TypyVrcholu)int.Parse(radek[3]); //pretypovat string na typy vrcholu, vyzkouset jestli jde
                vseckyVrcholy.Add(v);
            }
            return vseckyVrcholy;
        }

        public Vrchol najdiVrchol(float x, float y)
        {
            foreach (Vrchol w in vsechnyVrcholy)
            {
                if (w.XSouradniceVrcholu == x && w.YSouradniceVrcholu == y)
                {
                    return w;
                }
            }
            throw new Exception("vrchol nenalezen");
        }

        public List<Hrana> vytvorHrany()
        {
            NacteniCSV nacteniCSV = new NacteniCSV();
            List<string[]> objekt = nacteniCSV.NactiSoubor(@"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Cesty.csv");
            List<Hrana> listHran = new List<Hrana>();
            foreach (string[] radek in objekt)
            {
                //vytvorime novou instanci hrany
                Hrana novaHrana = new Hrana();
                //najdemem pocatecni vrchol hrany  v listu vrcholů jiz nactenych
                Vrchol pocatecniVrchol = najdiVrchol(float.Parse(radek[0]), float.Parse(radek[1]));
                //nalezeny vrchol nastavime jako pocatek hrany
                novaHrana.PocatekHrany = pocatecniVrchol;
                //pocatecnimu vrcholu priradime hranu
                pocatecniVrchol.ListHran.Add(novaHrana);   
                Vrchol konecnyVrchol = najdiVrchol(float.Parse(radek[2]), float.Parse(radek[3]));
                novaHrana.KonecHrany = konecnyVrchol;
                konecnyVrchol.ListHran.Add(novaHrana);
                novaHrana.DelkaHrany = short.Parse(radek[4]);
                listHran.Add(novaHrana);
            }
            return listHran;
        }

        public List<Vrchol> GetVrcholy()
        {
            return vsechnyVrcholy;
        }

    }
}
