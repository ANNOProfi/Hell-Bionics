using Verse;

namespace HellBionics
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main() //our constructor
        {
            Log.Message("Main.cs geladen"); //Outputs "Hello World!" to the dev console.
        }
    }
}