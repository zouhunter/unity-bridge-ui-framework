using System;

namespace NodeGraph {
	public static class CustomScriptUtility {

		static readonly bool debug = false;

		public static string DecodeString(string data) {
			if(data.StartsWith(NGSettings.BASE64_IDENTIFIER)) {
				var bytes = Convert.FromBase64String(data.Substring(NGSettings.BASE64_IDENTIFIER.Length));
				data = System.Text.Encoding.UTF8.GetString(bytes);
			}
			return data;
		}
		public static string EncodeString(string data) {
			if(debug) {
				return data;
			} else {
				return NGSettings.BASE64_IDENTIFIER + 
					Convert.ToBase64String( System.Text.Encoding.UTF8.GetBytes(data));
			}
		}
	}
}
