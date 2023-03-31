using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkInstaller.Validators
{
    public abstract class ConsoleValidator
    {
        public void ShowErrorToUser(params string[] message)
        {
            foreach (var line in message)
                Console.WriteLine(line);

            Console.WriteLine("Press enter to exit ...");
            Console.ReadLine();
        }

        public abstract bool IsValid();
    }
}
