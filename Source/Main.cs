using Verse;

namespace HellBionics
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main() //our constructor
        {
            Log.Message("HellBionics loaded"); //Outputs "Hello World!" to the dev console.
        }
    }
}