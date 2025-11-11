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


        public MainWindow() // v 1.2
        {
            InitializeComponent();
            NowDateTextBlock.Text = $"{DateTime.Today:d MMMM yyyy}";
            DayToday = $"{DateTime.Today:dd MM yyyy}";

            TimeCounting();
            UpdateTasks();
            LastUpdateDate = DayToday;

            #region primer
            //DayToday = "10 11 2025";
            //LastUpdateDate = DayToday;
            //PlanList =
            //[
            //    "Помыть посуду",
            //    "Пропылесосить",
            //    "Влажная уборка",
            //    "Постирать",
            //    "Побриться",
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
            #endregion

            #region test Properties.Settings.Default
            //foreach (var plan in BreakBetweenTasks)
            //    PlanTextBolck.Text += $"{plan}\n";

            //foreach (var plan in DaysToCompleteTasks)
            //    PlanTextBolck.Text += $"{plan}\n";


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
            if (!PlanList.Contains(NewTaskNameTextBox.Text) && !String.IsNullOrEmpty(NewTaskNameTextBox.Text)
                && !String.IsNullOrEmpty(NewTaskDayTextBox.Text))
            {
                PlanList.Add(NewTaskNameTextBox.Text);
                BreakBetweenTasks.Add(NewTaskDayTextBox.Text);
                DaysToCompleteTasks.Add("0");

                TasksToday.Clear();
                TaskListBox.ItemsSource = null;
                UpdateTasks();
                ClearNewTaskStackPanel();
            }
            else
            {
                MessageBox.Show("Возникла ошибка! Возможно такая задаяча уже есть или вы не заполнили поле", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                NewTaskNameTextBox.Text = "";
            }
        }

        private void ClearNewTaskStackPanel()
        {
            NewTaskNameTextBox.Text = "";
            NewTaskDayTextBox.Text = "";
            NewTaskStackPanel.Visibility = Visibility.Collapsed;
        }

        private void ClearNewTaskButton_Click(object sender, RoutedEventArgs e)
        {
            ClearNewTaskStackPanel();
        }

        private void NewTaskDayTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void TaskCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            int id = 0;
            if (sender is CheckBox cb)
            {
                cb.Visibility = Visibility.Collapsed;
                foreach (string? name in PlanList)
                {
                    if ((string)cb.Content == name)
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

            foreach (string? days in DaysToCompleteTasks)
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
        }

        public void Save()
        {
            Properties.Settings.Default.PlanList = PlanList;
            Properties.Settings.Default.BreakBetweenTasks = BreakBetweenTasks;
            Properties.Settings.Default.DaysToCompleteTasks = DaysToCompleteTasks;
            Properties.Settings.Default.LastUpdateDate = LastUpdateDate;
            Properties.Settings.Default.Save();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Save();
        }
    }
}