using Microsoft.Data.Sqlite;
using System.Diagnostics;
using Tester.Model;

namespace Tester.Services
{
    public class Database
    {


        private aTask task;

        private SqliteConnection ldb;
        private bool isOpen;

        public Database()
        {
            // create sqlite database for local task storage
            ldb = new SqliteConnection("data Source=taskList.db");
            Debug.Write(Directory.GetCurrentDirectory());
            try
            {
                ldb.Open();

                this.isOpen = true;
                var command = ldb.CreateCommand();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS tasks (id int AUTO_INCREMENT, Title varchar(16),Description varchar(128), isDone INTEGER, PRIMARY KEYS(id));";

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

                command.CommandText = @"INSERT INTO tasks (`title`,`description`,`isDone`) VALUES ($Title,$Description,0)";

                command.Parameters.AddWithValue("$Title", Title);
                command.Parameters.AddWithValue("$Description", Description);

                command.ExecuteNonQuery();
            }
        }

        public void editTask(aTask task)
        {
            if (this.isOpen)
            {
                var command = ldb.CreateCommand();

                command.CommandText = @"UPDATE tasks SET `Title`=$Title, `Description`=$Description WHERE `id`=$id;";

                command.Parameters.AddWithValue("$id", task.Id);
                command.Parameters.AddWithValue("$Title", task.Title);
                command.Parameters.AddWithValue("$Description", task.Description);

                command.ExecuteNonQuery();
            }
        }

        public void closeTask(aTask task)
        {
            if (this.isOpen)
            {
                var command = ldb.CreateCommand();

                command.CommandText = @"UPDATE tasks SET `isDone`=1 WHERE `id`=$id;";

                command.Parameters.AddWithValue("$id", task.Id);

                command.ExecuteNonQuery();
            }
        }
        public void openTask(aTask task)
        {
            if (this.isOpen)
            {
                var command = ldb.CreateCommand();

                command.CommandText = @"UPDATE tasks SET `isDone`=0 WHERE `id`=$id;";

                command.Parameters.AddWithValue("$id", task.Id);

                command.ExecuteNonQuery();
            }
        }

        public List<aTask> getTasks()
        {
            if (this.isOpen)
            {
                List<aTask> taskList = new List<aTask>();
                var command = ldb.CreateCommand();

                command.CommandText = @"SELECT * FROM tasks ORDER BY isDone ASC";

                var reader = command.ExecuteReader();


                //while it has stuff to read. aka rows
                while (reader.Read())
                {
                    task = new aTask();
                    task.Id = reader.GetInt32(1);
                    task.Title = String.Format("{0}", reader.GetString(1));
                    task.Description = String.Format("{0}", reader.GetString(2));
                    task.IsDone = reader.GetBoolean(3);
                    taskList.Add(task);

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
