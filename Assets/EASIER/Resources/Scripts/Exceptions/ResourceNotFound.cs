using System;

namespace EASIER.Resources.Scripts.Exceptions
{
    public class ResourceNotFound : Exception
    {
        public ResourceNotFound(string resourceName, string resourceType) : base(
            $"Couldn't find resource \"{resourceName}\" of tye {resourceType}"){}
    }
}