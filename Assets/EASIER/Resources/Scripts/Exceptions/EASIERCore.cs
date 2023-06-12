using System;

namespace EASIER.Resources.Scripts.Exceptions
{
    public class EASIERCore : Exception
    {
        public EASIERCore() : base("EASIER core exception !"){}
        public EASIERCore(string msg) : base($"EASIER core exception !\n\t{msg}"){}
    }
}