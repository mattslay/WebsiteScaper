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
            string styleToSearch = "C5";
            string minYear = "2001";
            string maxPrice = "25,000";
            string maxMiles = "50";
            bool includeConvertibles = false;
            bool includeZ06 = false;

            string siteUrl = @"http://www.buyavette.net";
            string url = siteUrl + "/" + styleToSearch + ".php";

            string html = new WebClient().DownloadString(url);
            
            string expr = "(<td align=\"left\" valign=\"top\">)(.*)(\\n*?\\r*?\\t*?\\s*.*Price.*\\.\\s?</span>)";

            MatchCollection matches = Regex.Matches(html, expr);
            bool isConvertible;
            bool isZ06;

            //< a href = "2004 Corvette Z06 Custom For Sale Stock 113204N.php" >



            foreach (Match match in matches)
            {
                isConvertible = false;

                string yearRegex = "(<span class=\"style74\">)(\\d\\d\\d\\d)\\s*(Corvette.*)(</span></a> )";
                MatchCollection yearMatch = Regex.Matches(match.Value, yearRegex, RegexOptions.Multiline);
                string year = yearMatch[0].Groups[2].ToString();
                string corvette = yearMatch[0].Groups[3].ToString();
                string description = yearMatch[0].Groups[4].ToString();
                corvette = corvette.Replace("Stock</span> <span class=\"style74\">", "Stock "); // Need a little cleanup on this sometimes.

                string priceRegex = "(.*Price:\\s*<span class=\"style89\">)(.*)(\\.\\s*?</span>)";


                string urlRegex = "<a href=\"(.*Stock.*.php)\"";
                MatchCollection urlMatch = Regex.Matches(match.Value, urlRegex);
                if (urlMatch.Count > 0)
                {
                    url = siteUrl + "/" + urlMatch[0].Groups[1].Value;
                    url = url.Replace(" ", "%20");
                }
                else
                    url = "";


                MatchCollection priceMatch = Regex.Matches(match.Value, priceRegex, RegexOptions.Multiline);
                var price = priceMatch[0].Groups[2].ToString();

                isConvertible = corvette.ToLower().Contains("convertible") ;
                isZ06 = corvette.ToUpper().Contains("Z06");

                if (String.Compare(year, minYear) < 0 || 
                    (isConvertible && !includeConvertibles) || 
                    (isZ06 && !includeZ06) ||
                    String.Compare(price, maxPrice) > 0)
                    continue;

                Console.WriteLine("Year: " + year);
                Console.WriteLine(corvette);

                string milesRegex = "(<strong>)(.*)(K\\s*?Miles.*</strong>)";
                MatchCollection milesMatch = Regex.Matches(match.Value, milesRegex, RegexOptions.Multiline);
                if (milesMatch.Count > 0 && milesMatch[0].Groups[2].ToString().Length < 10)
                {
                    var miles = milesMatch[0].Groups[2].ToString();
                    Console.WriteLine("Miles: " + miles + ",000");
                }

                Console.WriteLine("Price: " + price);
                Console.WriteLine("Link: " + url);


                Console.WriteLine("=========================================================");
            }

            Console.ReadKey();


        }
    }
}
