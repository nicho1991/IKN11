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

<<<<<<< HEAD
=======
		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(String[] args)
	    {
			///check for link lag

			Linklaget.Link client = new Linklaget.Link (BUFSIZE, APP);
			byte[] req = new byte[256];
			//send en fil request
			string request = "/root/Desktop/IKN11/Exercise_11_csharp/files/penis.txt";
			Console.WriteLine($"trying to send {request}");
			req = Encoding.ASCII.GetBytes(request);
>>>>>>> parent of d47112f... hjej


<<<<<<< HEAD
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";

=======
			
>>>>>>> parent of d47112f... hjej

		//main
		public static void Main (string[] args)
		{
			Console.WriteLine (APP);
			Console.WriteLine ("reques a file with a path");
			string path = Console.ReadLine ();
			file_client client = new file_client (path);

		}


<<<<<<< HEAD
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
=======
			//find networkstream
			NetworkStream serverstreamIO = clientSocket.GetStream ();
			Console.WriteLine ("made networkstream");
			receiveFile (args[1].ToString(), serverstreamIO);
			*/
	    }
>>>>>>> parent of d47112f... hjej

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