using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class KoordinatorLesnichDobrodruzstvi
    {
        public List<DataVrcholu> vsechnyVrcholy;
        public List<DataHrany> vsechnyHrany;
        private EditaceCSV editujCSV = new EditaceCSV();
        // vrcholy jsou obce
        private string cestaKsouboruObce = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Obce.csv";
        //hrany jsou cesty
        private string cestaKsouboruCesty = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Cesty.csv";

        public KoordinatorLesnichDobrodruzstvi()
        {
            vsechnyVrcholy = vytvorVrcholy();
            vsechnyHrany = vytvorHrany();
        }

        public List<DataVrcholu> vytvorVrcholy()
        {
            EditaceCSV nacteniCSV = new EditaceCSV();
            List<string[]> objekt = nacteniCSV.NactiSoubor(cestaKsouboruObce);
            List<DataVrcholu> vseckyVrcholy = new List<DataVrcholu>();

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
                v.TypVrcholu = (TypyVrcholu)int.Parse(radek[3]); //pretypovat string na typy vrcholu, vyzkouset jestli jde
                vseckyVrcholy.Add(v);
            }
            return vseckyVrcholy;
        }

        public DataVrcholu najdiVrchol(float x, float y)
        {
            foreach (DataVrcholu w in vsechnyVrcholy)
            {
                if (w.XSouradniceVrcholu == x && w.YSouradniceVrcholu == y)
                {
                    return w;
                }
            }
            throw new Exception("vrchol nenalezen");
        }

        public List<DataHrany> vytvorHrany()
        {
            EditaceCSV nacteniCSV = new EditaceCSV();
            List<string[]> objekt = nacteniCSV.NactiSoubor(cestaKsouboruCesty);
            List<DataHrany> listHran = new List<DataHrany>();

            //ukladani poradi radku do int, aby se pak dala vynechat hlavicka souboru Cesty
            int poradiRadku = 0;
            foreach (string[] radek in objekt)
            {
                poradiRadku++;
                if (poradiRadku == 1)
                {
                    continue;
                }
                //vytvorime novou instanci hrany
                DataHrany novaHrana = new DataHrany();
                //najdemem pocatecni vrchol hrany  v listu vrcholů jiz nactenych
                DataVrcholu pocatecniVrchol = najdiVrchol(float.Parse(radek[0]), float.Parse(radek[1]));
                //nalezeny vrchol nastavime jako pocatek hrany
                novaHrana.PocatekHrany = pocatecniVrchol;
                //pocatecnimu vrcholu priradime hranu
                pocatecniVrchol.ListHran.Add(novaHrana);
                DataVrcholu konecnyVrchol = najdiVrchol(float.Parse(radek[2]), float.Parse(radek[3]));
                novaHrana.KonecHrany = konecnyVrchol;
                konecnyVrchol.ListHran.Add(novaHrana);
                novaHrana.DelkaHrany = short.Parse(radek[4]);
                listHran.Add(novaHrana);
            }
            return listHran;
        }

        public List<DataVrcholu> GetVrcholy()
        {
            return vsechnyVrcholy;
        }

        public DataVrcholu vlozVrchol(int x, int y)
        {
            DataVrcholu v = new DataVrcholu();
            v.XSouradniceVrcholu = x;
            v.YSouradniceVrcholu = y;
            vsechnyVrcholy.Add(v);
            return v;
        }

        public void ulozMapu()
        {
            string vrcholy = "nazevVrcholu;Xsouradnice;Ysouradnice\n";
            foreach (DataVrcholu v in vsechnyVrcholy)
            {
                vrcholy += string.Format("{0};{1};{2};{3}\n",
                    v.NazevVrcholu,
                    v.XSouradniceVrcholu,
                    v.YSouradniceVrcholu,
                    (int)v.TypVrcholu);
                    

            }
            editujCSV.ZapisDoCSV(cestaKsouboruObce, vrcholy);

            string hrany = "pocatekX;pocatekY;konecX;konecY;delkaCesty\n";
            foreach (DataHrany h in vsechnyHrany)
            {
                hrany += string.Format("{0};{1};{2};{3};{4}\n",
                    h.PocatekHrany.XSouradniceVrcholu,
                    h.PocatekHrany.YSouradniceVrcholu,
                    h.KonecHrany.XSouradniceVrcholu,
                    h.KonecHrany.YSouradniceVrcholu,
                    h.DelkaHrany);
            }
            editujCSV.ZapisDoCSV(cestaKsouboruCesty, hrany);
        }
    }
}
