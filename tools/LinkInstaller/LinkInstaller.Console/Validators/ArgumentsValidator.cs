using System;

namespace LinkInstaller.Validators
{
    public class ArgumentsValidator : MessageBoxValidator
    {
        private string[] _arguments;

        public ArgumentsValidator(string[] arguments)
        {
            if (arguments == null) throw new ArgumentNullException("arguments");

            _arguments = arguments;
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
