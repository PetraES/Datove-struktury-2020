using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Datove_struktury_2020.data_sem_c
{
    class AbstraktniSoubor<K, Z> where K : IComparable
    {
        int velikostRB = 24;
        int velikostZaznamu = 152;

        FileStream fs;

        Blok b;
        RidiciBlok rb = new RidiciBlok();

        public AbstraktniSoubor(string cesta)
        {
            fs = new FileStream(cesta, FileMode.OpenOrCreate);
        }

        // Convert an object to a byte array
        private static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        // Convert a byte array to an Object
        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        /// <summary>
        /// Odebrani zaznamu ze souboru.
        /// </summary>
        /// <param name="klic">Hodnota, kterou chceme smazat.</param>
        /// <returns>Zaznam</returns>
        public Z Smaz(K klic)
        {
            Z zz = Vyhledej(klic, ZpusobVyhledvani.Binarni );
            // smazani z RAM
            b.poleZaznamu[rb.AktualniZaznam] = null;
            // zapsat novou podobu bloku do souboru
            ZapisBlok(rb.AktualniBlok);
            return zz;
        }

        private void CtiBlok(int i)
        {
            NastavPozici(i);
            if (i == 0)
            {
                byte[] prozatimnibufferRB = new byte[velikostRB];
                fs.Read(prozatimnibufferRB);
                rb = (RidiciBlok)ByteArrayToObject(prozatimnibufferRB);
            }
            else
            {
                byte[] prozatimniBufferBlok = new byte[velikostZaznamu];
                fs.Read(prozatimniBufferBlok);
                b = (Blok)ByteArrayToObject(prozatimniBufferBlok);
            }
            rb.AktualniBlok = i;
            rb.AktualniZaznam = 0;
        }

        private void ZapisBlok(int indexBloku)
        {
            NastavPozici(indexBloku);

            if (indexBloku == 0)
            {
                byte[] ridiciBlok = ObjectToByteArray(rb);
                fs.Write(ridiciBlok);
            }
            else
            {
                byte[] blok = ObjectToByteArray(b);
                fs.Write(blok);
            }
        }

        private void NastavPozici(int indexBloku)
        {
            if (indexBloku == 0)
            {
                //nastavi idex na nulu a jde od zacatku
                fs.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                int poziceBlokuNaZadanemIndexuVSouboru = velikostRB + (velikostZaznamu * rb.BlokovyFaktor * (indexBloku - 1));
                fs.Seek(poziceBlokuNaZadanemIndexuVSouboru, SeekOrigin.Begin);
            }
        }

        public void VybudovaniSouboru(IEnumerable<KeyValuePair<K, Z>> kolekceDat, int f)
        {
            rb.BlokovyFaktor = f;
            InicializujBlok();
            rb.AktualniBlok = 1;
            rb.PocetBloku = 1;
            if (kolekceDat != null)
            {
                int pomocnaPromenna = 0;
                foreach (KeyValuePair<K, Z> data in kolekceDat)
                {
                    Zaznam z = new Zaznam(data.Key, data.Value);
                    VlozZaznamDoBloku(z, pomocnaPromenna++);
                }
                ZapisBlok(rb.AktualniBlok);
            }
            else 
            {
                throw new Exception("Kolekce dat je nulova. Neni co budovat.");
            }
            ZapisBlok(0);
        }

        private void VlozZaznamDoBloku(Zaznam zaznam, int citac)
        {
            int indexBloku = citac / rb.BlokovyFaktor + 1;
            int indexZaznamu = citac % rb.BlokovyFaktor;

            // pokud je blok plny, vytvorim dalsi
            if (rb.AktualniBlok != indexBloku)
            {
                ZapisBlok(rb.AktualniBlok);
                InicializujBlok();
                rb.AktualniBlok++;
                rb.PocetBloku++;
            }
            b.poleZaznamu[indexZaznamu] = zaznam;
        }

        private void InicializujBlok()
        {
            b = new Blok(rb.BlokovyFaktor);
        }

        public Z Vyhledej(K klic, ZpusobVyhledvani zpusob)
        {
            switch (zpusob)
            {
                case ZpusobVyhledvani.Binarni:
                    return VyhledejBinarne(klic);
                case ZpusobVyhledvani.Interpolacni:
                    return VyhledejInterpolacne(klic);
                default:
                    throw new Exception("Mission impossible, zpusob vyhledavani neodpovida.");
            }
        }

        private Z VyhledejBinarne(K klic)
        {
            int pravyInterval = rb.PocetBloku;
            int levyInterval = 1;

            while (true)
            {
                if (pravyInterval >= 1)
                {
                    int polovinaIntervalu = 1 + ((pravyInterval - levyInterval) / 2);
                    CtiBlok(polovinaIntervalu);
                    if (b != null)
                    {
                        // prohledavani jednotlivych zaznamu v danem bloku
                        // foreach (Zaznam zaznamBloku in buffer.poleZaznamu)
                        for (int i = 0; i < b.poleZaznamu.Length; i++)
                        {
                            rb.AktualniZaznam = i;
                            // ktery chci index
                            Zaznam zaznamBloku = b.poleZaznamu[i];
                            //je-li vyhledavany v bloku
                            if (klic.CompareTo(zaznamBloku.klic) == 0)
                            {
                                return zaznamBloku.zaznam;
                            }
                            // vyhledavany klic je mensi nez klic v zaznamu = jdeme do leva
                            else if (klic.CompareTo(zaznamBloku.klic) == -1)
                            {
                                pravyInterval = polovinaIntervalu - 1;
                                break;
                            }
                            // jdeme doprava
                            //TODO doresit kdyz budou v zaznamech nejaky data nuly
                            else if (i == b.poleZaznamu.Length - 1 && klic.CompareTo(zaznamBloku.klic) == 1)
                            {
                                levyInterval = polovinaIntervalu + 1;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Pri binarnim vyhledavani prvek nenalezen.");
                }
            }
        }

        private Z VyhledejInterpolacne(K klic)
        {
            float d;
            return default;
        }

        private class Blok
        {
            public bool[] platny; // 1 byte v pameti
            public Zaznam[] poleZaznamu;

            public Blok(int f)
            {
                platny = new bool[f];
                poleZaznamu = new Zaznam[f];
            }
        }

        /// <summary>
        /// Uchovava zaznam, ktery je v bloku.
        /// // x-16byte, y-16 byte, klic v csv - 60 byte (30*char po 2 bytech), K klic - 60 byte = 152 byte
        /// </summary>
        private class Zaznam
        {
            public Z zaznam;
            public K klic;

            public Zaznam(K klic, Z zaznam)
            {
                this.zaznam = zaznam;
                this.klic = klic;
            }
        }

        private class RidiciBlok
        {
            public int PocetBloku { get; set; }

            /// <summary>
            /// Pocet zaznamu v bloku
            /// </summary>
            public int BlokovyFaktor { get; set; }
            public int AktualniBlok { get; set; }
            public int AktualniZaznam { get; set; }

            // Nepotrebujeme, nevkladame.
            //public int PrvniVolny { get; set; }
            //public int PrvniObsazeny { get; set; }

        }
    }

}
