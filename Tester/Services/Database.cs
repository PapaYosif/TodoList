using Microsoft.Data.Sqlite;
using System.Diagnostics;
using Tester.Model;

namespace Tester.Services
{
    public class Database
    {

        private List<aTask> taskList = new List<aTask>();

        private aTask task;

        private SqliteConnection ldb;
        private bool isOpen;

        public Database()
        {
            // create sqlite database for local task storage
            ldb = new SqliteConnection("data Source=taskList.db");
            try
            {
                ldb.Open();

                this.isOpen = true;
                var command = ldb.CreateCommand();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS tasks (id int, Title varchar(16),Description varchar(128), isDone INTEGER);";

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void createTask(string Title, string Description)
        {
            if (this.isOpen)
            {
                var command = ldb.CreateCommand();

                command.CommandText = @"INSERT INTO tasks VALUES ( `Title`= $Title, `Description`=$Description, `isDone`=0)";

                command.Parameters.AddWithValue("$Title", Title);
                command.Parameters.AddWithValue("$Description", Description);

                command.ExecuteNonQuery();
            }
        }

        public void editTask(int id, string Title, string Description)
        {
            if (this.isOpen)
            {
                var command = ldb.CreateCommand();

                command.CommandText = @"UPDATE tasks SET `Title` = $Title, `Description`=$Description WHERE `id`=$id";

                command.Parameters.AddWithValue("$id", id);
                command.Parameters.AddWithValue("$Title", Title);
                command.Parameters.AddWithValue("$Description", Description);

                command.ExecuteNonQuery();
            }
        }
        public List<aTask> getTasks()
        {
            if (this.isOpen)
            {
                var command = ldb.CreateCommand();

                command.CommandText = @"SELECT * FROM tasks";

                var reader = command.ExecuteReader();

                task = new aTask();

                //while it has stuff to read. aka rows
                while (reader.Read())
                {
                    task.Id = (int)reader[0];
                    task.Title = String.Format("{0}", reader[1]);
                    task.Description = String.Format("{0}", reader[2]);
                    task.IsDone = (bool)reader[3];
                    this.taskList.Add(task);

                }


                return taskList;
            }
            else
            {
                return null;
            }
        }
    }
}
