using System;

namespace EASIER.Resources.Scripts.Exceptions
{
    public class UnknownSceneObjectType : Exception
    {
        public UnknownSceneObjectType(string t) : base($"Unknown type {t} for scene objects.")
        {
        }
    }
}