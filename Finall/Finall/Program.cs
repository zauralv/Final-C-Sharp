using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Final
{
    static class ShuffleExt
    {
        private static Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }

    public class Program
    {
        static Random random = new Random();

        static Spoil spoil = new Spoil();

        static int dayCount = 1;

        static double income = 0;
        static double totalIncome = 0;

        static void Main(string[] args)
        {
            //Timer timer = new Timer();
            //timer.Elapsed +=Timer_Elapsed;

            #region Initializing

            Stopwatch sw = Stopwatch.StartNew();

            //Spoil spoil = new Spoil();

            string[] conditions = new string[] { "new", "normal", "rotten", "toxic" };
            string[] vegetableNames = new string[] { "tomato", "potato", "onion", "pepper" };
            double[] prices = new double[] { 5d, 3d, 4d, 3.5d };

            List<Buyer> buyersList = new List<Buyer>();
            Queue<Buyer> buyers = new Queue<Buyer>();
            buyers.Enqueue(new Buyer("Javidan", vegetableNames[random.Next(0, 4)]));
            buyers.Enqueue(new Buyer("Ibrahim", vegetableNames[random.Next(0, 4)]));
            buyers.Enqueue(new Buyer("Kamil", vegetableNames[random.Next(0, 4)]));
            buyers.Enqueue(new Buyer("Kamal", vegetableNames[random.Next(0, 4)]));
            buyers.Enqueue(new Buyer("John", vegetableNames[random.Next(0, 4)]));
            buyers.Enqueue(new Buyer("Melikmemmed", vegetableNames[random.Next(0, 4)]));
            buyers.Enqueue(new Buyer("Ehmed", vegetableNames[random.Next(0, 4)]));
            buyers.Enqueue(new Buyer("Eli", vegetableNames[random.Next(0, 4)]));

            Stack<Vegetable> tomatoes = new Stack<Vegetable>();
            Stack<Vegetable> potatoes = new Stack<Vegetable>();
            Stack<Vegetable> onions = new Stack<Vegetable>();
            Stack<Vegetable> peppers = new Stack<Vegetable>();

            for (int i = 0; i < 10; i++)
            {
                tomatoes.Push(new Vegetable(vegetableNames[0], "new", 5));
                potatoes.Push(new Vegetable(vegetableNames[1], "new", 3));
                onions.Push(new Vegetable(vegetableNames[2], "new", 4));
                peppers.Push(new Vegetable(vegetableNames[3], "new", 3.5));
            }

            foreach (var tomato in tomatoes)
            {
                spoil.SpoilEvent += tomato.Spoil;
            }
            foreach (var potato in potatoes)
            {
                spoil.SpoilEvent += potato.Spoil;
            }
            foreach (var onion in onions)
            {
                spoil.SpoilEvent += onion.Spoil;
            }
            foreach (var pepper in peppers)
            {
                spoil.SpoilEvent += pepper.Spoil;
            }

            Dictionary<string, Stack<Vegetable>> vegetables = new Dictionary<string, Stack<Vegetable>>();
            vegetables.Add("tomato", tomatoes);
            vegetables.Add("potato", potatoes);
            vegetables.Add("onion", onions);
            vegetables.Add("pepper", peppers);


            Stand stand1 = new Stand(vegetables);

            Stand[] stands = new Stand[] { stand1 };

            string[] workers = new string[] { "worker1", "worker2" };

            Store store = new Store("Ibrahim dayinin bazari", 4.0f, 150, stands, workers);

            sw.Start();

            #endregion

            using var fs = new FileStream("Report.txt", FileMode.Create);
            fs.Close();
            fs.Dispose();

            while (true)
            {
            start:
                Console.Clear();
                if (sw.ElapsedMilliseconds >= 24000)
                {
                    store.Rating *= 1.2f;

                    PrintStand(stand1, vegetableNames);

                    spoil.SpoilVegetable();

                    #region addVegetables

                    Dictionary<double, int> newVegPrices = new Dictionary<double, int>();

                    int index = 0;
                    foreach (var stand in store.Stands)
                    {
                        foreach (var vegetable in stand.AllVegetables)
                        {
                            newVegPrices.Add(prices[index++], 10 - vegetable.Value.Count);
                        }
                    }

                    index = 0;
                    foreach (var vegPrice in newVegPrices)
                    {
                        if (vegPrice.Key * vegPrice.Value < store.Money)
                        {
                            for (int i = 0; i < vegPrice.Value; i++)
                            {
                                Vegetable v = new Vegetable(vegetableNames[index], "new", vegPrice.Key);
                                spoil.SpoilEvent += v.Spoil;
                                store.Stands[0].AllVegetables[vegetableNames[index]].Push(v);
                            }
                            store.Money -= vegPrice.Value * vegPrice.Key;
                        }
                        else
                        {
                            break;
                        }
                        index++;
                    }

                    #endregion

                    //workers clean rotten vegetables
                    double workerExp = CleanRottens(store);

                    //daily report
                    Console.WriteLine($"day {dayCount} just finished\n\nToday's income {String.Format("{0:0.00}", totalIncome)}\nBudget {String.Format("{0:0.00}", store.Money)}\nRating {String.Format("{0:0.0}", store.Rating)}\nExpenses for workers {workerExp}\n");

                    using var fss = new FileStream("Report.txt", FileMode.Append);
                    using var strw = new StreamWriter(fss, Encoding.Unicode);
                    strw.WriteLine($"Day {dayCount++}\n\nToday's income {String.Format("{0:0.00}", totalIncome)}\nBudget {String.Format("{0:0.00}", store.Money)}\nRating {String.Format("{0:0.0}", store.Rating)}\nExpenses for workers {workerExp}\n");
                    totalIncome = 0;

                    Console.WriteLine("press Enter to continue or Escape to exit the application\n");
                    ConsoleKeyInfo choice = Console.ReadKey();

                    //serializing
                    if (choice.Key == ConsoleKey.Escape)
                    {
                        var str = JsonConvert.SerializeObject(store.Stands[0].AllVegetables, Formatting.Indented);
                        File.WriteAllText("Report.json",str);

                        break;
                    }

                    //new day timer
                    sw.Restart();
                    Console.Clear();
                }


                //Buyer choose process
                foreach (var stand in store.Stands)
                {

                    PrintStand(stand, vegetableNames);

                    Console.WriteLine("buyer is choosing product...");

                    Thread.Sleep(1500);

                    try
                    {
                        BuyerChoice(buyers, buyersList, vegetableNames, stand, store, ref income);
                        income = 0;
                        Thread.Sleep(1500);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        store.Rating *= 0.9f;
                        goto start;
                    }
                }


            }

        }

        private static void PrintStand(Stand stand, string[] vegetableNames)
        {
            foreach (var vegName in vegetableNames)
            {
                Console.Write(vegName + "\t\t\t");
            }
            Console.WriteLine("\n");
            int max = stand.AllVegetables.OrderByDescending(x => x.Value.Count).First().Value.Count;
            for (int i = 0; i < max; i++)
            {
                foreach (var item in stand.AllVegetables)
                {
                    if (item.Value.Count > i)
                        Console.Write(item.Value.ToArray()[i] + "\t\t");
                    else
                        Console.Write("\t\t\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void BuyerChoice(Queue<Buyer> buyers, List<Buyer> buyersList, string[] vegetableNames, Stand stand, Store store, ref double income)
        {
            if (buyers.Count == 0)
            {
                buyersList.Shuffle();
                foreach (var buyer in buyersList)
                {
                    buyers.Enqueue(buyer);
                }
                buyersList.Clear();
            }


            Buyer b = buyers.Dequeue();
            buyersList.Add(b);

            int vegNum = random.Next(vegetableNames.Length);
            int vegCh = random.Next(stand.AllVegetables[vegetableNames[vegNum]].Count);

            Vegetable v = stand.AllVegetables[vegetableNames[vegNum]].ToArray()[vegCh];

            List<Vegetable> vegList = stand.AllVegetables[v.Name].ToList();

            int randAmount = random.Next(1, 4);

            income += v.Price * 1.3 * randAmount;
            store.Money += income;
            totalIncome += income;

            //Console.WriteLine(store.Money +" "+ income);

            if (vegList.Count(x => x.Name == v.Name && x.Price == v.Price) < randAmount)
            {
                throw new Exception($"sorry {b.Name}, we don't have {randAmount} {v.Name + " " + v.Condition} :(");
            }

            for (int i = 0; i < randAmount; i++)
            {
                foreach (var item in vegList)
                {
                    if (item.Condition == v.Condition && item.Name == v.Name)
                    {
                        vegList.Remove(item);
                        break;
                    }
                }
            }

            Console.WriteLine($"\n{b.Name} bought {randAmount} of {v}\n");

            stand.AllVegetables[vegetableNames[vegNum]].Clear();

            for (int i = vegList.Count - 1; i >= 0; i--)
            {
                stand.AllVegetables[vegetableNames[vegNum]].Push(vegList[i]);
            }
        }

        private static double CleanRottens(Store store)
        {
            double payAmount = 0;
            foreach (var stand in store.Stands)
            {
                foreach (var vegStack in stand.AllVegetables.Values)
                {
                    if (vegStack.Count != 0 && vegStack.Last().Condition == "rotten")
                    {
                        List<Vegetable> vegList = vegStack.ToList();
                        payAmount += vegList.Count(x => x.Condition == "rotten") * 1.5;
                        vegList.RemoveAll(x => x.Condition == "rotten");
                        stand.AllVegetables[vegStack.First().Name].Clear();
                        for (int i = vegList.Count - 1; i >= 0; i--)
                        {
                            stand.AllVegetables[vegList[vegList.Count - 1 - i].Name].Push(vegList[i]);
                        }
                    }
                }
            }
            store.Money -= payAmount;

            return payAmount;
        }
    }

}

