using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;
using System.Threading;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";

		/*		
		IPAddress localAddr = IPAddress.Parse("10.0.0.1");
		byte[] buff = new byte[BUFSIZE];
		*/

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			//Link link = new Link(BUFSIZE,APP);
			Transport transport = new Transport (BUFSIZE, APP);
			Console.WriteLine("Server started");
			while (true) 
			{
				byte[] responsebuff = new byte[BUFSIZE]; 
				byte[] buffer = new byte[BUFSIZE];
				
				if (transport.receive (ref buffer) > 0) { 

					Console.WriteLine (buffer.Length+System.Text.Encoding.ASCII.GetString(buffer));
					//check what we got here
					string received = System.Text.Encoding.ASCII.GetString(buffer);
					//this must be a filename!
					Console.WriteLine($"After link layer server got filename: {received}");


					//find the file
					long filesize = LIB.check_File_Exists(received);


					//Thread.Sleep (500);
					if (filesize != 0) {
						Console.WriteLine ("\n Sendte: " + "\n Filnavn: " + received + "\n Størrelse: " + filesize);
						string request = "Filnavn: " + received + "Størrelse: " + filesize;
						responsebuff = Encoding.ASCII.GetBytes(request);
						transport.send (responsebuff, responsebuff.Length);

					}




				}
				else {
					
				}
			}



			/*
			while (true) {
				//wait for client
				clientSocket = serverSocket.AcceptTcpClient ();
				Console.WriteLine ("client connected");

				//opretter en stream fra client
				NetworkStream serverStreamIO = clientSocket.GetStream (); 
				Console.WriteLine (" >> Accepted connection from client");

				//modtager filnavn
				string fileDir; //= @"/root/Desktop/IKNServerClientTCP/Exercise_6_server/file_server/bin/Debug/files/";
				string userfile = tcp.LIB.readTextTCP (serverStreamIO);

				fileDir = userfile;
				//check for exsitens af fil
				long lengthOfFile = tcp.LIB.check_File_Exists (fileDir);

				if (lengthOfFile != 0) {//filen findes
					Console.WriteLine ("filen findes " + fileDir);
					//find størrelsen på filen
					//long filesize = new System.IO.FileInfo (fileDir).Length;
					long filesize = LIB.check_File_Exists(fileDir);
					//send the file
					sendFile (fileDir, filesize, serverStreamIO);
				} else { //filen exsitere ikke
					Console.WriteLine ("Filen findes ikke " + fileDir);
					tcp.LIB.writeTextTCP (serverStreamIO, "filen findes ikke");
				}
				//clientSocket.Close ();
			}


			serverSocket.Stop();
			*/
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String fileName, long fileSize, Transport transport)
		{

			//send the size
			string filesizestring = fileSize.ToString();



			byte[] filesizebuf = new byte[BUFSIZE];

			filesizebuf = Encoding.ASCII.GetBytes (filesizestring);

			//send it
			//Console.WriteLine(filesizebuf.Length);
			transport.send (filesizebuf, filesizebuf.Length);



			//get file
			FileStream fs = new FileStream (fileName, FileMode.Open, FileAccess.Read);

			int numberOfPackages = Convert.ToInt32 (Math.Ceiling (Convert.ToDouble (fileSize) / Convert.ToDouble (BUFSIZE)));
			long currentPacketLength = 0;
			long totalLength = fileSize;

			//write out
			for (int i = 0; i < numberOfPackages; i++) {
				if (totalLength > BUFSIZE) {
					currentPacketLength = BUFSIZE;
					totalLength -= BUFSIZE;


				} else {

					currentPacketLength = totalLength;
				}

				byte[] sendingBuffer = new byte[currentPacketLength];

				fs.Read (sendingBuffer, 0, (int)currentPacketLength);
				transport.send(sendingBuffer, sendingBuffer.Length);
			}
			fs.Close ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			file_server x = new file_server();
		}
	}
}