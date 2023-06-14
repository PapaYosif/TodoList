using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Tester.Model;
using Tester.Services;

namespace Tester.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        // application freezes not sure why last thing i did was add database stuff and try catches

        Database db;

        Settings settings;


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
        string error;

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

            settings = new Settings();

            List<string> settingsList = settings.getSettings();


            TaskOpened = false;
            db = new Database();

            if (settingsList != null)
            {
                db.setSettings(settingsList);
            }

            TaskList = new ObservableCollection<aTask>();
            loadSettings();
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


        [RelayCommand]
        void SaveTask(aTask task)
        {
            if (task != null)
            {
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
            TaskInput = string.Empty;
            DescriptionInput = string.Empty;
            RefreshTasks();
        }

        [RelayCommand]
        void SyncDB()
        {
            string returnWaarde = db.syncDB();
            if (returnWaarde == "ok")
            {
                RefreshTasks();
            }
            else
            {
                Error = returnWaarde;
            }
        }

        [RelayCommand]
        void OpenTask(aTask task)
        {
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
            List<string> settingsArray = new List<string>();

            if (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(ServerIP) || !string.IsNullOrEmpty(DbName) || !string.IsNullOrEmpty(DbUser))
            {
                settingsArray.Add(Name);
                settingsArray.Add(ServerIP);
                settingsArray.Add(DbName);
                settingsArray.Add(DbUser);
                settingsArray.Add(DbPassword);


                settings.setSettings(settingsArray);

                db.setSettings(settings.getSettings());
            }

            else
            {
                Error = "please fill in all fields.";
            }

        }


        void loadSettings()
        {
            List<string> settingsList = settings.getSettings();

            if (settingsList != null)
            {
                Name = settingsList[0];
                ServerIP = settingsList[1];
                DbName = settingsList[2];
                DbUser = settingsList[3];
                DbPassword = settingsList[4];
            }

        }


    }
}
