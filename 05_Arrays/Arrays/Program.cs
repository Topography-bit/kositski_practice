using System;

namespace Arrays
{
    class Program
    {
     static void Main()
    	{
    		Console.WriteLine("=== Задание 15: Сортировка пузырьком по убыванию ===");
    		Console.WriteLine("Введите размер массива:");
    		int n = Convert.ToInt32(Console.ReadLine());
    		int[] arr = new int[n];
    		
    		Random rnd = new Random();
    		for (int i = 0; i < n; i++){
    			arr[i] = rnd.Next(-50, 50);
    		}

    		Console.WriteLine("Исходный массив:");
    		for (int i = 0; i < n; i++)
    			Console.Write(arr[i] + " ");
    		Console.WriteLine();

    		for (int i = 0; i < n - 1; i++){
    			for (int j = 0; j < n - i - 1; j++)
    			{
    				if (arr[j] < arr[j+1]){
    					int temp = arr[j];
    					arr[j] = arr[j+1];
    					arr[j+1] = temp;
    				}
    			}
    		}

    		Console.WriteLine("Отсортированный по убыванию:");
    		for (int i = 0; i < n; i++)
    			Console.Write(arr[i] + " ");
    		Console.WriteLine();
    		
    		
    		Console.WriteLine();
    		Console.WriteLine("=== Задание 15 (2D): Сумма положительных над главной диагональю ===");
    		Console.WriteLine("Введите N для матрицы NxN:");
    		int sz = Convert.ToInt32(Console.ReadLine());

    		int[,] matrix = new int[sz, sz];

    		for (int i = 0; i < sz; i++)
    			for (int j = 0; j < sz; j++)
    				matrix[i, j] = rnd.Next(-20, 20);

    		Console.WriteLine("Матрица:");
    		for (int i = 0; i < sz; i++){
    			for (int j = 0; j < sz; j++)
    				Console.Write("{0,5}", matrix[i, j]);
    			Console.WriteLine();
    		}

    		int sum = 0;
    		int count = 0;
    		for (int i = 0; i < sz; i++){
    			for (int j = i + 1; j < sz; j++){
    				if (matrix[i,j] > 0) {
    					sum += matrix[i,j];
    					count++;
    				}
    			}
    		}

    		Console.WriteLine($"Сумма положительных над диагональю: {sum}");
    		Console.WriteLine($"Количество: {count}");
    		
    		Console.ReadKey();
    	}
    }
}
