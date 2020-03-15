﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class KoordinatorLesnichDobrodruzstvi
    {
        public AbstraktniGraf<string, DataVrcholu, DataHran> AG = new AbstraktniGraf<string, DataVrcholu, DataHran>();
        private Dijkstra dijkstra;
        private EditaceCSV editujCSV = new EditaceCSV();

        // vrcholy jsou obce
        private string cestaKsouboruObce = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Obce.csv";
        //hrany jsou cesty
        private string cestaKsouboruCesty = @"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\Cesty.csv";

        public KoordinatorLesnichDobrodruzstvi()
        {
            dijkstra = new Dijkstra(AG);
            nactiVrcholyZCSV();
            nactiHranyZCSV();
        }

        public void nactiVrcholyZCSV()
        {

            List<string[]> objekt = editujCSV.NactiSoubor(cestaKsouboruObce);


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

        // mazat nebo prepsat
        public DataVrcholu najdiVrchol(string klicVrcholu)
        {
            return AG.VratVrchol(klicVrcholu);
        }

        public void nactiHranyZCSV()
        {
            List<string[]> objekt = editujCSV.NactiSoubor(cestaKsouboruCesty);
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

        public IEnumerable<DataHran> VratHrany()
        {
            return AG.VratSeznamHran();
        }

        public IEnumerable<DataVrcholu> GetVrcholy()
        {
            return AG.VratSeznamVrcholu();
        }

        public DataVrcholu vlozVrchol(int x, int y, TypyVrcholu typyVrcholu, string nazevVrcholu)
        {
            if (nazevVrcholu == "")
            {
                throw new Exception("Neplatny nazev bodu.");
            }
            else if(najdiVrchol(nazevVrcholu) != null)
            {
                throw new Exception("Bod jiz exitsuje.");
            }

            DataVrcholu v = new DataVrcholu();
            v.XSouradniceVrcholu = x;
            v.YSouradniceVrcholu = y;
            v.TypVrcholu = typyVrcholu;
            v.NazevVrcholu = nazevVrcholu;
            AG.PridejVrchol(v.NazevVrcholu, v);
            return v;
        }

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
            editujCSV.ZapisDoCSV(cestaKsouboruObce, vrcholy);

            string hrany = "klicPocatek;klicKonec;delkaCesty\n";
            foreach (DataHran h in AG.VratSeznamHran())
            {
                hrany += string.Format("{0};{1};{2}\n",
                    h.PocatekHrany,
                    h.KonecHrany,
                    h.DelkaHrany);
            }
            editujCSV.ZapisDoCSV(cestaKsouboruCesty, hrany);
        }

        public Cesta najdiCestu(string pocatek, string konec)
        {
            dijkstra.NajdiZastavku(pocatek, konec);
            return dijkstra.vratNejkratsiCestu();
        }

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
