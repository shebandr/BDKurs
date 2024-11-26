using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BDKurs.Pages
{
    public class IndexModel : PageModel
    {
        public List<List<string>> TableList = new List<List<string>>();
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

            
            TableList = DBLib.GetFullTable(table);

            _logger.LogInformation("Data count: {Count}", TableList.Count);
            if (TableList.Count > 0)
            {
                Headers = TableList[0];
                TableList.RemoveAt(0); 
            }
            else
            {
                Headers = new List<string>();
            }
        }
        public void OnPost(string action)
        {

            Console.WriteLine($"Button clicked: {action}");
            table = action;
            Table = action;
            switch (action)
            {
                case "Members":
                    TableList = DBLib.GetFullTable("Members");
                    break;
                case "Trainers":
                    TableList = DBLib.GetFullTable("Trainers");
                    break;
                case "Trainings":
                    TableList = DBLib.GetFullTable("Trainings");
                    break;
                case "MemberTrainings":
                    TableList = DBLib.GetFullTable("MemberTrainings");
                    break;
                default:
                    TableList = DBLib.GetFullTable("Members");
                    break;
            }
            
            Headers = TableList[0];
            TableList.RemoveAt(0);
        }
        public IActionResult OnPostAddRecord()
        {
            Table = Request.Form["Table"];
            Headers = DBLib.GetFullTable(Table)[0];

            var formData = new Dictionary<string, string>();
            Console.WriteLine("Õ” “”“ ƒŒÀ∆ÕŒ ∆≈ ◊“Œ-“Œ ¬€¬≈—“»—‹");
            Console.WriteLine(Table);
            Console.WriteLine(Headers.Count);
            foreach (var header in Headers)
            {
                if (Headers[0] != header)
                {
                    var value = Request.Form[header];
                    if (string.IsNullOrEmpty(value))
                    {
                        ModelState.AddModelError(header, $"{header} ÔÓÎÂ ÌÂÓ·ıÓ‰ËÏÓ.");
                    }
                    else if (header == "BirthDate" )
                    {
                        value = ConvertDateToSqlDateTime(value);
                    }
                    formData[header] = value;
                    Console.WriteLine($"{header} {formData[header]}");
                }
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Console.WriteLine($"Selected Table: {Table}");
            Console.WriteLine($"Headers: {string.Join(", ", Headers)}");
            Console.WriteLine($"Data: {string.Join(", ", formData.Values)}");

            DBLib.AddNewRow(Table, Headers, new List<string>(formData.Values));

            return RedirectToPage();
        }

        public static string ConvertDateToSqlDateTime(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                throw new ArgumentException("Date string cannot be null or empty.");
            }
            DateTime date = DateTime.Parse(dateString);
            return date.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}

