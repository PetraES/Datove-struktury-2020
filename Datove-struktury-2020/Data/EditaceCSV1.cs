using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Datove_struktury_2020.Data
{
    /// <summary>
    /// Třída obsluhuje práci s CSV soborem.
    /// </summary>
    class EditaceCSV
    {
        //vrati roztrhane stringy oddelene carkama
        public List<string[]> NactiSoubor(string cestaKSouboru)
        {
            List<string[]> result = new List<string[]>();
            foreach (string obecnyRadek in NactiRadky(cestaKSouboru))
            {
                result.Add(RozsekejRadkyPoCarkach(obecnyRadek));
            }
            return result;
        }

        /// <summary>
        /// Nacita data ze CSV souboru a rozseka je na radky.
        /// </summary>
        /// <param name="cestakSouboru">Cesta k souboru.</param>
        /// <returns>List řádků souboru.</returns>
        private List<string> NactiRadky(string cestakSouboru)
        {
            List<string> radkySouboru = new List<string>();
            using (StreamReader sr = new StreamReader(cestakSouboru))
            {
                string radek;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((radek = sr.ReadLine()) != null)
                {
                    radkySouboru.Add(radek);
                }
            }
            return radkySouboru;
        }

        /// <summary>
        /// Rozseka radek v CSV podle carky.
        /// </summary>
        /// <param name="radek">Řádek v soboru.</param>
        /// <returns>Vrací jednotlivé sloupce.</returns>
        private string[] RozsekejRadkyPoCarkach(string radek)
        {
            return radek.Split(';');
        }

        public void ZapisDoCSV(string cestaKSouboru, string data) 
        {
            StreamWriter sw = new StreamWriter(cestaKSouboru);
            sw.Write(data);
            sw.Close();
        }
    }
}
