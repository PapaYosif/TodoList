using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Tester.Model;
using Tester.Services;

namespace Tester.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        // application freezes not sure why last thing i did was add database stuff and try catches

        Database db;

        [ObservableProperty]
        ObservableCollection<aTask> taskList;

        [ObservableProperty]
        aTask openedTask;

        [ObservableProperty]
        string taskInput;

        [ObservableProperty]
        bool taskOpened;

        [ObservableProperty]
        String title;
        [ObservableProperty]
        String description;

        public MainViewModel()
        {
            TaskOpened = false;
            db = new Database();

            TaskList = new ObservableCollection<aTask>();
            title = "Todo List";
            var tasks = db.getTasks();

            if (tasks != null)
            {

                foreach (aTask task in tasks)
                {
                    TaskList.Add(task);
                }
            }
        }

        [RelayCommand]
        void SaveTask(aTask task)
        {
            Debug.WriteLine(task.Description);
            db.editTask(task.Id, task.Title, task.Description);
            TaskOpened = false;
        }

        [RelayCommand]
        void addTask()
        {
            if (string.IsNullOrEmpty(TaskInput))
            {
                return;
            }
            //db.addTask(taskInput);
            Debug.WriteLine("test");
            //TaskInput = string.Empty;
        }

        [RelayCommand]
        void SyncDB()
        {
            Debug.Write("syncysinc");
        }

        [RelayCommand]
        void OpenTask(aTask task)
        {
            OpenedTask = task;
            TaskOpened = true;
        }
    }
}
