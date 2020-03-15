using System;
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
        
        // private SearchHeap searchHeap;

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

        public void SkrytPrvkyVytvorBod()
        {
            //visibility tu to zelene je enum
            tlacitko_ANO.Visibility = Visibility.Hidden;
            tlacitko_NE.Visibility = Visibility.Hidden;
            TypVrcholu_comboBox.Visibility = Visibility.Hidden;
            nazevVrcholuLabel.Visibility = Visibility.Hidden;
            nazevVrcholuTextBox.Visibility = Visibility.Hidden;
        }

        public void ZobrazitPrvkyVytvorBod()
        {
            //visibility tu to zelene je enum
            tlacitko_ANO.Visibility = Visibility.Visible;
            tlacitko_NE.Visibility = Visibility.Visible;
            TypVrcholu_comboBox.Visibility = Visibility.Visible;
            nazevVrcholuLabel.Visibility = Visibility.Visible;
            nazevVrcholuTextBox.Visibility = Visibility.Visible;
        }

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

            //if (vrchol.VLeteckemRaiusu == true)
            //{
            //    currentDot.Fill = new SolidColorBrush(Colors.Aqua);
            //}
            //else
            //{
            //    currentDot.Fill = new SolidColorBrush(Colors.Green);
            //}
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

            //if (vrchol.LeteckeStredisko == true && vrchol.Radius > 0)
            //{
            //    Ellipse e = new Ellipse();
            //    e.Stroke = new SolidColorBrush(Colors.DarkCyan);
            //    e.StrokeThickness = 1;
            //    Canvas.SetZIndex(e, 3);
            //    e.Opacity = 0.4;
            //    float xL = vrchol.getX() * scaleX - (vrchol.Radius * scaleX);
            //    float yL = vrchol.getY() * scaleY - (vrchol.Radius * scaleY);

            //    e.Height = (vrchol.Radius * scaleX * 2);
            //    e.Width = (vrchol.Radius * scaleY * 2);
            //    e.Margin = new Thickness(xL, yL, 0, 0);
            //    canvasElem.Children.Add(e);
            //}

        }

        private void KresliSilnici(DataHran lesniStezka)
        {
            bool jeOznacena = false;
            if (lesniStezka.OznaceniHrany)
            {
                jeOznacena = true;
            }

            DataVrcholu pocatekHrany = mapa.najdiVrchol(lesniStezka.PocatekHrany);
            DataVrcholu konecHrany = mapa.najdiVrchol(lesniStezka.KonecHrany);

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
        /// Obsluhuje udalost kliku na bod v mape (zastavka, odpocivadlo nebo krizovatka).
        /// Vyuziva se na urceni pocatecniho a koncoveho vrcholu pro vyhledavani nejkratsi trasy v mape nebo pro pridani nove cesty do mapy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnEllipseMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dot = (Ellipse)sender;
            string klicVrcholu = dot.Name.Replace("_", " ");

            DataVrcholu hledanyVrcholvMape = mapa.najdiVrchol(klicVrcholu);

            if (hledanyVrcholvMape != null)
            {
                if (urcenPocatecniBod)
                {
                    pocatek = hledanyVrcholvMape;
                    urcenPocatecniBod = false;
                    urcenKonecnyBod = true;
                    //sdelim ye obsah v Label1 je typu textblock a tim dosahnu zalamovani textu  vLabel1
                    ((TextBlock)label1.Content).Text = "Pocatecni bod je " + pocatek.ToString() + ".\nVyber cil.";
                }
                else if (urcenKonecnyBod)
                {
                    foreach (DataHran h in mapa.VratHrany())
                    {
                        h.OznaceniHrany = false;
                    }
                    konec = hledanyVrcholvMape;
                    urcenKonecnyBod = false;
                    ((TextBlock)label1.Content).Text = "Pocatecni bod je " + pocatek.ToString() + ".\n"
                        + "Konecny bod je " + konec.ToString() + ". Hledam cestu.";
                    
                    Cesta cesta = mapa.najdiCestu(pocatek.NazevVrcholu, konec.NazevVrcholu);
                    if (cesta == null)
                    {
                        ((TextBlock)label1.Content).Text = "cesta nenalezena";
                        return;
                    }
                    string vypisCesty = "Pocatecni bod je " + pocatek.ToString() + ".\n"
                        + "Konecny bod je " + konec.ToString() + ". \n";
                    foreach (DataHran h in cesta.NavstiveneHrany)
                    {
                        h.OznaceniHrany = true;
                        vypisCesty += "(" + h.PocatekHrany + ", " + h.KonecHrany + "), ";
                    }
                    vykresliMapu();
                    ((TextBlock)label1.Content).Text = vypisCesty;
                }
                //  pro ulozeni pocatecniho vrcholu cesty, vytvoreni a vykresleni hrany
                else if (stisknutoVytvorCestu)
                {
                    if (pocatek == null)
                    {
                        pocatek = hledanyVrcholvMape;
                        ((TextBlock)label1.Content).Text = "Vyberte konečný bod.";
                    }
                    else
                    {
                        konec = hledanyVrcholvMape;
                        DataHran novaHrana = mapa.vytvorHranu(pocatek.NazevVrcholu, konec.NazevVrcholu, (short)spocitejDelkuHrany(pocatek, konec));
                        KresliSilnici(novaHrana);
                        stisknutoVytvorCestu = false;
                        ((TextBlock)label1.Content).Text = "Cesta byla vytvořena.";
                    }
                }
            }
        }

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

        //ObecInfo info = new ObecInfo(o);
        //info.ShowDialog();
        //if (info.needRedraw == true)
        //{
        //    searchHeap.unMarkFinish();
        //    if (info.obec.LeteckeStredisko == true && info.obec.Radius > 0)
        //    {
        //        mapa.najdiObceVRadiusu(info.obec);
        //        vykresliMapu();
        //    }
        //    else
        //    {
        //        if (info.obec.LeteckyObsluzne != null)
        //        {
        //            mapa.zrusLeteckouObsluhu(info.obec);
        //        }
        //    }
        //    vykresliMapu();

        //}

        //if (info.smazatObec == true)
        //{
        //    mapa.odeberObec(o);
        //    searchHeap.unMarkFinish();
        //    vykresliMapu();
        //}

        void OnLineMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var line = (Line)sender;
            string[] kliceVrcholu = line.Name.Split("|");
            float xZ = (float)((line.X1 - 4) / scaleX);
            float yZ = (float)((line.Y1 - 4) / scaleY);
            float xDo = (float)((line.X2 - 4) / scaleX);
            float yDo = (float)((line.Y2 - 4) / scaleY);

            // vrchol Z jako zacatek
            DataVrcholu vrhcholZ = mapa.najdiVrchol(kliceVrcholu[0]);
            if (vrhcholZ != null)
            {
                // DataHran silnice = (DataHran)(from item in vrhcholZ.ListHran where item.KonecHrany.XSouradniceVrcholu.Equals(xDo) && item.KonecHrany.YSouradniceVrcholu.Equals(yDo) select item).First();
                //SilniceInfo info = new SilniceInfo(silnice);
                //info.ShowDialog();

                //if (info.needRedraw == true)
                //{
                //    // searchHeap.unMarkFinish();
                //    vykresliMapu();
                //}

                //if (info.smazatSilnici == true)
                //{
                //    mapa.odeberSilnici(info.silnice);
                //    searchHeap.unMarkFinish();
                //    vykresliMapu();
                //}

                //if (info.silnice.Nehoda == true)
                //{
                //    mapa.setNehoda(info.silnice);
                //    searchHeap.unMarkFinish();
                //    vykresliMapu();
                //}
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Chcete uložit změny před ukončením?", "Uložit?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //mapa.ulozitDataDoCSV(Settings.Default.pathFileObce, Settings.Default.pathFileCesty);
            }
        }

        private void NajdiCestuButt_Click(object sender, RoutedEventArgs e)
        {
            urcenPocatecniBod = true;
            ((TextBlock)label1.Content).Text = "Vyberte počáteční bod.";
        }

        
        private void VlozBodButton_Click(object sender, RoutedEventArgs e)
        {
            stisknutoVytvorVrchol = true;
            ((TextBlock)label1.Content).Text = "Oznacte misto na mape, kde ma vzniknout dalsi bod.";
            // bod se vytvari pri kliknuti do canvasu metoda canvasElem_MouseLeftButtonDown
            }

        private void canvasElem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (stisknutoVytvorVrchol == true)
            {
                var dot = (Canvas)sender;
                System.Windows.Point g = e.GetPosition((IInputElement)sender);
                int x = (int)(g.X / scaleX);
                int y = (int)(g.Y / scaleY);
                ((TextBlock)label1.Content).Text = "Vytvářený bod má souřadnice: " + x + "," + y + ".";


                ((TextBlock)label1.Content).Text += " Chcete tento vrchol vlozit do lesa?";

                ZobrazitPrvkyVytvorBod();
                gBod.X = x;
                gBod.Y = y;
                stisknutoVytvorVrchol = false;
            }
        }

        private void NE_Button_Click(object sender, RoutedEventArgs e)
        {
            ((TextBlock)label1.Content).Text = "Dobra, nic se vkladat nebude.";
            SkrytPrvkyVytvorBod();
        }

        private void ANO_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TypyVrcholu typVrcholu = (TypyVrcholu)TypVrcholu_comboBox.SelectedIndex;
                string nazevVrcholu = nazevVrcholuTextBox.Text;
                DataVrcholu pridanyVrchol = mapa.vlozVrchol((int)gBod.X, (int)gBod.Y, typVrcholu, nazevVrcholu);
                vykresliObec(pridanyVrchol);
                SkrytPrvkyVytvorBod();
                ((TextBlock)label1.Content).Text = "Hotovo";
            } catch(Exception ex)
            {
                ((TextBlock)label1.Content).Text = "Nastala chyba: " + ex.Message;
            }
        }

        private void PridejCestuButton_Click(object sender, RoutedEventArgs e)
        {

            // nastaveni pomocne promene na true v if else v metode OnEllipseMouseLeftButtonDow
            stisknutoVytvorCestu = true;
            pocatek = null;
            konec = null;
            ((TextBlock)label1.Content).Text = "Vyberte počáteční bod.";

        }

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
                mapa.ulozMapu();
            }
        }
    }

}
