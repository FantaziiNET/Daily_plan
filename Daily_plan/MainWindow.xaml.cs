using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Daily_Plan.Properties;

namespace Daily_plan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _path = "../../../Save Data/data.txt";//"../Save Data/data.txt";

        public List<string> TasksToday = [];
        public string DayToday;
        public int DaysLater;

        public StringCollection PlanList = Settings.Default.PlanList;
        public StringCollection BreakBetweenTasks= Settings.Default.BreakBetweenTasks;
        public StringCollection DaysToCompleteTasks = Settings.Default.DaysToCompleteTasks;
        public string? LastUpdateDate = Settings.Default.LastUpdateDate;


        public MainWindow() // v 1.4.2
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

        /// <summary>
        /// 
        ///  base methods
        /// 
        /// </summary>
        public void TimeCounting()
        {
            try
            {
                DaysLater = (Convert.ToDateTime(LastUpdateDate) - Convert.ToDateTime(DayToday)).Days;
            }
            catch
            {
                Read(_path);
                DaysLater = (Convert.ToDateTime(LastUpdateDate) - Convert.ToDateTime(DayToday)).Days;
            }

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
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                    TasksToday.Add(PlanList[id]);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                id++;
            }

            TaskListBox.ItemsSource = TasksToday;
        }

        public void Save()
        {
            Settings.Default.PlanList = PlanList;
            Settings.Default.BreakBetweenTasks = BreakBetweenTasks;
            Settings.Default.DaysToCompleteTasks = DaysToCompleteTasks;
            Settings.Default.LastUpdateDate = LastUpdateDate;
            Settings.Default.Save();
            File.Delete(_path);
            Write(_path);
        }

        #region write/save methods
        public void Write(string path)
        {
            using StreamWriter writer = new(path);

            foreach (string? task in PlanList)
            {
                writer.WriteLine(task);
            }
            writer.WriteLine("+++1");
            foreach (string? day in BreakBetweenTasks)
            {
                writer.WriteLine(day);
            }
            writer.WriteLine("+++2");
            foreach (string? day in DaysToCompleteTasks)
            {
                writer.WriteLine(day);
            }
            writer.WriteLine("+++3");
            writer.WriteLine(LastUpdateDate);
            writer.WriteLine("+++4");
        }

        public void Read(string path)
        {
            PlanList = [];
            BreakBetweenTasks = [];
            DaysToCompleteTasks = [];

            string? line;
            bool stage_one = true, stage_two = false, stage_three = false, stage_four = false;
            bool end = false;
            string point_stage2 = "+++1", point_stage3 = "+++2", point_stage4 = "+++3", point_end = "+++4";

            using StreamReader reader = new(path);
            try
            {
                while (!end)
                {
                    line = reader.ReadLine();

                    if (line == point_stage2)
                    {
                        stage_two = true;
                        line = reader.ReadLine();
                    }
                    else if (line == point_stage3)
                    {
                        stage_three = true;
                        line = reader.ReadLine();
                    }
                    else if (line == point_stage4)
                    {
                        stage_four = true;
                        line = reader.ReadLine();
                    }
                    else if (line == point_end)
                    {
                        end = true;
                        break;
                    }

                    if (stage_four)
                        LastUpdateDate = line;
                    else if (stage_three)
                        DaysToCompleteTasks.Add(line);
                    else if (stage_two)
                        BreakBetweenTasks.Add(line);
                    else if (stage_one)
                        PlanList.Add(line);
                    else
                        throw new Exception("упс... что то пошло не так при чтении данных из файла");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        private void ClearNewTaskStackPanel()
        {
            NewTaskNameTextBox.Text = "";
            NewTaskDayTextBox.Text = "";
            NewTaskStackPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 
        ///  methods with sender
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

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

        private void Window_Closed(object sender, EventArgs e)
        {
            Save();
        }
    }
}