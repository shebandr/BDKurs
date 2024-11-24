using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BDKurs.Pages
{
    public class IndexModel : PageModel
    {
        public List<List<string>> testList = new List<List<string>>();
        public List<string> Headers = new List<string>();
        private readonly ILogger<IndexModel> _logger;
        public string Table { get; set; } = "Members";
        public string table = "Members";
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            DBLib.InitializeDatabase();

            
            testList = DBLib.GetFullTable(table);

            _logger.LogInformation("Data count: {Count}", testList.Count);
            if (testList.Count > 0)
            {
                Headers = testList[0];
                testList.RemoveAt(0); 
            }
            else
            {
                Headers = new List<string>();
            }
            /*DBLib.AddNewRow("Members", new List<string> { "FirstName", "LastName", "BirthDate" }, new List<string> { "John", "Doe", "1985-05-15" });*/
        }
        public void OnPost(string action)
        {

            Console.WriteLine($"Button clicked: {action}");
            table = action;
            Table = action;
            switch (action)
            {
                case "Members":
                    testList = DBLib.GetFullTable("Members");
                    break;
                case "Trainers":
                    testList = DBLib.GetFullTable("Trainers");
                    break;
                case "Classes":
                    testList = DBLib.GetFullTable("Classes");
                    break;
                case "MemberClasses":
                    testList = DBLib.GetFullTable("MemberClasses");
                    break;
                default:
                    testList = DBLib.GetFullTable("Members");
                    break;
            }
            Headers = testList[0];

        }
        public IActionResult OnPostAddRecord()
        {
            Table = Request.Form["Table"];
            // Получаем данные из формы
            Headers = DBLib.GetFullTable(Table)[0];
            
            var formData = new Dictionary<string, string>();
            Console.WriteLine("НУ ТУТ ДОЛЖНО ЖЕ ЧТО-ТО ВЫВЕСТИСЬ");
            Console.WriteLine(Table);
            Console.WriteLine(Headers.Count);
            foreach (var header in Headers)
            {
                if (Headers[0] != header)
                {
                    var value = Request.Form[header];
                    if (string.IsNullOrEmpty(value))
                    {
                        ModelState.AddModelError(header, $"The {header} field is required.");
                    }
                    else if (header == "BirthDate" )
                    {
                        // Преобразуем дату в формат yyyy-MM-dd HH:mm:ss
                        value = ConvertDateToSqlDateTime(value);
                    }
                    formData[header] = value;
                    Console.WriteLine($"{header} {formData[header]}");
                }
            }

            // Проверка на ошибки валидации
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Вывод данных для отладки
            Console.WriteLine($"Selected Table: {Table}");
            Console.WriteLine($"Headers: {string.Join(", ", Headers)}");
            Console.WriteLine($"Data: {string.Join(", ", formData.Values)}");

            // Вызываем функцию для добавления новой записи
            DBLib.AddNewRow(Table, Headers, new List<string>(formData.Values));

            // Перенаправляем на страницу с обновленными данными
            return RedirectToPage();
        }

        public static string ConvertDateToSqlDateTime(string dateString)
        {
            // Проверка на пустую строку
            if (string.IsNullOrEmpty(dateString))
            {
                throw new ArgumentException("Date string cannot be null or empty.");
            }

            // Преобразование строки в DateTime
            DateTime date = DateTime.Parse(dateString);

            // Форматирование в формат yyyy-MM-dd HH:mm:ss
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}

