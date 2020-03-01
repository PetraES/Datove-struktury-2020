using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    class KoordinatorLesnichDobrodruzstvi
    {
        public List<Vrchol> vytvorVrcholy()
        {
            NacteniCSV nacteniCSV = new NacteniCSV();
            List<string[]> objekt = nacteniCSV.NactiSoubor(@"C:\Users\petra\source\repos\Datove-struktury-2020\Datove-struktury-2020\Resources\vrcholy_csv2020.csv");

            List<Vrchol> vseckyVrcholy = new List<Vrchol>();
            foreach (string[] radek in objekt)
            {
                Vrchol v = new Vrchol();
                v.NazevVrcholu = radek[0];
                v.XSouradniceVrcholu = float.Parse(radek[1]);
                v.YSouradniceVrcholu = float.Parse(radek[2]);
                v.TypVrcholu = (TypyVrcholu)int.Parse(radek[3]); //pretypovat string na typy vrcholu, vyzkouset jestli jde
                vseckyVrcholy.Add(v);
            }
            return vseckyVrcholy;
        }
    }
}
