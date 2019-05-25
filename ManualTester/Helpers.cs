using System.IO;

namespace WindowsFormsApp1
{
    public static class Helpers
    {

        public static void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
