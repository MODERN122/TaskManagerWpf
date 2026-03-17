using System.Runtime.ExceptionServices;

namespace TaskManagerWpf.Tests;

internal static class StaTest
{
    public static void Run(Action action)
    {
        Exception? ex = null;

        var t = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                ex = e;
            }
        });

        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join();

        if (ex is not null)
            ExceptionDispatchInfo.Capture(ex).Throw();
    }
}

