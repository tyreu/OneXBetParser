using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestParser
{
    public class Parser : IPrinter
    {
        public const string URL = "https://ua1xbet.com/ru/live/Football";
        public const string MobURL = "https://1xbetua.mobi";
        readonly IWebDriver driver = new ChromeDriver();

        public IList<Match> Matches { get; set; } = new List<Match>();
        public ParserOptions Options { get; set; }

        public Parser(ParserOptions options)
        {
            try
            {
                Options = options;
                driver.Navigate().GoToUrl(Options.Uri);
            }
            catch (WebDriverException)
            {
                Console.WriteLine("Не удалось загрузить страницу. Попробуйте еще раз.");
            }
        }

        public IList<IWebElement> GetDataByCssSelector(string selector) => driver.FindElements(By.CssSelector(selector), Options.TimeoutSeconds);

        public Match GetMatchById(int id) => Matches.FirstOrDefault(match => match.Id == id);

        public void ParseMatches()
        {
            var matches = GetDataByCssSelector("div[data-filter-con='live'] div[data-name='dashboard-champ-content'] div.c-events__item.c-events__item_col");
            Parallel.ForEach(matches, game =>
            {
                try
                {
                    var id = game.FindElement(By.CssSelector("a[data-idgame]"), true)?.GetAttribute("data-idgame");
                    var category = game.FindElement(By.CssSelector("a[data-idgame]"), true)?.GetAttribute("data-sportid");
                    var link = game.FindElement(By.CssSelector("div.c-events-scoreboard a.c-events__name"), true)?.GetAttribute("href");
                    var time = game.FindElement(By.CssSelector("div.c-events__time"), true).Text.Replace("\r\n", ", ");
                    var teams = game.FindElements(By.CssSelector("div.c-events__team")).Select(team => new Team(team.Text)).ToArray();
                    var score = game.FindElements(By.CssSelector("div.c-events-scoreboard__line")).Select(line => line.Text.Replace("\r\n", " ").Split(' ')).ToList();
                    for (int i = 0; i < teams.Length; i++)
                        teams[i].Score = score[i].Select(n => int.Parse(n)).ToArray();

                    var coefficients = game.FindElements(By.CssSelector("div.c-events__item.c-events__item_col div.c-bets a")).Take(3);
                    var coef1 = coefficients.ElementAt(0).Text;
                    var coefDraw = coefficients.ElementAt(1).Text;
                    var coef2 = coefficients.ElementAt(2).Text;

                    var match = new Match
                    {
                        Id = int.Parse(id ?? "-1"),
                        Category = (SportCategory)int.Parse(category ?? "0"),
                        Uri = link,
                        Time = time,
                        Team1 = teams[0],
                        Team2 = teams[1],
                        Coef1 = coef1,
                        Coef2 = coef2,
                        CoefDraw = coefDraw
                    };
                    Matches.Add(match);
                }
                catch (StaleElementReferenceException)
                {
                    Console.WriteLine("Ошибка парсинга.");
                }
            });
            Print();
        }

        public void Print()
        {
            foreach (var match in Matches)
                Console.WriteLine(match);
        }

        public bool SignIn()
        {
            driver.Navigate().GoToUrl(MobURL);
            var button = GetDataByCssSelector("#curLoginForm").First();
            if (button != null)
            {
                button.Click();
                new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(d => d.FindElement(By.CssSelector("#idOrMail"))).SendKeys(Options.Login);
                GetDataByCssSelector("#uPassword").First().SendKeys(Options.Password);
                GetDataByCssSelector("#userConButton").First().Click();
                return true;
            }
            else
                return false;
        }
    }
}
