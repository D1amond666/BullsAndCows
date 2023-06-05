using System;
using System.Collections.Generic;
using System.Linq;


namespace BullsAndCows
{
    class Program
    {
        static readonly Random RandomGenerator = new Random();

        /// <summary>
        /// Generates and returns the specified number of non-repeating digits.
        /// </summary>
        /// <param name="length">Number of digits. Cannot exceed 9.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if it is not possible to generate the required number of non-repeating digits.
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
        /// Extracts the required number of digits from the given string.
        /// </summary>
        /// <returns>
        /// Returns exactly <paramref name="length"/> digits extracted from the string,
        /// but only if the string contains nothing but the required number of digits.
        /// Otherwise, it returns null: if there are extra characters in the string or there are not enough of them.
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
        /// Prompts the user for the correct number length.
        /// </summary>
        static int AskLength()
        {
            while (true)
            {
                Console.Write("Enter the length of the guessed number:");
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
        /// Returns whether the given array contains duplicate numbers
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
        /// Prompts the user for the required number of digits until the user enters the correct number.
        /// </summary>
        static int[] AskUser(int length)
        {
            while (true)
            {
                Console.Write("Your guess:");
                var input = Console.ReadLine();

                // Documentation says that Console.ReadLine() may return null.
                if (input != null)
                {
                    // Trim whitespaces. User should not worry about few invisible characters.
                    input = input.Trim();

                    var digits = SplitNumber(input, length);
                    if (digits == null)
                    {
                        Console.WriteLine("     Enter true number");
                    }
                    else if (ContainsDuplicates(digits))
                    {
                        Console.WriteLine("    The number must not contain repeated digits");
                    }
                    else
                    {
                        return digits;
                    }
                }
            }
        }

        /// <summary>
        /// Plays exactly one round with the user until they win.
        /// </summary>
        /// <param name="length">The length of the given number.</param>
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
            Console.WriteLine("This is the Bulls and Cows game.»!");
            Console.WriteLine("The computer guesses the number, and the player will have to guess it.");
            Console.WriteLine("It is guaranteed that all digits in the given number are distinct.");
            Console.WriteLine("Also, the player cannot enter repeated numbers, this makes the game too easy.");
            Console.WriteLine("Bulls: the number of correct digits in the correct places.");
            Console.WriteLine("Cows: the number of digits that are present somewhere in the hidden number, but in the wrong place.");
            Console.WriteLine("");
            string choice;
            do
            {
                var length = AskLength();

                PlaySingleGame(length);

                Console.Write("Enter 'y' to play again. ");
                choice = Console.ReadLine();
            } while (choice == "y");
        }
    }
}
