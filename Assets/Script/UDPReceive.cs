using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
	[SerializeField]
	int LOCA_LPORT = 6000;

	static UdpClient udp;
	Thread thread;

	public static string text { private set; get;}

	void Start ()
	{
		udp = new UdpClient(LOCA_LPORT);
	//	udp.Client.ReceiveTimeout = 1000;
		thread = new Thread(new ThreadStart(ThreadMethod));
		thread.Start(); 
	}

	void Update ()
	{
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod());
	}

	void OnApplicationQuit()
	{
		thread.Abort();
	}

	private static void ThreadMethod()
	{
		while(true)
		{
			IPEndPoint remoteEP = null;
			byte[] data = udp.Receive(ref remoteEP);
			text = Encoding.ASCII.GetString(data);
			Debug.Log(text);
		}
	}
}