using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Reflection.PortableExecutable;

namespace BDKurs.Pages
{
    public class QueryModel : PageModel
    {
        private readonly ILogger<QueryModel> _logger;

        public List<List<string>> Table = new List<List<string>>();
        public List<string> Headers = new List<string>();
        public QueryModel(ILogger<QueryModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }


        public void OnPost(string QueryButton)
        {

            Console.WriteLine($"Button clicked: {QueryButton}");
            int query = Int32.Parse(QueryButton);
            string Day = "";

            string firstQuery = "SELECT t.TrainingName, t.Day, tr.FirstName AS TrainerFirstName, tr.LastName AS TrainerLastName FROM Members m JOIN MemberTrainings mt ON m.MemberID = mt.MemberID JOIN Trainings t ON mt.TrainingID = t.TrainingID JOIN Trainers tr ON t.TrainerID = tr.TrainerID WHERE m.FirstName = 'Екатерина' AND m.LastName = 'Васильева';";

            string secondQuery = "SELECT m.FirstName, m.LastName FROM Members m JOIN MemberTrainings mt ON m.MemberID = mt.MemberID GROUP BY m.MemberID HAVING COUNT(mt.TrainingID) > 1;";
            
            string thirdQuery = "SELECT DISTINCT m.FirstName, m.LastName FROM Members m JOIN MemberTrainings mt ON m.MemberID = mt.MemberID JOIN Trainings t ON mt.TrainingID = t.TrainingID WHERE t.Day = '";



			switch (query)
            {
                case 1:
                    Table = DBLib.ExecuteQueryWithReturn(firstQuery);
                    break;
                case 2:
                    Table = DBLib.ExecuteQueryWithReturn(secondQuery);
                    break;
                case 3:
                    Day = Request.Form["DayInput"];
                    Console.WriteLine(thirdQuery + Day + "';");
					Table = DBLib.ExecuteQueryWithReturn(thirdQuery + Day + "';");
					break;
            }
            Headers = Table[0];
            Table.RemoveAt(0);

        }


    }

}
