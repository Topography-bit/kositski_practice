using System;
     
public class Program
{
 public static void Main()
    {
        Console.WriteLine("Введите число E: ");
        double e = Convert.ToDouble(Console.ReadLine());
        
        double a1 = 1;
        double a2 = 2;
        double an = 0;
        
        Console.WriteLine("Seq: ");
        Console.WriteLine(a1);
        Console.WriteLine(a2);
        
        while (Math.Abs(a2-a1) >= e) {
            an = (a1 + a2) / 2;
            
            Console.WriteLine(an);
            a1 = a2;
            a2 = an;
        }
    }
}