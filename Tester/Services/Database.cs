using Microsoft.Data.Sqlite;
using MySqlConnector;
using System.Diagnostics;
using Tester.Model;

namespace Tester.Services
{
    public class Database
    {


        private MySqlConnection mysqlConnection;

        private String databaseName;

        private int idLength;

        private string name;
        private String server;

        private String user;
        private String password;

        private aTask task;

        private SqliteConnection ldb;
        private bool isOpen;

        public Database()
        {


            idLength = 999999999;
            // create sqlite database for local task storage
            string mainDir = FileSystem.Current.AppDataDirectory;

            ldb = new SqliteConnection("data Source=" + mainDir + "\\taskList.db");
            Debug.Write(Directory.GetCurrentDirectory());
            try
            {
                ldb.Open();

                this.isOpen = true;
                var command = ldb.CreateCommand();

                command.CommandText = @"CREATE TABLE IF NOT EXISTS tasks (id BIGINT NOT NULL,Title varchar(32) NOT NULL,Description varchar(4096), isDone INTEGER NOT NULL, UNIQUE(id));";

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }




        public string connectRemote()
        {
            if (!string.IsNullOrEmpty(this.name) || !string.IsNullOrEmpty(this.server) || !string.IsNullOrEmpty(this.user) || !string.IsNullOrEmpty(this.password) || !string.IsNullOrEmpty(this.databaseName))
            {


                mysqlConnection = new MySqlConnection($"database={databaseName};server={server};uid={user};password={password};");

                try
                {
                    mysqlConnection.Open();
                    // for first time connection
                    string sql = @$"CREATE TABLE IF NOT EXISTS `tasks_{name}` (`id` INT(14) NOT NULL , `Title` VARCHAR(32) NOT NULL , `Description` VARCHAR(4096) NOT NULL , `isDone` INT NOT NULL , PRIMARY KEY(`id`));";

                    MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
                    cmd.ExecuteNonQuery();


                    return "ok";
                }
                catch (Exception ex)
                {
                    return "Doet nie" + ex;


                }

            }
            return "no settings";
        }




        public void setSettings(List<string> settings)
        {
            if (settings != null && settings.Count > 0)
            {
                this.name = settings[0];
                this.server = settings[1];
                this.databaseName = settings[2];
                this.user = settings[3];
                this.password = settings[4];
            }
        }

        public void createTask(string Title, string Description)
        {
            if (this.isOpen)
            {
                if (String.IsNullOrEmpty(Description))
                {
                    Description = "";
                }

                Random rnd = new Random();

                int id = rnd.Next(0, idLength);

                var command = ldb.CreateCommand();

                while (true)
                {
                    command.CommandText = $"SELECT * FROM tasks WHERE `id`={id}";
                    var reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        id = rnd.Next(0, idLength);
                    }
                    else
                    {
                        reader.Close();
                        break;
                    }
                    reader.Close();
                }

                command.Parameters.AddWithValue("$id", id);

                command.Parameters.AddWithValue("$Title", Title);
                command.Parameters.AddWithValue("$Description", Description);


                command.CommandText = @"INSERT INTO tasks (`id`,`title`,`description`,`isDone`) VALUES ($id,$Title,$Description,0)";

                command.ExecuteNonQuery();
                syncDB();
            }
        }

        public void editTask(aTask task)
        {
            if (this.isOpen)
            {
                var command = ldb.CreateCommand();

                command.CommandText = @"UPDATE tasks SET `Title`=$Title, `Description`=$Description, `isDone`=$isDone WHERE `id`=$id;";

                command.Parameters.AddWithValue("$id", task.Id);
                command.Parameters.AddWithValue("$Title", task.Title);
                if (task.IsDone)
                {
                    command.Parameters.AddWithValue("$isDone", 1);
                }
                else
                {
                    command.Parameters.AddWithValue("$isDone", 0);
                }
                command.Parameters.AddWithValue("$Description", task.Description);

                command.ExecuteNonQuery();
                syncDB();
            }
        }


        public void deleteCompleted()
        {

            syncDB();
            var command = ldb.CreateCommand();

            command.CommandText = @"DELETE FROM tasks WHERE `isDone`=1";

            command.ExecuteNonQuery();

            if (connectRemote() == "ok")
            {
                string sql = $@"DELETE FROM tasks_{name} WHERE `isDone`=1";
                MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();
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

                syncDB();
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
                syncDB();
            }
        }



