using BDKurs.Pages;
using Microsoft.Extensions.Primitives;
using System;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Xml.Linq;

public static class DBLib
{
    // Строка подключения к базе данных SQLite
    private static string connectionString = "Data Source=FC.db;Version=3;";



    public static void InitializeDatabase()
    {

        // Создание базы данных и таблиц
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Создание таблицы Members (Участники)
            string createMembersTable = @"
                CREATE TABLE IF NOT EXISTS Members (
                    MemberID INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    BirthDate DATE NOT NULL
                );";

            ExecuteNonQuery(connection, createMembersTable);

            // Создание таблицы Trainers (Тренеры)
            string createTrainersTable = @"
                CREATE TABLE IF NOT EXISTS Trainers (
                    TrainerID INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    Specialization TEXT NOT NULL
                );";

            ExecuteNonQuery(connection, createTrainersTable);

            // Создание таблицы Classes (Занятия)
            string createClassesTable = @"
                CREATE TABLE IF NOT EXISTS Classes (
                    ClassID INTEGER PRIMARY KEY AUTOINCREMENT,
                    ClassName TEXT NOT NULL,
                    TrainerID INTEGER NOT NULL,
                    Day TEXT NOT NULL,
                    FOREIGN KEY (TrainerID) REFERENCES Trainers(TrainerID)
                );";

            ExecuteNonQuery(connection, createClassesTable);

            // Создание таблицы MemberClasses (Участники занятий)
            string createMemberClassesTable = @"
                CREATE TABLE IF NOT EXISTS MemberClasses (
                    MemberClassID INTEGER PRIMARY KEY AUTOINCREMENT,
                    MemberID INTEGER NOT NULL,
                    ClassID INTEGER NOT NULL,
                    FOREIGN KEY (MemberID) REFERENCES Members(MemberID),
                    FOREIGN KEY (ClassID) REFERENCES Classes(ClassID)
                );";

            ExecuteNonQuery(connection, createMemberClassesTable);
            if (GetFullTable("Members").Count == 1 && GetFullTable("Trainers").Count == 1 && GetFullTable("Classes").Count == 1 && GetFullTable("MemberClasses").Count == 1)
            {
                SeedDatabase(connection);
            }
/*            
*/        }
        
    }


    private static void SeedDatabase(SQLiteConnection connection)
    {
        // Заполнение таблицы Members
        string insertMembers = @"
            INSERT INTO Members (FirstName, LastName, BirthDate) VALUES
            ('John', 'Doe', '1985-05-15'),
            ('Jane', 'Smith', '1990-08-22'),
            ('Alice', 'Johnson', '1988-11-30');";

        ExecuteNonQuery(connection, insertMembers);

        // Заполнение таблицы Trainers
        string insertTrainers = @"
            INSERT INTO Trainers (FirstName, LastName, Specialization) VALUES
            ('Michael', 'Brown', 'Yoga'),
            ('Emily', 'Davis', 'Strength Training'),
            ('David', 'Wilson', 'Pilates');";

        ExecuteNonQuery(connection, insertTrainers);

        // Заполнение таблицы Classes
        string insertClasses = @"
            INSERT INTO Classes (ClassName, TrainerID, Day) VALUES
            ('Yoga Class', 1, 'Monday'),
            ('Strength Training', 2, 'Sunday'),
            ('Pilates', 3, 'Tuesday');";

        ExecuteNonQuery(connection, insertClasses);

        // Заполнение таблицы MemberClasses
        string insertMemberClasses = @"
            INSERT INTO MemberClasses (MemberID, ClassID) VALUES
            (1, 1),
            (2, 2),
            (3, 3);";

        ExecuteNonQuery(connection, insertMemberClasses);
    }
        public static void AddNewRow(string table, List<string> columns, List<string> data)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            // Формирование строки с именами столбцов
            columns.RemoveAt(0);
            string columnsString = string.Join(", ", columns);

            // Формирование строки со значениями данных с экранированием
            string dataString = string.Join(", ", data.Select(d => $"'{d.Replace("'", "''")}'"));

            // Формирование запроса
            string query = $"INSERT INTO {table} ({columnsString}) VALUES ({dataString});";
            Console.WriteLine(query);
            // Выполнение запроса
            ExecuteNonQuery(connection, query);
        }
    }


    private static void ExecuteNonQuery(SQLiteConnection connection, string query)
    {
        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.ExecuteNonQuery();
        }
    }



    // Метод для создания пустой таблицы
    public static void CreateTable()
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string sql = "CREATE TABLE IF NOT EXISTS MyTable (Id INTEGER PRIMARY KEY, Name TEXT)";

            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
            Console.WriteLine("функция отработала");
        }
    }

    // Метод для выполнения произвольного SQL-запроса
    public static void ExecuteQuery(string query)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }


    public static List<List<string>> GetFullTable(string table)
    {
        List<List<string>> data = new List<List<string>>();

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string sql = $"SELECT * FROM {table}";
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        List<string> fieldNames = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fieldNames.Add(reader.GetName(i));
                        }
                        data.Add(fieldNames);
                        while (reader.Read())
                        {
                            List<string> rowData = new List<string>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                rowData.Add(reader[i].ToString());
                            }

                            data.Add(rowData);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибки или вывод в консоль
            Console.WriteLine($"Error: {ex.Message}");
        }

        return data;
    }
}