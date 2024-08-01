namespace AAA_TEST_Console
{
    internal static class Shift
    {
        internal static void Test()
        {
            string testStr = "Different Ways to Convert Char Array to String in C#";

            Console.WriteLine($"{testStr}");

            testStr = ShiftString(testStr);

            Console.WriteLine($"{testStr}");

            testStr = ShiftString(testStr);

            Console.WriteLine($"{testStr}");
        }

        private static string ShiftString(string input)
        {
            int key = 55;
            List<char> chars = [];

            foreach (char ch in input)
            {
                chars.Add((char)(ch ^ key));
            }
            return new string(chars.ToArray());
        }
    }
}
