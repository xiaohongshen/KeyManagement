using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace KeyManagment.Models
{
    public class EntryData 
    {
        public string EntryId { get; set; }
        public Item EntryItem { get; set; }
        public EntryData()
        {
            EntryItem = new Item();
        }
    }

    public class Item 
    {
        private string pw;

        public string NameofApplication { get; set; }

        public string UserName { get; set; }

        public string PW { get; set; }

        public string Date { get; set; }

        public override bool Equals(Object obj)
        {
            return (obj is Item) && ((Item)obj).Date == Date;
        }
    }
}