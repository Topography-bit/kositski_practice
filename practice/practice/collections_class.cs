using System;
using System.Collections.Generic;

class Comment {
    public string Author {get;}
    public string Text {get;}
    public DateTime Date {get;}
    public static int CommentsCnt = 0;
    
    
    public Dictionary<string, string> Reactions = new Dictionary<string, string>();
    
    
    public Comment(string author, string text){
        Author = author;
        Text = text;
        
        Date = DateTime.Now;
        CommentsCnt++;
    }
    
    
    public void AddReact(string username, string reaction){
        Reactions.Add(username, reaction);
    }
    
    public static Comment[] FindByAuthor(List<Comment> allcomms, string author_name){
        List<Comment> found_lst = new List<Comment>();
        for (int i = 0; i < allcomms.Count; i++){
            if (allcomms[i].Author == author_name){
                found_lst.Add(allcomms[i]);
            }
        }
        return found_lst.ToArray();
    }
}



public class Test
{
 public static void Main()
 {
  List<Comment> lst = new List<Comment>();
  
  lst.Add(new Comment("Кирилл", "Факт"));
  lst.Add(new Comment("Топографи", "Привет"));
  
  for (int i = 0; i < lst.Count; i ++){
      Console.WriteLine(lst[i].Text);
  }
  
  Comment[] res = Comment.FindByAuthor(lst, "Кирилл");
     for (int i = 0; i < res.Length; i ++){
      Console.WriteLine($"{res[i].Author}, {res[i].Text}, {res[i].Date}");
  }
 }
}