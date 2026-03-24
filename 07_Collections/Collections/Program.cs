using System;
using System.Collections.Generic;

namespace Collections
{
    class Comment {
        public string Author {get;}
        public string Text {get;}
        public DateTime Date {get;}
        public static int CommentsCnt = 0;
        
        
        public Comment(string author, string text){
            Author = author;
            Text = text;
            Date = DateTime.Now;
            CommentsCnt++;
        }
        
        public int WordCount(){
            string[] words = Text.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }
        
        public static Comment[] FindByAuthor(List<Comment> allcomms, string author_name){
            List<Comment> found_lst = new List<Comment>();
            for (int i = 0; i < allcomms.Count; i++){
                if (allcomms[i].Author.ToLower() == author_name.ToLower()){
                    found_lst.Add(allcomms[i]);
                }
            }
            return found_lst.ToArray();
        }
        
        public static int CountAll(){
            return CommentsCnt;
        }
        
        public string GetShortText(int maxLen){
            if (Text.Length <= maxLen)
                return Text;
            return Text.Substring(0, maxLen) + "...";
        }
    }



    class Program
    {
     static void Main()
     {
      List<Comment> lst = new List<Comment>();
      
      lst.Add(new Comment("Кирилл", "Привет как дела у всех"));
      lst.Add(new Comment("Топографи", "Нормально всё работает"));
      lst.Add(new Comment("Кирилл", "Факт братан"));
      lst.Add(new Comment("Аноним", "Ну такое себе"));
      lst.Add(new Comment("Топографи",  "Чел ты чо"));
      
      Console.WriteLine("Все комментарии:");
      for (int i = 0; i < lst.Count; i ++){
          Console.WriteLine($"{lst[i].Author}: {lst[i].Text} (слов: {lst[i].WordCount()})");
      }

      Console.WriteLine();
      Console.WriteLine($"Всего комментариев: {Comment.CountAll()}");
      
      Console.WriteLine();
      Console.WriteLine("Комментарии Кирилла:");
      Comment[] res = Comment.FindByAuthor(lst, "кирилл");
      for (int i = 0; i < res.Length; i ++){
          Console.WriteLine($"  {res[i].Author}, {res[i].GetShortText(15)}, {res[i].Date}");
      }
      
      Console.WriteLine();
      
      Console.WriteLine("Поиск по слову 'всё':");
      for (int i = 0; i < lst.Count; i++){
          if (lst[i].Text.Contains("всё"))
              Console.WriteLine($"  Найдено у {lst[i].Author}: {lst[i].Text}");
      }
      
      Console.ReadKey();
     }
    }
}
