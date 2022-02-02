using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final
{
    internal class Buyer
    {
        public string Name { get; set; }

        public string WantsToBuy { get; set; }

        public Buyer()
        {

        }

        public Buyer(string name, string wantsToBuy)
        {
            Name=name;
            WantsToBuy=wantsToBuy;
        }
    }
}