        public void setSettings(String name, String server, String username, String password, String dbname)
        {
            this.name = name;
            this.server = server;
            this.user = username;
            this.password = password;
            this.databaseName = dbname;
        }




        public String syncDB()
        {
            if (this.isOpen)
            {
                string returnVal = connectRemote();
                if (returnVal != "ok")
                {
                    return returnVal;
                }


                //get lists


                List<aTask> localTaskList = this.GetTasks();

                List<aTask> remoteTaskList = new List<aTask>();

                List<aTask> syncedList = new List<aTask>();


                string sql = $"SELECT * FROM tasks_{name}";

                MySqlCommand cmd = new MySqlCommand(sql, mysqlConnection);

                MySqlDataReader rdr = cmd.ExecuteReader();

                //convert reader into list
                while (rdr.Read())
                {
                    task = new aTask();
                    task.Id = rdr.GetInt32(0);
                    task.Title = String.Format("{0}", rdr.GetString(1));
                    task.Description = String.Format("{0}", rdr.GetString(2));
                    task.IsDone = rdr.GetBoolean(3);
                    remoteTaskList.Add(task);

                }
                rdr.Close();

                foreach (aTask task in localTaskList)
                {
                    syncedList.Add(task);
                }

                foreach (aTask task in remoteTaskList)
                {

                    if (syncedList.Count(item => item.Id == task.Id) == 0)
                    {
                        syncedList.Add(task);
                    }
                }






                var command = ldb.CreateCommand();

                command.CommandText = @"DELETE FROM tasks";

                command.ExecuteNonQuery();


                sql = @$"DELETE FROM tasks_{name}";

                cmd = new MySqlCommand(sql, mysqlConnection);
                cmd.ExecuteNonQuery();


                // list containing ids that was changed in the foreach loop as changing task.Id in the foreach does not change it in the original list.
                List<int> changedToIds = new List<int>();
                int checkId;

                foreach (aTask task in syncedList)
                {
                    int isdone = 0;
                    if (task.IsDone)
                    {
                        isdone = 1;
                    }
                    else
                    {
                        isdone = 0;
                    }
                    Random rndm = new Random();

                    // id int to check if already exists in list
                    checkId = task.Id;



                    while (syncedList.Count(item => item.Id == checkId) > 1 || changedToIds.Contains(checkId))
                    {
                        Debug.WriteLine(syncedList.Count(item => item.Id == checkId) + "   " + task.Id);
                        checkId = rndm.Next(0, idLength);
                    }

                    task.Id = checkId;
                    changedToIds.Add(checkId);

                    sql = $"INSERT INTO tasks_{name} (`id`,`title`,`description`,`isDone`) VALUES ({task.Id},'{task.Title}','{task.Description}','{isdone}');";

                    command.CommandText = $"INSERT INTO tasks (`id`,`title`,`description`,`isDone`) VALUES ({task.Id},'{task.Title}','{task.Description}','{isdone}')";


                    command.ExecuteNonQuery();

                    cmd = new MySqlCommand(sql, mysqlConnection);
                    cmd.ExecuteNonQuery();


                }
                return "ok";




                //foreach (aTask task2 in remoteTaskList)
                //{
                //if (task.Title == task2.Title)
                //{
                //syncedList.Add(task);
                //break;
                //}

            }
            else
            {
                return "local database not opened?";
            }
        }




        public List<aTask> GetTasks()
        {
            if (this.isOpen)
            {
                List<aTask> taskList = new List<aTask>();
                var command = ldb.CreateCommand();

                command.CommandText = @"SELECT * FROM tasks ORDER BY isDone ASC";
                try
                {
                    var reader = command.ExecuteReader();
                    //while it has stuff to read. aka rows
                    while (reader.Read())
                    {
                        task = new aTask();
                        task.Id = reader.GetInt32(0);
                        task.Title = String.Format("{0}", reader.GetString(1));
                        task.Description = String.Format("{0}", reader.GetString(2));
                        task.IsDone = reader.GetBoolean(3);
                        taskList.Add(task);

                    }


                    return taskList;
                }
                catch (Exception e)
                {
                    Debug.Write(e);
                    return null;
                }


            }
            else
            {
                return null;
            }
        }
    }
}

