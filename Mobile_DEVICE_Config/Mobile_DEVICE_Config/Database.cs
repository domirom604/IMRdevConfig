using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mobile_DEVICE_Config
{
    public class Database
    {
        private readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Data>().Wait();
        }
        public Task<List<Data>> GetDataAsync()
        {
            return _database.Table<Data>().ToListAsync();
        }
        public Task<int> SaveDataAsync(Data data)
        {
            return _database.InsertAsync(data);
        }

        public Task DeleteByIdAsync(int Id)
        {
            return _database.ExecuteAsync("DELETE FROM [Data] WHERE Id = ?", Id);

        }

    }
}
