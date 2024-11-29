using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BDKurs.Pages
{
    public class IndexModel : PageModel
    {
        public List<string> Errors = new List<string>();
        
        public List<string> Headers = new List<string>();
        private readonly ILogger<IndexModel> _logger;
        public string Table { get; set; } = "Members";
        public string table = "Members";
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public List<List<string>> TableList = new List<List<string>>();
        public List<List<string>> MembersTable = new List<List<string>>();
        public List<List<string>> TrainersTable = new List<List<string>>();
        public List<List<string>> TrainingsTable = new List<List<string>>();
        public List<List<string>> MemberTrainingsTable = new List<List<string>>();

        public void OnGet()
        {
            DBLib.InitializeDatabase();

            MembersTable = DBLib.GetFullTable("Members");
            TrainersTable = DBLib.GetFullTable("Trainers");
            TrainingsTable = DBLib.GetFullTable("Trainings");
            MemberTrainingsTable = DBLib.GetFullTable("MemberTrainings");
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
            MembersTable = DBLib.GetFullTable("Members");
            TrainersTable = DBLib.GetFullTable("Trainers");
            TrainingsTable = DBLib.GetFullTable("Trainings");
            MemberTrainingsTable = DBLib.GetFullTable("MemberTrainings");
            Headers = TableList[0];
            TableList.RemoveAt(0);
        }
		public IActionResult OnPostAddRecord()
		{
            Console.WriteLine(Errors.Count);
            Errors = new List<string>();


			Table = Request.Form["Table"];
			Headers = DBLib.GetFullTable(Table)[0];

			var formData = new Dictionary<string, string>();
			Console.WriteLine(Table);
			Console.WriteLine(Headers.Count);

			foreach (var header in Headers)
			{
				if (Headers[0] != header)
				{
					var value = Request.Form[header];
					if (string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value))
					{
                        Errors.Add(header +  " поле не должно быть пустым");
					}
					else
					{
						if (header == "BirthDate")
						{
							value = ConvertDateToSqlDateTime(value);
						}

						formData[header] = value;
						Console.WriteLine($"{header} {formData[header]}");
					}
				}
			}
            Console.WriteLine(ModelState.IsValid);

			if (Errors.Count != 0)
			{
                return RedirectToPage();
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

