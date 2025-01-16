using System;
using System.Text;

namespace Algorytm_Genetyczny
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            aTextBox.Text = "-4";
            bTextBox.Text = "12";
            nTextBox.Text = "10";
            pkTextBox.Text = "0.7";
            pmTextBox.Text = "0.002";
            //dirComboBox.SelectedIndex = 0;
            dComboBox.SelectedIndex = 2;
            tTextBox.Text = "10";
            eliteCheckBox.Checked = true;
            //this.testDataGridView.;
            this.testDataGridView.Rows.Add(1, 80, 130, 0.8, 0.01, 1.99194825099);//1
            this.testDataGridView.Rows.Add(2, 80, 150, 0.8, 0.01, 1.99167246331);//2
            this.testDataGridView.Rows.Add(3, 80, 80, 0.9, 0.001, 1.99138288603);//3
            this.testDataGridView.Rows.Add(4, 80, 120, 0.9, 0.01, 1.99126500443);//4
            this.testDataGridView.Rows.Add(5, 80, 120, 0.9, 0.005, 1.99031841546);//5
            this.testDataGridView.Rows.Add(6, 80, 120, 0.8, 0.01, 1.99031841546);//6
            this.testDataGridView.Rows.Add(7, 75, 110, 0.9, 0.01, 1.99017589869);//7
            this.testDataGridView.Rows.Add(8, 65, 110, 0.65, 0.01, 1.99013587421);//8
            this.testDataGridView.Rows.Add(9, 80, 100, 0.9, 0.01, 1.98971738834);//9
            this.testDataGridView.Rows.Add(10, 80, 110, 0.85, 0.01, 1.98946356747);//10
        }
        int a;
        int b;
        int n;
        double d;
        int l;
        double pk;
        double pm;
        //string direction;
        int t;
        //public Random rand = new();
        private void Start_Click(object sender, EventArgs e)
        {
            this.dataGridView2.Rows.Clear();
            this.wykres.Reset();
            // clear this.wykres.Refresh() 

            a = int.Parse(aTextBox.Text);                                   //inicjalizacja zmiennych z 
            b = int.Parse(bTextBox.Text);
            //d = Convert.ToDouble(dListBox.SelectedItem.ToString());
            d = Convert.ToDouble(dComboBox.Text);
            l = (int)Math.Ceiling(Math.Log((b - a) / d + 1, 2)); // obliczanie pot�gi dw�jek, r�wnej wymaganej liczbie bit�w formy binarnej xBin1 i xBin2
            n = int.Parse(nTextBox.Text);
            pk = double.Parse(pkTextBox.Text);
            pm = double.Parse(pmTextBox.Text);
            //direction = dirComboBox.SelectedItem.ToString();
            t = int.Parse(tTextBox.Text);


            Pokolenie[] pArray = new Pokolenie[t]; //(int lower, int upper, int exp,int n, double precision, double randomDouble)
            pArray[0] = new Pokolenie(a, b, l, n, d);
            pArray[0].Dystrybuanta();
            pArray[0].AvgF1();
            pArray[0].MinF1();
            pArray[0].Elita1();
            for (int i = 0; i < n; ++i) pArray[0].array[i].y2 = pArray[0].array[i].y;
            for (int i = 1; i < t; ++i) // obliczanie warto�ci kolejnych 
            {
                //parray[i-1].Homogenizacja();
                pArray[i] = new Pokolenie(pArray[i - 1], n); // deklaracja inaczej by�o "instance of an object is not set to [co�tam]"
                Pokolenie.Selekcja(pArray[i - 1], pArray[i], n); //SELEKCJA NATURALNA
                pArray[i].Losowanie(pk); // rodzic�w
                List<int> pary = pArray[i].Parowanie(); // tych�e rodzic�w

                pArray[i].PunktCiecia(pary); // dla pary rodzic�w, czasem para ma r�ne punty ciecia

                pArray[i].Krzyzowanie(pary);// krzyzowanie rodzicow
                pary.Clear();
                pArray[i].Mutacja(pm); // losowana dla ka�dego bitu
                for (int j = 0; j < n; ++j) // 
                {
                    Osobnik.FromBinCalcIntRealY(pArray[i].array[j]);
                }

                if (eliteCheckBox.Checked)
                {
                    pArray[i].EnsureElite(pArray[i - 1].array[pArray[i - 1].elita], pArray[i - 1].elita); // ju� dzia�a
                }
                pArray[i].Dystrybuanta();
                pArray[i].AvgF2();
                pArray[i].MinF2();
                pArray[i].Elita2();
            }
            // WYKRES
            double[] dataX = new double[t];
            double[] dataAvgY = new double[t];
            double[] dataMinY = new double[t];
            double[] dataEliteY = new double[t];
            for (int i = 0; i < t; ++i)
            {
                dataX[i] = i;
                dataAvgY[i] = pArray[i].avgF;
                dataMinY[i] = pArray[i].minF;
                dataEliteY[i] = pArray[i].array[pArray[i].elita].y2;
            }
            this.wykres.Plot.XLabel("Numer Pokolenia");
            this.wykres.Plot.YLabel("Wartość f(x)");
            this.wykres.Plot.Title("Wartości f(x) na przestrzeni t pokoleń");
            this.wykres.Plot.AddScatter(dataX, dataEliteY, label: "MaxY");
            this.wykres.Plot.AddScatter(dataX, dataAvgY, label: "AvgY");
            this.wykres.Plot.AddScatter(dataX, dataMinY, label: "MinY");
            //this.wykres.Plot.XAxis. //(inty bym chcia�)
            this.wykres.Plot.Legend();
            this.wykres.Plot.AxisAuto();
            this.wykres.Plot.SetAxisLimitsY(-2.1, 2.1, 0);
            this.wykres.Render();

            //WY�WIETLANIE ZAK�ADKI: DANE
            static void exchange(List<int> data, int m, int n)
            {
                int temporary;

                temporary = data[m];
                data[m] = data[n];
                data[n] = temporary;
            }
            static void exchangestring(List<string> data, int m, int n)
            {
                string temporary;

                temporary = data[m];
                data[m] = data[n];
                data[n] = temporary;
            }
            void QuickSort(List<int> indeces, List<int> count, List<string> xbins, int l, int r)
            {
                int i, j;
                int x;

                i = l;
                j = r;

                x = indeces[(l + r) / 2]; // find pivot item
                while (i <= j)
                {
                    while (pArray[t - 1].array[indeces[i]].y2 > pArray[t - 1].array[x].y2)
                        i++;
                    while (pArray[t - 1].array[indeces[j]].y2 < pArray[t - 1].array[x].y2)
                        j--;
                    if (i <= j)
                    {
                        exchange(indeces, i, j);
                        exchange(count, i, j);
                        exchangestring(xbins, i, j);
                        i++;
                        j--;
                    }
                }
                if (l < j)
                    QuickSort(indeces, count, xbins, l, j);
                if (i < r)
                    QuickSort(indeces, count, xbins, i, r);
            }
            List<string> xbins = new List<string>();
            List<int> indeces = new List<int>();
            List<int> count = new List<int>();
            for (int i = 0; i < n; ++i)
            {
                if (!xbins.Contains(pArray[t - 1].array[i].xBin2))
                {
                    xbins.Add(pArray[t - 1].array[i].xBin2);
                    indeces.Add(i);
                    count.Add(1);
                }
                else
                {
                    int index = xbins.IndexOf(pArray[t - 1].array[i].xBin2);
                    count[index]++;
                }
            }
            QuickSort(indeces, count, xbins, 0, xbins.Count - 1);
            for (int i = 0; i < xbins.Count; ++i)
            {
                /*
                int xint = Convert.ToInt32(xbins[i], 2);
                double xreal = Math.Round((xint * (b - a)) / (Math.Pow(2, l)) + a, (int)((-1) * Math.Log10(d)));
                double f = ((xreal % 1) * (Math.Cos(20 * Math.PI * xreal) - Math.Sin(xreal)));
                this.dataGridView2.Rows.Add(i + 1, xreal, xbins[i], f, (double)count[i]/t);
                */
                this.dataGridView2.Rows.Add(i + 1, pArray[t - 1].array[indeces[i]].xReal3, /*pArray[t - 1].array[indeces[i]].xBin2*/xbins[i], pArray[t - 1].array[indeces[i]].y2, (double)count[i] * 100 / n);
            }
        }
    }
}