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
		/// Initializes a new instance of the <see cref="Linklaget.Link"/> class.
		/// </summary>
		/// <param name="BUFSIZE">BUFSIZ.</param>
		/// <param name="APP">AP.</param>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);


			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2) +2]; //maximum possible size if all is A's

			serialPort.ReadTimeout = 500;
			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();

		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name="buf">Buffer.</param>
		/// <param name="size">Size.</param>
		public void send (byte[] buf, int size)
		{
			var CountToSend = Insert (buf, size);
			serialPort.Write(buffer, 0, CountToSend);
		}

		private int Insert(byte[] buf, int size)
		{

			var number = 1;

			buffer[number-1] = DELIMITER; //start of file
			for (int i = 0; i < size; i++) 
			{
				if (buf[i] == DELIMITER) // this is A , becomes BC
				{
					buffer[number] = DELIMITERB;
					buffer[number+1] = DELIMITERC;
					number += 1;
				}
				else if (buf[i] == DELIMITERB) //this is B, becomes BD
				{
					buffer[number] = DELIMITERB;
					buffer[number+1] = DELIMITERD;
					number += 2; // two more in the sequence
				}
				else
				{
					buffer[number] = buf[i]; //do nothing
					number++;
				}
			}

			buffer[number] = DELIMITER; //end of file
			return number+1;
		}

		/// <summary>
		/// Receive the specified buf.
		/// </summary>
		/// <param name="buf">Buffer.</param>
		public int receive (ref byte[] buf)
		{	
			while((byte)serialPort.ReadByte() != DELIMITER)
			{
				//shouldnt really get here , safety feature
			}

			int ReceiveLength = 0;

			for (int Length = 1; Length < buffer.Length;Length++) {
				
				buffer[Length-1] = (byte)serialPort.ReadByte(); //arrays start at zero
				if (buffer[Length-1]== DELIMITER) 				// we got the full package
				{
					ReceiveLength = Length; 
					break;
				}
			}

			return Extract (ref buf, ReceiveLength); // find the real size
		}




		private int Extract(ref byte[] ExtractData, int size)
		{
			var counter = 0;
			for (var i = 1; i < size; i++)
			{
				if (buffer[i-1] == DELIMITERB) //check for A
				{
					ExtractData[counter++] = (DELIMITERC == buffer[i]) ? DELIMITER : DELIMITERB;
				}
				ExtractData[counter++] = buffer[i-1];
			}
			return counter;
		}


	}
}
