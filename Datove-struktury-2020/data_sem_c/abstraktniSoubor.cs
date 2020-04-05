using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Datove_struktury_2020.data_sem_c
{
    class AbstraktniSoubor<K, Z>
    {
        string cestaKSouboru;

        int velikostRB = 24;
        int velikostZaznamu = 152;

        FileStream fs;

        Blok buffer;
        RidiciBlok RB = new RidiciBlok();

        public AbstraktniSoubor(string cesta)
        {
            this.cestaKSouboru = cesta;
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

        public Z Cti(K klic)
        {
            return default;
        }

        public void Zapis(Z info)
        {

        }

        public void Zrus(K klic)
        {

        }

        public void Modifikuj(K klic, Z info)
        {

        }

        private void CtiBlok(int i)
        {

        }

        private void ZapisBlok(int i)
        {
            // fs.Seek(10 + (20 * i))
            // fs.Write(buffer);
        }

        public void VybudovaniSouboru(IEnumerable<KeyValuePair<K, Z>> kolekceDat, int f)
        {

            InicializujBlok(f);
            if (kolekceDat != null)
            {
                foreach (KeyValuePair<K, Z> data in kolekceDat)
                {
                    Zaznam z = new Zaznam(data.Key, data.Value);
                    PridejZaznamDoBloku(z);
                }
            }

       
        }

        private void PridejZaznamDoBloku(Zaznam zaznam)
        {
            if (RB.PrvniVolny == RB.BlokovyFaktor)
            {
                   
            }
        }

        private void InicializujBlok(int f)
        {
            buffer = new Blok(f);
        }

        /// <summary>
        /// Odebrani zaznamu ze souboru.
        /// </summary>
        /// <returns>Zaznam</returns>
        public Z odeberZaznam(string klic)
        {
            if (klic != null)
            {
            
            }

            return Z;
        }

        public void vyhledejBinarne() { }

        public void vyhledejInterpolacne()
        {
            float d;
        }


        private class Blok
        {            
            bool[] platny; // 1 byte v pameti
            Zaznam[] poleZaznamu;

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
            Z zaznam; 
            K klic;

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

            public int PrvniVolny { get; set; }
            public int PrvniObsazeny { get; set; }

        }
    }

}
