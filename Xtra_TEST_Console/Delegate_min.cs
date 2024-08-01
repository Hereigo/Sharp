namespace AAA_TEST_Console
{
    internal static class Delegate_min
    {
        public delegate string Delegate_func();

        internal static Delegate_func func; // em...?

        public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);



        internal static void Test()
        {

        }
    }
}
