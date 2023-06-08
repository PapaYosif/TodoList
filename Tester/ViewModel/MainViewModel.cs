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
        [ObservableProperty]
        int id;

        public MainViewModel()
        {
            TaskOpened = false;
            db = new Database();

            TaskList = new ObservableCollection<aTask>();
            title = "Todo List";
            RefreshTasks();
        }


        void RefreshTasks()
        {
            var tasks = db.getTasks();

            TaskList.Clear();

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
            if (task != null)
            {
                Debug.WriteLine(task.Id);
                db.editTask(task);
                TaskOpened = false;
                RefreshTasks();
            }
        }

        [RelayCommand]
        void addTask()
        {
            if (string.IsNullOrEmpty(TaskInput))
            {
                return;
            }
            db.createTask(TaskInput,"");
            Debug.WriteLine("test");
            TaskInput = string.Empty;
            RefreshTasks();
        }

        [RelayCommand]
        void SyncDB()
        {
            Debug.Write("syncysinc");
        }

        [RelayCommand]
        void OpenTask(aTask task)
        {
            Debug.WriteLine(task.Id);
            OpenedTask = task;
            TaskOpened = true;
        }

        [RelayCommand]
        void doneCheck(aTask task)
        {
            if(!task.IsDone)
            {
                db.closeTask(task);
            }
            else
            {
                db.openTask(task);
            }
        }
    }
}
