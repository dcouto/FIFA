using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Specialized;
using System.Web;

namespace FIFA.COM
{
	public static class NameValueCollectionExtensions
	{
		public static bool HasKey(this NameValueCollection QString, string Key) {

			foreach (string key in QString.Keys) {
				if (key.Equals(Key)) {
					return true;
				}
			}

			return false;
		}

		public static string ToQueryString(this NameValueCollection QString) {
			return ToQueryString(QString, new NameValueCollection());
		}

		public static string ToQueryString(this NameValueCollection QString, NameValueCollection OverrideKeys) {
			StringBuilder sb = new StringBuilder();

			string append = "?";
			foreach (string key in OverrideKeys.Keys) {
				if (!OverrideKeys[key].ToString().Equals("")) {
					sb.Append(append + key + "=" + HttpUtility.UrlEncode(OverrideKeys[key].ToString()));
					append = "&";
				}
			}

			foreach (string key in QString.Keys) {
				if (!OverrideKeys.HasKey(key) && !QString[key].ToString().Equals("")) {
					sb.Append(append + key + "=" + HttpUtility.UrlEncode(QString[key].ToString()));
				}
				append = "&";
			}

			return sb.ToString();
		}

	}

	public static class DateTimeExtensions
	{
		/// <summary>
		/// Gets a DateTime representing the first day in the current month
		/// </summary>
		/// <param name="current">The current date</param>
		/// <returns></returns>
		public static DateTime First(this DateTime current) {
			DateTime first = current.AddDays(1 - current.Day);
			return first;
		}

		/// <summary>
		/// Gets a DateTime representing the first specified day in the current month
		/// </summary>
		/// <param name="current">The current day</param>
		/// <param name="dayOfWeek">The current day of week</param>
		/// <returns></returns>
		public static DateTime First(this DateTime current, DayOfWeek dayOfWeek) {
			DateTime first = current.First();

			if (first.DayOfWeek != dayOfWeek) {
				first = first.Next(dayOfWeek);
			}

			return first;
		}

		/// <summary>
		/// Format date/time object into html representation of 'relative time' format, eg. "5 seconds ago".
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string FormatDateTimeRelative(this DateTime value) {
			TimeSpan oSpan = DateTime.Now.Subtract(value);
			double TotalMinutes = oSpan.TotalMinutes;
			string Suffix = " ago";

			if (TotalMinutes < 0.0) {
				TotalMinutes = Math.Abs(TotalMinutes);
				Suffix = " from now";
			}

			Dictionary<double, Func<string>> aValue = new Dictionary<double, Func<string>>();
			aValue.Add(0.75, () => "less than a minute");
			aValue.Add(1.5, () => "about a minute");
			aValue.Add(45, () => string.Format("{0} minutes", Math.Round(TotalMinutes)));
			aValue.Add(90, () => "about an hour");
			aValue.Add(1440, () => string.Format("about {0} hours", Math.Round(Math.Abs(oSpan.TotalHours)))); // 60 * 24
			aValue.Add(2880, () => "a day"); // 60 * 48
			aValue.Add(43200, () => string.Format("{0} days", Math.Floor(Math.Abs(oSpan.TotalDays)))); // 60 * 24 * 30
			aValue.Add(86400, () => "about a month"); // 60 * 24 * 60
			aValue.Add(525600, () => string.Format("{0} months", Math.Floor(Math.Abs(oSpan.TotalDays / 30)))); // 60 * 24 * 365
			aValue.Add(1051200, () => "about a year"); // 60 * 24 * 365 * 2
			aValue.Add(double.MaxValue, () => string.Format("{0} years", Math.Floor(Math.Abs(oSpan.TotalDays / 365))));

			return aValue.First(n => TotalMinutes < n.Key).Value.Invoke() + Suffix;
		}

		/// <summary>
		/// Gets a DateTime representing the last day in the current month
		/// </summary>
		/// <param name="current">The current date</param>
		/// <returns></returns>
		public static DateTime Last(this DateTime current) {
			int daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);

			DateTime last = current.First().AddDays(daysInMonth - 1);
			return last;
		}

