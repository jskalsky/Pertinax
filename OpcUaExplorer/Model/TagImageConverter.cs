using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace OpcUaExplorer.Model
{
    public class TagImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imagePath;
            switch((NodeClass)value)
            {
                case NodeClass.Object: imagePath = "ClassIcon.png";
                    break;
                case NodeClass.Variable: imagePath = "Union-Friend_536.png";
                    break;
                case NodeClass.Method:
                    imagePath = "Method_636.png";
                    break;
                case NodeClass.ObjectType:
                    imagePath = "Type_527.png";
                    break;
                case NodeClass.VariableType:
                    imagePath = "Structure_507.png";
                    break;
                case NodeClass.ReferenceType:
                    imagePath = "Table_748.png";
                    break;
                case NodeClass.DataType:
                    imagePath = "TypeDefinition_521.png";
                    break;
                case NodeClass.View:
                    imagePath = "View_8933_16x.png";
                    break;
                default: imagePath = "Pointer_6127.png";
                    break;
            }
            BitmapImage image = new BitmapImage(new Uri("pack://application:,,,/Icons/" + imagePath));
            
            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
