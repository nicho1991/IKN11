using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_client
	{



		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";


		//main
		public static void Main (string[] args)
		{
			Console.WriteLine (APP);
			string path = Console.ReadLine ();
			file_client client = new file_client (path);

		}


		//client
		private file_client(string path)
		{
			if (path.Length > 1) {
				Transport transport = new Transport (BUFSIZE,APP);
				byte[] buffer = new byte[BUFSIZE];
				string filePath = path;
				Console.WriteLine ("Requesting file: " + filePath);

				transport.send (Encoding.UTF8.GetBytes (filePath), filePath.Length);
				int size = transport.receive(ref buffer);
				int fsize = 0;
				if (fsize != 0) {
					fsize = int.Parse (Encoding.UTF8.GetString (buffer, 0, size));
					Console.WriteLine ("File Found");
					if (fsize > 0) {
						receiveFile (filePath, fsize, transport);
					}
				}
				else
					Console.WriteLine ("The file does not exist!");
			}
			else
				Console.WriteLine ("Invalid arguments!", path.Length);
		}

		//receive
		private void receiveFile (String path, long fileSize,Transport transport)
		{
			// TO DO Your own code
			byte[] fileBuffer = new byte[BUFSIZE];
			string fileName = LIB.extractFileName (path);
			var readSize = 0;
			var read = 0;

			Console.WriteLine ("Waiting for transfer to complete...");

			FileStream fs = new FileStream (fileName, FileMode.Create, FileAccess.Write);
			while(read < fileSize && (readSize = transport.receive(ref fileBuffer)) > 0)
			{
				fs.Write (fileBuffer, 0, readSize);
				read += readSize;
			}

			if (read == fileSize)
				Console.WriteLine ("Success!");
			else
				Console.WriteLine ("An error occured!");

			fs.Close ();
		}
			
	}
}