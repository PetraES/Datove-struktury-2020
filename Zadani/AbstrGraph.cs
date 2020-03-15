using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph.DataStructure
{
    public class AbstrGraph<K, V, H>
    {
        /// <summary>
        /// Primarni struktura - hash tabulka vrcholu
        /// </summary>
        private IDictionary<K, Vrchol> tabulkaVrcholu = new Dictionary<K, Vrchol>();

        /// <summary>
        /// Prida vrchol s klicem do grafu
        /// </summary>
        /// <param name="klicVrcholu"></param>
        /// <param name="data"></param>
        public void PridejVrchol(K klicVrcholu, V data)
        {
            tabulkaVrcholu.Add(klicVrcholu, new Vrchol() { Data = data });
        }

        /// <summary>
        /// Prida hranu s klici krajnich vrcholu do grafu
        /// </summary>
        /// <param name="klicVrcholuZacatek"></param>
        /// <param name="klicVrcholuKonec"></param>
        /// <param name="data"></param>
        public void PridejHranu(K klicVrcholuZacatek, K klicVrcholuKonec, H data)
        {
            if (tabulkaVrcholu.TryGetValue(klicVrcholuZacatek, out Vrchol vrcholZacatek)
                && tabulkaVrcholu.TryGetValue(klicVrcholuKonec, out Vrchol vrcholKonec))
            {
                Hrana hrana = new Hrana() { Data = data };
                vrcholZacatek.SeznamHran.AddLast(hrana);
                vrcholKonec.SeznamHran.AddLast(hrana);
            }
            else
            {
                throw new Exception("Neexistuje vrchol");
            }
        }

        /// <summary>
        /// Vrati incidentnich hran dle klice vrcholu
        /// </summary>
        /// <param name="klicVrcholu"></param>
        /// <returns></returns>
        public List<H> IncidentniHrany(K klicVrcholu)
        {
            if (tabulkaVrcholu.TryGetValue(klicVrcholu, out Vrchol vrchol))
            {
                return vrchol.SeznamHran.Select(v => v.Data).ToList();
            }

            return new List<H>();
        }

        /// <summary>
        /// Iterator dat vrcholu
        /// </summary>
        /// <returns></returns>
        public IEnumerable<V> DejSeznamVrcholu()
        {
            foreach (Vrchol vrchol in tabulkaVrcholu.Values)
            {
                yield return vrchol.Data;
            }
        }
        
        /// <summary>
        /// Iterator dat hran
        /// </summary>
        /// <returns></returns>
        public IEnumerable<H> DejSeznamHran()
        {
            List<H> prosleHrany = new List<H>();

            foreach (var vrchol in tabulkaVrcholu)
            {
                List<H> hrany = this.IncidentniHrany(vrchol.Key);
                hrany = hrany.Where(h => !prosleHrany.Contains(h)).ToList();
                prosleHrany.AddRange(hrany);

                foreach (H dataHrany in hrany)
                {
                    yield return dataHrany;
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
            public K VrcholZacatek { get; set; }
            public K VrcholKonec { get; set; }
            public H Data { get; set; }
        }
    }
}
