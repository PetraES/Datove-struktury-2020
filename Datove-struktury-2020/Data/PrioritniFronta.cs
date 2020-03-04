using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class PrvekHaldy<TInformace>
    {
        public int Priorita { set; get; }
        public TInformace Informace { set; get; }
    }

    //implementace prioritni fronty haldou 
    class PrioritniFronta<TData>
    {
        public List<PrvekHaldy<TData>> gertruda = new List<PrvekHaldy<TData>>();

        public void VlozPrvek(PrvekHaldy<TData> vkladanyPrvek)
        {
            gertruda.Add(vkladanyPrvek);

            int indexDitete = gertruda.Count - 1;
            int indexOtce = (indexDitete - 1) / 2;

            while (gertruda[indexOtce].Priorita > gertruda[indexDitete].Priorita)
            {
                PrvekHaldy<TData> docasnyPrvek = gertruda[indexDitete];
                gertruda[indexDitete] = gertruda[indexOtce];
                gertruda[indexOtce] = docasnyPrvek;

                indexDitete = indexOtce;
                indexOtce = (indexDitete - 1) / 2;
            }
        }

        public PrvekHaldy<TData> OdeberPrvek()
        {
            if (gertruda.Count == 0)
            {
                return default;
            }
            PrvekHaldy<TData> a = gertruda[0];
            int pocetPrvkuVgertrude = gertruda.Count;
            int posledniPrvekVgertrude = pocetPrvkuVgertrude - 1;
            gertruda[0] = gertruda[posledniPrvekVgertrude];
            gertruda.RemoveAt(posledniPrvekVgertrude);

            int indexOtce = 0;
            int indexmensihoZPotomku;
            while (true)
            {
                int indexLevehoPotomka = 2 * indexOtce + 1;
                int indexPravehoPotomka = 2 * indexOtce + 2;
                bool existujeLevyPotomek = indexLevehoPotomka < gertruda.Count;
                bool existujePravyPotomek = indexPravehoPotomka < gertruda.Count;

                indexmensihoZPotomku = -1;
             
                if (existujeLevyPotomek && existujePravyPotomek && gertruda[indexLevehoPotomka].Priorita < gertruda[indexPravehoPotomka].Priorita)
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

                if (indexmensihoZPotomku > -1 && gertruda[indexmensihoZPotomku].Priorita < gertruda[indexOtce].Priorita)
                {
                    PrvekHaldy<TData> c = gertruda[indexmensihoZPotomku];
                    gertruda[indexmensihoZPotomku] = gertruda[indexOtce];
                    gertruda[indexOtce] = c;
                    indexOtce = indexmensihoZPotomku;
                }
                else
                {
                    break;
                }
            }
            return a;
        }

        public void PrerovnejHypu()
        {
            List<PrvekHaldy<TData>> provizorniList = gertruda;
            gertruda = new List<PrvekHaldy<TData>>();
            foreach (PrvekHaldy<TData> i in provizorniList)
            {
                VlozPrvek(i);
            }
        }
    }
}
