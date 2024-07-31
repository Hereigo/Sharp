namespace AAA_TEST_Console
{
    public class Person
    {
        public required string Name { get; set; }
        public int Age { get; set; }
    }

    public static class Delegates
    {
        public static void TestDelegates()
        {
            List<Person> people = [
                new Person() { Name = "John", Age = 41 },
                new Person() { Name = "Jane", Age = 69 },
                new Person() { Name = "Jake", Age = 12 },
                new Person() { Name = "Jessie", Age = 25 }
            ];

            //Invoke DisplayPeople using appropriate delegate:

            DisplayPeople("Children:", people, IsChild);
            DisplayPeople("Adults:", people, IsAdult);
            DisplayPeople("Seniors:", people, IsSenior);

            Console.Read();
        }

        static void DisplayPeople(string title, List<Person> people, FilterDelegateFunc filter)
        {
            Console.WriteLine("\n\n" + title);
            foreach (Person p in people)
            {
                if (filter(p))  //  Delegate FILTERS func!
                {
                    Console.WriteLine("{0}, {1} years old", p.Name, p.Age);
                }
            }
        }

        public delegate bool FilterDelegateFunc(Person p);

        // ========== FILTERS : ===================

        static bool IsChild(Person p)
        {
            return p.Age < 18;
        }

        static bool IsAdult(Person p)
        {
            return p.Age >= 18;
        }

        static bool IsSenior(Person p)
        {
            return p.Age >= 65;
        }
    }
}