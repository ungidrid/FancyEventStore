namespace FancyEventStore.Common
{
    public static class ObjectExtensions
    {
        public static string GetTypeName(this object obj)
        {
            return obj.GetType().AssemblyQualifiedName;
        }
    }
}
