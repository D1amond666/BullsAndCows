using System;
using System.Collections.Generic;
using System.Linq;


namespace BullsAndCows
{
    class Program
    {
        static readonly Random RandomGenerator = new Random();

        /// <summary>
        /// Генерирует и возвращает указанное количество неповторяющихся цифр.
        /// </summary>
        /// <param name="length">Количество цифр. Не может превышать 9.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Выбрасывается если невозможно сгенерировать требуемое количество неповторяющихся цифр.
        /// </exception>
        static int[] GetRandomDigits(int length)
        {
            // Check that this function provided with correct length.
            // Otherwise throw an exception and crash application.
            if (length < 0 || length > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            // This list contains digits that can be placed to the result.
            // But it does not contain zero (yet), because first digit can't be zero.
            var availableDigits = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var result = new int[length];
            for (var i = 0; i < length; i++)
            {
                // Select random digit from pool.
                var digitIndex = RandomGenerator.Next(0, availableDigits.Count);

                // And place it to the result.
                result[i] = availableDigits[digitIndex];

                // Then remove chosen digit from pool, so it won't be selected again.
                availableDigits.RemoveAt(digitIndex);

                if (i == 0)
                {
                    // Zero can be chosen after we generated first digit.
                    availableDigits.Add(0);
                }
            }

            return result;
        }

        /// <summary>
        /// Выделяет из переданной строки требуемое количество цифр.
        /// </summary>
        /// <returns>
        /// Возвращает ровно <paramref name="length"/> выделенных из строки цифр,
        /// но только если строка не содержит ничего кроме необходимого числа цифр.
        /// Иначе возвращает null: если в строке есть лишние симфолы или их не хватает.
        /// </returns>
        static int[] SplitNumber(string str, int length)
        {
            if (str.Length != length)
            {
                return null;
            }

            var result = new int[length];
            for (var i = 0; i < length; i++)
            {
                var character = str[i].ToString();
                if (int.TryParse(character, out var digit))
                {
                    result[i] = digit;
                }
                else
                {
                    return null;
                }
            }

            return result;
        }

        /// <summary>
        /// Запрашивает у пользователя корректную длину числа.
        /// </summary>
        static int AskLength()
        {
            while (true)
            {
                Console.Write("Введите длину загадываемого числа: ");
                var input = Console.ReadLine();
                if (input == null)
                {
                    continue;
                }

                input = input.Trim();

                var isParsed = int.TryParse(input, out var number);
                if (!isParsed)
                {
                    Console.WriteLine("Нужно ввести число");
                    continue;
                }
                var isInRange = number > 0 && number < 10;
                if (!isInRange)
                {
                    Console.WriteLine("Длина должна быть больше нуля и не более девяти");
                    continue;
                }

                return number;
            }
        }

        /// <summary>
        /// Возвращает, есть ли в переданном массиве повторяющиеся числа
        /// </summary>
        static bool ContainsDuplicates(int[] numbers)
        {
            for (var i = 0; i < numbers.Length; i++)
            {
                for (var j = i + 1; j < numbers.Length; j++)
                {
                    if (numbers[i] == numbers[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Запрашивает у пользователя требуемое количество цифр до тех пор пока он не введет корректное число.
        /// </summary>
        static int[] AskUser(int length)
        {
            while (true)
            {
                Console.Write("Ваша догадка: ");
                var input = Console.ReadLine();

                // Documentation says that Console.ReadLine() may return null.
                if (input != null)
                {
                    // Trim whitespaces. User should not worry about few invisible characters.
                    input = input.Trim();

                    var digits = SplitNumber(input, length);
                    if (digits == null)
                    {
                        Console.WriteLine("    Введите корректное число.");
                    }
                    else if (ContainsDuplicates(digits))
                    {
                        Console.WriteLine("    Число не должно содержать повторяющихся цифр");
                    }
                    else
                    {
                        return digits;
                    }
                }
            }
        }

        /// <summary>
        /// Играет ровно один раунд с пользователем до тех пор, пока он не победит.
        /// </summary>
        /// <param name="length">Длина загаданного числа.</param>
        static void PlaySingleGame(int length)
        {
            var number = GetRandomDigits(length);
            while (true)
            {
                var guess = AskUser(length);

                var correct = 0;
                var misplaced = 0;
                for (var i = 0; i < length; i++)
                {
                    var digit = guess[i];
                    if (digit == number[i])
                    {
                        correct += 1;
                    }
                    else if (number.Contains(digit))
                    {
                        misplaced += 1;
                    }
                }

                if (correct != length)
                {
                    Console.WriteLine($"Быков: {correct}");
                    Console.WriteLine($"Коров: {misplaced}");
                }
                else
                {
                    Console.WriteLine("Все быки на месте!");
                    return;
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Это игра «Быки и коровы»!");
            Console.WriteLine("Компьютер загадывает число, а игрок должен будет его отгадать.");
            Console.WriteLine("Гарантируется, что все цифры в загаданном числе различны.");
            Console.WriteLine("Также и игроку нельзя вводить повторяющиеся цифры, это слишком облегчает игру.");
            Console.WriteLine("Быки: количество правильных цифр на правильных местах.");
            Console.WriteLine("Коровы: количество цифр, которые присутствуют где-то в загаданном числе, но не на том месте.");
            Console.WriteLine("");
            string choice;
            do
            {
                var length = AskLength();

                PlaySingleGame(length);

                Console.Write("Введите 'y' чтобы сыграть ещё раз. ");
                choice = Console.ReadLine();
            } while (choice == "y");
        }
    }
}
