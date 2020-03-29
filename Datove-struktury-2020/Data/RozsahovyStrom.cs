using System;
using System.Collections.Generic;
using System.Text;

namespace Datove_struktury_2020.Data
{
    /// <summary>
    /// Datová struktura tzv. Range strom. Plnohodnotné prvky se nacházejí pouze
    /// na úrovni listů, kde jsou mezi sebou zřetězeny a zároveň seřazeny podle jedné z dimenzí.
    /// Ostatní vrcholy stromu tvoří navigační strukturu a mají uloženu informaci o intervalu, do
    /// kterého patří všichni jejich potomci.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class RozsahovyStrom<T> where T : ISouradnice
    {
        PrvekRozsahovehoStromu koren;
        int pocetPrvkuVeStrukture;
        int pocetUrovniStromu;

        //vždy, když se zavolá inervalové hledání tak se vytvoří nová instance 
        List<T> vysledekIntervalovehoHledani;

        //delegát porovnani - reseno CompareTo

        /// <summary>
        /// Porovnání součadnic v navigačním vrcholu podle X.
        /// Využívá metodu porovnejČísla.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static int porovnejPodleX(T a, T b)
        {
            return porovnejCisla(a.vratX(), b.vratX());
        }

        /// <summary>
        /// Porovnání součadnic v navigačním vrcholu podle Y.
        /// Využívá metodu porovnejČísla.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static int porovnejPodleY(T a, T b)
        {
            return porovnejCisla(a.vratY(), b.vratY());
        }

        /// <summary>
        /// Porovnání - delegát podle List<T>.Sort Method, Comparison<T> Delegate.
        /// </summary>
        /// <param name="hodnotaA">int</param>
        /// <param name="hodnotaB">int</param>
        /// <returns>Vrací int buď 0, 1, -1, podle toho, jestli jsou hodnoty stejné nebo ne. 
        /// Když jsou stejné vrací 0. 
        /// Když je první porovnávaná hodnota větší vrací 1.</returns>
        static int porovnejCisla(int hodnotaA, int hodnotaB)
        {
            if (hodnotaA == hodnotaB)
            {
                return 0;
            }
            else if (hodnotaA > hodnotaB)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Postupně buduje rozsahovy strom. Využívá k tomu metodu Vybuduj Strom a metodu Vybuduj podstrom.
        /// Pokud je předáné více než jeden prvek (seznamPrvků) vzniká tzv. navigační vrchol hlavního stromu.
        /// Za první dimenzi je v realizaci zvolena souřadnice x, do navigačního se předá nejmenší možný interval se všema prvkama.
        /// Ty se následně seřadí podle první dimenze a rozdělí se na poloviny.První polovina bude tvořit 
        /// levý podstrom navigačního vrcholu, druhá polovina pravý podstrom. Toto se opakuje dokud nezbyde jeden poslední prvek, 
        /// ten se pak stane liste, kde se zřetězí s dalšíma prvkama.
        /// </summary>
        /// <param name="seznamPrvku"></param>
        public void Vybuduj(List<T> seznamPrvku)
        {
            if (seznamPrvku == null || seznamPrvku.Count == 0)
            {
                throw new Exception("Strukturu Rozsahový strom nešlo vybudovat, list vrcholů je prázdný nebo neexistuje.");
            }
            else
            {
                pocetPrvkuVeStrukture = seznamPrvku.Count;
                //vybudovani stromu prvni dimenze
                koren = VybudujStrom(seznamPrvku, null, true);
            }
        }

        /// <summary>
        ///  Pomocná metoda pro postupné budování rozsahového stromu. 
        /// </summary>
        /// <param name="seznamPrvku">Poskytne seznam prvků, které vstupují so stromu.</param>
        /// <param name="prvekDruheDimenze">prvekDruheDimenze, aby se odkázal Ypsylonovy interval do Xoveho intervalu.</param>
        /// <param name="dimenzeX">Parametr, aby bylo zřejmé, jestli se bude budovat od dimenze X nebo Y.</param>
        /// <returns>Vrací "pomocny", což je pomocný abstraktní uzel, který drží prvek rozsahového stromu,
        /// ať už jde o navigační vrchol nebo plnohodnotný prvek</returns>
        private PrvekRozsahovehoStromu VybudujStrom(List<T> seznamPrvku, PrvekRozsahovehoStromu prvekDruheDimenze, bool dimenzeX)
        {
            PrvekRozsahovehoStromu pomocny = VybudujPodstrom(seznamPrvku, null, null, dimenzeX);
            //případné spojení vrcholu podstromu s prvkem opačné dimenze
            if (prvekDruheDimenze != null)
            {
                pomocny.druhaDimenze = prvekDruheDimenze;
            }
            return pomocny;
        }

        /// <summary>
        /// Postupně vybudovává podstrom v rozsahovém stromu. Pro seřazení prvků využívá v listu metodu sort. 
        /// </summary>
        /// <param name="seznamPrvku">Poskytne seznam prvků, které vstupují so stromu.</param>
        /// <param name="predek">Prvek rozsahového stromu.</param>
        /// <param name="predchoziZListu">Předcházející hodnota ze seznamu prvků, které se vkládají postupně do stromu.</param>
        /// <param name="dimenzeX">Parametr, aby bylo zřejmé, jestli se bude budovat od dimenze X nebo Y.</param>
        /// <returns>Vrací "pomocny", což je pomocný abstraktní uzel, který drží prvek rozsahového stromu,
        /// ať už jde o navigační vrchol nebo plnohodnotný prvek</returns>
        private PrvekRozsahovehoStromu VybudujPodstrom(List<T> seznamPrvku, PrvekRozsahovehoStromu predek,
            PrvekRozsahovehoStromu predchoziZListu, bool dimenzeX)
        {
            PrvekRozsahovehoStromu pomocny;
            if (seznamPrvku.Count >= 2)
            {
                // nastaveni intervalu pro navigacni prvek (ten ktery rika jake prvky jsou pod nim)
                if (dimenzeX == true)
                {
                    // sort vraci void nikoli serazenou kolekci, tzn ze zmeni zdrojova data
                    seznamPrvku.Sort(porovnejPodleX);
                    // utridit seznam vrcholu, vzit prvni vrchol a jeho x ovou souradnici 
                    // pouzit jako (zacatek intervalu) navigacniho vrcholu a jeho dalsi souradnici 
                    // pouzit jako (konec intervalu) navigacniho vrcholu, tim vytvorit navigacni vrchol
                    int zacatekIntervalu = seznamPrvku[0].vratX();
                    int konecIntervalu = seznamPrvku[seznamPrvku.Count - 1].vratX();
                    // vytvorit instanci prvku pomocneho rozsahoveho stromu - navigacniho vrcholu                  
                    pomocny = new PrvekRozsahovehoStromu(zacatekIntervalu, konecIntervalu);
                }
                else
                {
                    seznamPrvku.Sort(porovnejPodleY);
                    int zacatekIntervalu = seznamPrvku[0].vratY();
                    int konecIntervalu = seznamPrvku[seznamPrvku.Count - 1].vratY();
                    pomocny = new PrvekRozsahovehoStromu(zacatekIntervalu, konecIntervalu);
                }
                //vytvoreni seznamů prvků pro levý a pravý podstrom, do nichž se prvky rozdělí
                //diky teto skvele metode dost pravdepodobne pri lichem poctu bude v pravem podstromu o jeden prvek vice 
                int x = seznamPrvku.Count; //pocet prvku v seznamu
                int y = x / 2; //deleni int odsekava desetiny
                List<T> prvkyLevehoPodstromu = new List<T>(seznamPrvku.GetRange(0, y)); //getRange chce start index a pocetPrvku
                List<T> prvkyPravyhoPodstromu = new List<T>(seznamPrvku.GetRange(y, (x - y)));
                // vybudovani podstromů z rozdělených prvků, rekurze
                pomocny.levyPotomek = VybudujPodstrom(prvkyLevehoPodstromu, pomocny, predchoziZListu, dimenzeX);
                pomocny.pravyPotomek = VybudujPodstrom(prvkyPravyhoPodstromu, pomocny, predchoziZListu, dimenzeX);
            }
            // byl-li pro budování předán jediný prvek, stane se plnohodnotným prvkem
            else
            {   // pokud je pocet prvku je jedna >?< - neni v Diplomce reseno
                pomocny = new PrvekRozsahovehoStromu(seznamPrvku[0]);

                // TODO: oveřít podmínku u if - pomocny != null?
                // zřetězení prvků na úrovni listů (plnohodnotné prvky)  
                if (predchoziZListu != null)
                {
                    predchoziZListu.dalsiPrvekRozsahovehoStromu = pomocny;
                    pomocny.predchoziPrvekzRozsahovehoStromu = predchoziZListu;
                }
            }
            // pro navig. vrchol ve stromu první dimenze je vybudován strom druhé dimenze
            if (dimenzeX == true && pomocny.platny == false)
            {
                pomocny.druhaDimenze = VybudujStrom(seznamPrvku, pomocny, false);
            }
            pomocny.otec = predek;
            return pomocny;
        }

        /// <summary>
        /// Hledání prvku se zadanými souřadnicemi probíhá pouze ve stromu první dimenze, který je
        /// organizován podle zeměpisné šířky.Prohledávání začíná od kořene stromu směrem
        /// k listům.V každém navigačním vrcholu se testuje, zda interval vrcholu obsahuje hledanou
        /// hodnotu zeměpisné šířky. V případě záporné odpovědi hledání skončí, protože lze
        /// jednoznačně určit, že hledaný prvek se v range stromu nenachází. V opačném případě se
        /// pokračuje do obou podstromů aktuálního vrcholu. Při dosažení plnohodnotného prvku
        /// (listu stromu) se porovnají jeho souřadnice se souřadnicemi hledanými. --text z literatury-- 
        /// </summary>
        /// <param name="obeSouradnice">Proměnná uchovávající obě souřadnice.</param>
        /// <returns>Prvek Rozsahového stromu </returns>
        public T Najdi(ISouradnice obeSouradnice)
        {
            // je-li kořen platným vrcholem, porovnají se souřadnice a hledání skončí
            if (koren != null && koren.platny == true)
            {
                if (koren.nositelDat.vratX() == obeSouradnice.vratX() && koren.nositelDat.vratY() == obeSouradnice.vratY())
                {
                    return koren.nositelDat;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                // v opačném případě je strom prohledáván traverzováním směrem k listům
                PrvekRozsahovehoStromu pomocny = koren;
                while (true)
                {
                    if (pomocny == null)
                    {
                        return default(T);
                    }
                    // je-li levý syn platným vrcholem, porovnají se jeho souřadnice  
                    if (pomocny.levyPotomek != null
                            && pomocny.levyPotomek.platny == true
                            && pomocny.levyPotomek.nositelDat.vratX() == obeSouradnice.vratX()
                            && pomocny.levyPotomek.nositelDat.vratY() == obeSouradnice.vratY())
                    {
                        return pomocny.levyPotomek.nositelDat;
                    }
                    // je-li pravý syn platným vrcholem, porovnají se jeho souřadnice
                    if (pomocny.pravyPotomek != null
                            && pomocny.pravyPotomek.platny == true
                            && pomocny.pravyPotomek.nositelDat.vratX() == obeSouradnice.vratX()
                            && pomocny.pravyPotomek.nositelDat.vratY() == obeSouradnice.vratY())
                    {
                        return pomocny.pravyPotomek.nositelDat;
                    }
                    // jinak se rozhodne, kterým směrem pokračovat
                    // TODO: projit vetsi mensi jestli je spravne nakodovano
                    if (pomocny.levyPotomek != null
                        && pomocny.levyPotomek.platny == false
                        && pomocny.levyPotomek.zacatekIntervalu <= obeSouradnice.vratX()
                        && pomocny.levyPotomek.konecIntervalu >= obeSouradnice.vratX())
                    {
                        pomocny = pomocny.levyPotomek;
                    }
                    else if (pomocny.pravyPotomek != null
                        && pomocny.pravyPotomek.platny == false
                        && pomocny.pravyPotomek.zacatekIntervalu <= obeSouradnice.vratX()
                        && pomocny.pravyPotomek.konecIntervalu >= obeSouradnice.vratX())
                    {
                        pomocny = pomocny.pravyPotomek;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }

        /// <summary>
        /// Verejna implementace pro Intervalove vyhledavani. Vola privatni metodu NajdiInterval, kde je implementovano vyledavani.
        /// </summary>
        /// <param name="levyHorniBod">Levý horní roh v oblasti intervalového vyhledávání.</param>
        /// <param name="pravyDolniBod">Pravý horní roh v oblasti intervalového vyhledávání.</param>
        /// <returns>Vracé seznam vrcholů ve vybraném intervalu. </returns>
        public List<T> NajdiInterval(ISouradnice levyHorniBod, ISouradnice pravyDolniBod)
        {
            vysledekIntervalovehoHledani = new List<T>();
            //pri prvnim volani se nastavi vrchol jako koren a dimenzeX na true
            NajdiInterval(levyHorniBod, pravyDolniBod, koren, true);
            return vysledekIntervalovehoHledani;
        }

        /// <summary>
        /// Pro vyhledání prvků v obdélníkovém segmentu. Hledání opět začíná od kořene stromu směrem k listům. Při
        /// dosažení plnohodnotného prvku(listu stromu) se zkontrolují jeho souřadnice, a pokud
        /// spadají do hledaného segmentu, vykoná se na aktuálním prvku zadaná akce.Při
        /// traverzování přes navigační vrcholy se rozhoduje, jakým způsobem v hledání pokračovat.
        /// Uplatňují se přitom daná pravidla.
        /// </summary>
        /// <param name="levyHorniBod"></param>
        /// <param name="pravyDolniBod"></param>
        /// <param name="vrchol"></param>
        /// <param name="dimenzeX"></param>
        /// <returns></returns>
        private void NajdiInterval(ISouradnice levyHorniRohIntervalu, ISouradnice pravyDolniRohIntervalu,
            PrvekRozsahovehoStromu vrchol, bool dimenzeX)
        {
            if (vrchol != null)
            {
                // při dosažení plnohodnotného prvku se zkontrolují jeho souřadnice
                if (vrchol.platny == true)
                {
                    if (vrchol.nositelDat.vratX() >= levyHorniRohIntervalu.vratX()
                        && vrchol.nositelDat.vratX() <= pravyDolniRohIntervalu.vratX()
                        && vrchol.nositelDat.vratY() >= levyHorniRohIntervalu.vratY()
                        && vrchol.nositelDat.vratY() <= pravyDolniRohIntervalu.vratY()) // TODO: k uvaze
                    {                                               
                        vysledekIntervalovehoHledani.Add(vrchol.nositelDat);
                    }
                }
                else
                {
                    if (dimenzeX == true)
                    {
                        // patří-li celý interval do hledaného rozsahu, hledá se ve druhé dimenzi
                        if (levyHorniRohIntervalu.vratX() <= vrchol.zacatekIntervalu
                            && pravyDolniRohIntervalu.vratX() >= vrchol.konecIntervalu)
                        {
                            NajdiInterval(levyHorniRohIntervalu, pravyDolniRohIntervalu, vrchol.druhaDimenze, !dimenzeX);
                        }
                        // patří-li část intervalu do hledaného rozsahu, pokračuje se oběma syny
                        else if (levyHorniRohIntervalu.vratX() >= vrchol.zacatekIntervalu
                            || pravyDolniRohIntervalu.vratX() <= vrchol.konecIntervalu)
                        {
                            NajdiInterval(levyHorniRohIntervalu, pravyDolniRohIntervalu, vrchol.levyPotomek, dimenzeX);
                            NajdiInterval(levyHorniRohIntervalu, pravyDolniRohIntervalu, vrchol.pravyPotomek, dimenzeX);
                        }
                    }
                    else
                    // patří-li celý interval do hledaného rozsahu, provede se prohlídka podstromu
                    {
                        if (vrchol.zacatekIntervalu >= levyHorniRohIntervalu.vratY()
                            && vrchol.konecIntervalu <= pravyDolniRohIntervalu.vratY())
                        {
                            // TODO funkce prohlidka (vsechny listy podstromu a ulozi je do listu)
                            Prohlidka(vrchol, dimenzeX);
                        }
                        else if (vrchol.zacatekIntervalu <= levyHorniRohIntervalu.vratY()
                            || vrchol.konecIntervalu >= pravyDolniRohIntervalu.vratY())
                        {
                            NajdiInterval(levyHorniRohIntervalu, pravyDolniRohIntervalu, vrchol.levyPotomek, dimenzeX);
                            NajdiInterval(levyHorniRohIntervalu, pravyDolniRohIntervalu, vrchol.pravyPotomek, dimenzeX);
                        }
                    }

                }
            }
        }

        // prohlidka(PrvekRange < T > vrchol, AkceCallback akce, bool dimenzeX) – metoda
        // provádějící prohlídku, dotraverzuje k nejlevějšímu listu stromu, jehož kořen odpovídá
        // parametru vrchol, a následně projde zřetězený seznam se všemi plnohodnotnými prvky,
        // nad kterými vykoná zvolenou akci(během fáze ladění navíc kontroluje korektnost
        // umístění prvků ve struktuře),
        private void Prohlidka(PrvekRozsahovehoStromu vrchol, bool dimenzeX)
        {  
            // dotraverzovat k platnemu vrcholu
            if(vrchol != null)
            {
                if (vrchol.platny == true)
                {
                    vysledekIntervalovehoHledani.Add(vrchol.nositelDat);
                    if (vrchol.dalsiPrvekRozsahovehoStromu !=null)
                    {
                        Prohlidka(vrchol.dalsiPrvekRozsahovehoStromu, dimenzeX);
                    }
                }
                else
                {
                    if (vrchol.levyPotomek != null)
                    {
                        Prohlidka(vrchol.levyPotomek, dimenzeX);
                    }
                }
            }
            
        }                             


        /// <summary>
        /// Uzel stromu.
        /// </summary>
        private class PrvekRozsahovehoStromu
        {
            public PrvekRozsahovehoStromu levyPotomek, pravyPotomek, otec, druhaDimenze,
                predchoziPrvekzRozsahovehoStromu, dalsiPrvekRozsahovehoStromu;

            // proměnná platný určuje, zda se jedná o plnohodnotný nebo navigační vrchol
            // je-li prvek platný (true), nese finální data 
            // je-li prvek platný (false), nenese finalni data
            public bool platny = false;

            public T nositelDat;

            public int zacatekIntervalu, konecIntervalu;
             
            // pro prvek prvek rozsahoveho stromu - "list"
            public PrvekRozsahovehoStromu(T s)
            {
                this.platny = true; //datovym typem T zabezpeceno nastaveni
                nositelDat = s;
            }

            // pro prvek prvek rozsahoveho stromu - "navigacni vrchol"
            public PrvekRozsahovehoStromu(int navigacniVrcholX, int navigacnivrcholY)
            {
                this.zacatekIntervalu = navigacniVrcholX;
                this.konecIntervalu = navigacnivrcholY;
                platny = false;
            }

        }

    }
}
