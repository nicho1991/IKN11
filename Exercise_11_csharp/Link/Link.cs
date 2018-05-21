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
		const byte DELIMITERB = (byte)'B';
		const byte DELIMITERC = (byte)'C';
		const byte DELIMITERD = (byte)'D';
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

			buffer = new byte[(BUFSIZE*2) +2];

			// Uncomment the next line to use timeout
			//serialPort.ReadTimeout = 500;
			serialPort.ReadTimeout = 500;
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
			var byteCount = Framing (buf, size);
			serialPort.Write(buffer, 0, byteCount);

		}


		public int receive (ref byte[] buf)
		{	
			while (!ReceiveWaiter()) { }
			int length = 0;

			while (length < buffer.Length)
			{
				var received = (byte)serialPort.ReadByte();
				buffer[length++] = received;
				if (received == DELIMITER) // we got the full package
					break;
			}

			int TotalSize = length;
			var size = Deframing (ref buf, TotalSize);

			return size;
		}



		private bool ReceiveWaiter()
		{
			var received = (byte)serialPort.ReadByte();
			if (received == DELIMITER)
				return true;

			return false;
		}

		private int Deframing(ref byte[] Data, int size)
		{
			var counter = 0;
			for (var i = 0; i < size - 1; i++)
			{
				if (buffer[i] == DELIMITERB) //check for A
				{
					if (buffer[++i] == DELIMITERC) //see if it really was A
						Data[counter++] = DELIMITER; //make A
					else
						Data[counter++] = DELIMITERB; //keep the B

					continue;
				}
				Data[counter++] = buffer[i];
			}
			return counter;
		}

		private int Framing(byte[] buf, int size)
		{
			var counter = 0;
			var inserted = 0;

			buffer[inserted++] = DELIMITER;

			while (counter < size)
			{
				if (buf[counter] == DELIMITER)
				{
					buffer[inserted++] = DELIMITERB;
					buffer[inserted++] = DELIMITERC;
				}
				else if (buf[counter] == DELIMITERB)
				{
					buffer[inserted++] = DELIMITERB;
					buffer[inserted++] = DELIMITERD;
				}
				else
				{
					buffer[inserted++] = buf[counter];
				}
				counter++;
			}

			buffer[inserted++] = DELIMITER;
			return inserted;
		}
	}
}
