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
			}
			char startEnd = 'A';
			//convert to string so we can manipulate
			string request = System.Text.Encoding.ASCII.GetString (buf);
			//follow protocol
			string send = startEnd + request.Replace ("B", "BD").Replace ("A", "BC") + startEnd;
			//convert back to byte[]
			buf = System.Text.Encoding.ASCII.GetBytes (send);
			//send the message
			serialPort.Write (buf, 0, buf.Length);

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
			int bytesToRead = serialPort.BytesToRead;
			serialPort.Read(buf, 0, bytesToRead);



			return buf.Length;
		}
	}
}
