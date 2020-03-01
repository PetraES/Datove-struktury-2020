using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class NacteniCSV
    {
        //vrati roytrhane stringy oddelene carkama
        public List<string[]> NactiSoubor(string cestaKSouboru)
        {
            List<string[]> result = new List<string[]>();
            foreach (string obecnyRadek in NactiRadky(cestaKSouboru))
            {
                result.Add(RozsekejRadkyPoCarkach(obecnyRadek));
            }
            return result;
        }

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

        private string[] RozsekejRadkyPoCarkach(string radek)
        {
            return radek.Split(',');
        }

    }
}
