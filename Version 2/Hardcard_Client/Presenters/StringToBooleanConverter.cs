using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RacingEventsTrackSystem.Presenters
{
	public class StringToBooleanConverter : IValueConverter
	{
		#region IValueConverter Members
		public object Convert( object value,
				               Type targetType, 
				               object parameter,
				               System.Globalization.CultureInfo culture)
		{
			string data = value as string;
			if (data != null)
			{
				return (data == "Yes" || data == "True") ?
				true : false;
			}
			else
			{
				new ArgumentException("Conversion " + "failed");
			}
			return false;
		}
		
		public object ConvertBack( object value,
				                   Type targetType, 
				                   object parameter,
				                   System.Globalization.CultureInfo culture)
		{
			try
			{
				bool data = (bool)value;
				return (data) ? "Yes" : "No";
			}
			catch (Exception)
			{
				new ArgumentException("Conversion failed");
			}
			return false;
		}
		#endregion
    } //public class StringToBooleanConverter : IValueConverter

    //
    //if Competeter has property Deleted=true, then use Red color
    //
    public class BoolToColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value,
                                Type targetType,
                                object parameter,
                                System.Globalization.CultureInfo culture)
        {
            bool data = (bool)value;
            if (data) return Brushes.Red;
            else return Brushes.Green;
        }

        public object ConvertBack(object value,
                                    Type targetType,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {
            //you only need to implement this for two-way conversions
            throw new NotImplementedException();
        }

        #endregion
    }//class DeletedToColorConverter : IValueConverter

    //
    //if Competeter has property Deleted=false, then return Active to match Status list values.
    //
    public class BooleanToStatusConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value,
                                Type targetType,
                                object parameter,
                                System.Globalization.CultureInfo culture)
        {
            bool data = (bool)value;
            if (data) return "Nonactive";
            else return "Active";
        }

        public object ConvertBack(object value,
                                    Type targetType,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {
            string data = (string)value;
            if (data == "Nonactive") return true;
            else if (data == "Active") return false;
            return false;
        }


        #endregion
    }//class BooleanToStatusConverter : IValueConverter

    public class StatusToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value,
                                Type targetType,
                                object parameter,
                                System.Globalization.CultureInfo culture)
        {
            string data = value as string;
            if (data != null)
            {
                return (data == "Active") ? true : false;
            }
            else
            {
                new ArgumentException("Conversion " + "failed");
            }
            return false;
        }

        public object ConvertBack(object value,
                                    Type targetType,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {

            try
            {
                bool data = (bool)value;
                return (data) ? "Nonactive" : "Active";
            }
            catch (Exception)
            {
                new ArgumentException("Conversion failed");
            }
            return false;
       }


        #endregion
    }//class StatusToBooleanConverter : IValueConverter



    public class PhoneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = value as string;

            if (!string.IsNullOrEmpty(result))
            {
                string filteredResult = FilterNonNumeric(result);

                long theNumber = System.Convert.ToInt64(filteredResult);

                switch (filteredResult.Length)
                {
                    case 11:
                        result = string.Format("{0:+# (###) ###-####}", theNumber);
                        break;
                    case 10:
                        result = string.Format("{0:(###) ###-####}", theNumber);
                        break;
                    case 7:
                        result = string.Format("{0:###-####}", theNumber);
                        break;
                }
            }

            return result;
        }

        private static string FilterNonNumeric(string stringToFilter)
        {
            if (string.IsNullOrEmpty(stringToFilter)) return string.Empty;

            string filteredResult = string.Empty;

            foreach (char c in stringToFilter)
            {
                if (Char.IsDigit(c))
                    filteredResult += c;
            }

            return filteredResult;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return FilterNonNumeric(value as string);
        }
    }//class PhoneConverter : IValueConverter

    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            long data = (long)value;
            TimeSpan result = new TimeSpan();
            if (data != 0) result = TimeSpan.FromMilliseconds(data);
            string str = string.Format(result.ToString(@"hh\:mm\:ss\.FFFFF"));
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //you only need to implement this for two-way conversions
            throw new NotImplementedException();
        }
    }//class PhoneConverter : IValueConverter
}
