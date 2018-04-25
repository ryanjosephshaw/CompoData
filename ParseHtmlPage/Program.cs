using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace ParseHtmlPage
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: This is not taking into account any human error. We need to come up with test cases and do error handling.
            var headerRows = new List<CompoDataRow>();
            var filePath = args[0];
            var doc = new HtmlDocument();
            doc.Load(filePath);
            var rows = doc.DocumentNode.SelectNodes("//tr[@id]");
            foreach (var row in rows) {
                var phonePattern = new Regex("[\n<p>â˜ ]");
                var addressPattern = new Regex("[â˜ž]");
                var headerRow = new CompoDataRow();
                headerRow.Id = Convert.ToInt32(row.Id);
                var rowCells = doc.DocumentNode.SelectNodes("//tr[@id=" + headerRow.Id.ToString() + "]/td");
                headerRow.Case = rowCells[1].InnerText.Replace("add comment", "").Trim();
                var descriptiontext = rowCells[2].InnerHtml.Split(new string[] { "<br>" }, StringSplitOptions.None);
                headerRow.Phone = phonePattern.Replace(descriptiontext[0], "");
                headerRow.Address = addressPattern.Replace(descriptiontext[1], "").Trim();
                headerRow.City = rowCells[3].InnerText;
                headerRow.SubContractor = rowCells[4].InnerText;
                headerRow.Team = rowCells[5].InnerText;
                headerRow.AssignedAt = rowCells[6].InnerText;
                headerRow.DaysSinceAssigned = rowCells[7].InnerText;
                headerRow.CompletedAt = rowCells[8].InnerText;
                headerRow.Status = rowCells[9].InnerText;
                headerRows.Add(headerRow);
                var detailRows = doc.DocumentNode.SelectNodes("//table[contains(@id, 'subtasks-table-" + headerRow.Id + "')]//tr");
                foreach (var detailRow in detailRows) {
                    if (!detailRow.Equals(detailRows.LastOrDefault()))
                    {
                        var numberPattern = new Regex(@"#(\d+)");
                        var headerDetailRow = new DetailRow();
                        var children = detailRow.ChildNodes;
                        var descriptionLine = children[1].InnerText;
                        headerDetailRow.Number = Convert.ToInt32(numberPattern.Match(descriptionLine).Groups[1].Value);
                        //TODO: pull description and quantity out. Need to link the headerRow with detail by having a collection of detail.
                        var teamLine = children[2].InnerText;
                        headerDetailRow.Status = children[3].InnerText.Trim();
                    }
                }
            }
         }
    }
}
