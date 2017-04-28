using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Text.RegularExpressions;

namespace CorevetteScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = @"http://www.buyavette.net/C5.php";
            string html = new WebClient().DownloadString(url);

            string expr = "(<td align=\"left\" valign=\"top\">)(.*)(\\n*?\\r*?\\t*?\\s*.*Price.*\\.\\s?</span>)";

            MatchCollection matches = Regex.Matches(html, expr);

            foreach (Match match in matches)
            {
                string yearRegex = "(<span class=\"style74\">)(\\d\\d\\d\\d\\sCorvette.*)(</span></a> )";
                MatchCollection yearMatch = Regex.Matches(match.Value, yearRegex, RegexOptions.Multiline);
                string corvette = yearMatch[0].Groups[2].ToString();
                Console.WriteLine(corvette);

                string milesRegex = "(<strong>)(.*)(K\\s*?Miles.*</strong>)";
                MatchCollection milesMatch = Regex.Matches(match.Value, milesRegex, RegexOptions.Multiline);
                if (milesMatch.Count > 0 && milesMatch[0].Groups[2].ToString().Length < 10)
                {
                    var miles = milesMatch[0].Groups[2].ToString();
                    Console.WriteLine("Miles: " + miles + ",000");
                }

                string priceRegex = "(.*Price:\\s*<span class=\"style89\">)(.*)(\\.\\s*?</span>)";
                MatchCollection priceMatch = Regex.Matches(match.Value, priceRegex, RegexOptions.Multiline);
                var price = priceMatch[0].Groups[2].ToString();
                Console.WriteLine("Price: " + price);

                Console.WriteLine("=========================================================");
            }

            Console.ReadKey();


        }
    }
}
