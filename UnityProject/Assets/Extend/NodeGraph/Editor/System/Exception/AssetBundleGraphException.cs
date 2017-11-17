using System;
using UnityEngine;
using System.Collections;

namespace NodeGraph {

	public class AssetBundleGraphException : Exception {
		public AssetBundleGraphException(string message) : base(message) {
		}
	}
}