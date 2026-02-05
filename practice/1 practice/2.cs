using System;
     
public class Program
{
 public static void Main()
	{
		Console.WriteLine("Введите число");
		int num = Convert.ToInt32(Console.ReadLine());
		
		int n1 = num % 10;
		num = num / 10;
		int n2 = num % 10;
		num = num / 10;  
		int n3 = num % 10;
		num = num / 10;  
		int n4 = num % 10;
		num = num / 10;
		  
		long product = n1 * n2 * n3 * n4;
		Console.WriteLine(product);
	}
}