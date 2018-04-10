// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.IO;
using System.Text;

namespace BridgeUI.NRefactory
{
    /// <summary>
    /// Static helper class that constructs lexer and parser objects.
    /// </summary>
    public static class ParserFactory
    {
        public static Parser.ILexer CreateLexer(TextReader textReader)
        {
            return new BridgeUI.NRefactory.Parser.CSharp.Lexer(textReader);
        }

        public static IParser CreateParser(TextReader textReader)
        {
            Parser.ILexer lexer = CreateLexer(textReader);
            return new BridgeUI.NRefactory.Parser.CSharp.Parser(lexer);
        }

        public static IParser CreateParser(string fileName)
        {
            return CreateParser(fileName, Encoding.UTF8);
        }

        public static IParser CreateParser(string fileName, Encoding encoding)
        {
            string ext = Path.GetExtension(fileName);
            if (ext.Equals(".cs", StringComparison.InvariantCultureIgnoreCase))
                return CreateParser(new StreamReader(fileName, encoding));
            return null;
        }
    }
}
