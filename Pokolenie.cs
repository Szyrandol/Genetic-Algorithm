using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace Algorytm_Genetyczny
{
    internal class Pokolenie
    {
        public Pokolenie(int lower, int upper, int exp, int n, double precision) // kontruktor pokolenia poczatkowego
        {
            length = n;
            Random rnd = new Random();
            array = new Osobnik[n];
            for (int i = 0; i < n; ++i)                     //populate
                array[i] = new Osobnik(lower, upper, exp, precision, rnd.NextDouble());
        }
        public Pokolenie(Pokolenie prev, int n) // konstruktor na podstawie poprzedniego pokolenia
        {
            length = n;
            array = new Osobnik[n];
            for(int i = 0; i < n; ++i)
                array[i] = prev.array[i];
        }
        public int length;
        public Osobnik[] array;
        public double minF;
        public double avgF;
        public int elita;
        public void MinF1()
        {
            this.minF = this.array[0].y;
            for (int i = 0; i < length; ++i)
            {
                if (this.minF > this.array[i].y)
                    this.minF = this.array[i].y;
            }
        }
        public void AvgF1()
        {
            double sum = 0;
            for (int i = 0; i < length; ++i)
                sum += this.array[i].y;
            this.avgF = sum / length;
        }
        public void Elita1()
        {
            double currMaxF = this.array[0].y;
            int index = 0;
            for (int i = 0; i < this.array.Length; ++i)
            {
                if (this.array[i].y > currMaxF)
                    index = i;
            }
            this.elita = index;
        }
        public void MinF2()
        {
            this.minF = this.array[0].y2;
            for(int i = 0; i < length; ++i)
            {
                if (this.minF > this.array[i].y2)
                    this.minF = this.array[i].y2;
            }
        }
        public void AvgF2()
        {
            double sum = 0;
            for(int i = 0 ; i < this.array.Length; ++i)
                sum += this.array[i].y2;
            this.avgF = sum / this.array.Length;
        }
        public void Elita2()
        {
            double currMaxF = this.array[0].y2;
            int index = 0;
            for(int i = 0; i < this.array.Length; ++i)
            {
                if (this.array[i].y2 > currMaxF)
                {
                    index = i;
                    currMaxF = this.array[i].y2;
                }
            }
            this.elita = index;
        }
        public void Dystrybuanta() // arg: string dir
        { // i ewaluacja / g(x)
            double minF = this.array[0].y;
            double maxF = this.array[0].y;
            double fitnessValSum = 0;
            for (int i = 0; i < this.array.Length; ++i)
            {
                if (this.array[i].y < minF) minF = this.array[i].y;
                if (this.array[i].y > maxF) maxF = this.array[i].y;
            }
            for (int i = 0; i < this.array.Length; ++i)
            {
                this.array[i].fitnessVal = this.array[i].y - minF + this.array[i].d;
                fitnessValSum += this.array[i].fitnessVal;
            }
            this.array[0].dist = this.array[0].fitnessVal / fitnessValSum;
            this.array[0].prob = this.array[0].fitnessVal / fitnessValSum;
            for (int i = 1; i < this.array.Length; ++i)
            {
                this.array[i].prob = this.array[i].fitnessVal / fitnessValSum;
                this.array[i].dist = this.array[i].fitnessVal / fitnessValSum + this.array[i - 1].dist;
            }
        }
        public static void Selekcja(Pokolenie pokolenie1, Pokolenie pokolenie2, int n)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < n; ++i)
            {
                double rnd = rand.NextDouble();
                int parentIndex = 0;
                for (int j = 1; j < pokolenie1.array.Length; j++)
                {
                    if (pokolenie1.array[j - 1].dist < rnd && rnd <= pokolenie1.array[j].dist)
                        parentIndex = j;
                }
                // parentIndex = Osobnik.Selekcja(pokolenie);
                pokolenie2.array[i] = new Osobnik(pokolenie1.array[parentIndex]);
                pokolenie1.array[i].r = rnd;
            }
        }
        public void EnsureElite(Osobnik elita, int elitaIndeks)
        {
            double g = this.array[0].fitnessVal;

            if (this.array[elitaIndeks] != elita)
            {
                if(this.array[elitaIndeks].y2 < elita.y2) this.array[elitaIndeks] = elita;
                else
                {
                    for (int i = 0; i < this.length; ++i)
                    {
                        if (this.array[i].y2 < elita.y2)
                        {
                            this.array[i] = elita;
                            break;
                        }
                    }
                }
            }
        }
        /*
        */
        public void Losowanie(double pk)
        { // decyduje czy osobnik jest rodzicem 
            int parentCount = 0;
            Random rnd = new Random();
            while (parentCount < 2)
            {
                for (int i = 0; i < this.array.Length; ++i)
                {
                    double x = rnd.NextDouble();
                    if (x <= pk)
                    {
                        this.array[i].isParent = true;
                        parentCount++;
                    }
                }
            }
        }
        public List<int> Parowanie()
        { // przypisuje pary rodzicom, zwraca liste indeksow rodzicow
            List<int> list = new List<int>();
            int prevParent = -1;
            int firstParent = 0;
            for (int i = 0; i < this.array.Length; ++i)
            {
                if (this.array[i].isParent)
                {
                    list.Add(i);
                    if (prevParent == -1) prevParent = i;
                    else
                    {
                        this.array[i].para = prevParent;
                        this.array[prevParent].para = i;
                        prevParent = -1;
                    }
                }
            }
            if (prevParent != -1)
            {
                this.array[prevParent].para = firstParent;
            }
            return list;
        }
        public void PunktCiecia(List<int> list)
        { // wybiera punkt ciecia dla pary
            int l = this.array[0].l;
            for (int i = 0; i < list.Count; i += 2)
            {
                Random rand = new Random(Guid.NewGuid().GetHashCode());
                if (i + 2 <= list.Count) this.array[list[i]].punktCiecia =  this.array[list[i + 1]].punktCiecia = rand.Next(l - 2);
                else if (i + 1 <= list.Count) this.array[i].punktCiecia = rand.Next(l - 2);
            }
        }
        public void Krzyzowanie(List<int> list)
        {
            for(int i = 0; i < list.Count; ++i)
            {
                if (i + 2 <= list.Count)
                {
                    string tmp = this.array[list[i]].xBin;
                    string tmp2 = this.array[list[i + 1]].xBin;
                    this.array[list[i]].xBin = Osobnik.Krzyzowanie(this.array[list[i]], tmp2);
                    this.array[list[i + 1]].xBin = Osobnik.Krzyzowanie(this.array[list[i + 1]], tmp);
                }
                else if (i + 1 <= list.Count)
                    this.array[list[i]].xBin = Osobnik.Krzyzowanie(this.array[list[i]], this.array[list[0]].xBin);// drugi argument osobnik -> string
            }
        }
        public void Mutacja(double pm)
        {
            for (int i = 0; i < this.array.Length; ++i)
            {
                this.array[i].xBin2 = Osobnik.Mutacja(this.array[i], pm);
                StringBuilder tmp1 = new StringBuilder(this.array[i].xBin);
                StringBuilder tmp2 = new StringBuilder(this.array[i].xBin2);
                StringBuilder final = new StringBuilder();
                for (int j = 0; j < this.array[0].l; ++j)
                {
                    if (tmp1[j] != tmp2[j])
                    {
                        final.Append(j + 1);
                        if (j + 1 != this.array[0].l) final.Append(", ");
                    }
                }
                if (final.Length > 0) final.Remove(final.Length - 2, 2);
                this.array[i].mutatedStr = final.ToString();
            }
        }
    }
}
