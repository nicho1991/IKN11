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
			int size = 0;
			while (true) 
			{
				byte[] responsebuff = new byte[BUFSIZE]; 
				byte[] buffer = new byte[BUFSIZE];

				//receive
				while((size = transport.receive (ref buffer)) == 0)
				{
					Console.Write("\rwaiting for name");
				}
				Console.WriteLine ("received a name");

				//Console.WriteLine (buffer.Length+System.Text.Encoding.ASCII.GetString(buffer));
				//check what we got here
				string received = Encoding.UTF8.GetString(buffer,0,size -4 );
				//this must be a filename!
				//Console.WriteLine($"After link layer server got filename: {received}");

				//find the file
				long filesize = LIB.check_File_Exists(received);


				//send størrelse:
				if (filesize != 0) {
					Console.WriteLine ("found the file");
					//Console.WriteLine ("\n Sender: " + "\n Størrelse: " + filesize);

					string filename = LIB.extractFileName (received);
					Console.WriteLine ("filename is " + filename);


					//Console.WriteLine ("sender om lidt..");
					//Thread.Sleep (1000);
					Console.WriteLine ("sender navn " + filename);

					string requestName = filename;
					responsebuff = Encoding.ASCII.GetBytes (requestName);
					transport.send (responsebuff, responsebuff.Length);
					Console.WriteLine ("navn sendt afsted");


					string requestSize = filesize.ToString();
					responsebuff = Encoding.ASCII.GetBytes (requestSize);
					Console.WriteLine ("sender størrelse " + filesize);

					transport.send (responsebuff, responsebuff.Length);
					Console.WriteLine ("størrelse sendt afsted");

					sendFile (received, filesize, transport);


					//send data	
					/*
					Console.WriteLine ("Sending file: " + received + " to client");

					var fileBuf = new byte[BUFSIZE];
					var bytesRead = 0;

					using (var fileStream = File.Open (received, FileMode.Open)) {

						while ((bytesRead = ReadChunk (fileStream, ref fileBuf)) != 0) {
							transport.send (fileBuf, bytesRead);
						}

					}
						Console.WriteLine ("The requested file was sent");

					*/

				} 
				else {
					Console.WriteLine ("file not found");
				}
				
				//done reset transport
				transport = new Transport (BUFSIZE, APP);


				}




		}

		/// <summary>
		/// Reads the chunk.  https://stackoverflow.com/questions/5659189/how-to-split-a-large-file-into-chunks-in-c
		/// </summary>
		/// <returns>index of the chunk</returns>
		/// <param name="stream">Stream.</param>
		/// <param name="chunk">Chunk.</param>
		private int ReadChunk(FileStream stream, ref byte[] chunk)
		{
			int index = 0;
			while (index < chunk.Length) {
				int bytesRead = stream.Read (chunk, index, chunk.Length - index);
				if (bytesRead == 0) {
					break;
				}
				index += bytesRead;
			}
			return index;
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

			byte[] filesizebuf = new byte[BUFSIZE];

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