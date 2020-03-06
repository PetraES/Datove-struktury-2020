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

        bool vyberPocatecni;
        bool vyberKonecny;
        bool vytvorBodvLese;

        //je to struktura
        System.Windows.Point gBod;

        Vrchol pocatek;
        Vrchol konec;

        private KoordinatorLesnichDobrodruzstvi mapa;
        private Dijkstra dijkstra;
        // private SearchHeap searchHeap;

        public MainWindow()
        {

            InitializeComponent();
            //visibility tu to zelene je enum
            tlacitko_ANO.Visibility = Visibility.Hidden;
            tlacitko_NE.Visibility = Visibility.Hidden;

            //searchHeap = new SearchHeap(mapa);
            dijkstra = new Dijkstra();
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

        public void vykresliMapu()
        {
            canvasElem.Children.Clear();
            foreach (Vrchol vrchol in mapa.GetVrcholy())
            {
                vykresliObec(vrchol);
            }
        }

        private void vykresliObec(Vrchol vrchol)
        {
            Ellipse currentDot = new Ellipse();
            if (vrchol.TypVrcholu == TypyVrcholu.odpocivadlo)
            {
                currentDot.Stroke = new SolidColorBrush(Colors.DarkBlue);
                currentDot.StrokeThickness = 10;
                currentDot.Height = 20;
                currentDot.Width = 20;
            }
            else if (vrchol.TypVrcholu == TypyVrcholu.zastavka)
            {
                currentDot.Stroke = new SolidColorBrush(Colors.Yellow);
                currentDot.StrokeThickness = 10;
                currentDot.Height = 12;
                currentDot.Width = 12;
            }
            else if (vrchol.TypVrcholu == TypyVrcholu.None)
            {
                currentDot.Stroke = new SolidColorBrush(Colors.Brown);
                currentDot.StrokeThickness = 8;
                currentDot.Height = 10;
                currentDot.Width = 10;
            }
            Canvas.SetZIndex(currentDot, 3);

            //if (vrchol.VLeteckemRaiusu == true)
            //{
            //    currentDot.Fill = new SolidColorBrush(Colors.Aqua);
            //}
            //else
            //{
            //    currentDot.Fill = new SolidColorBrush(Colors.Green);
            //}
            currentDot.Margin = new Thickness(vrchol.XSouradniceVrcholu * scaleX, vrchol.YSouradniceVrcholu * scaleY, 0, 0);
            currentDot.MouseLeftButtonDown += OnEllipseMouseLeftButtonDown;

            TextBlock TB = new TextBlock();
            TB.Text = vrchol.NazevVrcholu;
            BitmapCacheBrush bcb = new BitmapCacheBrush(TB);
            TB.Margin = new Thickness(vrchol.XSouradniceVrcholu * scaleX - 15, vrchol.YSouradniceVrcholu * scaleY + 10, 0, 0);
            TB.FontSize = 10;
            Canvas.SetZIndex(TB, 20);

            canvasElem.Children.Add(TB);
            canvasElem.Children.Add(currentDot);

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

            foreach (Hrana lesniCesta in vrchol.ListHran)
            {
                // Kreslíme to jakoby jenom od vrcholu 1 každé hrany - to nám zajistí, že nebudou silnice vykresleny 2x
                if (lesniCesta.PocatekHrany.XSouradniceVrcholu == vrchol.XSouradniceVrcholu && lesniCesta.PocatekHrany.YSouradniceVrcholu == vrchol.YSouradniceVrcholu)
                {
                    KresliSilnici(lesniCesta);
                }
            }
        }

        private void KresliSilnici(Hrana lesniStezka)
        {
            bool jeOznacena = false;
            if (lesniStezka.OznaceniHrany)
            {
                jeOznacena = true;
            }

            Line myline = new Line
            {
                Stroke = jeOznacena ? Brushes.Black : Brushes.Brown,
                StrokeThickness = 10,
                X1 = lesniStezka.PocatekHrany.XSouradniceVrcholu * scaleX + 4,
                Y1 = lesniStezka.PocatekHrany.YSouradniceVrcholu * scaleY + 4,
                X2 = lesniStezka.KonecHrany.XSouradniceVrcholu * scaleX + 4,
                Y2 = lesniStezka.KonecHrany.YSouradniceVrcholu * scaleY + 4
            };
            myline.Opacity = 0.5;
            myline.MouseLeftButtonDown += OnLineMouseLeftButtonDown;
            canvasElem.Children.Add(myline);

            float xLabel = lesniStezka.PocatekHrany.XSouradniceVrcholu + (lesniStezka.KonecHrany.XSouradniceVrcholu - lesniStezka.PocatekHrany.XSouradniceVrcholu) * 1 / 2;
            float yLabel = lesniStezka.PocatekHrany.YSouradniceVrcholu + (lesniStezka.KonecHrany.YSouradniceVrcholu - lesniStezka.PocatekHrany.YSouradniceVrcholu) * 1 / 2;
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

        void OnEllipseMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dot = (Ellipse)sender;
            float x = (float)(dot.Margin.Left / scaleX);
            float y = (float)(dot.Margin.Top / scaleY);

            Vrchol o = mapa.najdiVrchol(x, y);

            if (o != null)
            {
                if (vyberPocatecni)
                {
                    pocatek = o;
                    vyberPocatecni = false;
                    vyberKonecny = true;
                    label1.Content = "Pocatecni bod je " + pocatek.ToString() + ".\nVyber cil.";
                }
                else if (vyberKonecny)
                {
                    foreach (Hrana h in mapa.vsechnyHrany)
                    {
                        h.OznaceniHrany = false;
                    }
                    konec = o;
                    vyberKonecny = false;
                    label1.Content = "Pocatecni bod je " + pocatek.ToString() + ".\n"
                        + "Konecny bod je " + konec.ToString() + ". Hledam cestu.";
                    dijkstra.NajdiZastavku(pocatek, konec);
                    Cesta cesta = dijkstra.vratNejkratsiCestu();
                    if (cesta == null)
                    {
                        label1.Content = "cesta nenalezena";
                        return;
                    }
                    string vypisCesty = "Pocatecni bod je " + pocatek.ToString() + ".\n"
                        + "Konecny bod je " + konec.ToString() + ". ";
                    foreach (Hrana h in cesta.NavstiveneHrany)
                    {
                        h.OznaceniHrany = true;
                        vypisCesty += "(" + h.PocatekHrany.NazevVrcholu + ", " + h.KonecHrany.NazevVrcholu + "), ";
                    }
                    vykresliMapu();
                    label1.Content = vypisCesty;
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

            }
        }

        void OnLineMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var line = (Line)sender;
            float xZ = (float)((line.X1 - 4) / scaleX);
            float yZ = (float)((line.Y1 - 4) / scaleY);
            float xDo = (float)((line.X2 - 4) / scaleX);
            float yDo = (float)((line.Y2 - 4) / scaleY);

            // vrchol Z jako zacatek
            Vrchol vrhcholZ = mapa.najdiVrchol(xZ, yZ);
            if (vrhcholZ != null)
            {
                Hrana silnice = (Hrana)(from item in vrhcholZ.ListHran where item.KonecHrany.XSouradniceVrcholu.Equals(xDo) && item.KonecHrany.YSouradniceVrcholu.Equals(yDo) select item).First();
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

        private void PridatCestuButt_Click(object sender, RoutedEventArgs e)
        {
            //PridatSilnici modal = new PridatSilnici();
            //modal.ShowDialog();

            //if (modal.zObce != null)
            //{
            //    try
            //    {
            //        mapa.vlozSilnici(modal.zObce, modal.doObce, modal.cena);
            //    }
            //    catch (NotFoundException ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //    searchHeap.unMarkFinish();
            //    vykresliMapu();
            //}
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
            vyberPocatecni = true;
            label1.Content = "Vyber pocatecni bod.";
        }

        private void VlozBodButton_Click(object sender, RoutedEventArgs e)
        {
            vytvorBodvLese = true;
            label1.Content = "Oznacte misto na mape, kde ma vzniknout dalsi bod.";

            //sem dopsat kod na zadani dalsich bodu
        }

        private void canvasElem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (vytvorBodvLese == true)
            {
                var dot = (Canvas)sender;
                System.Windows.Point g = e.GetPosition((IInputElement)sender);
                int x = (int)(g.X / scaleX);
                int y = (int)(g.Y / scaleY);
                label1.Content = "Souradnice: " + x + "," + y + " Chcete tento vrchol vlozit do lesa?";
                tlacitko_ANO.Visibility = Visibility.Visible;
                tlacitko_NE.Visibility = Visibility.Visible;
                gBod.X = x;
                gBod.Y = y;
                vytvorBodvLese = false;
            }
        }

        private void NE_Button_Click(object sender, RoutedEventArgs e)
        {
            label1.Content = "Dobra, nic se vkladat nebude.";
            tlacitko_ANO.Visibility = Visibility.Hidden;
            tlacitko_NE.Visibility = Visibility.Hidden;
        }

        private void ANO_Button_Click(object sender, RoutedEventArgs e)
        {
            Vrchol pridanyVrchol = mapa.vlozVrchol((int)gBod.X, (int)gBod.Y);
            vykresliObec(pridanyVrchol);
            tlacitko_ANO.Visibility = Visibility.Hidden;
            tlacitko_NE.Visibility = Visibility.Hidden;
        }
    }

}
