using Clicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace EnemyEditor
{
    public partial class GameWindow : Window
    {
        private EnemyStorage storage;
        private Player player;
        private Enemy? currentEnemy;
        private string iconsFolder;

        // Таймер и сферы
        private DispatcherTimer? gameTimer;
        private List<CCollectable> activeSpheres;
        private double spawnTimer = 0;
        private Random rng = new Random();

        public GameWindow()
        {
            InitializeComponent();
            storage = new EnemyStorage();
            player = new Player();
            activeSpheres = new List<CCollectable>();

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            iconsFolder = System.IO.Path.Combine(desktopPath, "ClickerData", "Icons");

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

        // Закрытие окна (останавливаем таймер)
        private void Window_Closed(object sender, EventArgs e)
        {
            gameTimer?.Stop();
        }

        // Таймер, который тикает каждые 33 мс (обновляет игру)
        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            double delta = 0.033;

            // Обновляем игрока (перезарядка и баффы)
            player.Update(delta);

            // Обновляем индикатор перезарядки
            if (player.CanClick()) CooldownBar.Value = 1;
            else CooldownBar.Value = 0;

            // Обновляем сферы
            UpdateSpheres(delta);

            // Обновляем UI
            UpdateUI();
        }

        // Создание и удаление сфер
        private void UpdateSpheres(double delta)
        {
            // Удаляем просроченные сферы
            for (int i = activeSpheres.Count - 1; i >= 0; i--)
            {
                if (activeSpheres[i].updateLifetime(delta))
                {
                    GameScene.Children.Remove(activeSpheres[i].getSprite());
                    activeSpheres.RemoveAt(i);
                }
            }

            // Спавним новые сферы (раз в 3 секунды)
            spawnTimer += delta;
            if (spawnTimer >= 3.0)
            {
                spawnTimer = 0;
                SpawnSphere();
            }
        }

        // Создать бонусную сферу
        private void SpawnSphere()
        {
            double x = rng.NextDouble() * (GameScene.ActualWidth - 40) + 20;
            double y = rng.NextDouble() * (GameScene.ActualHeight - 40) + 20;
            double size = 30;
            double lifetime = 3.0;

            CCollectable sphere;
            // 50% на золото, 50% на урон
            if (rng.NextDouble() < 0.5)
                sphere = new CGoldGiver(new Point(x, y), size, lifetime, player.Lvl);
            else
                sphere = new CDamageBooster(new Point(x, y), size, lifetime);

            activeSpheres.Add(sphere);
            GameScene.Children.Add(sphere.getSprite());
        }

        // Клик по сцене (для сбора сфер)
        private void GameScene_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(GameScene);

            for (int i = activeSpheres.Count - 1; i >= 0; i--)
            {
                if (activeSpheres[i].onClick(player, mousePos))
                {
                    GameScene.Children.Remove(activeSpheres[i].getSprite());
                    activeSpheres.RemoveAt(i);
                    return; // Собираем только одну сферу за клик
                }
            }
        }

        // Создать следующего противника
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

            string iconPath = System.IO.Path.Combine(iconsFolder, currentEnemy.IconPath);
            if (!System.IO.File.Exists(iconPath))
                iconPath = System.IO.Path.Combine(iconsFolder, "Monsters", currentEnemy.IconPath);

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

        // Обновить интерфейс
        private void UpdateUI()
        {
            if (currentEnemy == null) return;

            EnemyNameText.Text = currentEnemy.Name;
            EnemyHPText.Text = currentEnemy.CurrentHitPoints.ToString();
            EnemyGoldText.Text = currentEnemy.GoldReward.ToString();

            LevelText.Text = player.Lvl.ToString();
            GoldText.Text = player.Gold.ToString();
            DamageText.Text = player.Damage.ToString();
            UpgradeCostText.Text = player.UpgradeCost.ToString();
            UpgradeCooldownCostText.Text = player.UpgradeCooldownCost.ToString();
        }

        // Клик по противнику
        private void EnemyButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentEnemy == null) return;

            // Проверяем перезарядку
            if (!player.CanClick()) return; // Если не готов, игнорируем клик

            BigNumber damage = player.DealDamage();
            bool killed = currentEnemy.TakeDamage(damage, out BigNumber reward);

            player.ResetClick(); // Сброс перезарядки

            if (killed)
            {
                player.AddGold(reward);
                SpawnNextEnemy();
            }

            UpdateUI();
        }

        // Апгрейд урона
        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgrade()) { }
            else { MessageBox.Show("Не хватает золота!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
            UpdateUI();
        }

        // Апгрейд перезарядки
        private void UpgradeCooldownButton_Click(object sender, RoutedEventArgs e)
        {
            if (player.TryUpgradeCooldown()) { }
            else { MessageBox.Show("Не хватает золота!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); }
            UpdateUI();
        }
    }
}
