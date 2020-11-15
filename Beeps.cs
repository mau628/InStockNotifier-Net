using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace InStockNotifier
{
    public static class Beeps
    {
        public static void GoingUp()
        {
            for (int x = 100; x <= 10000; x += 500)
                Console.Beep(x, 100);
        }
    }
}
