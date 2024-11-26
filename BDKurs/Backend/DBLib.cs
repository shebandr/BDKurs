using BDKurs.Pages;
using Microsoft.Extensions.Primitives;
using System;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Xml.Linq;

public static class DBLib
{
    private static string connectionString = "Data Source=FC.db;Version=3;";



    public static void InitializeDatabase()
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string createMembersTable = @"
                CREATE TABLE IF NOT EXISTS Members (
                    MemberID INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    BirthDate DATE NOT NULL
                );";

            ExecuteNonQuery(connection, createMembersTable);

            string createTrainersTable = @"
                CREATE TABLE IF NOT EXISTS Trainers (
                    TrainerID INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    Specialization TEXT NOT NULL
                );";

            ExecuteNonQuery(connection, createTrainersTable);

            string createClassesTable = @"
                CREATE TABLE IF NOT EXISTS Trainings (
                    TrainingID INTEGER PRIMARY KEY AUTOINCREMENT,
                    TrainingName TEXT NOT NULL,
                    TrainerID INTEGER NOT NULL,
                    Day TEXT NOT NULL,
                    FOREIGN KEY (TrainerID) REFERENCES Trainers(TrainerID)
                );";

            ExecuteNonQuery(connection, createClassesTable);

            string createMemberClassesTable = @"
                CREATE TABLE IF NOT EXISTS MemberTrainings (
                    MemberClassID INTEGER PRIMARY KEY AUTOINCREMENT,
                    MemberID INTEGER NOT NULL,
                    TrainingID INTEGER NOT NULL,
                    FOREIGN KEY (MemberID) REFERENCES Members(MemberID),
                    FOREIGN KEY (TrainingID) REFERENCES Trainings(TrainingID)
                );";

            ExecuteNonQuery(connection, createMemberClassesTable);
            if (GetFullTable("Members").Count == 1 && GetFullTable("Trainers").Count == 1 && GetFullTable("Trainings").Count == 1 && GetFullTable("MemberTrainings").Count == 1)
            {
                SeedDatabase(connection);
            }
/*            
*/        }
        
    }


    private static void SeedDatabase(SQLiteConnection connection)
    {
        string insertMembers = @"
            INSERT INTO Members (FirstName, LastName, BirthDate) VALUES
            ('Алексей', 'Смирнов', '1992-03-10'),
            ('Екатерина', 'Васильева', '1987-07-18'),
            ('Дмитрий', 'Козлов', '1995-11-25'),
            ('Ольга', 'Николаева', '1980-09-05'),
            ('Сергей', 'Морозов', '1998-04-30'),
            ('Мария', 'Лебедева', '1983-12-12'),
            ('Александр', 'Петров', '1991-06-20'),
            ('Анна', 'Иванова', '1989-08-14'),
            ('Игорь', 'Соколов', '1994-02-28'),
            ('Татьяна', 'Федорова', '1986-10-07'),
            ('Антон', 'Алексеев', '1988-11-30');";

        ExecuteNonQuery(connection, insertMembers);

        string insertTrainers = @"
            INSERT INTO Trainers (FirstName, LastName, Specialization) VALUES
            ('Михаил', 'Новиков', 'Йога'),
            ('Дмитрий', 'Дмитриев', 'Тяжелая атлетика'),
            ('Олег', 'Антонов', 'Футбол'),
            ('Роман', 'Кузьмин', 'Бокс'),
            ('Денис', 'Денисов', 'Спортивная стрельба');";

        ExecuteNonQuery(connection, insertTrainers);

        string insertClasses = @"
            INSERT INTO Trainings (TrainingName, TrainerID, Day) VALUES
            ('Занятие йогой', 1, 'Понедельник'),
            ('Силовые упражнения', 2, 'Вторник'),
            ('Футбольная тренировка', 3, 'Среда'),
            ('Секция Бокса', 4, 'Четверг'),
            ('Тир', 5, 'Пятница'),
            ('Тир', 5, 'Суббота');";

        ExecuteNonQuery(connection, insertClasses);

        string insertMemberClasses = @"
            INSERT INTO MemberTrainings (MemberID, TrainingID) VALUES
            (1, 1),
            (2, 1),
            (3, 2),
            (4, 2),
            (7, 2),
            (6, 3),
            (5, 4),
            (8, 4),            
            (9, 3),
            (10, 4),
            (11, 4),
            (2, 5),
            (3, 6);";

        ExecuteNonQuery(connection, insertMemberClasses);
    }
        public static void AddNewRow(string table, List<string> columns, List<string> data)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            columns.RemoveAt(0);
            string columnsString = string.Join(", ", columns);

            string dataString = string.Join(", ", data.Select(d => $"'{d.Replace("'", "''")}'"));

            string query = $"INSERT INTO {table} ({columnsString}) VALUES ({dataString});";
            Console.WriteLine(query);
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
            Console.WriteLine($"Error: {ex.Message}");
        }

        return data;
    }


    public static List<List<string>> ExecuteQueryWithReturn(string sql)
    {
        List<List<string>> data = new List<List<string>>();

        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

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
            Console.WriteLine($"Error: {ex.Message}");
        }

        return data;
    }


}