		/// <summary>
		/// Gets a DateTime representing the last specified day in the current month
		/// </summary>
		/// <param name="current">The current date</param>
		/// <param name="dayOfWeek">The current day of week</param>
		/// <returns></returns>
		public static DateTime Last(this DateTime current, DayOfWeek dayOfWeek) {
			DateTime last = current.Last();

			last = last.AddDays(Math.Abs(dayOfWeek - last.DayOfWeek) * -1);
			return last;
		}

		/// <summary>
		/// Gets a DateTime representing midnight on the current date
		/// </summary>
		/// <param name="current">The current date</param>
		public static DateTime Midnight(this DateTime current) {
			DateTime midnight = new DateTime(current.Year, current.Month, current.Day);
			return midnight;
		}

		/// <summary>
		/// Gets a DateTime representing the first date following the current date which falls on the given day of the week
		/// </summary>
		/// <param name="current">The current date</param>
		/// <param name="dayOfWeek">The day of week for the next date to get</param>
		public static DateTime Next(this DateTime current, DayOfWeek dayOfWeek) {
			int offsetDays = dayOfWeek - current.DayOfWeek;

			if (offsetDays <= 0) {
				offsetDays += 7;
			}

			DateTime result = current.AddDays(offsetDays);
			return result;
		}

		/// <summary>
		/// Gets a DateTime representing noon on the current date
		/// </summary>
		/// <param name="current">The current date</param>
		public static DateTime Noon(this DateTime current) {
			DateTime noon = new DateTime(current.Year, current.Month, current.Day, 12, 0, 0);
			return noon;
		}

		/// <summary>
		/// Sets the time of the current date with minute precision
		/// </summary>
		/// <param name="current">The current date</param>
		/// <param name="hour">The hour</param>
		/// <param name="minute">The minute</param>
		public static DateTime SetTime(this DateTime current, int hour, int minute) {
			return SetTime(current, hour, minute, 0, 0);
		}

		/// <summary>
		/// Sets the time of the current date with second precision
		/// </summary>
		/// <param name="current">The current date</param>
		/// <param name="hour">The hour</param>
		/// <param name="minute">The minute</param>
		/// <param name="second">The second</param>
		/// <returns></returns>
		public static DateTime SetTime(this DateTime current, int hour, int minute, int second) {
			return SetTime(current, hour, minute, second, 0);
		}

