using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WycinarkaTwarzy.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            var files = 
                Directory.GetFiles(@"D:\ZDJECIA CD_ARCHIVE\ZDJECIA\CD Zdjecia\12\UK", 
                    "*.jpg", SearchOption.AllDirectories);

            var wycinarka = new Wycinarka();

            foreach(var file in files)
            {
                System.Console.WriteLine($"{file}");
                wycinarka.DetectMultiScale(file);
            }


        }
    }
}
