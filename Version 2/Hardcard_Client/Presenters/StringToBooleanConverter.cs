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

    //
    //
    //
    public class BooleanToDeletedConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value,
                                Type targetType,
                                object parameter,
                                System.Globalization.CultureInfo culture)
        {
            if (value == null) return "ssdfdel";
            bool data = (bool)value;
            if (data) return "Deleted";
            return ""; // Not deleted
        }

        public object ConvertBack(object value,
                                    Type targetType,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {
            string data = (string)value;
            if (data == "Deleted") return true;
            return false;
        }


        #endregion
    }//class BooleanToDeletedConverter : IValueConverter

    //
    //Converts Boolean(bit) in DataBase into string "Invalid" in Passing
    //
    public class BooleanToInvalidConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value,
                                Type targetType,
                                object parameter,
                                System.Globalization.CultureInfo culture)
        {
            if (value == null) return "ssdfdinv";
            bool data = (bool)value;
            if (data) return "MinLapTime";
            return ""; // Not deleted
        }

        public object ConvertBack(object value,
                                    Type targetType,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {
            string data = (string)value;
            if (data == "MinLapTime") return true;
            return false;
        }


        #endregion
    }//class BooleanToInvalidConverter : IValueConverter

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


    public class UnixTimeToDateTimeConverter : IValueConverter
    {

        //Converts Seconds to DateTime
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //internal static DateTime ConvertFromUnixTime(double unixTime)
        {
            //if (value == null) value = 1333850000000;

            double unixTime = (double)((long)value/1000);
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(unixTime);
        }

        //Converts DateTime to Seconds
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //internal static double ConvertToUnixTime(DateTime date)
        {
            string str = (string)value;
            DateTime date = DateTime.Parse(str); // "09/23/201 //todo exception if Parse faild
            //DateTime date = (DateTime)value;
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (Math.Floor(diff.TotalSeconds)) * 1000;
        }
    }//class UnixTimeToDateTimeConverter : IValueConverter

    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            string str = "";
            if (value == null) return str; // return emty string
            long data = (long)value;
            TimeSpan result = new TimeSpan();
            //if (data != 0) return result = TimeSpan.FromMilliseconds(data);
            //else return result;
            
            if (data != 0) result = TimeSpan.FromMilliseconds(data);
            str = string.Format(result.ToString(@"hh\:mm\:ss\.FFFFF"));
            return str;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //you only need to implement this for two-way conversions
            throw new NotImplementedException();
        }
    }//class TimeSpanConverter : IValueConverter

    public class MilitaryTime : IValueConverter
    {
         public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      //public static string WriteMilitaryTime(DateTime date)
        {
            //
            // Convert hours and minutes to 24-hour scale.
            //
            DateTime date = (DateTime)value;
            //string format = "MMM ddd d HH:mm yyyy";
            string format = "MM/dd/yyyy HH:mm:ss";
            return date.ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime date;
            //return date.ToString(format);
            //return date.ToString("MM/dd/yyyy HH:mm");
            string str = (string)value;
            //string format = "MM/dd/yyyy HH:mm";
            return date = DateTime.Parse(str); // "09/23/2012 23:00"
        }
        
        
        private static DateTime ParseMilitaryTime(string time, int year, int month, int day)
        {
            //
            // Convert hour part of string to integer.
            //
            string hour = time.Substring(0, 2);
            int hourInt = int.Parse(hour);
            if (hourInt >= 24)
            {
                throw new ArgumentOutOfRangeException("Invalid hour");
            }
            //
            // Convert minute part of string to integer.
            //
            string minute = time.Substring(2, 2);
            int minuteInt = int.Parse(minute);
            if (minuteInt >= 60)
            {
                throw new ArgumentOutOfRangeException("Invalid minute");
            }
            //
            // Return the DateTime.
            //
            return new DateTime(year, month, day, hourInt, minuteInt, 0);
        }
    }//class MilitaryTime : IValueConverter

    public class CheckBoxToBitConverter : IValueConverter
    {
        #region IValueConverter Members

        public object ConvertBack(object value,
                              Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            try
            {
                bool data = (bool)value;
                return (data) ? 1 : 0;
            }
            catch (Exception)
            {
                new ArgumentException("CheckBoxToBitConverter Conversion failed");
            }
            return 0;
        }
        public object Convert( object value,
                                   Type targetType,
                                   object parameter,
                                   System.Globalization.CultureInfo culture)
        {
            int data = (int)value;
            if (data == 1 || data == 0)
            {
                return (data == 1) ? true : false;
            }
            else
            {
                new ArgumentException(" CheckBoxToBitConverter Back Conversion failed");
            }
            return false;
        }

        #endregion
    } //public class CheckBoxToBitConverter : IValueConverter

    
}
