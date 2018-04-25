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
            }
         }
    }
}
