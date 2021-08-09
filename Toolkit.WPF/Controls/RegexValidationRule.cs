using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Toolkit.WPF.Controls
{
    public class RegexValidationRule : ValidationRule
    {
        public string Regex { get; set; }

        public string ErrorMessage { get; set; }

        private Regex regex;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (regex == null)
            {
                regex = new Regex(this.Regex, RegexOptions.Compiled);
            }

            if (regex.IsMatch(value.ToString()))
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, this.ErrorMessage);
        }
    }
}
