using System;
using System.Threading.Tasks;

namespace KeyManagment.Models
{
    public class Item
    {
       // public string Id { get; set; }

        public string NameofApplication { get; set; }

        public string PW { get; set; }

        public string Date { get; set; }

        public override bool Equals(Object obj)
        {
            return (obj is Item) && ((Item)obj).Date == Date;
        }
    }
}