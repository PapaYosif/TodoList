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

		Database db;

		[ObservableProperty]
		ObservableCollection<aTask> taskList;

		[ObservableProperty]
		aTask openedTask;

		[ObservableProperty]
		string taskInput;

		[ObservableProperty]
		String title;

		public MainViewModel()
		{
			db = new Database();

			TaskList = new ObservableCollection<aTask>();
			title = "Todo List";

			foreach (aTask task in db.GetTasks())
			{
				TaskList.Add(task);
			}
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
			Debug.Write("el teste\n");
			Debug.Write(task.Title + " \n" + task.Description);
		}
	}
}
