using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorytm_Genetyczny
{
    internal class Osobnik
    {
        public Osobnik(int lower, int upper, int exp, double precision, double randomDouble)
        {   // konstruktor dla osobników pierwszego pokolenia
            a = lower;
            b = upper;
            l = exp;
            d = precision;
            rand = randomDouble;
            xReal1 = Newreal(a, b, d, rand);
            xInt1 = Realtoint(a, b, l, xReal1);
            xBin = Inttobin(xInt1, l);
            xInt2 = Bintoint(xBin);
            xReal2 = Inttoreal(a, b, xInt2, l, d);
            y = ((xReal2 % 1) * (Math.Cos(20 * Math.PI * xReal2) - Math.Sin(xReal2)));
        }
        public Osobnik(Osobnik prev)
        {   //kontruktor osobników pokoleń wtórnych
            a = prev.a;
            b = prev.b;
            l = prev.l;
            d = prev.d;
            rand = prev.rand;
            if (prev.xReal3 == 0) prev.xReal3 = prev.xReal2;
            xReal1 = prev.xReal3;
            xInt1 = Realtoint(a, b, l, xReal1);
            xBin = Inttobin(xInt1, l);
            xInt2 = Bintoint(xBin);
            xReal2 = Inttoreal(a, b, xInt2, l, d);
            y = ((xReal2 % 1) * (Math.Cos(20 * Math.PI * xReal2) - Math.Sin(xReal2)));
        }
        public Osobnik(int lower, int upper, string bin, int exp, double precision) { // od xbina
            a = lower;
            b = upper;
            l = exp;
            d = precision;
            xInt2 = Bintoint(bin);
            xReal2 = Inttoreal(a, b, xInt2, l, d);
            y = ((xReal2 % 1) * (Math.Cos(20 * Math.PI * xReal2) - Math.Sin(xReal2)));
        }
        public int a;
        public int b;
        public int l;
        public double d;
        public double rand;

        public double xReal1;
        public int xInt1;
        public string xBin;
        public double xReal2;
        public int xInt2;
        public double y;

        public double r;
        public double fitnessVal;
        public double prob;
        public double dist;
        public bool isParent;
        public int para;
        public int punktCiecia;
        //public List<int> mutated = new List<int> { };
        public string mutatedStr;
        public string xBin2;
        public int xInt3;
        public double xReal3;
        public double y2;
        private static double Newreal(int a, int b, double d, double rand) {
            return Math.Round(rand * (b - a) + a, (int)((-1) * Math.Log10(d)));
        }
        private static int Realtoint(int a, int b, int l, double xreal) {
            return (int)((1 / (double)(b - a)) * (xreal - a) * (Math.Pow(2, l) - 1));
        }
        private static string Inttobin(int xint, int l) {
            return Convert.ToString(xint, 2).PadLeft(l, '0');
        }
        private static int Bintoint(string xbin) {
            return Convert.ToInt32(xbin, 2);
        }
        private static double Inttoreal(int a, int b, int xint, int l, double d) {
            return Math.Round((xint * (b - a)) / (Math.Pow(2, l)) + a, (int)((-1) * Math.Log10(d)));
        }
        public static void FromBinCalcIntRealY(Osobnik osobnik)
        {
            osobnik.xInt3 = Bintoint(osobnik.xBin2);
            osobnik.xReal3 = Inttoreal(osobnik.a, osobnik.b, osobnik.xInt3, osobnik.l, osobnik.d);
            osobnik.y2 = ((osobnik.xReal3 % 1) * (Math.Cos(20 * Math.PI * osobnik.xReal3) - Math.Sin(osobnik.xReal3)));
        }
        public static string Krzyzowanie(Osobnik parent1, Osobnik parent2) { // tylko jeden parent ma inny przypadek
            string kidBin;
            int pc = parent1.punktCiecia;
            StringBuilder sb1 = new StringBuilder(parent1.xBin);
            StringBuilder sb2 = new StringBuilder(parent2.xBin);
            sb1.Remove(pc + 1, parent1.l - pc - 1);
            sb2.Remove(0, pc + 1);
            sb1.Append(sb2);
            kidBin = sb1.ToString();
            return kidBin;
        }
        public static string Krzyzowanie(Osobnik parent1, string str)
        {
            string kidBin;
            int pc = parent1.punktCiecia;
            StringBuilder sb1 = new StringBuilder(parent1.xBin);
            StringBuilder sb2 = new StringBuilder(str);
            sb1.Remove(pc, parent1.l - pc);
            sb2.Remove(0, pc);
            sb1.Append(sb2);
            kidBin = sb1.ToString();
            return kidBin;
        }

        public static string Mutacja(Osobnik osobnik, double pm) { // indeksy zmutowane
            StringBuilder sb = new StringBuilder(osobnik.xBin);
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            for(int i = 0; i < osobnik.l; ++i) {
                double r = rand.NextDouble();
                if(r < pm) {
                    if (sb[i] == '0') sb[i] = '1';
                    else if (sb[i] == '1') sb[i] = '0';
                }
            }
            return sb.ToString();
        }
    }
}

