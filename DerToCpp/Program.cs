using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerToCpp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                Console.WriteLine($"DerToCpp jmeno souboru certifikatu jmeno souboru klice");
                return;
            }
            string key = args[0];
            string cert = args[1];
            string outName = args[2];
            ToCpp(cert, key, outName);
        }

        static void ToCpp(string certFileName, string keyFileName, string outName)
        {
            string outFileName = outName;
            using (FileStream fsCert = new FileStream(certFileName, FileMode.Open))
            using (BinaryReader brCert = new BinaryReader(fsCert))
            using (FileStream fsKey = new FileStream(keyFileName, FileMode.Open))
            using (BinaryReader brKey = new BinaryReader(fsKey))
            using (StreamWriter sw = new StreamWriter(outFileName))
            {
                sw.WriteLine("#include \"open62541/types.h\"");
                sw.WriteLine();
                sw.WriteLine($"int\tPrivateKeyLength = {fsKey.Length};");
                sw.WriteLine("UA_Byte\tPrivateKey[] = {");
                WriteToCppFile(sw, fsKey, brKey);

                sw.WriteLine($"int\tCertificateLength = {fsCert.Length};");
                sw.WriteLine("UA_Byte\tCertificate[] = {");
                WriteToCppFile(sw, fsCert, brCert);
            }
            Console.WriteLine("Ok");
        }

        static void WriteToCppFile(StreamWriter sw, FileStream fs, BinaryReader br)
        {
            for (long i = 0; i < fs.Length; ++i)
            {
                if (i % 16 == 0 && i != 0)
                {
                    sw.WriteLine();
                }
                sw.Write($"0x{br.ReadByte():X2}");
                if (i != fs.Length - 1)
                {
                    sw.Write(", ");
                }
                else
                {
                    sw.WriteLine("};");
                }
            }
        }
    }
}
