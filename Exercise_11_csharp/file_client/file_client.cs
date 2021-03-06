using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";



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

			//Linklaget.Link client = new Linklaget.Link (BUFSIZE, APP);
			byte[] req = new byte[BUFSIZE];
			//send en fil request
			string request = "/root/Desktop/IKN11/Exercise_11_csharp/files/penis.txt";
			Console.WriteLine($"trying to send {request}");
			req = Encoding.ASCII.GetBytes(request);


			///check for transport lag
			Transport transport = new Transport(BUFSIZE,APP);
			transport.send (req, req.Length);

			Console.WriteLine ("the requst has been send");
			//int size = transport.receive (ref req);
			Console.WriteLine ("waiting to receive");

			//vent på at modtage fil her
			receiveFile(request, transport);

			/*
			System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient ();
			//opretter sockets
			Console.WriteLine("client made");

			//connecting
			clientSocket.Connect(args[0].ToString(),PORT);
			Console.WriteLine("connected to server");

			//find networkstream
			NetworkStream serverstreamIO = clientSocket.GetStream ();
			Console.WriteLine ("made networkstream");
			receiveFile (args[1].ToString(), serverstreamIO);
			*/
		}

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String fileName, Transport transport)
		{

			int size = 0;
			byte[] filesizebyte = new byte[BUFSIZE];

			//first we receive name
			size = transport.receive (ref filesizebyte);
			string fileNameReceived = Encoding.UTF8.GetString(filesizebyte,0,size -4 );
			Console.WriteLine ("modtog filnavn: " + fileNameReceived);


			//then receive size in string format
			size = transport.receive(ref filesizebyte);
			string fileSizeReceived = Encoding.UTF8.GetString(filesizebyte,0,size -4 );
			Console.WriteLine ("modtog størrelse: " + fileSizeReceived);






			//modtag fil
			int fileSize = Int32.Parse (fileSizeReceived);

			FileStream fs = new FileStream(fileNameReceived,FileMode.Create,FileAccess.Write);

			Int32 bytesReceived = 0;
			Int64 totalbytedReceived = 0;
			Int64 megaByte = 1048576;
			while ((bytesReceived = transport.receive (ref filesizebyte))>0) {
				totalbytedReceived += bytesReceived;
				fs.Write (filesizebyte, 0, bytesReceived-4);
				int percentCompleted = (int)Math.Round(((double)(totalbytedReceived/(double)fileSize)*100));
				Console.Write("\r{0} ", "Received: " + totalbytedReceived/megaByte + " Mbytes" + " Out of " + fileSize/megaByte + " Mbytes" + " total: " +  percentCompleted + " %");
			}
			if (totalbytedReceived > 0) {
				Console.WriteLine("You have received a file! congratulations!");
			}
			else{
				Console.WriteLine ("Sadly you did not receive a file :( ");
			}









			// TO DO Your own code
			/*
			 			Int64 fileSize = 0;
			//make byte array
			Byte[] buff = new byte[BUFSIZE];

			//tell what file we want
			tcp.LIB.writeTextTCP (io, fileName);
		

			//modtag fil størrelse
			fileSize = Int64.Parse( tcp.LIB.readTextTCP(io));
			Console.WriteLine ("size is " + fileSize);


			//modtag fil
			FileStream fs = new FileStream(fileName,FileMode.OpenOrCreate,FileAccess.Write);

			Int32 bytesReceived = 0;
			Int64 totalbytedReceived = 0;
			Int64 megaByte = 1048576;
			while ((bytesReceived = io.Read (buff,0,buff.Length))>0) {
				totalbytedReceived += bytesReceived;
				fs.Write (buff, 0, bytesReceived);
				int percentCompleted = (int)Math.Round(((double)(totalbytedReceived/(double)fileSize)*100));
				Console.Write("\r{0} ", "Received: " + totalbytedReceived/megaByte + " Mbytes" + " Out of " + fileSize/megaByte + " Mbytes" + " total: " +  percentCompleted + " %");
			}
			if (totalbytedReceived > 0) {
				Console.WriteLine("You have received a file! congratulations!");
			}
			else{
				Console.WriteLine ("Sadly you did not receive a file :( ");
			}



			io.Close ();
			fs.Close ();


			 */
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts...");
			new file_client(args);
		}
	}
}