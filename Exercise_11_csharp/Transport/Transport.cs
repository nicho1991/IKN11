using System;
using Linklaget;
using System.Threading;
using System;
using System.IO;
using System.Text;

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
			int size = link.receive (ref buf);
			if (size != (int)TransSize.ACKSIZE)
				return DEFAULT_SEQNO;
			if(!checksum.checkChecksum(buf, (int)TransSize.ACKSIZE) ||
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
			buffer[2] = seqNo;

			buffer[3] = 0; //data

			for (int i = 0; i < size; i++)
			{
				buffer[i+4] = buf[i];
			}


			checksum.calcChecksum(ref buffer,buffer.Length);
			while (errorCount < 5) 
			{
				try
				{
					do
					{
						//Console.WriteLine("t");
						link.send(buffer, size+ 4);
						//Console.WriteLine("got no ack seq is: " + seqNo );
					} while (receiveAck() != seqNo);


					//Console.WriteLine(seqNo);
					break;

				}
				catch(TimeoutException){
					errorCount++;
					//Console.WriteLine ("timeout");
				}

			}

			nextSeqNo();
			errorCount = 0;
		}



		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{

			int receiveSize = 0;
			//check if theres something to receive.
			while (receiveSize == 0 && errorCount < 5) 
			{				
				try {
					while(( receiveSize = link.receive(ref buf)) > 0)
					{
						var checke = checksum.checkChecksum (buf, buf.Length);
						//Console.WriteLine (checke);
						if (checke) {
	

							if(buf[(int) TransCHKSUM.SEQNO] == seqNo){

								sendAck (true);
								//receiveSize = buffer.Length < receiveSize - (int)TransSize.ACKSIZE? buf.Length : receiveSize - (int)TransSize.ACKSIZE;
								var tempbuf = buf;

								Array.Copy(tempbuf,(int)TransSize.ACKSIZE, buf,0,receiveSize);
								break;
							}
							else
							 sendAck (false);

							continue;
						}


						
					}

				} catch (TimeoutException) {
					errorCount++;
					//Console.WriteLine ("timeout");
					receiveSize = 0;
				}
			}
			nextSeqNo();
			errorCount = 0;
			return receiveSize;
		}
	}
}