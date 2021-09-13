namespace SisMaper.Tools
{
    public static class InterfaceExtension
    {
        public static bool IsTrue(this bool? b)
        {
            return b.HasValue && b.Value;
        }
    }
}