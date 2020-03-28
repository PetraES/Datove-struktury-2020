using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    /// <summary>
    /// Implementace prioritná fronty haldou.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    class PrioritniFronta<TData>
    {
        private List<PrvekHaldy> haldaGertruda = new List<PrvekHaldy>();

        public void VlozPrvek(TData vkladanyPrvek, int priorita)
        {
            PrvekHaldy obecnyPrvek = new PrvekHaldy();
            obecnyPrvek.Informace = vkladanyPrvek;
            obecnyPrvek.Priorita = priorita;
            VlozPrvek(obecnyPrvek);
        }

        /// <summary>
        /// Vkládá prvek do haldy.
        /// </summary>
        /// <param name="vkladanyPrvek">Prvek, který se bude vkládat do haldy.</param>
        private void VlozPrvek(PrvekHaldy vkladanyPrvek)
        {
            haldaGertruda.Add(vkladanyPrvek);

            int indexDitete = haldaGertruda.Count - 1;
            int indexOtce = (indexDitete - 1) / 2;

            while (haldaGertruda[indexOtce].Priorita > haldaGertruda[indexDitete].Priorita)
            {
                PrvekHaldy docasnyPrvek = haldaGertruda[indexDitete];
                haldaGertruda[indexDitete] = haldaGertruda[indexOtce];
                haldaGertruda[indexOtce] = docasnyPrvek;
                indexDitete = indexOtce;
                indexOtce = (indexDitete - 1) / 2;
            }
        }

        /// <summary>
        /// Odebere prvek z haldy prvek s nejvyšší prioritou.
        /// Číselně vyjádřená hodnota priority je nejnižší.
        /// Prostě ten první na řadě.
        /// </summary>
        /// <returns>Vrací odebraný prvek.</returns>
        public TData OdeberPrvek()
        {
            if (haldaGertruda.Count == 0)
            {
                return default;
            }
            PrvekHaldy a = haldaGertruda[0];
            int pocetPrvkuVgertrude = haldaGertruda.Count;
            int posledniPrvekVgertrude = pocetPrvkuVgertrude - 1;
            haldaGertruda[0] = haldaGertruda[posledniPrvekVgertrude];
            haldaGertruda.RemoveAt(posledniPrvekVgertrude);

            int indexOtce = 0;
            int indexmensihoZPotomku;
            while (true)
            {
                int indexLevehoPotomka = 2 * indexOtce + 1;
                int indexPravehoPotomka = 2 * indexOtce + 2;
                bool existujeLevyPotomek = indexLevehoPotomka < haldaGertruda.Count;
                bool existujePravyPotomek = indexPravehoPotomka < haldaGertruda.Count;

                indexmensihoZPotomku = -1;
             
                if (existujeLevyPotomek && existujePravyPotomek && haldaGertruda[indexLevehoPotomka].Priorita < haldaGertruda[indexPravehoPotomka].Priorita)
                {
                    indexmensihoZPotomku = 2 * indexOtce + 1;
                }
                else if (existujeLevyPotomek && !existujePravyPotomek)
                {
                    indexmensihoZPotomku = 2 * indexOtce + 1;
                }
                else if (existujePravyPotomek)
                {
                    indexmensihoZPotomku = 2 * indexOtce + 2;
                }

                if (indexmensihoZPotomku > -1 && haldaGertruda[indexmensihoZPotomku].Priorita < haldaGertruda[indexOtce].Priorita)
                {
                    PrvekHaldy c = haldaGertruda[indexmensihoZPotomku];
                    haldaGertruda[indexmensihoZPotomku] = haldaGertruda[indexOtce];
                    haldaGertruda[indexOtce] = c;
                    indexOtce = indexmensihoZPotomku;
                }
                else
                {
                    break;
                }
            }
            return a.Informace;
        }

        /// <summary>
        /// Přerovnání haldy  znovusestavením haldy.
        /// </summary>
        private void PrerovnejHypu()
        {
            List<PrvekHaldy> provizorniList = haldaGertruda;
            haldaGertruda = new List<PrvekHaldy>();
            foreach (PrvekHaldy i in provizorniList)
            {
                VlozPrvek(i);
            }
        }

        public List<TData> vratPrvky() 
        {
            List<TData> prvky = new List<TData>();
            foreach(PrvekHaldy prvekHaldy in haldaGertruda)
            {
                prvky.Add(prvekHaldy.Informace);
            }
            return prvky;
        }

        public bool NahradPrvek(TData cil, TData novy, int priorita)
        {
            for (int i = 0; i < haldaGertruda.Count; i++)
            {
                if (haldaGertruda[i].Informace.Equals(cil))
                {
                    haldaGertruda[i].Informace = novy;
                    haldaGertruda[i].Priorita = priorita;
                    PrerovnejHypu();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Pomocná třída reprezentuje prvek haldy.
        /// </summary>
        private class PrvekHaldy
        {
            public int Priorita { set; get; }
            public TData Informace { set; get; }
        }
    }
}
