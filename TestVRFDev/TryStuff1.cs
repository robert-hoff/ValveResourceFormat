using System.Diagnostics;

namespace TestVRFDev
{
    class TryStuff1
    {
        public static void Mainz()
        {
            Trial1();
        }

        public static void Trial1()
        {
            string query = @"SELECT foo, bar
FROM table
WHERE id = 42";

            Debug.WriteLine(query);
        }
    }
}

