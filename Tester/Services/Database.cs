using Tester.Model;

namespace Tester.Services
{
	public class Database
	{

		List<aTask> taskList = new List<aTask>();

		aTask task;

		public List<aTask> GetTasks()
		{

			for (int i = 0; i < 20; i++)
			{
				task = new aTask();
				task.Title = "task" + i;
				taskList.Add(task);
			}

			return taskList;
		}
	}
}
