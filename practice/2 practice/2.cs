using System;
     
public class Program
{
 public static void Main()
	{
		Console.WriteLine("Введите координаты точек (x1, y1)");
		double x1 = Convert.ToDouble(Console.ReadLine());
		double y1 = Convert.ToDouble(Console.ReadLine());
		Console.WriteLine("Введите координаты точек (x2, y2)");
		double x2 = Convert.ToDouble(Console.ReadLine());
		double y2 = Convert.ToDouble(Console.ReadLine());  
		Console.WriteLine("Введите координаты точек (x3, y3)");
		double x3 = Convert.ToDouble(Console.ReadLine());
		double y3 = Convert.ToDouble(Console.ReadLine());
		
		double val1 = (x2-x1)*(y3-y1);
		double val2 = (x3-x1)*(y2-y1);
		
		if (val1 == val2) {
			Console.WriteLine("Лежат");
		}
		else {
			Console.WriteLine("Не лежат");
		}
	}
}