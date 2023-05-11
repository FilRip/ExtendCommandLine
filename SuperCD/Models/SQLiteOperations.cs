using System.Data;
using System.Data.SQLite;
using System.IO;

using static ExtendCommandLineLib.ExtensionsCommandLineArguments;

namespace SuperCD.Models
{
    internal class SQLiteOperations
    {
        private SQLiteConnection _database;

        internal SQLiteOperations()
        {
            Connect();
        }

        internal DataTable Result(string query)
        {
            SQLiteCommand command = _database.CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            SQLiteDataAdapter adapter = new(command);
            DataSet ds = new();
            adapter.Fill(ds);
            adapter.Dispose();
            command.Dispose();
            return ds.Tables[0];
        }

        internal void Insert(string fullpath, string name)
        {
            SQLiteCommand command = _database.CreateCommand();
            command.CommandText = $"insert into scan (fullpath, name) values ('{fullpath}', '{name}')";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }

        internal void RemoveSubDir(string fullpath)
        {
            SQLiteCommand command = _database.CreateCommand();
            command.CommandText = $"delete from scan where fullpath like '{fullpath}%'";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }

        private void Connect()
        {
            _database = new SQLiteConnection($"Data Source={ArgumentRepertoireExe() + Path.DirectorySeparatorChar + "supercd.sqlite3;Version=3"}");
            _database.Open();
            DataTable dt = Result("select * from sqlite_master where type='table'");
            if (dt.Rows.Count == 0)
                Create();
        }

        internal void Create()
        {
            SQLiteCommand cmd = _database.CreateCommand();
            cmd.CommandText = "create table scan(id int primary key, name text, fullpath text)";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
        }
    }
}
