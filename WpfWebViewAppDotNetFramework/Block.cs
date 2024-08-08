using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WpfWebViewApp
{
    [ComVisible(true)]
    [Guid("7B04036C-B842-4D13-9B4A-2A9A39AC2C5A")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Block : IBlock
    {
        // Properties
        public int NumberOfLots { get; set; }
        public List<int> LotIds { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }

        // Constructor to initialize with random values
        public Block(Random random)
        {
            NumberOfLots = random.Next(1, 101); // Random number of lots between 1 and 100
            LotIds = new List<int>();
            for (int i = 0; i < NumberOfLots; i++)
            {
                LotIds.Add(random.Next(1000, 10000)); // Random lot IDs between 1000 and 9999
            }
            Name = GenerateRandomString(random, 10); // Random name of 10 characters
            Id = random.Next(10000, 99999); // Random ID between 10000 and 99999
        }

        // Method to generate a random string of a given length
        private string GenerateRandomString(Random random, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            return new string(stringChars);
        }

        // Method to return lot IDs as a comma-separated string
        public string GetLotIds()
        {
            return string.Join(", ", LotIds);
        }
    }
}
