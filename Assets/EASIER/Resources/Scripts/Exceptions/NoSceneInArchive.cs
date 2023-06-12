using System;

namespace EASIER.Resources.Scripts.Exceptions
{
    public class NoSceneInArchive : Exception
    {
        public NoSceneInArchive() : base("No scene found in given archive")
        {
        }
    }
}