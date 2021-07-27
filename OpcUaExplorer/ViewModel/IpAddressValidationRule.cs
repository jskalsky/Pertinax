using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OpcUaExplorer.ViewModel
{
    class IpAddressValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int part;
            if(int.TryParse(value.ToString(), out part))
            {
                if(part >= 0 && part <= 255)
                {
                    return new ValidationResult(true, null);
                }
            }
            return new ValidationResult(false, "Please enter valid IP address.");
        }
    }
}
