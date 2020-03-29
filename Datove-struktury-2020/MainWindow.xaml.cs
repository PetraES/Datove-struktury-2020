﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Datove_struktury_2020.Data;
using System.Drawing;

namespace Datove_struktury_2020
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int scaleX = 4; // Settings.Default.mapXScale;
        private int scaleY = 2; // Settings.Default.mapYScale;

        bool urcenPocatecniBod; // true pokud stisknu tlacitko vyhledej cestu
        bool urcenKonecnyBod; // je-li vybran/urcen pocatecni bod pak se nastavi na true
        bool stisknutoVytvorVrchol; //pokud dojde ke stsknuti tlacitka, nastavi se na true
        bool stisknutoVytvorCestu; //pokud dojde ke stsknuti tlacitka, nastavi se na true

        //je to struktura
        System.Windows.Point gBod;

        DataVrcholu pocatek;
        DataVrcholu konec;

        private KoordinatorLesnichDobrodruzstvi mapa;

        /// <summary>
        /// Hlavní okno aplikace, hlavní vstup.
        /// <exception> Vyhodí výjimku v případě, že se nepodaří načíst data grafu. </exception>
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SkrytPrvkyVytvorBod();
            try
            {
                mapa = new KoordinatorLesnichDobrodruzstvi();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Nepodařilo se načíst les!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            vykresliMapu();
        }

        /// <summary>
        /// Nastavení viditelnosti prvků funkce vytvoř bod. Na neviditelné.
        /// </summary>
        public void SkrytPrvkyVytvorBod()
        {
            //visibility tu to zelene je enum
            tlacitko_ANO.Visibility = Visibility.Hidden;
            tlacitko_NE.Visibility = Visibility.Hidden;
            TypVrcholu_comboBox.Visibility = Visibility.Hidden;
            nazevVrcholuLabel.Visibility = Visibility.Hidden;
            nazevVrcholuTextBox.Visibility = Visibility.Hidden;
            druhVrcholuLabel.Visibility = Visibility.Hidden;
            ANO_NEVrcholuLabel.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Nastaveni viditelnosti prvků funkce vytvoř bod. Na viditelné.
        /// </summary>
        public void ZobrazitPrvkyVytvorBod()
        {
            //visibility tu to zelene je enum
            tlacitko_ANO.Visibility = Visibility.Visible;
            tlacitko_NE.Visibility = Visibility.Visible;
            TypVrcholu_comboBox.Visibility = Visibility.Visible;
            nazevVrcholuLabel.Visibility = Visibility.Visible;
            nazevVrcholuTextBox.Visibility = Visibility.Visible;
            druhVrcholuLabel.Visibility = Visibility.Visible;
            ANO_NEVrcholuLabel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Postupně prochází body a hrany grafu a vykresluje je.
        /// </summary>
        public void vykresliMapu()
        {
            canvasElem.Children.Clear();
            foreach (DataVrcholu vrchol in mapa.GetVrcholy())
            {
                vykresliObec(vrchol);
            }
            foreach (DataHran hrana in mapa.VratHrany())
            {
                KresliSilnici(hrana);
            }
        }

        /// <summary>
        /// Vykresluje vrcholy/body/mista na mapě.
        /// </summary>
        /// <param name="vrchol"></param>
        private void vykresliObec(DataVrcholu vrchol)
        {
            Ellipse teckaNaVrcholu = new Ellipse();
            teckaNaVrcholu.Name = vrchol.NazevVrcholu.Replace(" ", "_");
            if (vrchol.TypVrcholu == TypyVrcholu.odpocivadlo)
            {
                teckaNaVrcholu.Stroke = new SolidColorBrush(Colors.DarkBlue);
                teckaNaVrcholu.StrokeThickness = 10;
                teckaNaVrcholu.Height = 15;
                teckaNaVrcholu.Width = 20;
            }
            else if (vrchol.TypVrcholu == TypyVrcholu.zastavka)
            {
                teckaNaVrcholu.Stroke = new SolidColorBrush(Colors.Cyan);
                teckaNaVrcholu.StrokeThickness = 10;
                teckaNaVrcholu.Height = 12;
                teckaNaVrcholu.Width = 12;
            }
            else if (vrchol.TypVrcholu == TypyVrcholu.None)
            {
                teckaNaVrcholu.Stroke = new SolidColorBrush(Colors.Brown);
                teckaNaVrcholu.StrokeThickness = 8;
                teckaNaVrcholu.Height = 10;
                teckaNaVrcholu.Width = 10;
            }
            Canvas.SetZIndex(teckaNaVrcholu, 3);

            teckaNaVrcholu.Margin = new Thickness(vrchol.XSouradniceVrcholu * scaleX, vrchol.YSouradniceVrcholu * scaleY, 0, 0);

            //DELEGAT tu se zaregistruje funkce OnElipse.. na jako event handler na akci MouseLeft.. - DELEGAT
            teckaNaVrcholu.MouseLeftButtonDown += OnEllipseMouseLeftButtonDown;

            TextBlock TB = new TextBlock();
            TB.Text = vrchol.NazevVrcholu;
            BitmapCacheBrush bcb = new BitmapCacheBrush(TB);
            TB.Margin = new Thickness(vrchol.XSouradniceVrcholu * scaleX - 15, vrchol.YSouradniceVrcholu * scaleY + 10, 0, 0);
            TB.FontSize = 10;
            Canvas.SetZIndex(TB, 20);

            canvasElem.Children.Add(TB);
            canvasElem.Children.Add(teckaNaVrcholu);
        }

        /// <summary>
        /// Vykresluje 1 cestu na mapě. 
        /// </summary>
        /// <param name="lesniStezka">nese <c>DataHran<c> potřebné k vytvoření hrany</param>
        private void KresliSilnici(DataHran lesniStezka)
        {
            bool jeOznacena = false;
            if (lesniStezka.OznaceniHrany)
            {
                jeOznacena = true;
            }

            DataVrcholu pocatekHrany = mapa.NajdiVrchol(lesniStezka.PocatekHrany);
            DataVrcholu konecHrany = mapa.NajdiVrchol(lesniStezka.KonecHrany);

            Line myline = new Line
            {
                // Name = String.Format("{0}__{1}", lesniStezka.PocatekHrany, lesniStezka.KonecHrany),
                Stroke = jeOznacena ? Brushes.Black : Brushes.Beige,
                StrokeThickness = 10,
                X1 = pocatekHrany.XSouradniceVrcholu * scaleX + 4,
                Y1 = pocatekHrany.YSouradniceVrcholu * scaleY + 4,
                X2 = konecHrany.XSouradniceVrcholu * scaleX + 4,
                Y2 = konecHrany.YSouradniceVrcholu * scaleY + 4
            };
            myline.Opacity = 0.9;
            myline.MouseLeftButtonDown += OnLineMouseLeftButtonDown;
            canvasElem.Children.Add(myline);

            float xLabel = pocatekHrany.XSouradniceVrcholu + (konecHrany.XSouradniceVrcholu - pocatekHrany.XSouradniceVrcholu) * 1 / 2;
            float yLabel = pocatekHrany.YSouradniceVrcholu + (konecHrany.YSouradniceVrcholu - pocatekHrany.YSouradniceVrcholu) * 1 / 2;
            TextBlock TB = new TextBlock();
            TB.Text = $"{lesniStezka.DelkaHrany} min";
            BitmapCacheBrush bcb = new BitmapCacheBrush(TB);
            TB.Margin = new Thickness(xLabel * scaleX - 10, yLabel * scaleY + 5, 0, 0);
            TB.FontSize = 10;
            Canvas.SetZIndex(TB, 20);
            canvasElem.Children.Add(TB);

            if (lesniStezka.JeFunkcniCesta == false)
            {
                Ellipse currentDot = new Ellipse();
                currentDot.Stroke = new SolidColorBrush(Colors.Red);
                currentDot.StrokeThickness = 1;
                Canvas.SetZIndex(currentDot, 10);
                currentDot.Height = 10;
                currentDot.Width = 10;
                currentDot.Fill = new SolidColorBrush(Colors.Red);
                currentDot.Margin = new Thickness(xLabel * scaleX, yLabel * scaleY, 0, 0);
                canvasElem.Children.Add(currentDot);
            }
        }

        /// <summary>
        /// Modifikuje text z label1 na TexBlock formát, hromadné využití.
        /// </summary>
        /// <param name="text"></param>
        private void nastavTextLabelu(string text)
        {
            // label1.Content = text;
            ((TextBlock)label1.Content).Text = text;
        }

        /// <summary>
        /// Smaze cestu kdyz dojde ke stisku dalsi moznosti z vyberu.
        /// </summary>
        private void smazNalezenouCestu()
        {
            foreach (DataHran h in mapa.VratHrany())
            {
                h.OznaceniHrany = false;
            }
            vykresliMapu();
        }

        /// <summary>
        /// Vyresetuje uzivatelske rozhrani do zakladniho stavu.
        /// </summary>
        private void resetAkci()
        {
            smazNalezenouCestu();
            SkrytPrvkyVytvorBod();
            urcenPocatecniBod = false;
            urcenKonecnyBod = false;
            stisknutoVytvorVrchol = false;
            stisknutoVytvorCestu = false;

            pocatek = null;
            konec = null;
        }

        /// <summary>
        /// Obsluhuje udalost kliku na bod v mape (zastavka, odpocivadlo nebo krizovatka).
        /// Vyuziva se na urceni pocatecniho a koncoveho vrcholu pro vyhledavani nejkratsi trasy v mape 
        /// nebo pro pridani nove cesty do mapy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnEllipseMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dot = (Ellipse)sender;
            string klicVrcholu = dot.Name.Replace("_", " ");

            DataVrcholu hledanyVrcholvMape = mapa.NajdiVrchol(klicVrcholu);

            if (hledanyVrcholvMape != null)
            {
                if (urcenPocatecniBod)
                {
                    pocatek = hledanyVrcholvMape;
                    urcenPocatecniBod = false;
                    urcenKonecnyBod = true;
                    //sdelim ye obsah v Label1 je typu textblock a tim dosahnu zalamovani textu  vLabel1
                    nastavTextLabelu("Pocateční bod je " + pocatek.ToString() + ".\n Vyber cíl.");
                }
                else if (urcenKonecnyBod)
                {
                    konec = hledanyVrcholvMape;
                    urcenKonecnyBod = false;
                    nastavTextLabelu("Pocatecni bod je: " + pocatek.ToString() + ".\n"
                        + "Konečný bod je: " + konec.ToString() + ". Hledám nejkratší cestu...");

                    Cesta cesta = mapa.NajdiCestu(pocatek.NazevVrcholu, konec.NazevVrcholu);
                    if (cesta == null)
                    {
                        nastavTextLabelu("Cestu se nepodařilo nalézt.");
                        return;
                    }
                    string vypisCesty = "Počáteční bod je " + pocatek.ToString() + ". \n"
                        + "Konecny bod je " + konec.ToString() + ". \n";
                    foreach (DataHran h in cesta.NavstiveneHrany)
                    {
                        h.OznaceniHrany = true;
                        vypisCesty += "(" + h.PocatekHrany + ", " + h.KonecHrany + "), ";
                    }
                    vykresliMapu();
                    nastavTextLabelu(vypisCesty);
                }
                //  pro ulozeni pocatecniho vrcholu cesty, vytvoreni a vykresleni hrany
                else if (stisknutoVytvorCestu)
                {
                    if (pocatek == null)
                    {
                        pocatek = hledanyVrcholvMape;
                        nastavTextLabelu("Vyberte konečný bod.");
                    }
                    else
                    {
                        konec = hledanyVrcholvMape;
                        DataHran novaHrana = mapa.VytvorHranu(pocatek.NazevVrcholu, konec.NazevVrcholu, (short)spocitejDelkuHrany(pocatek, konec));
                        KresliSilnici(novaHrana);
                        stisknutoVytvorCestu = false;
                        nastavTextLabelu("Cesta byla vytvořena.");
                    }
                }
            }
        }

        /// <summary>
        /// Vypočítá vzdálenost bodů dvou vrcholů.
        /// </summary>
        /// <param name="zacatekHrany">Počátecní bod hrany</param>
        /// <param name="konecHrany">Konečný bod hrany</param>
        /// <returns>Délku hrany ve formátu double.</returns>
        private double spocitejDelkuHrany(DataVrcholu zacatekHrany, DataVrcholu konecHrany)
        {
            double delkaHrany = 0;
            double xa = zacatekHrany.XSouradniceVrcholu;
            double ya = zacatekHrany.YSouradniceVrcholu;
            double xb = konecHrany.XSouradniceVrcholu;
            double yb = konecHrany.YSouradniceVrcholu;
            //(int)Math.Round(Sqrt)
            delkaHrany = Math.Abs(Math.Sqrt(((xb - xa) * (xb - xa)) + ((yb - ya) * (yb - ya))));
            return delkaHrany;
        }


        void OnLineMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var line = (Line)sender;
            string[] kliceVrcholu = line.Name.Split("|");
            float xZ = (float)((line.X1 - 4) / scaleX);
            float yZ = (float)((line.Y1 - 4) / scaleY);
            float xDo = (float)((line.X2 - 4) / scaleX);
            float yDo = (float)((line.Y2 - 4) / scaleY);

            // vrchol Z jako zacatek
            DataVrcholu vrhcholZ = mapa.NajdiVrchol(kliceVrcholu[0]);
            if (vrhcholZ != null)
            {
                // DataHran silnice = (DataHran)(from item in vrhcholZ.ListHran where item.KonecHrany.XSouradniceVrcholu.Equals(xDo) && item.KonecHrany.YSouradniceVrcholu.Equals(yDo) select item).First();

            }
        }

        /// <summary>
        /// Slouží pro zpětnou vazbu od uživatele při zavírání hlavního okna.
        /// Vyvolá dialog ANO/NE. Podle volby ukládá změny do .csv souboru.
        /// </summary>
        /// <remakrs> ANO = uloží provedené změny, NE = neuloží provedené změny</remakrs>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Chcete uložit změny před ukončením?", "Uložit?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //mapa.ulozitDataDoCSV(Settings.Default.pathFileObce, Settings.Default.pathFileCesty);
            }
        }

        /// <summary>
        /// Po stisku tlačítka vyzve k vybrání počátečního bodu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NajdiCestuButt_Click(object sender, RoutedEventArgs e)
        {
            resetAkci();
            urcenPocatecniBod = true;
            nastavTextLabelu("Vyberte počáteční bod.");
        }

        /// <summary>
        /// Vstupní fuknce pro vkládání bodu. Vyzve uživatele.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VlozBodButton_Click(object sender, RoutedEventArgs e)
        {
            resetAkci();
            stisknutoVytvorVrchol = true;
            nastavTextLabelu("Označte prosím místo na mapě, kde má vzniknout další bod.");
            // bod se vytvari pri kliknuti do canvasu metoda canvasElem_MouseLeftButtonDown
        }

        /// <summary>
        /// Obsluhuje akci, když uživatel v oblati <c>canvasElem_MouseLeftButtonDown<c> zmáčkne levé tlačítko myši.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canvasElem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (stisknutoVytvorVrchol == true)
            {
                var dot = (Canvas)sender;
                System.Windows.Point g = e.GetPosition((IInputElement)sender);
                int x = (int)(g.X / scaleX);
                int y = (int)(g.Y / scaleY);
                string textdoLabelu = "Vytvářený bod má souřadnice: " + x + "," + y + ". " +
                    "Můžete zvolit název a druh bodu. \nPoté stiknutím tlačítka ANO/NE zvolit jeho umístění do mapy.";
                nastavTextLabelu(textdoLabelu);
                ZobrazitPrvkyVytvorBod();
                gBod.X = x;
                gBod.Y = y;
                stisknutoVytvorVrchol = false;
            }
        }

        private void NE_Button_Click(object sender, RoutedEventArgs e)
        {
            nastavTextLabelu("Pro tentokrát bude volba zapomenuta.");
            SkrytPrvkyVytvorBod();
        }

        /// <summary>
        /// Obsluhuje tlačítko ANO. V případě zadání všech potřebných parametrů vykreslí daný bod a oznámí hototvo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ANO_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TypyVrcholu typVrcholu = (TypyVrcholu)TypVrcholu_comboBox.SelectedIndex;
                string nazevVrcholu = nazevVrcholuTextBox.Text;
                DataVrcholu pridanyVrchol = mapa.VlozVrchol((int)gBod.X, (int)gBod.Y, typVrcholu, nazevVrcholu);
                vykresliObec(pridanyVrchol);
                SkrytPrvkyVytvorBod();
                nastavTextLabelu("Hotovo, vypadá to, že máme nový bod.");
            }
            catch (Exception ex)
            {
                nastavTextLabelu("Nastala chyba: " + ex.Message);
            }
        }

        /// <summary>
        /// Zahajuje přidávání nové cesty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PridejCestuButton_Click(object sender, RoutedEventArgs e)
        {
            resetAkci();
            // nastaveni pomocne promene na true v if else v metode OnEllipseMouseLeftButtonDow
            stisknutoVytvorCestu = true;
            pocatek = null;
            konec = null;
            nastavTextLabelu("Vyberte počáteční bod.");
            // ((TextBlock)label1.Content).Text = "Vyberte počáteční bod.";
        }

        /// <summary>
        /// Obsluhuje ukládání změn při zavírání hlavního okna programu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string msg = "Než se zavře okno, mají se změny uložit?";
            MessageBoxResult result =
              MessageBox.Show(
                msg,
                "Název okna",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                // kdyz se zmackne Yes tak se ulozi nova podoba mapy do .csv souboru
                mapa.UlozMapu();
            }
        }

        ISouradnice zacatekOblastiPolom;
        ISouradnice konecOblastiPolom;
        System.Windows.Shapes.Rectangle a;

        private int PorovnejSouradnice(int s1, int s2)
        {            
            if (s1 < s2)
            {
                return s1;
            }
            else
            {
                return s2;
            }                         
        }

        private void VykresliRectangle()
        {
            bool novy = false;
            if (a == null)
            {
                novy = true;
                a = new System.Windows.Shapes.Rectangle();
            }
            int x1 = zacatekOblastiPolom.vratX();
            int y1 = zacatekOblastiPolom.vratY();
            int x2 = konecOblastiPolom.vratX();
            int y2 = konecOblastiPolom.vratY();
            int xmin = PorovnejSouradnice(x1, x2);
            int ymin = PorovnejSouradnice(y1, y2);
            a.Margin = new Thickness(xmin, ymin, 0, 0);
            a.Width = Math.Abs(x2 - x1);
            a.Height = Math.Abs(y2 - y1);
            a.Stroke = new SolidColorBrush(Colors.GreenYellow);
            a.StrokeThickness = 5;
            if (novy)
            {
                canvasElem.Children.Add(a);
            }
        }

        private void canvasElem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            zacatekOblastiPolom = new Data2Dim((int)point.X, (int)point.Y);
        }

        private void canvasElem_MouseMove(object sender, MouseEventArgs e)
        {
            if (zacatekOblastiPolom != null)
            {
                var point = e.GetPosition((IInputElement)sender);
                konecOblastiPolom = new Data2Dim((int)point.X, (int)point.Y);
                VykresliRectangle();
            }
        }

        private void canvasElem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition((IInputElement)sender);
            konecOblastiPolom = new Data2Dim((int)point.X, (int)point.Y);
            VykresliRectangle();
            List<DataVrcholu> polamaneVrcholy =  mapa.ZpracujInterval(zacatekOblastiPolom, konecOblastiPolom);

            //aby se neprekresloval rectangl pri pohybu mysi
            zacatekOblastiPolom = null;

            foreach (DataVrcholu vrchol in polamaneVrcholy) 
            {
                IEnumerable<DataHran> polamaneHrany = mapa.VratIncedencniHrany(vrchol.NazevVrcholu);
                foreach (DataHran hrana in polamaneHrany)
                {
                    hrana.JeFunkcniCesta = false;
                }
            }
            vykresliMapu();            
        }
    }

}
