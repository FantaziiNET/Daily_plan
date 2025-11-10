using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Daily_plan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public StringCollection PlanList = Properties.Settings.Default.PlanList;
        public StringCollection BreakBetweenTasks = Properties.Settings.Default.BreakBetweenTasks;
        public StringCollection DaysToCompleteTasks = Properties.Settings.Default.DaysToCompleteTasks;

        public List<string> TasksToday = [];
        public string LastUpdateDate = Properties.Settings.Default.LastUpdateDate;
        public string DayToday;
        public int DaysLater;


        public MainWindow()
        {
            InitializeComponent();
            NowDateTextBlock.Text = $"{DateTime.Today:d MMMM yyyy}";
            DayToday = $"{DateTime.Today:dd MM yyyy}";
            //DayToday = "23 11 2025"; // clear this
            // new estb???



            TimeCounting();
            UpdateTasks();



            Save();



            //foreach (var plan in PlanList)
            //    PlanTextBolck.Text += $"{plan}\n";

            #region test Properties.Settings.Default
            //foreach (var plan in BreakBetweenTasks)
            //    PlanTextBolck.Text += $"{plan}\n";

            //foreach (var plan in DaysToCompleteTasks)
            //    PlanTextBolck.Text += $"{plan}\n";

            //PlanList =
            //[
            //    "• Помыть посуду",
            //    "• Пропылесосить",
            //    "• Влажная уборка",
            //    "• Постирать",
            //    "• Побриться",
            //];
            //BreakBetweenTasks =
            //[
            //    "2",
            //    "4",
            //    "4",
            //    "3",
            //    "3",
            //];
            //DaysToCompleteTasks =
            //[
            //    "0",
            //    "0",
            //    "0",
            //    "0",
            //    "0",
            //];
            //DaysToCompleteTasks = Properties.Settings.Default.DaysToCompleteTasks;
            //BreakBetweenTasks = Properties.Settings.Default.BreakBetweenTasks;
            //Properties.Settings.Default.Save();
            #endregion
        }

        private void NewTask_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewTaskStackPanel.Visibility = Visibility.Visible;
        }

        private void SaveTaskButton_Click(object sender, RoutedEventArgs e)
        {
            NewTaskStackPanel.Visibility = Visibility.Collapsed;
        }

        private void TaskCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int id = 0;
            if (sender is CheckBox cb)
            {
                cb.Visibility = Visibility.Collapsed;
                foreach (string name in PlanList)
                {
                    if (cb.Content == name)
                    {
                        DaysToCompleteTasks[id] = BreakBetweenTasks[id];
                        break;
                    }
                    id++;
                }
            }
        }

        public void TimeCounting()
        {
            DaysLater = (Convert.ToDateTime(LastUpdateDate) - Convert.ToDateTime(DayToday)).Days;
            StringCollection strings = [];
            int id = 0;

            foreach (string days in DaysToCompleteTasks)
            {
                int day = Convert.ToInt32(days) + DaysLater;
                strings.Add($"{day}");
                id++;
            }
            DaysToCompleteTasks = strings;
        }

        public void UpdateTasks()
        {
            int id = 0;

            foreach (string? days in DaysToCompleteTasks)
            {
                if (Convert.ToInt32(days) <= 0)
                    TasksToday.Add(PlanList[id]);
                id++;
            }

            TaskListBox.ItemsSource = TasksToday;

            LastUpdateDate = DayToday;
        }

        public void Save()
        {
            Properties.Settings.Default.PlanList = PlanList;
            Properties.Settings.Default.BreakBetweenTasks = BreakBetweenTasks;
            Properties.Settings.Default.DaysToCompleteTasks = DaysToCompleteTasks;
            Properties.Settings.Default.LastUpdateDate = LastUpdateDate;
            Properties.Settings.Default.Save();
        }
    }
}