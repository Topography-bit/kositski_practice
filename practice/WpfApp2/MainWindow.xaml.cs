using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPGCharacterCreator
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
        }

        private void InitializeSkillCosts(){
            skillCosts[SmithCheck] = 3;
            skillCosts[AlchemyCheck] = 4;
            skillCosts[HerbalismCheck] = 4;
            skillCosts[LockpickCheck] = 2;
            skillCosts[StealthCheck] = 3;
        }

        private void ClassRadio_Checked(object sender, RoutedEventArgs e){
            UpdateSkillsAvailability();
            UpdateInterfaceVisuals();
        }

        private void UpdateSkillsAvailability(){
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
             
            if (selectedClass == "Маг")
            {
                AlchemyCheck.IsEnabled = true;
                HerbalismCheck.IsEnabled = true;
            }

            else if (selectedClass == "Вор"){ 
                LockpickCheck.IsEnabled = true;
            }
            else if (selectedClass == "Лучник"){
                StealthCheck.IsEnabled = true;
            } 
            else if (selectedClass == "Воин")
            {
                SmithCheck.IsEnabled = true; 
            }
        }

        private void UpdateInterfaceVisuals() {
            string selectedClass = GetSelectedClass();
            SolidColorBrush backgroundBrush = new SolidColorBrush(Colors.White);

            if (selectedClass == "Маг"){
                backgroundBrush = new SolidColorBrush(Colors.LightBlue);
            }
            else if (selectedClass == "Вор"){
                backgroundBrush = new SolidColorBrush(Colors.LightGray);
            }
            else if (selectedClass == "Лучник"){
                backgroundBrush = new SolidColorBrush(Colors.LightGreen);
            }

            else if (selectedClass == "Воин")
            {
                backgroundBrush = new SolidColorBrush(Colors.LightCoral);
            }

            this.Background = backgroundBrush;
        }

        private void SkillCheck_Changed(object sender, RoutedEventArgs e){
            int totalCost = CalculateTotalCost();
            if (totalCost > maxSkillPoints){
                ((CheckBox)sender).IsChecked = false;
                MessageBox.Show($"Превышен лимит очков навыков! (Макс. {maxSkillPoints})");
            }
        }

        private int CalculateTotalCost()
        {
            int total = 0;
            foreach (var kvp in skillCosts){
                if (kvp.Key.IsChecked == true)
                {
                    total += kvp.Value;
                }
            }
            return total;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите имя персонажа!");
                return;
            }

            string className = GetSelectedClass();
            if (string.IsNullOrEmpty(className)) {
                MessageBox.Show("Выберите класс персонажа!");
                return;
            }
            List<string> skills = GetSelectedSkills();
            int totalCost = CalculateTotalCost();
            if (totalCost > maxSkillPoints){
                MessageBox.Show($"Превышен лимит очков навыков! (Макс. {maxSkillPoints})");
                return;
            }
            if (!ValidateSkillsForClass(className, skills)) {
                MessageBox.Show("Выбраны недоступные навыки для класса!");
                return;
            }

            string result = $"Имя: {name}\nКласс: {className}\nНавыки: {(skills.Count > 0 ? string.Join(", ", skills) : "Нет навыков")}\nОчки навыков: {totalCost}/{maxSkillPoints}";

            ResultTextBlock.Text = result;
        }

        private bool ValidateSkillsForClass(string className, List<string> skills) {
            if (className == "Маг"){
                return skills.TrueForAll(s => s == "Алхимия" || s == "Зельеварение");
            }
            else if (className == "Вор"){
                return skills.TrueForAll(s => s == "Взлом замков");
            }
            else if (className == "Лучник") {
                return skills.TrueForAll(s => s == "Скрытность");
            }

            else if (className == "Воин"){
                return skills.TrueForAll(s => s == "Кузнечное дело");
            }
            return false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = string.Empty;
            WarriorRadio.IsChecked = false;
            MageRadio.IsChecked = false;
            ArcherRadio.IsChecked = false;
            ThiefRadio.IsChecked = false;
            SmithCheck.IsChecked = false;
            AlchemyCheck.IsChecked = false;
            HerbalismCheck.IsChecked = false;
            LockpickCheck.IsChecked = false;
            StealthCheck.IsChecked = false;
            ResultTextBlock.Text = string.Empty;
            UpdateSkillsAvailability();
            UpdateInterfaceVisuals();
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = "Персонаж" + random.Next(1, 100);

            RadioButton[] classes = { WarriorRadio, MageRadio, ArcherRadio, ThiefRadio };
            int classIndex = random.Next(classes.Length);
            classes[classIndex].IsChecked = true;
            UpdateSkillsAvailability();
            UpdateInterfaceVisuals();
            CheckBox[] allSkills = { SmithCheck, AlchemyCheck, HerbalismCheck, LockpickCheck, StealthCheck };
            int currentCost = 0;
            foreach (var skill in allSkills)
            {
                if (skill.IsEnabled && currentCost < maxSkillPoints)
                {
                    if (random.Next(2) == 1)
                    {
                        int cost = skillCosts[skill];
                        if (currentCost + cost <= maxSkillPoints)
                        {
                            skill.IsChecked = true;
                            currentCost += cost;
                        }
                    }
                }
            }
        }

        private string GetSelectedClass(){
            if (WarriorRadio.IsChecked == true) return "Воин";
            if (MageRadio.IsChecked == true) return "Маг";
            if (ArcherRadio.IsChecked == true) return "Лучник";
            if (ThiefRadio.IsChecked == true) return "Вор";
            return string.Empty;
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