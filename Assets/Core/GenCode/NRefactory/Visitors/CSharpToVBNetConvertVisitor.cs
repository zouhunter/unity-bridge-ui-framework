// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2517 $</version>
// </file>

using System;
using BridgeUI.NRefactory.Ast;

namespace BridgeUI.NRefactory.Visitors
{
	/// <summary>
	/// This class converts C# constructs to their VB.NET equivalents.
	/// </summary>
	[Obsolete("Use CSharpConstructsVisitor + ToVBNetConvertVisitor instead")]
	public class CSharpToVBNetConvertVisitor : CSharpConstructsVisitor
	{
		public override object VisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			base.VisitCompilationUnit(compilationUnit, data);
			compilationUnit.AcceptVisitor(new ToVBNetConvertVisitor(), data);
			return null;
		}
	}
}
