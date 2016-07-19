using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFace {
    class Logger {
        public void log(string message) {
            Console.WriteLine(DateTime.UtcNow + " :: GifFace :: " + message);
        }
    }
}
