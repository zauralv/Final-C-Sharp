using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    internal class Stand
    {
        public string Name { get; set; }

        public Dictionary<string, Stack<Vegetable>> AllVegetables { get; set; }

        public Stand()
        {

        }

        public Stand(Dictionary<string, Stack<Vegetable>> allVegetables)
        {
            AllVegetables=allVegetables;
            Name = allVegetables.First().Key;
        }

        public void AddVegetable(Vegetable v)
        {
            AllVegetables[v.Name].Push(v);
        }

        public void BuyVegetable()
        {

        }
    }
}
