using System;

namespace LinkInstaller.Validators
{
    public class ArgumentsValidator : ConsoleValidator
    {
        private string[] _arguments;

        public ArgumentsValidator(string[] arguments)
        {
            _arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        public override bool IsValid()
        {
            if (_arguments.Length < 3)
            {
                ShowErrorToUser(new string[]
                {
                    "Too few arguments. I need:",
                    "  Tekla version",
                    "  Path to source .gha",
                    "  Path to GH 'Libraries' directory"
                });
                return false;
            }

            return true;
        }
    }
}
