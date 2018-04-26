using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.IO;

namespace ParseHtmlPage
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: This is not taking into account any human error. We need to come up with test cases and do error handling.
            List<CompoDataRow> projects = ScrapeHtml(args[0]);
            GenerateCSV(projects, Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]) + ".csv");
         }

        public static List<CompoDataRow> ScrapeHtml(string fileName) {
            var headerRows = new List<CompoDataRow>();
            var doc = new HtmlDocument();
            doc.Load(fileName);
            var rows = doc.DocumentNode.SelectNodes("//tr[@id]");
            foreach (var row in rows)
            {
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
                headerRow.Details = new List<DetailRow>();
                var detailRows = doc.DocumentNode.SelectNodes("//table[contains(@id, 'subtasks-table-" + headerRow.Id + "')]//tr");
                foreach (var detailRow in detailRows)
                {
                    if (!detailRow.Equals(detailRows.LastOrDefault()))
                    {
                        var numberPattern = new Regex(@"#(\d+)");
                        var quantityPattern = new Regex(@"Qty: (\d+)");
                        var headerDetailRow = new DetailRow();
                        var children = detailRow.ChildNodes;
                        var descriptionLine = children[1].InnerText;
                        headerDetailRow.Number = Convert.ToInt32(numberPattern.Match(descriptionLine).Groups[1].Value);
                        descriptionLine = descriptionLine.Replace("#" + headerDetailRow.Number, "").Trim();
                        headerDetailRow.Quantity = Convert.ToInt32(quantityPattern.Match(descriptionLine).Groups[1].Value);
                        descriptionLine = descriptionLine.Replace("Qty: " + headerDetailRow.Quantity, "");
                        headerDetailRow.Description = descriptionLine.Replace(".", "").Trim().Replace("&amp;", "&");
                        headerDetailRow.Team = children[2].InnerText.Replace("Team:", "").Trim();
                        headerDetailRow.Status = children[3].InnerText.Trim();
                        headerRow.Details.Add(headerDetailRow);
                    }
                }
                headerRows.Add(headerRow);
            }
            return headerRows;
        }

        public static void GenerateCSV(List<CompoDataRow> projects, string output) {
            var csv = new StringBuilder();
            csv.AppendLine("\"Case\",\"Phone\",\"Address\",\"City\",\"Sub Contractor\",\"Team\",\"Assigned At\",\"Days Since Assigned\",\"Completed At\",\"Status\"");
            foreach (var project in projects) {
                csv.AppendLine("\"" + project.Case + "\",\"" + project.Phone + "\",\"" + project.Address + "\",\"" + project.City + "\",\"" + project.SubContractor + "\",\"" + project.Team + "\",\"" + project.AssignedAt + "\",\"" + project.DaysSinceAssigned + "\",\"" + project.CompletedAt + "\",\"" + project.Status + "\"");
                csv.AppendLine("\"Tasks\",\"Number\",\"Description\",\"Quantity\",\"Team\",\"Status\"");
                foreach (var detail in project.Details) {
                    csv.AppendLine("\"\",\"" + detail.Number.ToString() + "\",\"" + detail.Description + "\",\"" + detail.Quantity.ToString() + "\",\"" + detail.Team + "\",\"" + detail.Status + "\"");
                }
            }
            File.WriteAllText(output, csv.ToString());
        }
    }
}
