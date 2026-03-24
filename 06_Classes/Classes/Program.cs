using System;

namespace Classes
{
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
        float TimeLimit { get; }
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

    public abstract class Quest : IAcceptable, IRewardeable, ITrackeable
    {
        public string Name { get; set; }
        public float Progress { get; set; }
        public bool IsActive { get; set; }
        public Reward QuestReward { get; }

        public Quest(string name, Reward reward, float progress)
        {
            Name = name;
            QuestReward = reward;
            Progress = progress;
            IsActive = false;
        }

        public void Accept()
        {
            IsActive = true;
            Console.WriteLine($"Квест '{Name}' принят!");
        }

        public void TrackProgress()
        {
            Console.WriteLine($"[{Name}] Прогресс: {Progress*100}%");
        }

        public abstract void ShowInfo();
    }

    public class MainQuest : Quest, IChainable
    {
        public Quest NextQuest { get; set; }

        public MainQuest(string name, Reward reward, float progress, Quest nextQuest = null)
            : base(name, reward, progress)
        {
            NextQuest = nextQuest;
        }

        public override void ShowInfo(){
            Console.WriteLine($"[Основной квест] {Name}");
            if (NextQuest != null)
                Console.WriteLine($"  Следующий: {NextQuest.Name}");
        }
    }

    public class SideQuest : Quest
    {
        public string Location { get; set; }

        public SideQuest(string name, Reward reward, float progress, string location)
            : base(name, reward, progress)
        {
            Location = location;
        }

        public override void ShowInfo(){
            Console.WriteLine($"[Побочный квест] {Name}, Место: {Location}");
        }
    }

    public class DailyQuest : Quest, ITimeLimited
    {
        public float TimeLimit { get; }
        
        public DailyQuest(string name, Reward reward, float progress, float timeLimit)
            : base(name, reward, progress)
        {
            TimeLimit = timeLimit;
        }

        public void CheckTime(){
            Console.WriteLine($"Осталось времени: {TimeLimit} часов");
        }

        public override void ShowInfo() {
            Console.WriteLine($"[Ежедневное] {Name}, Лимит: {TimeLimit}ч");
        }
    }

    public class EventQuest : Quest, ITimeLimited
    {
        public float TimeLimit { get; }
        public string EventName { get; }

        public EventQuest(string name, Reward reward, float progress, float timeLimit, string eventName)
            : base(name, reward, progress)
        {
            TimeLimit = timeLimit;
            EventName = eventName;
        }

        public void CheckTime(){
            Console.WriteLine($"Событие '{EventName}' заканчивается через {TimeLimit} часов");
        }

        public override void ShowInfo(){
            Console.WriteLine($"[Событийный] {Name}, Событие: {EventName}");
        }
    }

    public class RepeatableQuest : Quest, ITimeLimited
    {
        public int TimesCompleted { get; set; }
        public float TimeLimit { get; }

        public RepeatableQuest(string name, Reward reward, float progress, float timeLimit)
            : base(name, reward, progress)
        {
            TimeLimit = timeLimit;
            TimesCompleted = 0;
        }

        public void Complete() {
            TimesCompleted++;
            Progress = 0;
            Console.WriteLine($"Квест '{Name}' выполнен! Всего раз: {TimesCompleted}");
        }

        public void CheckTime(){
            Console.WriteLine($"Сброс через {TimeLimit} часов");
        }
        
        public override void ShowInfo(){
            Console.WriteLine($"[Повторяемый] {Name}, Выполнено раз: {TimesCompleted}");
        }
    }


    class Program
    {
        static void Main()
        {
            Reward darkClaymore = new Reward("Дарк клеймор", "Меч с миллиардом дамага лол");
            Reward goldCoins = new Reward("100 золотых", "Куча монет");
            Reward magicRing = new Reward("Кольцо силы", "Бонус к атаке");
            
            DailyQuest daily = new DailyQuest("Собрать 10 трав", goldCoins, 0.5f, 24);
            SideQuest side = new SideQuest("Очистить пещеру", magicRing, 0f, "Тёмная пещера");
            MainQuest main1 = new MainQuest("Спасти принцессу", darkClaymore, 0.3f);
            MainQuest main2 = new MainQuest("Победить дракона", darkClaymore, 0f, main1);
            EventQuest eventQ = new EventQuest("Праздничный сбор", goldCoins, 0f, 48, "Фестиваль урожая");
            RepeatableQuest repeat = new RepeatableQuest("Убить 5 слаймов", goldCoins, 0f, 12);

            Quest[] allQuests = { main1, main2, side, daily, eventQ, repeat };

            foreach (var q in allQuests)
            {
                q.Accept();
                q.TrackProgress();
                q.ShowInfo();
                Console.WriteLine($"  Награда: {q.QuestReward.Name}");
                Console.WriteLine();
            }
            
            daily.CheckTime();
            
            repeat.Complete();
            repeat.Complete();
            repeat.ShowInfo();
            
            Console.ReadKey();
        }
    }
}
