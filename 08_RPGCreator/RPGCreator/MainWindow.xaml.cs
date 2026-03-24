using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPGCreator
{
    public partial class MainWindow : Window
    {
        private Random random = new Random();
        private int maxSkillPoints = 10;
        private Dictionary<CheckBox, int> skillCosts = new Dictionary<CheckBox, int>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeSkillCosts();
            UpdateSkillsAvailability();
            UpdateVisuals();
        }

        private void InitializeSkillCosts(){
            skillCosts[SmithCheck] = 3;
            skillCosts[AlchemyCheck] = 4;
            skillCosts[HerbalismCheck] = 4;
            skillCosts[LockpickCheck] = 2;
            skillCosts[StealthCheck] = 3;
        }

        private void ClassRadio_Checked(object sender, RoutedEventArgs e){
            if (SmithCheck == null) return;
            UpdateSkillsAvailability();
            UpdateVisuals();
        }

        private void UpdateSkillsAvailability(){
            if (SmithCheck == null) return;
            SmithCheck.IsEnabled = false;
            AlchemyCheck.IsEnabled = false;
            HerbalismCheck.IsEnabled = false;
            LockpickCheck.IsEnabled = false;
            StealthCheck.IsEnabled = false; 
            SmithCheck.IsChecked = false;
            AlchemyCheck.IsChecked = false;
            HerbalismCheck.IsChecked = false; 
            LockpickCheck.IsChecked = false;
            StealthCheck.IsChecked = false;

            string selectedClass = GetSelectedClass();
             
            if (selectedClass == "Маг"){
                AlchemyCheck.IsEnabled = true;
                HerbalismCheck.IsEnabled = true;
            }
            else if (selectedClass == "Вор"){ 
                LockpickCheck.IsEnabled = true;
                StealthCheck.IsEnabled = true;
            }
            else if (selectedClass == "Лучник"){
                StealthCheck.IsEnabled = true;
                SmithCheck.IsEnabled = true;
            } 
            else if (selectedClass == "Воин"){
                SmithCheck.IsEnabled = true; 
            }
            
            UpdatePointsLabel();
        }

        private void UpdateVisuals() {
            string cl = GetSelectedClass();
            SolidColorBrush bg = new SolidColorBrush(Colors.White);

            if (cl == "Маг") bg = new SolidColorBrush(Colors.LightBlue);
            else if (cl == "Вор") bg = new SolidColorBrush(Colors.LightGray);
            else if (cl == "Лучник") bg = new SolidColorBrush(Colors.LightGreen);
            else if (cl == "Воин") bg = new SolidColorBrush(Colors.LightCoral);

            this.Background = bg;
        }

        private void SkillCheck_Changed(object sender, RoutedEventArgs e){
            if (skillCosts.Count == 0) return;
            int totalCost = CalcTotalCost();
            if (totalCost > maxSkillPoints){
                ((CheckBox)sender).IsChecked = false;
                MessageBox.Show($"Превышен лимит очков навыков! (Макс. {maxSkillPoints})");
            }
            UpdatePointsLabel();
        }
        
        private void UpdatePointsLabel(){
            if (PointsLabel != null)
                PointsLabel.Text = $"Очки: {CalcTotalCost()}/{maxSkillPoints}";
        }

        private int CalcTotalCost(){
            int total = 0;
            foreach (var kvp in skillCosts)
                if (kvp.Key.IsChecked == true) total += kvp.Value;
            return total;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name)){
                MessageBox.Show("Введите имя персонажа!");
                return;
            }

            string className = GetSelectedClass();
            List<string> skills = GetSelectedSkills();

            string result = $"=== Персонаж создан ===\n\nИмя: {name}\nКласс: {className}\n";
            result += $"Очки навыков: {CalcTotalCost()}/{maxSkillPoints}\n\n";
            
            if (skills.Count > 0){
                result += "Навыки:\n";
                foreach (string skill in skills) result += $" • {skill}\n";
            }
            else result += "Навыки: не выбраны\n";

            ResultTextBlock.Text = result;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = string.Empty;
            WarriorRadio.IsChecked = true;
            SmithCheck.IsChecked = false;
            AlchemyCheck.IsChecked = false;
            HerbalismCheck.IsChecked = false;
            LockpickCheck.IsChecked = false;
            StealthCheck.IsChecked = false;
            ResultTextBlock.Text = string.Empty;
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            string[] names = { "Артур", "Мерлин", "Леголас", "Арагорн", "Гэндальф", "Торин" };
            NameTextBox.Text = names[random.Next(names.Length)];

            RadioButton[] classes = { WarriorRadio, MageRadio, ArcherRadio, ThiefRadio };
            classes[random.Next(classes.Length)].IsChecked = true;
            UpdateSkillsAvailability();
            
            CheckBox[] allSkills = { SmithCheck, AlchemyCheck, HerbalismCheck, LockpickCheck, StealthCheck };
            int currentCost = 0;
            foreach (var skill in allSkills){
                if (skill.IsEnabled && currentCost < maxSkillPoints)
                    if (random.Next(2) == 1){
                        int cost = skillCosts[skill];
                        if (currentCost + cost <= maxSkillPoints){
                            skill.IsChecked = true;
                            currentCost += cost;
                        }
                    }
            }
        }

        private string GetSelectedClass(){
            if (WarriorRadio.IsChecked == true) return "Воин";
            if (MageRadio.IsChecked == true) return "Маг";
            if (ArcherRadio.IsChecked == true) return "Лучник";
            if (ThiefRadio.IsChecked == true) return "Вор";
            return "";
        }

        private List<string> GetSelectedSkills(){
            List<string> skills = new List<string>();
            if (SmithCheck.IsChecked == true) skills.Add("Кузнечное дело");
            if (AlchemyCheck.IsChecked == true) skills.Add("Алхимия");
            if (HerbalismCheck.IsChecked == true) skills.Add("Зельеварение");
            if (LockpickCheck.IsChecked == true) skills.Add("Взлом замков");
            if (StealthCheck.IsChecked == true) skills.Add("Скрытность");
            return skills;
        }
    }
}
