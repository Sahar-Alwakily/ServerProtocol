using System;

namespace CoincheServer
{
    public class Card
    {
        public Card(char t, char n, int p)
        {
            Type = t;
            Number = n;
            Power = p;
        }

        public char Type { get; set; }

        public char Number { get; set; }

        public int Power { get; set; }

        public void Info()
        {
            Console.WriteLine("Power: " + Power + " Type: " + Type + " Number: " + Number);
        }

    }
}
