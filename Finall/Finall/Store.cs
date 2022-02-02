using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    internal class Store
    {
        public string Name { get; set; }

        public float Rating { get; set; }

        public double Money { get; set; }

        public Stand[] Stands { get; set; }

        public string[] Workers { get; set; }
        public Store()
        {

        }

        public Store(string name, float rating, double money, Stand[] stands, string[] workers)
        {
            Name=name;
            Rating=rating;
            Money=money;
            Stands=stands;
            Workers=workers;
        }
    }
}
