using UnityEngine;
using System;
using System.Collections;

namespace NodeGraph {

	public class LogUtility {

		public static readonly string kTag = "Node";

		private static Logger s_logger;

		public static Logger Logger {
			get {
				if(s_logger == null) {
					#if UNITY_2017_1_OR_NEWER
					s_logger = new Logger(Debug.unityLogger.logHandler);
					#else
					s_logger = new Logger(Debug.logger.logHandler);
					#endif
				}

				return s_logger;
			}
		}
	}
}
