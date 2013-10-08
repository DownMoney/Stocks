using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Stocks
{
    class Program
    {
        struct Day
        {
            public DateTime date;
            public Stock[] stock;
        }

        struct Pair
        {
            public double days;
            public double cci;
        }

        struct Stock
        {
            public string name;
            public string symbol;
            public DateTime date;
            public double open;
            public double close;
            public double high;
            public double low;
            public double vol;
            public double cci;
            public double tp;
            public double predicted;
            public double diff;
        }

        static int upper = 100;
        static int lower = -100;

        static bool _cci = false;
        static Day[] days = new Day[0];
      // static Stock[] stocks = new Stock[0];

        static void error(string s)
        {
           Console.ForegroundColor = ConsoleColor.Yellow;
           Console.WriteLine("\n" + s);
           Console.ForegroundColor = ConsoleColor.Gray;
        }

        static void addStock(string[] s, ref Stock[] _stocks)
        {
            try
            {
                Array.Resize<Stock>(ref _stocks, _stocks.Length + 1);
                _stocks[_stocks.Length - 1].name = s[0];
                _stocks[_stocks.Length - 1].symbol = s[1];
                DateTime d = new DateTime(Convert.ToInt32(s[2].Substring(0, 4)), Convert.ToInt32(s[2].Substring(4, 2)), Convert.ToInt32(s[2].Substring(6)));
                _stocks[_stocks.Length - 1].date = d;
                _stocks[_stocks.Length - 1].open = Convert.ToDouble(s[3]);
                _stocks[_stocks.Length - 1].high = Convert.ToDouble(s[4]);
                _stocks[_stocks.Length - 1].low = Convert.ToDouble(s[5]);
                _stocks[_stocks.Length - 1].close = Convert.ToDouble(s[6]);
                _stocks[_stocks.Length - 1].vol = Convert.ToDouble(s[7]);
                _stocks[_stocks.Length - 1].tp = (_stocks[_stocks.Length - 1].high + _stocks[_stocks.Length - 1].low + _stocks[_stocks.Length - 1].close) / 3;
            }
            catch (Exception ex)
            {                
                error(ex.Message);
            }
        }

        static void showStock(int date)
        {
            Console.Write("\nsymbol>");
            string symbol = Console.ReadLine();
            startShowStock(date, symbol);
        }

        static void startShowStock(int date, string symbol)
        {
            int i = getIndex(symbol, days[date].stock);
            if (i != -1)
            {
                Console.WriteLine("\nName | Symbol | Open | Close | High | Low | Vol | TP | CCI");
                if (days[date].stock[i].cci > upper)
                    Console.ForegroundColor = ConsoleColor.Green;
                if (days[date].stock[i].cci < lower)
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n" + days[date].stock[i].name + " | " + days[date].stock[i].symbol + " | " + days[date].stock[i].open.ToString() + " | " + days[date].stock[i].close.ToString() + " | " + days[date].stock[i].high.ToString() + " | " + days[date].stock[i].low.ToString() + " | " + days[date].stock[i].vol.ToString() + " | " + days[date].stock[i].tp.ToString() + " | " + days[date].stock[i].cci.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        static void openFile(string fileName)
        {
            Console.WriteLine("\nOpening File " + fileName);
            StreamReader file = new StreamReader(fileName);
            Array.Resize<Day>(ref days, days.Length + 1);
            days[days.Length - 1].stock = new Stock[0];
            file.ReadLine();
            while(!file.EndOfStream)
            {
                string[] s = file.ReadLine().Split(',');
                addStock(s,ref days[days.Length - 1].stock);
            }
            file.Close();
            days[days.Length - 1].date = days[days.Length - 1].stock[1].date;
            Console.WriteLine("\nOpened file");
            Console.WriteLine("\nAdded " + days[days.Length - 1].stock.Length.ToString() + " stocks");
        }

        static void loadFile()
        {
            Console.Write("\npath>");
            openFile(Console.ReadLine());
        }

        static void Main(string[] args)
        {
            Console.Write("Stock Analysis - Michael Lotkowski ®\n>");
            string input = "";
            while ((input = Console.ReadLine()) != "quit")
            {
                switch (input.ToLower())
                {
                    case "show stock":
                        {
                            showStock(selectDay());
                            break;
                        }
                    case "load file":
                        {
                            loadFile();
                            break;
                        }
                    case "calculate cci":
                        {
                            cci(ref days[selectDay()].stock);
                            break;
                        }
                    case "clear":
                        {
                            Console.Clear();
                            Console.Write("Stock Analysis - Michael Lotkowski ®");
                            break;
                        }
                    case "show stocks":
                        {
                            displayStocks(days[selectDay()].stock);
                            break;
                        }
                    case "set upper bound":
                        {
                            setUpper();
                            break;
                        }
                    case "set lower bound":
                        {
                            setLower();
                            break;
                        }
                    case "show to buy":
                        {
                            toBuy(days[selectDay()].stock);
                            break;
                        }
                    case "show to sell":
                        {
                            toSell(days[selectDay()].stock);
                            break;
                        }
                    case "calculate r":
                        {
                            normalization(days[selectDay()].stock);
                            break;
                        }
                    case "analyze":
                        {
                            chart();
                            break;
                        }
                    case "auto":
                        {
                            auto();
                            break;
                        }
                    case "extrapolate":
                        {
                            extraploation();
                            break;
                        }
                    case "predict":
                        {
                            predictedToBuy(days[selectDay()].stock);
                            break;
                        }

                }
                Console.Write("\n>");
            }
                       
        }

        static void auto()
        {
            Console.Write("\nfolder>");
            startAuto(Console.ReadLine());          
        }

        static void startAuto(string folder)
        {
            Console.WriteLine("\nStarted looking for files in " + folder);
            string[] s = Directory.GetFiles(folder);
            Console.WriteLine("\nFound " + s.Length.ToString() + " files");
            foreach (string file in s)
            {
                openFile(file);
                cci(ref days[days.Length - 1].stock);
            }
            Console.WriteLine("\nAdded and calculated " + s.Length.ToString() + " files!");
        }

        static int getIndex(string sym, Stock[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (sym.ToLower() == s[i].symbol.ToLower())
                    return i;
            }
            return -1;
        }

        static void chart()
        {
            Console.Write("\nsymbol>");
            string symbol = Console.ReadLine();
            Console.Write("\nfile name>");
            string fileName = Console.ReadLine();
            Analyze(symbol, fileName);
        }

        static void Analyze(string s, string fileName)
        {
            StreamWriter file = new StreamWriter(fileName);
            file.WriteLine("Symbol, CCI, Date");
            for (int i = 0; i < days.Length; i++)
            {
                int index = getIndex(s, days[i].stock);
                if (index != -1)
                {
                    file.WriteLine(days[i].stock[index].symbol + ", " + days[i].stock[index].cci + ", " + days[i].stock[index].date.ToShortDateString());
                }
            }
            file.Close();
        }

        static int selectDay()
        {
            for (int i = days.Length-1; i >=0; i--)
            {
                Console.WriteLine("\n" + i.ToString() + ". " + days[i].date.ToShortDateString());
            }
            Console.Write("\nselect date ["+(days.Length-1).ToString()+"]>");
            string input = Console.ReadLine();
            try
            {
                return Convert.ToInt32(input);
            }
            catch
            {
                return days.Length - 1;
            }
        }

        static void normalization(Stock[] s)
        {
            Stock r = getR(s);

            Console.WriteLine("\nOpen " + r.open);
            Console.WriteLine("\nClose " + r.close);
            Console.WriteLine("\nHigh " + r.high);
            Console.WriteLine("\nLow " + r.low);
            Console.WriteLine("\nVol " + r.vol);
        }

        static void predictedToBuy(Stock[] s)
        {
           
            Console.WriteLine("\nSymbol | Name | CCI | Predicted | Diff");
            int j = 0;
            for (int i = 0; i < s.Length; i++)
            {
                double d = startExtrapolate(s[i].symbol, days.Length);
                s[i].predicted = d;
                if ((d > s[i].cci)&&(d!=0))
                {
                    j++;
                    s[i].diff = d - s[i].cci;                   
                }
            }

            IEnumerable<Stock> predicted = s.OrderBy(val => val.diff);

            foreach (Stock val in predicted)
            {
                Console.WriteLine("\n" + val.symbol + " | " + val.name + " | " + val.cci.ToString() + " | " + val.predicted.ToString()+" | "+val.diff.ToString());
            }

            Console.WriteLine("\nIncrease " + j.ToString() + " out of " + s.Length.ToString());
        }

        static void toBuy(Stock[] s)
        {
            Stock[] tobuy = new Stock[0];
            if (_cci)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].cci > upper)
                    {
                        Array.Resize<Stock>(ref tobuy, tobuy.Length + 1);
                        tobuy[tobuy.Length - 1] = s[i];                      
                    }
                }
                IEnumerable<Stock> buy = tobuy.OrderBy(val => val.cci);
                buy = buy.Reverse<Stock>();
                
                foreach(Stock val in buy)
                {                     
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n" + val.name + " - " + val.symbol+" - " +val.cci.ToString());
                        Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            else
                error("The CCI has not been calculated or parameters has been changed! Recalculate CCI!");
            
        }

        static void extraploation()
        {
            Console.Write("\nsymbol>");
            string symbol = Console.ReadLine();
            Console.Write("\ndays ["+days.Length.ToString()+"]>");
            int d = 0;
            try
            {
                d = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                d = days.Length;
            }
            Console.WriteLine("\n>" + startExtrapolate(symbol, d).ToString());
        }

        static double startExtrapolate(string sym, int d)
        {
            DateTime start = new DateTime(9999,1,1);
            double all = 0;
            Pair[] pairs = new Pair[0];
            for (int i = 0; i < days.Length; i++)
            {
                int index = getIndex(sym, days[i].stock);
                if (index != -1)
                {
                    if (start > days[i].stock[index].date)
                        start = days[i].stock[index].date;
                    Array.Resize<Pair>(ref pairs, pairs.Length + 1);
                    pairs[pairs.Length - 1] = new Pair();
                    pairs[pairs.Length - 1].days = days[i].stock[index].date.Subtract(start).Days;
                    pairs[pairs.Length - 1].cci = days[i].stock[index].cci;
                }
            }

            for (int i = 0; i < pairs.Length; i++)
            {
                double l=1;
                for (int j = 0; j < pairs.Length; j++)
                {
                    if(j!=i)
                   // l += pairs[i].cci * ((d - pairs[i + 1].days) / (pairs[i].days - pairs[i + 1].days)) * ((d - pairs[i + 2].days) / (pairs[i].days - pairs[i + 2].days));
                        l*=((d-pairs[j].days)/(pairs[i].days-pairs[j].days));
                }
                l*=pairs[i].cci;
                all+=l;
            }

            return all;
        }


        static void toSell(Stock[] s)
        {
            Stock[] tosell = new Stock[0];
            if (_cci)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i].cci < lower)
                    {
                        Array.Resize<Stock>(ref tosell, tosell.Length + 1);
                        tosell[tosell.Length - 1] = s[i];
                    }
                }
                IEnumerable<Stock> sell = tosell.OrderBy(val => val.cci);
                sell = sell.Reverse<Stock>();

                foreach (Stock val in sell)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n" + val.name + " - " + val.symbol + " - " + val.cci.ToString());
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            else
                error("The CCI has not been calculated or parameters has been changed! Recalculate CCI!");
        }


        static void setUpper()
        {
            Console.Write("\nupper>");
            upper = Convert.ToInt32(Console.ReadLine());
            _cci = false;
        }

        static void setLower()
        {
            Console.Write("\nlower>");
            lower = Convert.ToInt32(Console.ReadLine());
            _cci = false;
        }


        static void displayStocks(Stock[] s)
        {
            Console.WriteLine("\nName | Symbol | Open | Close | High | Low | Vol | TP");

            for (int i = 0; i < s.Length; i++)
            {
                Console.WriteLine("\n" + s[i].name + " | " + s[i].symbol + " | " + s[i].open + " | " + s[i].close + " | " + s[i].high + " | " + s[i].low + " | " + s[i].vol + " | " + s[i].tp);
            }
        }

        static void cci(ref Stock[] s)
        {
            Console.WriteLine("\nInitializing calculation of CCI!");
            double a = 0;
            for (int i = 0; i < days.Length; i++)
            {
                a += sma(days[i].stock);
            }
            
            double std = stdDeviation(s);
            for (int i = 0; i < s.Length; i++)
            {
                s[i].cci = ((((s[i].high + s[i].low + s[i].close) / 3) - a) / std) * (1 / 0.015);
              /*  if (s[i].cci > 100)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n"+s[i].name+" - "+s[i].symbol);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                if (s[i].cci < -100)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n" + s[i].name + " - " + s[i].symbol);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }*/
            }

            _cci = true;
            Console.WriteLine("\nCCI calculated!");
        }

        static double sma(Stock[] s)
        {
            double d = 0;

            for (int i = 0; i < s.Length; i++)
            {
                d += s[i].tp;
            }

            d /= s.Length;

            return d;
        }

        static double stdDeviation(Stock[] s)
        {
            double _mean = sma(s);
            double count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                count += Math.Pow((s[i].tp - _mean), 2);
            }
            return Math.Pow(count / s.Length, 0.5);
        }



        static Stock getR(Stock[] s)
        {
            Stock cov = covariance(s);
            Stock std = stdDev(s);

            Stock count = new Stock();

            count.open = cov.open / std.open;
            count.close = cov.close / std.close;
            count.high = cov.high / std.high;
            count.low = cov.low / std.low;
            count.vol = cov.vol / std.vol;

            return count;
        }

        static Stock avg(Stock[] s)
        {
            Stock a = new Stock();
            a = sum(s);
            a.open /= s.Length;
            a.close /= s.Length;
            a.high /= s.Length;
            a.low /= s.Length;
            a.vol /= s.Length;

            return a;
        }

        static Stock covariance(Stock[] s)
        {
            Stock count = new Stock();
            Stock a = avg(s);

          /*  count.open = 1;
            count.close = 1;
            count.high = 1;
            count.low = 1;
            count.vol = 1;*/

            for (int i = 0; i < s.Length; i++)
            {
                count.open += (s[i].open - a.open);
                count.close += (s[i].close - a.close);
                count.high += (s[i].high - a.high);
                count.low += (s[i].low - a.low);
                count.vol += (s[i].vol - a.vol);
            }

            count.open /= s.Length;
            count.close /= s.Length;
            count.high /= s.Length;
            count.low /= s.Length;
            count.vol /= s.Length;

            return count;
        }

        static Stock stdDev(Stock[] s)
        {
            Stock count = variance(s);

            count.open = Math.Pow(count.open, 0.5);
            count.close = Math.Pow(count.close, 0.5);
            count.high = Math.Pow(count.high, 0.5);
            count.low = Math.Pow(count.low, 0.5);
            count.vol = Math.Pow(count.vol, 0.5);

            return count;
        }

        static Stock variance(Stock[] s)
        {
            Stock _mean = avg(s);
            Stock count = new Stock();
            for (int i = 0; i < s.Length; i++)
            {
                count.open += Math.Pow((s[i].open - _mean.open), 2);
                count.close += Math.Pow((s[i].close - _mean.close), 2);
                count.high += Math.Pow((s[i].high - _mean.high), 2);
                count.low += Math.Pow((s[i].low - _mean.low), 2);
                count.vol += Math.Pow((s[i].vol - _mean.vol), 2);
            }

            count.open /= s.Length;
            count.close /= s.Length;
            count.high /= s.Length;
            count.low /= s.Length;
            count.vol /= s.Length;

            return count;
        }

        static Stock sum(Stock[] d)
        {
            Stock count = new Stock();
            for (int i = 0; i < d.Length; i++)
            {
                count.open += d[i].open;
                count.close += d[i].close;
                count.high += d[i].high;
                count.low += d[i].low;
                count.vol += d[i].vol;
            }
            return count;
        }
    }
}
