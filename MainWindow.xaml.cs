using System;
using System.ComponentModel;
using System.Windows;

namespace WorkTimeLog
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        const string DB_FILENAME = WorkTimeLogEntryStore.DB_FILENAME;
        const string DB_COLLECTION = WorkTimeLogEntryStore.DB_COLLECTION;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WorkState type;
            if (RadioBeginWork.IsChecked == true)
            {
                type = WorkState.上班;
            }
            else if (RadioEndWork.IsChecked == true)
            {
                type = WorkState.下班;
            }
            else
            {
                return;
            }
            (this.FindResource("LogStore") as WorkTimeLogEntryStore).Insert(new WorkTimeLogEntry() { Time = DateTime.UtcNow, Type = type });
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            (this.FindResource("LogStore") as WorkTimeLogEntryStore).Clear();
        }
    }
}
