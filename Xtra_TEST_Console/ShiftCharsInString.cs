namespace AAA_TEST_Console
{
    internal static class ShiftCharsInString
    {
        internal static void Test()
        {
            string testStr = "Different Ways to Convert Char Array to String in C#";
            int key = 55;

            Console.WriteLine($"{testStr}");

            testStr = Shift(testStr, key);
            Console.WriteLine($"{testStr}");

            testStr = Shift(testStr, key);
            Console.WriteLine($"{testStr}");

            key = 100;

            testStr = Shift(testStr, key);
            Console.WriteLine($"{testStr}");

            testStr = Shift(testStr, key);
            Console.WriteLine($"{testStr}");
        }

        private static string Shift(string input, int key)
        {
            var chars = new List<char>();

            foreach (char ch in input)
            {
                chars.Add((char)(ch ^ key));
            }
            // return new string(chars.ToArray());
            return new string([.. chars]);
        }
    }
}
