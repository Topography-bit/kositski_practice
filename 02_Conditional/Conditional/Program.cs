using System;
     
namespace Conditional
{
    class Program
    {
     static void Main()
    	{
    		Console.WriteLine("Введите x1, y1:");
    		double x1 = Convert.ToDouble(Console.ReadLine());
    		double y1 = Convert.ToDouble(Console.ReadLine());
    		
    		Console.WriteLine("Введите x2, y2:");
    		double x2 = Convert.ToDouble(Console.ReadLine());
    		double y2 = Convert.ToDouble(Console.ReadLine());

    		Console.WriteLine("Введите x3, y3:");
    		double x3 = Convert.ToDouble(Console.ReadLine());
    		double y3 = Convert.ToDouble(Console.ReadLine());

    		double s = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);
    		
    		if (Math.Abs(s) < 1e-9)
    			Console.WriteLine("Точки лежат на одной прямой");
    		else
    			Console.WriteLine("Точки НЕ лежат на одной прямой");
    		
    		Console.ReadKey();
    	}
    }
}
