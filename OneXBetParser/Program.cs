using System;

namespace TestParser
{
    class Program
    {
        static void Menu()
        {
            Console.Clear();
            Console.WriteLine("Добро пожаловать в парсер 1хBet!\n");
            Console.WriteLine("1. Вывести в консоль список футбольных матчей которые сейчас проходят.");
            Console.WriteLine("2. Получить информацию о матче по его ID.");
            Console.WriteLine("3. Авторизоваться.");
        }

        static void Main(string[] args)
        {
            int userChoice = 0;
            bool repeat = false;
            bool error = false;
            ParserOptions options = null;
            Parser parser = null;
            do
            {
                Menu();

                if (error) Console.WriteLine("Ошибка ввода. Попробуйте еще раз.");
                error = repeat = false;
                if (int.TryParse(Console.ReadLine(), out userChoice))
                {
                    if (options == null && parser == null)
                    {
                        options = new ParserOptions(5, Parser.URL);
                        parser = new Parser(options);
                        Console.Clear();
                    }
                    switch (userChoice)
                    {
                        case 1:
                            parser.ParseMatches();
                            break;
                        case 2:
                            GetMatchById(parser);
                            break;
                        case 3:
                            SignIn(parser);
                            break;
                        default:
                            Console.WriteLine("Введен неверный код операции.");
                            break;
                    }
                    Console.WriteLine("Продолжить? 1 - да");
                    if (int.TryParse(Console.ReadLine(), out int repeatNumber))
                        repeat = repeatNumber == 1;
                }
                else error = true;
            } while (error || repeat);
            Console.WriteLine("Good bye!");
        }

        private static void SignIn(Parser parser)
        {
            Console.WriteLine("Введите логин: ");
            parser.Options.Login = Console.ReadLine();
            Console.WriteLine("Введите пароль: ");
            parser.Options.Password = Console.ReadLine();
            if (!parser.SignIn())
                Console.WriteLine("Не удалось авторизоваться.");
        }

        private static void GetMatchById(Parser parser)
        {
            Console.Clear();
            parser.ParseMatches();
            Console.WriteLine("Введите ID матча: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var match = parser.GetMatchById(id);
                if (match != null)
                {
                    Console.WriteLine($"Коэффициент на победу первой команды:{match.Coef1}");
                    Console.WriteLine($"Коэффициент на ничью:{match.CoefDraw}");
                    Console.WriteLine($"Коэффициент на победу второй команды:{match.Coef2}");
                }
                else
                    Console.WriteLine("Матч не найден :(");
            }
            else Console.WriteLine("Ошибка ввода ID.");
        }
    }
}
