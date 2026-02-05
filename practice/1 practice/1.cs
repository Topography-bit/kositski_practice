using System;
     
public class Program
{
	public static void Main() 
    { 
		Console.WriteLine("Введите x"); 
		double x = Convert.ToDouble(Console.ReadLine());
		double res = 2 * (1 / Math.Tan(3 * x)) - (Math.Log(Math.Cos(x)) / Math.Log(1+x*x));
        Console.WriteLine(res);
   } 
}