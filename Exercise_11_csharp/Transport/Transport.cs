using System;
using Linklaget;
using System.Threading;


/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
	/// <summary>
	/// Transport.
	/// </summary>
	public class Transport
	{
		/// <summary>
		/// The link.
		/// </summary>
		private Link link;
		/// <summary>
		/// The 1' complements checksum.
		/// </summary>
		private Checksum checksum;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
		private byte seqNo;
		/// <summary>
		/// The old_seq no.
		/// </summary>
		private byte old_seqNo;
		/// <summary>
		/// The error count.
		/// </summary>
		private int errorCount;
		/// <summary>
		/// The DEFAULT_SEQNO.
		/// </summary>
		private const int DEFAULT_SEQNO = 2;
		/// <summary>
		/// The data received. True = received data in receiveAck, False = not received data in receiveAck
		/// </summary>
		private bool dataReceived;
		/// <summary>
		/// The number of data the recveived.
		/// </summary>
		private int recvSize = 0;



		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE, string APP)
		{

			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE, APP);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
			dataReceived = false;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private byte receiveAck()
		{
			byte[] buf = new byte[(int)TransSize.ACKSIZE];
			int size = link.receive (ref buffer);
			if (size != (int)TransSize.ACKSIZE)
				return DEFAULT_SEQNO;
			if(!checksum.checkChecksum(buffer, (int)TransSize.ACKSIZE) ||
				buf[(int)TransCHKSUM.SEQNO] != seqNo ||
				buf[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				return DEFAULT_SEQNO;

			return seqNo;

		}



		private void nextSeqNo()
		{
			seqNo = (byte)((seqNo + 1) % 2);
		}

		/// <summary>
		/// Sends the ack.
		/// </summary>
		/// <param name='ackType'>
		/// Ack type.
		/// </param>
		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);
			link.send(ackBuf, (int)TransSize.ACKSIZE);
		}

		/// <summary>
		/// Send the specified buffer and size.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send(byte[] buf,int size )
		{
			do
			{
				buffer[2] = seqNo;

				buffer[3] = 0; //data

				for (int i = 0; i < size; i++)
				{
					buffer[i+4] = buf[i];
				}

				Checksum checksum = new Checksum();
				checksum.calcChecksum(ref buffer,buffer.Length);

				//check what we got
				//for(int i = 0 ; i < 10 ; i++)
				//{
				//	Console.WriteLine(buffer[i]);
				//}					
				//Console.WriteLine(buffer.Length);
				link.send(buffer, size +4 );

				//Thread.Sleep(250);


			} while (receiveAck() != seqNo);
			nextSeqNo();
			old_seqNo = DEFAULT_SEQNO;
		}



		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			//check if theres something to receive.

			int receiveSize = link.receive (ref buf);
			while (receiveSize == 0) {
				receiveSize = link.receive (ref buf);
			}

				var checke = checksum.checkChecksum(buf,buf.Length);

				if (checke) {
					//Console.WriteLine (checke);
					sendAck (true);	
				}
				sendAck (false);
				return receiveSize;

				//sende ack eller ikke
			//hej
				//check what we got
				//Console.WriteLine("something");
				//for(int i = 0 ; i < 10 ; i++)
				//{
				//	Console.WriteLine(buf[i]);
				//}		
		}
	}
}