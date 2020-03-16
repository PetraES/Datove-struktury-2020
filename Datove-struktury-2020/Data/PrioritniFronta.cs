using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    /// <summary>
    /// Pomocná třída reprezentuje prvek haldy.
    /// </summary>
    /// <typeparam name="TInformace">Data uchovávané v prvku haldy.</typeparam>
    class PrvekHaldy<TInformace>
    {
        public int Priorita { set; get; }
        public TInformace Informace { set; get; }
    }

    /// <summary>
    /// Implementace prioritná fronty haldou.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    class PrioritniFronta<TData>
    {
        public List<PrvekHaldy<TData>> haldaEma = new List<PrvekHaldy<TData>>();

        /// <summary>
        /// Vkládá prvek do haldy.
        /// </summary>
        /// <param name="vkladanyPrvek">Prvek, který se bude vkládat do haldy.</param>
        public void VlozPrvek(PrvekHaldy<TData> vkladanyPrvek)
        {
            haldaEma.Add(vkladanyPrvek);

            int indexDitete = haldaEma.Count - 1;
            int indexOtce = (indexDitete - 1) / 2;

            while (haldaEma[indexOtce].Priorita > haldaEma[indexDitete].Priorita)
            {
                PrvekHaldy<TData> docasnyPrvek = haldaEma[indexDitete];
                haldaEma[indexDitete] = haldaEma[indexOtce];
                haldaEma[indexOtce] = docasnyPrvek;
                indexDitete = indexOtce;
                indexOtce = (indexDitete - 1) / 2;
            }
        }

        /// <summary>
        /// Odebere prvek z haldy.
        /// </summary>
        /// <returns>Vrací odebraný prvek.</returns>
        public PrvekHaldy<TData> OdeberPrvek()
        {
            if (haldaEma.Count == 0)
            {
                return default;
            }
            PrvekHaldy<TData> a = haldaEma[0];
            int pocetPrvkuVgertrude = haldaEma.Count;
            int posledniPrvekVgertrude = pocetPrvkuVgertrude - 1;
            haldaEma[0] = haldaEma[posledniPrvekVgertrude];
            haldaEma.RemoveAt(posledniPrvekVgertrude);

            int indexOtce = 0;
            int indexmensihoZPotomku;
            while (true)
            {
                int indexLevehoPotomka = 2 * indexOtce + 1;
                int indexPravehoPotomka = 2 * indexOtce + 2;
                bool existujeLevyPotomek = indexLevehoPotomka < haldaEma.Count;
                bool existujePravyPotomek = indexPravehoPotomka < haldaEma.Count;

                indexmensihoZPotomku = -1;
             
                if (existujeLevyPotomek && existujePravyPotomek && haldaEma[indexLevehoPotomka].Priorita < haldaEma[indexPravehoPotomka].Priorita)
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

                if (indexmensihoZPotomku > -1 && haldaEma[indexmensihoZPotomku].Priorita < haldaEma[indexOtce].Priorita)
                {
                    PrvekHaldy<TData> c = haldaEma[indexmensihoZPotomku];
                    haldaEma[indexmensihoZPotomku] = haldaEma[indexOtce];
                    haldaEma[indexOtce] = c;
                    indexOtce = indexmensihoZPotomku;
                }
                else
                {
                    break;
                }
            }
            return a;
        }

        /// <summary>
        /// Přerovnání haldy  znovusestavením haldy.
        /// </summary>
        public void PrerovnejHypu()
        {
            List<PrvekHaldy<TData>> provizorniList = haldaEma;
            haldaEma = new List<PrvekHaldy<TData>>();
            foreach (PrvekHaldy<TData> i in provizorniList)
            {
                VlozPrvek(i);
            }
        }
    }
}
