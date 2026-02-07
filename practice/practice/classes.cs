using System;

public interface IAcceptable{
    void Accept();
}

public interface ITrackeable{    
    void TrackProgress();
}

public interface IRewardeable{
    Reward QuestReward { get; }
}

public interface ITimeLimited{
    void CheckTime();
}

public interface IChainable{
    Quest NextQuest { get; set; }
}

public class Reward{
    public string Name { get; }
    public string Description { get; }

    public Reward(string name, string description)
    {
        Name = name;
        Description = description;
    }
}

public class Quest : IAcceptable, IRewardeable, ITrackeable
{
    public string Name { get; set; }
    public float Progress { get; set; }
    public Reward QuestReward { get; }

    public Quest(string name, Reward reward, float progress)
    {
        Name = name;
        QuestReward = reward;
        Progress = progress;
    }

    public void Accept()
    {
        Console.WriteLine("Вы приняли квест");
    }
    public string GetReward => QuestReward.Name;

    public string GetRewardName => QuestReward.Name;

    public void TrackProgress()
    {
        Console.WriteLine($"Ваш прогресс: {Progress}");
    }
}

public class MainQuest : Quest, IChainable
{
    public Quest NextQuest { get; set; }

    public MainQuest(string name, Reward reward, float progress, Quest nextQuest)
        : base(name, reward, progress)
    {
        NextQuest = nextQuest;
    }

    public void PrintNextQuest()
    {
        Console.WriteLine(NextQuest.Name);
    }
}

public class DailyQuest : Quest, ITimeLimited
{
    public float TimeLeft { get; set; }

    public DailyQuest(string name, Reward reward, float progress, float timeLeft)
        : base(name, reward, progress)
    {
        TimeLeft = timeLeft;
    }

    public void CheckTime()
    {
        Console.WriteLine($"Осталось времени: {TimeLeft} часов");
    }
}

public class Program
{
    public static void Main()
    {
        Reward darkClaymore = new Reward("Дарк клеймор", "Меч с миллиардом дамага лол");

        Console.WriteLine($"{darkClaymore.Name}, {darkClaymore.Description}");

        Quest firstQuest = new Quest("Пройти ММ7", darkClaymore, 0.3f);
        firstQuest.Accept();
        firstQuest.TrackProgress();
        Console.WriteLine($"Награда: {firstQuest.GetRewardName}");
    
        DailyQuest firstDay = new DailyQuest("Пройти ММ4", darkClaymore, 0.3f, 10.4f);
        
        firstDay.CheckTime();
        firstDay.TrackProgress();
        
    }
    
    
}