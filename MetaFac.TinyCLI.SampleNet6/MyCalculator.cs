// See https://aka.ms/new-console-template for more information

namespace MiniCLI.SampleNet6
{
    internal static class MyCalculator
    {
        public static async ValueTask<int> add(int value1, int value2)
        {
            await Task.Delay(0);
            _ = value1 + value2;
            return 0;
        }

        public static async ValueTask<int> sub(int value1, int value2)
        {
            await Task.Delay(0);
            _ = value1 - value2;
            return 0;
        }

        public static async ValueTask<int> div(int value1, int value2)
        {
            await Task.Delay(0);
            _ = value1 / value2;
            return 0;
        }

    }

}