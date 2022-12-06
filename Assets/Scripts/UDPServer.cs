using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private Socket socket; //Ŀ��socket
    private EndPoint clientEnd; //�ͻ���
    private IPEndPoint ipEnd; //�����˿�
    private string recvStr; //���յ��ַ���
    private string sendStr; //���͵��ַ���
    private byte[] recvData = new byte[1024]; //���յ����ݣ�����Ϊ�ֽ�
    private byte[] sendData = new byte[1024]; //���͵����ݣ�����Ϊ�ֽ�
    private int recvLen; //���յ����ݳ���
    private Thread connectThread; //�����߳�

    private void Start()
    {
        InitSocket();
    }
    /// <summary>
    /// ��ʼ��
    /// </summary>
    void  InitSocket()
    {
        //���������˿ڣ������κ�IP
        ipEnd = new IPEndPoint(IPAddress.Any, 8999);
        Debug.Log(ipEnd);
        //�����׽������ͣ������߳��ж���
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //�������Ҫ��ip
        socket.Bind(ipEnd);
        //����ͻ���
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 9000);
        clientEnd = (EndPoint)sender;
        Debug.Log(clientEnd);
        print("waiting for UDP dgram");

        //����һ���߳����ӣ�����ģ��������߳̿���
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();

    }

    //����������
    void SocketSend(string sendStr)
    {
        //��շ��ͻ���
        sendData = new byte[1024];
        //��������ת��
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //���͸�ָ���ͻ���
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnd);
    }
    //����������
    void SocketReceive()
    {
        //�������ѭ��
        while (true)
        {
            //��data����
            recvData = new byte[1024];
            //��ȡ�ͻ��ˣ���ȡ�ͻ������ݣ������ø��ͻ��˸�ֵ
            recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
            print("message from: " + clientEnd.ToString()); //��ӡ�ͻ�����Ϣ
                                                            //������յ�������
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print("���Ƿ����������յ��ͻ��˵�����" + recvStr);
            //�����յ������ݾ��������ٷ��ͳ�ȥ
            sendStr = "From Server: " + recvStr;
            SocketSend(sendStr);
        }
    }
    /// <summary>
    /// ���ӹر�
    /// </summary>
     void SocketQuit()
    {
        if(connectThread!=null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        if(socket!=null)
        {
            socket.Close();
        }
        print("disconnect");
    }
    private void OnApplicationQuit()
    {
        SocketQuit();
    }
}
