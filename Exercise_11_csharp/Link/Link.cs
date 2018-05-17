using System;
using System.IO.Ports;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
				serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);


			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)];

			// Uncomment the next line to use timeout
			//serialPort.ReadTimeout = 500;

			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();

		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
			if (!serialPort.IsOpen) {
				return;

				serialPort.Write (buf, 0, size);

			}
			//check what we got
			Console.WriteLine ("Link");
			for(int i = 0 ; i < size ; i++)
			{
				Console.WriteLine(buf[i]);
			}	


			char startEnd = 'A';
			//convert to string so we can manipulate

			//temp buff to avoid transport layer
			var tempBuf = new byte[size-4];
			for (int i = 4; i < size; i++) {
				tempBuf [i - 4] = buf [i];
			}


			string request = System.Text.Encoding.ASCII.GetString (tempBuf);





			//follow protocol



			string send = startEnd + request.Replace ("B", "BD").Replace ("A", "BC") + startEnd;
			Console.WriteLine($"Link laget sender {send}");


			var senderByteArray = new byte[size];
			//put in the checksum etc
			for (int i = 0; i < 4; i++) {
				senderByteArray [i] = buf[i];
			}
			//put in data
			for(int i = 4; i < size;i++ ){
				senderByteArray [i] = tempBuf [i - 4];
			}


			//send the message

			serialPort.Write (senderByteArray, 0, senderByteArray.Length);

		}

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public int receive (ref byte[] buf)
		{
			if (!serialPort.IsOpen) {
				return 0;
			}
			//read from the port
			int bytesToRead = serialPort.BytesToRead;

			//check if there is something to read
			if(bytesToRead > 0){
				serialPort.Read (buf, 0, bytesToRead);

				//convert to ascii so we can revert to normal
				string received = System.Text.Encoding.ASCII.GetString(buf);
				Console.WriteLine ($"Link laget modtog: {received}");


				//see that message is contained in A - A
				if(received.StartsWith("A") && received.EndsWith("A"))
				{
				//remove Start and end
				string normal = received.Replace("A","");
				normal = normal.Replace("BD","B").Replace("BC","A");
				//convert to byte[] and set buf
				buf = System.Text.Encoding.ASCII.GetBytes(normal);
				return buf.Length;
				}
				return 0;
			}

			return 0;


		}
	}
}
