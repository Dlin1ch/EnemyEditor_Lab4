using Clicker;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace EnemyEditor
{
    public partial class GameWindow : Window
    {
        private EnemyStorage storage;
        private Player player;
        private IEnemy? currentEnemy; // ИСПРАВЛЕНО: теперь IEnemy
        private string iconsFolder;

        // Таймер и сферы (из Лабы 3)
        private DispatcherTimer? gameTimer;
        private List<CCollectable> activeSpheres;
        private double spawnTimer = 0;
        private Random rng = new Random();

        // Сохранение игрока
        private string saveFilePath;
        private ISaveList<Player> playerSaver;

        public GameWindow()
        {
            InitializeComponent();
            storage = new EnemyStorage();
            player = new Player();
            activeSpheres = new List<CCollectable>();

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            iconsFolder = System.IO.Path.Combine(desktopPath, "ClickerData", "Icons");
            saveFilePath = System.IO.Path.Combine(desktopPath, "ClickerData", "player_save.json");

            playerSaver = new JsonPlayerSaver();

            try
            {
                storage.LoadFromDefaultPath();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            SpawnNextEnemy();
            UpdateUI();

            // Запускаем игровой таймер (30 FPS)
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 33);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            gameTimer?.Stop();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            double delta = 0.033;

            player.Update(delta);

            if (player.CanClick()) CooldownBar.Value = 1;
            else CooldownBar.Value = 0;

            UpdateSpheres(delta);
            UpdateUI();
        }

        private void UpdateSpheres(double delta)
        {
            for (int i = activeSpheres.Count - 1; i >= 0; i--)
            {
                if (activeSpheres[i].updateLifetime(delta))
                {
                    GameScene.Children.Remove(activeSpheres[i].getSprite());
                    activeSpheres.RemoveAt(i);
                }
            }

            spawnTimer += delta;
            if (spawnTimer >= 3.0)
            {
                spawnTimer = 0;
                SpawnSphere();
            }
        }

        private void SpawnSphere()
        {
            double x = rng.NextDouble() * (GameScene.ActualWidth - 40) + 20;
            double y = rng.NextDouble() * (GameScene.ActualHeight - 40) + 20;
            double size = 30;
            double lifetime = 3.0;

            CCollectable sphere;
            if (rng.NextDouble() < 0.5)
                sphere = new CGoldGiver(new Point(x, y), size, lifetime, player.Lvl);
            else
                sphere = new CDamageBooster(new Point(x, y), size, lifetime);

            activeSpheres.Add(sphere);
            GameScene.Children.Add(sphere.getSprite());
        }

        private void GameScene_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(GameScene);

            for (int i = activeSpheres.Count - 1; i >= 0; i--)
            {
                if (activeSpheres[i].onClick(player, mousePos))
                {
                    GameScene.Children.Remove(activeSpheres[i].getSprite());
                    activeSpheres.RemoveAt(i);
                    return;
                }
            }
        }

        private void SpawnNextEnemy()
        {
            CEnemyTemplate? template = storage.GetRandomTemplate();
            if (template == null)
            {
                MessageBox.Show("Нет шаблонов противников!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            currentEnemy = storage.CreateEnemy(template, player.Lvl);

            string iconPath = System.IO.Path.Combine(iconsFolder, currentEnemy.IconName);
            if (!System.IO.File.Exists(iconPath))
                iconPath = System.IO.Path.Combine(iconsFolder, "Monsters", currentEnemy.IconName);

            if (System.IO.File.Exists(iconPath))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(iconPath);
                bitmap.EndInit();
                EnemyImage.Source = bitmap;
            }
            else
            {
                EnemyImage.Source = null;
            }
        }

        private void UpdateUI()
        {
            if (currentEnemy == null) return;

            EnemyNameText.Text = currentEnemy.Name;
            EnemyHPText.Text = currentEnemy.CurrentLife.ToString(); // ИСПРАВЛЕНО
            EnemyGoldText.Text = currentEnemy.Gold.ToString(); // ИСПРАВЛЕНО

            LevelText.Text = player.Lvl.ToString();
            GoldText.Text = player.Gold.ToString();
            DamageText.Text = player.Damage.ToString();
            UpgradeCostText.Text = player.UpgradeCost.ToString();
            UpgradeCooldownCostText.Text = player.UpgradeCooldownCost.ToString();
        }

        private void EnemyButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentEnemy == null) return;
            if (!player.CanClick()) return;

            BigNumber damage = player.DealDamage();
            currentEnemy.TakeDamage(damage); // ИСПРАВЛЕНО

            player.ResetClick();

            if (currentEnemy.IsDead()) // ИСПРАВЛЕНО
            {
                player.AddGold(currentEnemy.Gold);
                SpawnNextEnemy();
            }

            UpdateUI();
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgrade()) { }
            else { MessageBox.Show("Не хватает золота!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
            UpdateUI();
        }

        private void UpgradeCooldownButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgradeCooldown()) { }
            else { MessageBox.Show("Не хватает золота!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
            UpdateUI();
        }

        // Сохранение и загрузка игры
        private void SaveGameButton_Click(object sender, RoutedEventArgs e)
        {
            playerSaver.Save(player, saveFilePath);
            MessageBox.Show("Игра сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(saveFilePath))
            {
                player = playerSaver.Load(saveFilePath);
                UpdateUI();
                MessageBox.Show("Игра загружена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Файл сохранения не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}