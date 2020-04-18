using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Datove_struktury_2020.data_sem_c
{
    class AbstraktniSoubor<K, Z> where K : IComparable where Z : IVelikostZaznamu
    {
        readonly int velikostRB = 24;
      
        FileStream fs;
        Blok b;
        RidiciBlok rb = new RidiciBlok();

        public AbstraktniSoubor(string cesta)
        {
            bool existujeSoubor = File.Exists(cesta);
            fs = new FileStream(cesta, FileMode.OpenOrCreate);
            if (existujeSoubor == true)
            {
                // CtiBlok(0);
            }
        }

        /// <summary>
        /// Convert an object to a byte array.
        /// </summary>
        /// <param name="obj">Objekt k serializaci.</param>
        /// <returns>Pole bajtu.</returns>
        private static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Convert a byte array to an Object.
        /// </summary>
        /// <param name="arrBytes">Pole bajtu, ze kterych se sklada objekt.</param>
        /// <returns>Deserializovany objekt.</returns>
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
        public Z OdeberSpecifickyZaznam(K klic)
        {
            try
            {
                Z zz = VyhledejSpecifickyZaznam(klic, ZpusobVyhledvani.Binarni);
                // smazani z RAM
                b.poleZaznamu[rb.AktualniZaznam] = null;
                // zapsat novou podobu bloku do souboru
                ZapisBlok(rb.AktualniBlok);
                return zz;
            }
            catch (Exception ex)
            {
                throw new Exception("Zaznam nebyl odebran.");
            }
           
        }

        /// <summary>
        /// Načte blok ze souboru do paměti, kde int i je pozice bloku v souboru.
        /// </summary>
        /// <param name="i">Pozice bloku v souboru.</param>
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
                byte[] prozatimniBufferBlok = new byte[rb.VelikostZaznamu];
                fs.Read(prozatimniBufferBlok);
                b = (Blok)ByteArrayToObject(prozatimniBufferBlok);
            }
            rb.AktualniBlok = i;
            rb.AktualniZaznam = 0;
        }

        /// <summary>
        /// Zapisuje blok do souboru. Nejprve nastavi pozici bloku v souboru.
        /// </summary>
        /// <param name="indexBloku">Poradi bloku v souboru.</param>
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

        /// <summary>
        /// Nastavuje pozici v rámci třídy ukazatel odkud je potřeba číst (kolik bytu přeskočit).
        /// </summary>
        /// <param name="indexBloku">Index bloku na který chceme nastavit pozici, odkud budeme číst soubor na disku.</param>
        private void NastavPozici(int indexBloku)
        {
            if (indexBloku == 0)
            {
                //nastavi idex na nulu a jde od zacatku
                fs.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                int poziceBlokuNaZadanemIndexuVSouboru = velikostRB + (rb.VelikostZaznamu * rb.BlokovyFaktor * (indexBloku - 1));
                fs.Seek(poziceBlokuNaZadanemIndexuVSouboru, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Vybudování struktury souboru pro manipulaci s daty.
        /// </summary>
        /// <param name="kolekceDat">Kolekce dat.</param>
        /// <param name="f">Blokovy faktor - počet bloků záznamů včetně řídícího.</param>
        public void VybudujSoubor(IEnumerable<KeyValuePair<K, Z>> kolekceDat, int f)
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
                    rb.VelikostZaznamu = data.Value.vratVelikostZaznamu();
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

        /// <summary>
        /// Vkládání záznamu do bloku.
        /// </summary>
        /// <param name="zaznam">Záznam.</param>
        /// <param name="citac">pořadí v bloku.</param>
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

        /// <summary>
        /// Vytvori nový objekt bloku v paměti.
        /// </summary>
        private void InicializujBlok()
        {
            b = new Blok(rb.BlokovyFaktor);
        }

        /// <summary>
        /// Vyhledává specifický záznam. 
        /// </summary>
        /// <param name="klic">Klíč vyhledávaného záznamu.</param>
        /// <param name="zpusob">Způsob - binární nebo interpolační vyhledávání.</param>
        /// <returns>Vyhledaný záznam <Z>.</returns>
        public Z VyhledejSpecifickyZaznam(K klic, ZpusobVyhledvani zpusob)
        {
            // stary zapis: 
            // switch (zpusob)
            //{
            //    case ZpusobVyhledvani.Binarni:
            //        return VyhledejBinarne(klic);
            //    case ZpusobVyhledvani.Interpolacni:
            //        return VyhledejInterpolacne(klic);
            //    default:
            //        throw new Exception("Mission impossible, zpusob vyhledavani neodpovida.");
            //}

            //novej zapis switche s lambda vyrazem
            return zpusob switch
            {
                ZpusobVyhledvani.Binarni => VyhledejBinarne(klic),
                ZpusobVyhledvani.Interpolacni => VyhledejInterpolacne(klic),
                _ => throw new Exception("Mission impossible, zpusob vyhledavani neodpovida."),
            };
        }

        /// <summary>
        /// Binární vyhledávání podle zadaného klíče.
        /// </summary>
        /// <param name="klic">Vyhledávaný klíč.</param>
        /// <returns>Data záznamu (data vyhledatelné klíčem).</returns>
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
                        b.ResetujPoziciAktualnihoZaznamu();
                        Zaznam pomocnaPromennaZaznamu; //pomocna promenna
                        bool jdiDoPrava = true; //pomocna promenna

                        //prvni prirazeni do pomocne promenne a pak porovnani, jestli je tam zaznam
                        while ((pomocnaPromennaZaznamu = b.VratDalsiZaznam()) != null)
                        {
                            rb.AktualniZaznam = b.poziceAktualnihoZaznamu;
                            //je-li vyhledavany v bloku
                            if (klic.CompareTo(pomocnaPromennaZaznamu.klic) == 0)
                            {
                                return pomocnaPromennaZaznamu.zaznam;
                            }
                            // vyhledavany klic je mensi nez klic v zaznamu = jdeme do leva
                            else if (klic.CompareTo(pomocnaPromennaZaznamu.klic) == -1)
                            {
                                pravyInterval = polovinaIntervalu - 1;
                                jdiDoPrava = false;
                                break;
                            }
                        }
                        if (jdiDoPrava == true)
                        {
                            levyInterval = polovinaIntervalu + 1;
                        }
                    }
                }
                else
                {
                    throw new Exception("Pri binarnim vyhledavani prvek nenalezen.");
                }
            }
        }

        // TODO
        private Z VyhledejInterpolacne(K klic)
        {
            int levyInterval = 1;
            int pravyInterval = rb.PocetBloku;

            K bl = default; //prvni blok
            K br = default; //posledni blok
            
            CtiBlok(1);
            bl = b.VratPrvniZaznam().klic;
            CtiBlok(rb.PocetBloku);
            br = b.VratPosledniZaznam().klic; 

            while (true)
            {
                if (pravyInterval >= 1)
                {
                    double d = (transformujKlic(klic) - transformujKlic(bl)) / (transformujKlic(br) - transformujKlic(bl));
                    int posiceBlokuVIntervalu = levyInterval + (int)((pravyInterval - levyInterval + 1) * d);
                    CtiBlok(posiceBlokuVIntervalu);
                    if (b != null)
                    {
                        b.ResetujPoziciAktualnihoZaznamu();
                        Zaznam pomocnaPromennaZaznamu; //pomocna promenna
                        bool jdiDoPrava = true; //pomocna promenna

                        //prvni prirazeni do pomocne promenne a pak porovnani, jestli je tam zaznam
                        while ((pomocnaPromennaZaznamu = b.VratDalsiZaznam()) != null)
                        {
                            rb.AktualniZaznam = b.poziceAktualnihoZaznamu;
                            //je-li vyhledavany v bloku
                            if (klic.CompareTo(pomocnaPromennaZaznamu.klic) == 0)
                            {
                                return pomocnaPromennaZaznamu.zaznam;
                            }
                            // vyhledavany klic je mensi nez klic v zaznamu = jdeme do leva
                            else if (klic.CompareTo(pomocnaPromennaZaznamu.klic) == -1)
                            {
                                pravyInterval = posiceBlokuVIntervalu - 1;
                                jdiDoPrava = false;
                                break;
                            }
                        }
                        if (jdiDoPrava ==true)
                        {
                            levyInterval = posiceBlokuVIntervalu + 1;
                        }
                    }
                }
                else
                {
                    throw new Exception("Pri Interpolacnim vyhledavani prvek nenalezen.");
                }
            }
        }

        //TODO
        private long transformujKlic(K klic)
        {
            if (klic != null)
            {
                String s = klic.ToString().ToLower();
                string abeceda = " aábcčdďeéěfghiíjklmnňoópqrřsštťuúůvwxyýzž";
                long docasna = 0;
                int mocnina = 0;

                for (int i = s.Length - 1; i >= 0; i--)
                {
                    int poziceVAbecede = abeceda.IndexOf(s[i]);
                    docasna = docasna + (long)(Math.Pow(abeceda.Length, mocnina) * poziceVAbecede);
                    mocnina++;
                }
                return docasna;
            } 
            else 
            {
                throw new Exception("Zadaný klíč nenalezen. Transformace na číslo neproběhla.");
            }
            

        }

        private class Blok
        {
            public bool[] platny; // 1 byte v pameti
            public Zaznam[] poleZaznamu;
            public int poziceAktualnihoZaznamu = -1;

            public Blok(int f)
            {
                platny = new bool[f];
                poleZaznamu = new Zaznam[f];
            }

            public Zaznam VratPrvniZaznam()
            {
                for (int i = 0; i < poleZaznamu.Length; i++)
                {
                    if (platny[i] == true && poleZaznamu[i] != null)
                    {                   
                        return poleZaznamu[i];
                    }                
                }
                return null;               
            }

            public Zaznam VratPosledniZaznam()
            {
                for (int i = poleZaznamu.Length - 1 ; i >= 0; i--)
                {
                    if (platny[i] == true && poleZaznamu[i] != null)
                    {
                        return poleZaznamu[i];
                    }
                }
                return null;
            }

            public Zaznam VratDalsiZaznam()
            {
                // zacneme hledat platny zaznam od posledniho vraceneho zaznamu
                for (int i = poziceAktualnihoZaznamu + 1; i < poleZaznamu.Length; i++)
                {
                    if (platny[i] == true && poleZaznamu[i] != null)
                    {
                        poziceAktualnihoZaznamu = i;
                        return poleZaznamu[i];
                    }
                }
                return null;
            }

            public void ResetujPoziciAktualnihoZaznamu()
            {
                poziceAktualnihoZaznamu = -1;
            }
        }

        /// <summary>
        /// Uchovava zaznam, ktery je v bloku.
        /// x-16 byte,
        /// y-16 byte,
        /// klic v csv - 60 byte (30*char po 2 bytech),
        /// K klic - 60 byte = 152 byte.
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
            public int VelikostZaznamu { get; set; }
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
