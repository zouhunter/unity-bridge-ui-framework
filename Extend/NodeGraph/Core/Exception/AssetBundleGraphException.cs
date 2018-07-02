using System;
using UnityEngine;
using System.Collections;

namespace NodeGraph {

	public class DataModelException : Exception {
		public DataModelException(string message) : base(message) {
		}
	}
}