		/// <summary>
		/// Sets the time of the current date with millisecond precision
		/// </summary>
		/// <param name="current">The current date</param>
		/// <param name="hour">The hour</param>
		/// <param name="minute">The minute</param>
		/// <param name="second">The second</param>
		/// <param name="millisecond">The millisecond</param>
		/// <returns></returns>
		public static DateTime SetTime(this DateTime current, int hour, int minute, int second, int millisecond) {
			DateTime atTime = new DateTime(current.Year, current.Month, current.Day, hour, minute, second, millisecond);
			return atTime;
		}

	}

	public static class DoubleExtensions
	{
		/// <summary>
		/// Convert to currency. 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="cultureName"></param>
		/// <example>
		/// string testString = test.ToCurrency("en-US"); 
		/// </example>
		/// <returns></returns>
		public static string ToCurrency(this double value, string cultureName) {
			CultureInfo currentCulture = new CultureInfo(cultureName);
			return (string.Format(currentCulture, "{0:C}", value));
		}
	}

	public static class FileExtensions
	{
		struct BY_HANDLE_FILE_INFORMATION
		{
			public uint FileAttributes;
			public System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;
			public System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;
			public System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;
			public uint VolumeSerialNumber;
			public uint FileSizeHigh;
			public uint FileSizeLow;
			public uint NumberOfLinks;
			public uint FileIndexHigh;
			public uint FileIndexLow;
		}

		//
		// CreateFile constants
		//
		const uint FILE_SHARE_READ = 0x00000001;
		const uint OPEN_EXISTING = 3;
		const uint GENERIC_READ = (0x80000000);
		const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr CreateFile(
			string lpFileName,
			uint dwDesiredAccess,
			uint dwShareMode,
			IntPtr lpSecurityAttributes,
			uint dwCreationDisposition,
			uint dwFlagsAndAttributes,
			IntPtr hTemplateFile);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool GetFileInformationByHandle(IntPtr hFile, out BY_HANDLE_FILE_INFORMATION lpFileInformation);

		public static bool IsSameFileAs(this FileSystemInfo file, string path) {
			BY_HANDLE_FILE_INFORMATION fileInfo1, fileInfo2;
			IntPtr ptr1 = CreateFile(file.FullName, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero);
			if ((int)ptr1 == -1) {
				System.ComponentModel.Win32Exception e = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
				throw e;
			}
			IntPtr ptr2 = CreateFile(path, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS, IntPtr.Zero);
			if ((int)ptr2 == -1) {
				System.ComponentModel.Win32Exception e = new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
				throw e;
			}
			GetFileInformationByHandle(ptr1, out fileInfo1);
			GetFileInformationByHandle(ptr2, out fileInfo2);

			return ((fileInfo1.FileIndexHigh == fileInfo2.FileIndexHigh) &&
				(fileInfo1.FileIndexLow == fileInfo2.FileIndexLow));
		}
	}

	public static class GenericListExtensions
	{
		public static DataTable ToDataTable<T>(this List<T> list) where T : class {
			DataTable dt = null;
			Type listType = list.GetType();
			if (listType.IsGenericType) {
				//determine the underlying type the List<> contains
				Type elementType = listType.GetGenericArguments()[0];

				//create empty table -- give it a name in case
				//it needs to be serialized
				dt = new DataTable(elementType.Name + "List");

				//define the table -- add a column for each public
				//property or field
				MemberInfo[] miArray = elementType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
				foreach (MemberInfo mi in miArray) {
					if (mi.MemberType == MemberTypes.Property) {
						PropertyInfo pi = mi as PropertyInfo;

						Type propType = pi.PropertyType;
						if (propType.IsGenericType &&
							propType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
							propType = Nullable.GetUnderlyingType(propType);
						}

						dt.Columns.Add(pi.Name, propType);
					}
					else if (mi.MemberType == MemberTypes.Field) {
						FieldInfo fi = mi as FieldInfo;
						dt.Columns.Add(fi.Name, fi.FieldType);
					}
				}

				//populate the table
				IList il = list as IList;
				foreach (object record in il) {
					int i = 0;
					object[] fieldValues = new object[dt.Columns.Count];
					foreach (DataColumn c in dt.Columns) {
						MemberInfo mi = elementType.GetMember(c.ColumnName)[0];
						if (mi.MemberType == MemberTypes.Property) {
							PropertyInfo pi = mi as PropertyInfo;
							fieldValues[i] = pi.GetValue(record, null);
						}
						else if (mi.MemberType == MemberTypes.Field) {
							FieldInfo fi = mi as FieldInfo;
							fieldValues[i] = fi.GetValue(record);
						}
						i++;
					}
					dt.Rows.Add(fieldValues);
				}
			}
			return dt;
		}

		/// <summary>
		/// This method will take a list of navigation hyperlinks and append the css class any link whose navigate url matches the current Request's RawUrl
		/// </summary>
		/// <param name="NavLinks">
		/// List of navigation links to check
		/// </param>
		/// <param name="ActiveStateCssClass">
		/// Css class that represents the active state to append to any link that is matched
		/// </param>
		/// <returns></returns>
		public static void SetActiveState(this List<HyperLink> NavLinks, string ActiveStateCssClass) {

			//call master method
			SetActiveState(NavLinks, HttpContext.Current.Request.RawUrl.ToLower(), ActiveStateCssClass);
		}

		/// <summary>
		/// This method will take a list of navigation hyperlinks and append the css class any link whose navigate url matches the url passed in
		/// </summary>
		/// <param name="NavLinks">
		/// List of navigation links to check
		/// </param>
		/// <param name="UrlToMatch">
		/// The url to match the Navigate Url against for matches
		/// </param>
		/// <param name="ActiveStateCssClass">
		/// Css class that represents the active state to append to any link that is matched
		/// </param>
		public static void SetActiveState(this List<HyperLink> NavLinks, string UrlToMatch, string ActiveStateCssClass) {

			foreach (HyperLink lnk in NavLinks) {
				if (UrlToMatch.Equals(lnk.NavigateUrl.ToLower())) {
					lnk.CssClass += " " + ActiveStateCssClass;
				}
			}
		}

		/// <summary>
		/// Randomizes a generic list.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static void Shuffle<T>(this IList<T> list) {
			Random rng = new Random();
			int n = list.Count;
			while (n > 1) {
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

	}

	public static class NullableBooleanExtensions
	{
		/// <summary>
		/// Convert to Integer specifying a default value.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static int ToInt(this bool? value, int defaultValue) {
			int res;
			try {
				res = value.HasValue ? Convert.ToInt32(value) : defaultValue;
			}
			catch (Exception) {
				res = 0;
			}
			return res;
		}

		/// <summary>
		/// Convert to Integer.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static int ToInt(this bool? value) {
			int res;
			try {
				res = value.HasValue ? Convert.ToInt32(value) : 0;
			}
			catch (Exception) {
				res = 0;
			}
			return res;
		}
	}

	public static class ObjectExtensions
	{
		/// <summary>
		/// Clone properties from an original object to a destination object.
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <param name="origin"></param>
		/// <param name="destination"></param>
		public static void CloneProperties<T1, T2>(this T1 origin, T2 destination) {
			// Instantiate if necessary
			if (destination == null)
				throw new ArgumentNullException("destination", "Destination object must first be instantiated.");
			// Loop through each property in the destination
			foreach (var destinationProperty in destination.GetType().GetProperties()) {
				// find and set val if we can find a matching property name and matching type in the origin with the origin's value
				if (origin != null && destinationProperty.CanWrite) {
					origin.GetType().GetProperties().Where(x => x.CanRead && (x.Name == destinationProperty.Name && x.PropertyType == destinationProperty.PropertyType))
						.ToList()
						.ForEach(x => destinationProperty.SetValue(destination, x.GetValue(origin, null), null));
				}
			}
		}
	}

	public static class StringExtensions
	{
		public static string Decrypt(this string stringToDecrypt, string encryptionKey) {
			byte[] inputByteArray = new byte[stringToDecrypt.Length];
			byte[] key = { };
			byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
			string strEncryptionKey = encryptionKey;

			stringToDecrypt = stringToDecrypt.Replace(" ", "+");

			key = System.Text.Encoding.UTF8.GetBytes(Left(strEncryptionKey, 8));
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			inputByteArray = Convert.FromBase64String(stringToDecrypt);
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
			cs.Write(inputByteArray, 0, inputByteArray.Length);
			cs.FlushFinalBlock();
			System.Text.Encoding encoding = System.Text.Encoding.UTF8;
			return encoding.GetString(ms.ToArray());
		}

		public static string Encrypt(this string stringToEncrypt, string encryptionKey) {
			byte[] key = { };
			byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
			string strEncryptionKey = encryptionKey;

			key = System.Text.Encoding.UTF8.GetBytes(Left(strEncryptionKey, 8));
			DESCryptoServiceProvider des = new DESCryptoServiceProvider();
			byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
			cs.Write(inputByteArray, 0, inputByteArray.Length);
			cs.FlushFinalBlock();
			return Convert.ToBase64String(ms.ToArray());
		}

		/// <summary>
		/// Indicates whether the regular expression finds a match in the input string using the regular expression specified in the pattern parameter.
		/// </summary>
		/// <param name="s">String to match</param>
		/// <param name="regularExpression">Regular expression, eg. [a-Z]{3}</param>
		/// <param name="matchEntirely">Return true only if string was matched entirely</param>
		/// <returns></returns>
		public static bool IsMatch(this string s, string regularExpression, bool matchEntirely) {
			return Regex.IsMatch(s, matchEntirely ? "\\A" + regularExpression + "\\z" : regularExpression);
		}

		/// <summary>
		/// Indicates whether the string is a null object or an empty string.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this string s) {
			return string.IsNullOrEmpty(s);
		}

		/// <summary>
		/// Enable quick and more natural string.Format calls
		/// </summary>
		/// <param name="s"></param>
		/// <param name="number_of_characters"></param>
		/// <returns></returns>
		public static string Format(this string s, params object[] args) {
			return string.Format(s, args);
		}

		/// <summary>
		/// Return the leftmost number_of_characters characters
		/// from the string.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="number_of_characters"></param>
		/// <returns></returns>
		public static string Left(this string s, int number_of_characters) {
			if (s.Length <= number_of_characters)
				return s;
			return s.Substring(0, number_of_characters);
		}

		/// <summary>
		/// Return the string with the leftmost
		/// number_of_characters characters removed.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="number_of_characters"></param>
		/// <returns></returns>
		public static string RemoveLeft(this string s, int number_of_characters) {
			if (s.Length <= number_of_characters)
				return string.Empty;
			return s.Substring(number_of_characters);
		}

		/// <summary>
		/// Return the string with the rightmost
		/// number_of_characters characters removed.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="number_of_characters"></param>
		/// <returns></returns>
		public static string RemoveRight(this string s, int number_of_characters) {
			if (s.Length <= number_of_characters)
				return string.Empty;
			return s.Substring(0, s.Length - number_of_characters);
		}

		/// <summary>
		/// Finds the specified Start Text and the End Text in this string instance, and returns a string
		/// containing all the text starting from startText, to the begining of endText. (endText is not
		/// included.)
		/// </summary>
		/// <param name="s">The string to retrieve the subset from.</param>
		/// <param name="startText">The Start Text to begin the Subset from.</param>
		/// <param name="endText">The End Text to where the Subset goes to.</param>
		/// <param name="ignoreCase">Whether or not to ignore case when comparing startText/endText to the string.</param>
		/// <returns>A string containing all the text starting from startText, to the begining of endText.</returns>
		public static string Substring(this string s, string startText, string endText, bool ignoreCase) {
			if (string.IsNullOrEmpty(startText) || string.IsNullOrEmpty(endText)) {
				throw new ArgumentException("Start Text and End Text cannot be empty.");
			}
			string temp = s;
			if (ignoreCase) {
				temp = s.ToUpperInvariant();
				startText = startText.ToUpperInvariant();
				endText = endText.ToUpperInvariant();
			}
			int start = temp.IndexOf(startText);
			int end = temp.IndexOf(endText, start);
			return Substring(s, start, end);
		}

		/// </summary>
		/// <param name="s">The string to retrieve the subset from.</param>
		/// <param name="startIndex">The specified start index for the subset.</param>
		/// <param name="endIndex">The specified end index for the subset.</param>
		/// <returns>A Subset string starting at the specified start index and ending and the specified end
		/// index.</returns>
		public static string Substring(this string s, int startIndex, int endIndex) {
			if (startIndex > endIndex) {
				throw new InvalidOperationException("End Index must be after Start Index.");
			}

			if (startIndex < 0) {
				throw new InvalidOperationException("Start Index must be a positive number.");
			}

			if (endIndex < 0) {
				throw new InvalidOperationException("End Index must be a positive number.");
			}

			return s.Substring(startIndex, (endIndex - startIndex));
		}

		///<summary>
		///Method to strip HTML tags from a string.
		///</summary>
		///<param name="str">The string to strip</param>
		///<returns></returns>
		///<remarks></remarks>
		public static string StripHTML(this string s) {
			return s.StripHTML(string.Empty);
		}

		///<summary>
		///Method to strip HTML tags from a string.
		///</summary>
		///<param name="str">The string to strip</param>
		///<param name="fillString">The string to replace any found matches</param>
		///<returns></returns>
		///<remarks></remarks>
		public static string StripHTML(this string s, string fillString) {
			return Regex.Replace(s, "<.*?>", fillString);
		}

		/// <summary>
		/// Convert to Boolean specifying a default value.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static bool ToBool(this string s, bool defaultValue) {
			bool res;
			if (!string.IsNullOrEmpty(s))
				return bool.TryParse(s, out res) ? res : defaultValue;
			else
				return false;
		}

		/// <summary>
		/// Convert to Boolean.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static bool ToBool(this string s) {
			bool res;
			if (!string.IsNullOrEmpty(s))
				return bool.TryParse(s, out res) ? res : false;
			else
				return false;
		}

		/// <summary>
		/// Convert to DateTime specifying a default value.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this string s, DateTime defaultValue) {
			DateTime res;
			if (!string.IsNullOrEmpty(s))
				return DateTime.TryParse(s, out res) ? res : defaultValue;
			else
				return DateTime.MinValue;
		}

		/// <summary>
		/// Convert to DateTime.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this string s) {
			DateTime res;
			if (!string.IsNullOrEmpty(s))
				return DateTime.TryParse(s, out res) ? res : DateTime.MinValue;
			else
				return DateTime.MinValue;
		}

		/// <summary>
		/// Convert to Decimal specifying a default value.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static decimal ToDecimal(this string s, decimal defaultValue) {
			decimal res;
			if (!string.IsNullOrEmpty(s))
				return decimal.TryParse(s, out res) ? res : defaultValue;
			else
				return 0;
		}

		/// <summary>
		/// Convert to Decimal.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static decimal ToDecimal(this string s) {
			decimal res;
			if (!string.IsNullOrEmpty(s))
				return decimal.TryParse(s, out res) ? res : 0;
			else
				return 0;
		}

		/// <summary>
		/// Convert to Integer specifying a default value.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static int ToInt(this string s, int defaultValue) {
			int res;
			if (!string.IsNullOrEmpty(s))
				return int.TryParse(s, out res) ? res : defaultValue;
			else
				return 0;
		}

		/// <summary>
		/// Convert to Integer.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int ToInt(this string s) {
			int res;
			try {
				if (!string.IsNullOrEmpty(s))
					res = int.Parse(s);
				else
					res = 0;
			}
			catch (OverflowException) {
				throw;
			}
			catch (Exception) {
				res = 0;
			}
			return res;
		}

		/// <summary>
		/// Convert to Long specifying a default value.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static long ToLong(this string s, long defaultValue) {
			long res;
			try {
				if (!string.IsNullOrEmpty(s))
					res = long.Parse(s);
				else
					res = 0;
			}
			catch (OverflowException) {
				throw;
			}
			catch (Exception) {
				res = defaultValue;
			}
			return res;
		}

		/// <summary>
		/// Convert to Long.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static long ToLong(this string s) {
			long res;
			if (!string.IsNullOrEmpty(s))
				return long.TryParse(s, out res) ? res : 0;
			else
				return 0;
		}

		/// <summary>
		/// Formats a string to title case.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ToTitleCase(this string s) {
			string rText = "";
			try {
				System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
				System.Globalization.TextInfo TextInfo = cultureInfo.TextInfo;
				rText = TextInfo.ToTitleCase(s.ToLower());
			}
			catch {
				rText = s;
			}
			return rText;
		}

		/// <summary>
		/// Truncate text by given input to x characters with trailing ellipsis.
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="text"></param>
		/// <param name="maxLength"></param>
		/// <returns></returns>
		public static string Truncate(this string s, int maxLength) {
			if (!string.IsNullOrEmpty(s))
				return s.Length > maxLength ? s.Substring(0, maxLength) + "&hellip;" : s;
			return string.Empty;
		}
	}

	public static class ValidatorExtensions
	{
		/// <summary>
		/// Return true if value is between a range of values.
		/// </summary>
		/// <returns></returns>
		public static bool Between(this int value, int start, int end, bool inclusive) {
			if (inclusive)
				return (value >= start && value <= end) ? true : false;
			return (value > start && value < end) ? true : false;
		}
	}

	public static class WebControlExtensions
	{
		/// <summary>
		/// Clears any selected items.
		/// </summary>
		/// <param name="ddl"></param>
		/// <returns></returns>
		public static void ClearSelected(this DropDownList ddl) {
			foreach (ListItem li in ddl.Items) {
				li.Selected = false;
			}
		}

		/// <summary>
		/// Clears any selected items.
		/// </summary>
		/// <param name="rdo"></param>
		/// <returns></returns>
		public static void ClearSelected(this RadioButtonList rdo) {
			foreach (ListItem li in rdo.Items) {
				li.Selected = false;
			}
		}

		/// <summary>
		/// Clears any selected items.
		/// </summary>
		/// <param name="ctrl"></param>
		/// <returns></returns>
		public static void ClearSelected(this ListBox ctrl) {
			foreach (ListItem li in ctrl.Items) {
				li.Selected = false;
			}
		}

		/// <summary>
		/// Gets a RadioButtonList's selected text.
		/// </summary>
		/// <param name="rdo"></param>
		/// <param name="value">The selected value</param>
		/// <returns></returns>
		public static string GetSelectedText(this RadioButtonList rdo) {
			string rText = string.Empty;
			foreach (ListItem li in rdo.Items) {
				if (li.Selected) {
					rText = li.Text;
					break;
				}
			}

			return rText;
		}

		/// <summary>
		/// Gets a RadioButtonList's selected value.
		/// </summary>
		/// <param name="rdo"></param>
		/// <param name="value">The selected value</param>
		/// <returns></returns>
		public static string GetSelectedValue(this RadioButtonList rdo) {
			string rText = string.Empty;
			foreach (ListItem li in rdo.Items) {
				if (li.Selected) {
					rText = li.Value;
					break;
				}
			}

			return rText;
		}

		/// <summary>
		/// Selects a DropDownList's selected text.
		/// </summary>
		/// <param name="ddl"></param>
		/// <param name="value">The selected value</param>
		/// <returns></returns>
		public static void SelectText(this DropDownList ddl, string value) {
			ddl.SelectedIndex = -1;
			foreach (ListItem li in ddl.Items) {
				if (li.Text == value) {
					li.Selected = true;
					break;
				}
			}
		}

		/// <summary>
		/// Selects a DropDownList's selected text.
		/// </summary>
		/// <param name="ddl"></param>
		/// <param name="value">The selected value</param>
		/// <returns></returns>
		public static void SelectText(this RadioButtonList rdo, string value) {
			foreach (ListItem li in rdo.Items) {
				if (li.Text == value) {
					li.Selected = true;
					break;
				}
			}
		}

		/// <summary>
		/// Selects a DropDownList's selected value.
		/// </summary>
		/// <param name="ddl"></param>
		/// <param name="value">The selected value</param>
		/// <returns></returns>
		public static void SelectValue(this DropDownList ddl, string value) {
			ddl.Items.FindByValue(value).Selected = true;
		}

		/// <summary>
		/// Sets a RadioButtonList's selected value.
		/// </summary>
		/// <param name="rdo"></param>
		/// <param name="value">The selected value</param>
		/// <returns></returns>
		public static void SelectValue(this RadioButtonList rdo, string value) {
			foreach (ListItem li in rdo.Items) {
				if (li.Value == value) {
					li.Selected = true;
					break;
				}
			}
		}

		public static bool IsItemTemplate(this RepeaterItemEventArgs e) {
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
				return true;
			else
				return false;
		}
	}

	public static class XMLExtensions
	{
		public static DataTable ToDataTable(this XmlNodeList nodeList) {
			DataTable dt = new DataTable();
			int TempColumn = 0;

			foreach (XmlNode node in nodeList.Item(0).ChildNodes) {
				TempColumn += 1;

				DataColumn dc = new DataColumn(node.Name, System.Type.GetType("System.String"));
				if ((dt.Columns.Contains(node.Name))) {
					dt.Columns.Add(dc.ColumnName + TempColumn.ToString());
				}
				else {
					dt.Columns.Add(dc);
				}
			}

			int ColumnsCount = dt.Columns.Count;
			for (int i = 0; i <= nodeList.Count - 1; i++) {
				DataRow dr = dt.NewRow();
				for (int j = 0; j < ColumnsCount; j++) {
					dr[j] = nodeList.Item(i).ChildNodes[j].InnerText;
				}
				dt.Rows.Add(dr);
			}

			return dt;
		}
	}

	public static class MiscExtensions
	{
		public static string GetLiveItemTitle_AnchorFriendly(string LiveItemTitle) {
			LiveItemTitle = LiveItemTitle.Replace(", ", "-");
			LiveItemTitle = LiveItemTitle.Replace(" ", "-");
			LiveItemTitle = LiveItemTitle.Replace("'", "");

			return LiveItemTitle;
		}

		public static bool IsPreviewMode() {
			return (HttpContext.Current.Session["ViewingMode"] != null && ((string)HttpContext.Current.Session["ViewingMode"]) == "Preview");
		}
	}
}