using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Final
{
    internal class Vegetable : ICloneable
    {

        public string Name { get; set; }

        public string Condition { get; set; }

        public double Price { get; set; }

        public Vegetable()
        {

        }

        public Vegetable(string name, string condition, double price)
        {
            Name=name;
            Condition=condition;
            Price=price;
        }

        public void Spoil()
        {
            if (Condition == "new")
                Condition = "normal";
            else if (Condition == "normal")
                Condition = "rotten";
            else if (Condition == "rotten")
                Condition = "toxic";
            Price *= 0.9;
        }

        public override string ToString()
        {
            return Name + " " + Condition;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
    class Spoil
    {
        public delegate void SpoilDel();
        public event SpoilDel SpoilEvent;

        public void SpoilVegetable()
        {
            SpoilEvent.Invoke();
        }
    }
}

