using System;
     
public class Program
{
	public static void Main()
	{
		Console.WriteLine("Введите число 1 - сторона a, 2 - выс h, 3 - r вписанной окр, 4 - r описанной окр");
		int type = Convert.ToInt32(Console.ReadLine());
		Console.WriteLine("Введите его значение");
		double value = Convert.ToDouble(Console.ReadLine());
		double a = 0, h, r, R;
		switch (type) {
			case 1:
			a = value;
			break;
		   case 2:
			a = 2 * value / Math.Sqrt(3);
			break;
		   case 3:
			a = 6 * value / Math.Sqrt(3);
			break;
		   case 4:
			a= 3 * value / Math.Sqrt(3);
			break;
		}
		  
		h = a * Math.Sqrt(3) / 2;
		r = a * Math.Sqrt(3) / 6;
		R = a * Math.Sqrt(3) / 3;
		  
		Console.WriteLine($"a = {a:F2}");
		Console.WriteLine($"h = {h:F2}");
		Console.WriteLine($"r = {r:F2}");
		Console.WriteLine($"R = {R:F2}");
	}
}