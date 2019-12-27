using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Notice.Data.Core;
using System.Security.Cryptography;
using System.IO;
using Notice.Data.Core;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string hashedPassword = HashWithSlat.GenerateHash("1");

            //string hashedPassword = HashWithSlat.GenerateHash("P@ssw0rd");

            var result = HashWithSlat.VerifyHashedPassword(hashedPassword, "P@ssw0rd");

            Console.WriteLine(result);
        }
    }
}
