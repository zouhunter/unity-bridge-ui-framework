using UnityEngine;
using System.Collections;
using System.Linq;
using System;


	public static class StringUtils {

		///Convert camelCase to words as the name implies.
		public static string SplitCamelCase(this string s){
			if (string.IsNullOrEmpty(s)) return s;
			s = s.Replace("_", " ");
			s = char.ToUpper(s[0]) + s.Substring(1);
			return System.Text.RegularExpressions.Regex.Replace(s, "(?<=[a-z])([A-Z])", " $1").Trim();
		}

		///Gets only the capitals of the string trimmed.
		public static string GetCapitals(this string s){
	    	if (string.IsNullOrEmpty(s)){
	    		return string.Empty;
	    	}
	    	var result = "";
	    	foreach(var c in s){
	    		if (char.IsUpper(c)){
	    			result += c.ToString();
	    		}
	    	}
	    	result = result.Trim();
	    	return result;			
		}

		///Returns the alphabet letter based on it's index.
		public static string GetAlphabetLetter(int index){
			if (index < 0){
				return null;
			}

			var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			if (index >= letters.Length){
				return index.ToString();
			}

			return letters[index].ToString();
		}
	}
