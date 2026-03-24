using System;
     
namespace Exceptions
{
    class Program
    {
     static void Main()
    	{
    		Console.WriteLine("Введите a:");
    		double a = Convert.ToDouble(Console.ReadLine());
    		Console.WriteLine("Введите b:");
    		double b = Convert.ToDouble(Console.ReadLine());
    		Console.WriteLine("Введите шаг h:");
    		double h = Convert.ToDouble(Console.ReadLine());

    		Console.WriteLine("{0,12} {1,15}", "x", "F(x)");
    		Console.WriteLine(new string('-', 30));

    		for (double x = a; x <= b; x += h)
    		{
    			try
    			{
    				double sinVal = Math.Sin(3 * x);
    				if (Math.Abs(sinVal) < 1e-10)
    					throw new DivideByZeroException();
    				if (Math.Abs(x) < 1e-10)
    					throw new DivideByZeroException();

    				double res = 1.0 / (x * sinVal * sinVal);
    				Console.WriteLine("{0,12:f4} {1,15:f4}", x, res);
    			}
    			catch (DivideByZeroException)
    			{
    				Console.WriteLine("{0,12:f4} {1,15}", x, "не определено");
    			}
    			catch (Exception ex) {
    				Console.WriteLine("Ошибка: " + ex.Message);
    			}
    		}

    		Console.WriteLine();
    		Console.WriteLine("Вычисления закончены");
    		Console.ReadKey();
    	}
    }
}
