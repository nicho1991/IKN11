using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_server
	{



		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";


		public static void Main (string[] args)
		{
			Console.WriteLine (APP);
			file_server server = new file_server ();
		}

		//server
		private file_server ()
		{

			//create transport
			Transport transport = new Transport (BUFSIZE);
			byte[] buffer = new byte[BUFSIZE];
			int size = 0;
			while (true) {

				while((size = transport.Receive(ref buffer)) == 0)
				{};


				if (size != 0) {
					string filePath = Encoding.UTF8.GetString (buffer, 0, size);

					string tmp = Path.GetFullPath ("File_Server_Home/"+filePath);
					sendFile (tmp,transport);

				}
				transport = new Transport (BUFSIZE);
			}
		}

		//sendfile
		private void sendFile(string filePath, Transport transport)
		{

			var size = (int)LIB.check_File_Exists (@filePath);
			var response = Encoding.UTF8.GetBytes (size.ToString());
			transport.Send (response,response.Length);

			if (size != 0) {

				Console.WriteLine ("File length:"+response.Length+ "File path" + @filePath);

				var buf= new byte[BUFSIZE];
				var bytes = 0;

				using (var fs = File.Open (@filePath, FileMode.Open)) {

					while ((bytes = fileStream (fs, ref buf)) != 0) {
						transport.Send (buf, bytes);
					}

				}
				Console.WriteLine ("File was sent");

			} else {
				Console.WriteLine ("File does not exist!");
			}
		}



		private int fileStream(FileStream fs, ref byte[] bytes)
		{
			int i = 0;
			while (i < bytes.Length) {
				int bytesRead = fs.Read (bytes, i, bytes.Length - i);
				if (bytesRead == 0) {
					break;
				}
				i += bytesRead;
			}
			return i;
		}
			
	}
}