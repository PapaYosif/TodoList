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
        String name;
        [ObservableProperty]
        String serverIP;
        [ObservableProperty]
        String dbName;
        [ObservableProperty]
        String dbUser;
        [ObservableProperty]
        String dbPassword;


        [ObservableProperty]
        ObservableCollection<aTask> taskList;

        [ObservableProperty]
        aTask openedTask;

        [ObservableProperty]
        string taskInput;
        [ObservableProperty]
        string descriptionInput;

        [ObservableProperty]
        bool taskOpened;

        [ObservableProperty]
        String title;
        [ObservableProperty]
        String description;

        [ObservableProperty]
        bool isDone;

        public MainViewModel()
        {
            TaskOpened = false;
            db = new Database();

            TaskList = new ObservableCollection<aTask>();
            RefreshTasks();
        }


        void RefreshTasks()
        {
            var tasks = db.GetTasks();

            TaskList.Clear();

            if (tasks != null)
            {

                foreach (aTask task in tasks)
                {
                    TaskList.Add(task);
                }
            }
        }


        public void secretSaveTask(aTask task)
        {
            if (task != null)
            {
                Debug.WriteLine(task.Id);
                Debug.WriteLine(task.Title);
                db.editTask(task);
                TaskOpened = false;
                RefreshTasks();
            }
        }


        [RelayCommand]
        void SaveTask(aTask task)
        {
            if (task != null)
            {
                Debug.WriteLine(task.Id);
                Debug.WriteLine(task.Title);
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
            db.createTask(TaskInput, DescriptionInput);
            Debug.WriteLine("test");
            TaskInput = string.Empty;
            DescriptionInput = string.Empty;
            RefreshTasks();
        }

        [RelayCommand]
        void SyncDB()
        {
            if (db.syncDB() == "ok")
            {
                RefreshTasks();
            }
        }

        [RelayCommand]
        void OpenTask(aTask task)
        {
            Debug.WriteLine(task.IsDone);
            OpenedTask = task;
            TaskOpened = true;
        }

        [RelayCommand]
        void deleteDoneTasks()
        {
            db.deleteCompleted();
            RefreshTasks();
        }

        [RelayCommand]
        void doneCheck(aTask task)
        {
            Debug.Write("CLIKKER");
            if (!task.IsDone)
            {
                db.closeTask(task);
            }
            else
            {
                db.openTask(task);
            }
        }


        [RelayCommand]
        void saveSettings()
        {

        }



    }
}
