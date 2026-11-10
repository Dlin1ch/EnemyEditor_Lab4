using Clicker;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnemyEditor
{
    public partial class MainWindow : Window
    {
        private CEnemyTemplateList enemyList;
        private List<EnemyIcon> enemyIcons;

        public MainWindow()
        {
            InitializeComponent();
            enemyList = new CEnemyTemplateList();
            enemyIcons = new List<EnemyIcon>();
        }

        // ==================== ЗАГРУЗКА ИКОНОК ====================

        private void LoadIconsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.Title = "Выберите папку с иконками";
            if (dialog.ShowDialog() == true)
            {
                LoadIconsFromFolder(dialog.FolderName);
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.Show();
        }

        private void LoadIconsFromFolder(string path)
        {
            enemyIcons.Clear();
            IconsListBox.Items.Clear();
            string filter = "*.png";
            string[] files = Directory.GetFiles(path, filter);
            foreach (string file in files)
            {
                EnemyIcon icon = new EnemyIcon
                {
                    Name = System.IO.Path.GetFileName(file),
                    ImagePath = file
                };
                enemyIcons.Add(icon);
                Image image = new Image()
                {
                    Source = new BitmapImage(new Uri(icon.ImagePath)),
                    Height = 64
                };
                IconsListBox.Items.Add(image);
            }
        }

        private void IconsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = IconsListBox.SelectedIndex;
            if (index >= 0 && index < enemyIcons.Count)
            {
                EnemyIcon icon = enemyIcons[index];
                IconNameTextBox.Text = icon.Name ?? "";
                if (!string.IsNullOrEmpty(icon.ImagePath))
                {
                    MainEnemyIcon.Source = new BitmapImage(new Uri(icon.ImagePath));
                }
            }
        }

        // ==================== ДОБАВЛЕНИЕ И УДАЛЕНИЕ ====================

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EnemyNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(IconNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(BaseLifeTextBox.Text) ||
                string.IsNullOrWhiteSpace(LifeModifierTextBox.Text) ||
                string.IsNullOrWhiteSpace(BaseGoldTextBox.Text) ||
                string.IsNullOrWhiteSpace(GoldModifierTextBox.Text) ||
                string.IsNullOrWhiteSpace(SpawnChanceTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string name = EnemyNameTextBox.Text;
                string iconName = IconNameTextBox.Text;
                int baseLife = int.Parse(BaseLifeTextBox.Text);
                double lifeModifier = double.Parse(LifeModifierTextBox.Text);
                int baseGold = int.Parse(BaseGoldTextBox.Text);
                double goldModifier = double.Parse(GoldModifierTextBox.Text);
                double spawnChance = double.Parse(SpawnChanceTextBox.Text);

                int typeIndex = EnemyTypeComboBox.SelectedIndex;
                CEnemyTemplate newEnemy;

                if (typeIndex == 1)
                    newEnemy = new CArmoredEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance, 50);
                else if (typeIndex == 2)
                    newEnemy = new CHealingEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance, 0.3);
                else
                    newEnemy = new CNormalEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance);

                enemyList.AddEnemy(newEnemy);
                UpdateEnemiesListBox();
                ClearInputFields();
            }
            catch (FormatException)
            {
                MessageBox.Show("Проверьте правильность ввода чисел!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите противника для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            enemyList.DeleteEnemyByIndex(EnemiesListBox.SelectedIndex);
            UpdateEnemiesListBox();
        }

        private void UpdateEnemiesListBox()
        {
            EnemiesListBox.Items.Clear();
            List<string> names = enemyList.GetListOfEnemyNames();
            foreach (string name in names)
            {
                EnemiesListBox.Items.Add(name);
            }
        }

        private void EnemiesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = EnemiesListBox.SelectedIndex;
            if (index >= 0 && index < enemyList.GetListOfEnemyNames().Count)
            {
                CEnemyTemplate? enemy = enemyList.GetEnemyByIndex(index);
                if (enemy != null)
                {
                    EnemyNameTextBox.Text = enemy.Name;
                    IconNameTextBox.Text = enemy.IconName;
                    BaseLifeTextBox.Text = enemy.BaseLife.ToString();
                    LifeModifierTextBox.Text = enemy.LifeModifier.ToString();
                    BaseGoldTextBox.Text = enemy.BaseGold.ToString();
                    GoldModifierTextBox.Text = enemy.GoldModifier.ToString();
                    SpawnChanceTextBox.Text = enemy.SpawnChance.ToString();

                    // НОВЫЙ БЛОК: Переключение типа в ComboBox
                    if (enemy is CArmoredEnemyTemplate)
                    {
                        EnemyTypeComboBox.SelectedIndex = 1;
                    }
                    else if (enemy is CHealingEnemyTemplate)
                    {
                        EnemyTypeComboBox.SelectedIndex = 2;
                    }
                    else
                    {
                        EnemyTypeComboBox.SelectedIndex = 0;
                    }

                    if (!string.IsNullOrEmpty(enemy.IconName))
                    {
                        foreach (EnemyIcon icon in enemyIcons)
                        {
                            if (icon.Name == enemy.IconName)
                            {
                                if (!string.IsNullOrEmpty(icon.ImagePath))
                                {
                                    MainEnemyIcon.Source = new BitmapImage(new Uri(icon.ImagePath));
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void ClearInputFields()
        {
            EnemyNameTextBox.Text = "";
            IconNameTextBox.Text = "";
            BaseLifeTextBox.Text = "";
            LifeModifierTextBox.Text = "";
            BaseGoldTextBox.Text = "";
            GoldModifierTextBox.Text = "";
            SpawnChanceTextBox.Text = "";
            MainEnemyIcon.Source = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.DefaultExt = ".json";
            dialog.FileName = "enemies";
            if (dialog.ShowDialog() == true)
            {
                enemyList.SaveToJson(dialog.FileName);
                MessageBox.Show("Сохранено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JSON files (*.json)|*.json";
            dialog.DefaultExt = ".json";
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    enemyList.LoadFromJson(dialog.FileName);
                    UpdateEnemiesListBox();
                    MessageBox.Show("Загружено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnemiesListBox.SelectedIndex == -1) return;

            if (string.IsNullOrWhiteSpace(EnemyNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(IconNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(BaseLifeTextBox.Text) ||
                string.IsNullOrWhiteSpace(LifeModifierTextBox.Text) ||
                string.IsNullOrWhiteSpace(BaseGoldTextBox.Text) ||
                string.IsNullOrWhiteSpace(GoldModifierTextBox.Text) ||
                string.IsNullOrWhiteSpace(SpawnChanceTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // 1. Запоминаем индекс и удаляем старого
                int index = EnemiesListBox.SelectedIndex;
                enemyList.DeleteEnemyByIndex(index);

                // 2. Читаем данные из полей
                string name = EnemyNameTextBox.Text;
                string iconName = IconNameTextBox.Text;
                int baseLife = int.Parse(BaseLifeTextBox.Text);
                double lifeModifier = double.Parse(LifeModifierTextBox.Text);
                int baseGold = int.Parse(BaseGoldTextBox.Text);
                double goldModifier = double.Parse(GoldModifierTextBox.Text);
                double spawnChance = double.Parse(SpawnChanceTextBox.Text);

                // 3. Создаем врага ТОГО ЖЕ ТИПА, что выбран в ComboBox
                int typeIndex = EnemyTypeComboBox.SelectedIndex;
                CEnemyTemplate newEnemy;

                if (typeIndex == 1) // Бронированный
                    newEnemy = new CArmoredEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance, 50);
                else if (typeIndex == 2) // Лечащийся
                    newEnemy = new CHealingEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance, 0.3);
                else // Обычный
                    newEnemy = new CNormalEnemyTemplate(name, iconName, baseLife, lifeModifier, baseGold, goldModifier, spawnChance);

                // 4. Добавляем обновленного
                enemyList.AddEnemy(newEnemy);
                UpdateEnemiesListBox();
                ClearInputFields();
                MessageBox.Show("Противник обновлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (FormatException)
            {
                MessageBox.Show("Проверьте правильность ввода чисел!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}