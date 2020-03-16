using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Datove_struktury_2020.Data
{
    class AbstraktniGraf<K, V, H>
    {
        private IDictionary<K, Vrchol> vsechnyVrcholy = new Dictionary<K, Vrchol>();

        //metoda na pridavani vrcholu
        public void PridejVrchol(K klic, V pridavanyVrchol)
        {
            Vrchol w = new Vrchol();
            w.Data = pridavanyVrchol;
            vsechnyVrcholy.Add(klic, w);
        }


        public void PridejHranu(K klicPocatecnihoVrcholu, K klicKoncovehoVrcholu, H pridavanaHrana)
        {
            if (vsechnyVrcholy.TryGetValue(klicPocatecnihoVrcholu, out Vrchol pocatecniVrchol)
                && vsechnyVrcholy.TryGetValue(klicKoncovehoVrcholu, out Vrchol koncovyVrchol))
            {
                Hrana h = new Hrana();
                h.Data = pridavanaHrana;
                pocatecniVrchol.SeznamHran.AddLast(h);
                koncovyVrchol.SeznamHran.AddLast(h);
            }
            else
            {
                throw new Exception("vrchol neexistuje");
            }
        }

        public List<H> VratIncidentniHrany(K klicVrcholu)
        {
            if (vsechnyVrcholy.TryGetValue(klicVrcholu, out Vrchol hledanyVrchol))
            {
                // mapovaci funkce
                return hledanyVrchol.SeznamHran.Select(h => h.Data).ToList();
            }
            else
            {   
                //pokud nenajdu nic v datech, vratim novy prazdny list hran
                return new List<H>();
            }
        }

        public V VratVrchol(K klic)
        {
            // pokusime se najit vrchol ve slovniku
            // pokud vrchol neexistuje vratime "null"
            if (vsechnyVrcholy.TryGetValue(klic, out Vrchol hledanyVrchol))
            {
                return hledanyVrchol.Data;
            }
            else
            {
                return default(V);
            }
        }

        public IEnumerable<V> VratSeznamVrcholu()
        {
            foreach (Vrchol w in vsechnyVrcholy.Values)
            {
                // prubezne vraci hodnoty
                yield return w.Data;
            }
        }

        public IEnumerable<H> VratSeznamHran()
        {
            //prochazi seznam vrcholu a vraci incidentni hrany
            List<H> vraceneHrany = new List<H>();
            foreach (var w in vsechnyVrcholy)
            {
                // prubezne vraci hodnoty
                List<H> incidentniHrany = VratIncidentniHrany(w.Key);
                
                // h je pomocna promenna pro uchopeni jednotlivych prvku v listu "incidentniHrany" 
                // pri filtrovani jednotlivych prvku
                incidentniHrany = incidentniHrany.Where(h => !vraceneHrany.Contains(h)).ToList<H>();

                foreach (H incidentniHrana in incidentniHrany) 
                {
                    vraceneHrany.Add(incidentniHrana);
                    //prubezne vraci hodnoty
                    yield return incidentniHrana; 
                }

            }
        }

        private class Vrchol
        {
            public V Data { get; set; }
            public LinkedList<Hrana> SeznamHran = new LinkedList<Hrana>();
        }

        private class Hrana
        { 
            public K PocatecniVrchol { get; set; }
            public K KonecnyVrchol { get; set; }
            public H Data { get; set; }
        }

    }
}
