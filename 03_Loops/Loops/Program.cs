using System;

namespace Loops
{
    class Program
    {
     static void Main()
        {
            Console.WriteLine("Введите число E: ");
            double e = Convert.ToDouble(Console.ReadLine());
            
            double a1 = 1;
            double a2 = 2;
            double an = 0;
            int n = 2;
            
            Console.WriteLine("Последовательность: ");
            Console.WriteLine(a1);
            Console.WriteLine(a2);
            
            while (Math.Abs(a2-a1) >= e) {
                an = (a1 + a2) / 2;
                n++;
                
                Console.WriteLine(an);
                a1 = a2;
                a2 = an;
            }

            Console.WriteLine();
            Console.WriteLine($"Наименьший номер: {n}");
            
            Console.ReadKey();
        }
    }
}
