using System;
     
public class Program
{
 public static void Main()
	{
		Console.WriteLine("Введите x");
		double x = Convert.ToDouble(Console.ReadLine());
		double res;
		if (x >= 0 && x <= 1) {
			res = x*x - x;
		}
		else {
			res = x*x - Math.Sin(Math.PI * x * x);
		}
	 
		Console.WriteLine(res);
	}
}