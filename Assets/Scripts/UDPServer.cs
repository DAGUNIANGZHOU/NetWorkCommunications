using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private Socket socket; //目标socket
    private EndPoint clientEnd; //客户端
    private IPEndPoint ipEnd; //侦听端口
    private string recvStr; //接收的字符串
    private string sendStr; //发送的字符串
    private byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    private byte[] sendData = new byte[1024]; //发送的数据，必须为字节
    private int recvLen; //接收的数据长度
    private Thread connectThread; //连接线程

    private void Start()
    {
        InitSocket();
    }
    /// <summary>
    /// 初始化
    /// </summary>
    void  InitSocket()
    {
        //定义侦听端口，侦听任何IP
        ipEnd = new IPEndPoint(IPAddress.Any, 8999);
        Debug.Log(ipEnd);
        //定义套接字类型，在主线程中定义
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //服务端需要绑定ip
        socket.Bind(ipEnd);
        //定义客户端
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 9000);
        clientEnd = (EndPoint)sender;
        Debug.Log(clientEnd);
        print("waiting for UDP dgram");

        //开启一个线程连接，必须的，否则主线程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();

    }

    //服务器发送
    void SocketSend(string sendStr)
    {
        //清空发送缓存
        sendData = new byte[1024];
        //数据类型转换
        sendData = Encoding.ASCII.GetBytes(sendStr);
        //发送给指定客户端
        socket.SendTo(sendData, sendData.Length, SocketFlags.None, clientEnd);
    }
    //服务器接收
    void SocketReceive()
    {
        //进入接收循环
        while (true)
        {
            //对data清零
            recvData = new byte[1024];
            //获取客户端，获取客户端数据，用引用给客户端赋值
            recvLen = socket.ReceiveFrom(recvData, ref clientEnd);
            print("message from: " + clientEnd.ToString()); //打印客户端信息
                                                            //输出接收到的数据
            recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
            print("我是服务器，接收到客户端的数据" + recvStr);
            //将接收到的数据经过处理再发送出去
            sendStr = "From Server: " + recvStr;
            SocketSend(sendStr);
        }
    }
    /// <summary>
    /// 连接关闭
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
