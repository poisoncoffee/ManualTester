namespace WindowsFormsApp1
{
    public interface ILogcatOperator
    {
        Logcat BeginLogcat(string packagename);

        Logcat UpdateLogcat(Logcat logcat, int offset);

        //TODO - Not Implemented Yet
        void EndLogcat(Logcat logcat);


    }


}
