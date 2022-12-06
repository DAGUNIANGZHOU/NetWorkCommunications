using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
/// <summary>
/// 其实服务器端就是 绑定IP 端口 然后监听   接收客户端的信息和给客户端发送信息
/// </summary>
public class TCPServer : MonoBehaviour
{

    private string info="NULL";//状态信息
    private string recMes = "NULL";// 接收到的信息
    private int recTimes = 0;//接收到的信息的次数

    private string inputIp = "127.0.0.1";//IP地址（本地）
    private string inputPort = "8080";//端口值
    private string inputMessage = "NULL";//用以发送的信息

    private Socket socketWatch;//用以监听的套接字
    private Socket socketSend;//用以和客户端通信的套接字

    private bool isSendData = false;//是否点击发送数据按钮
    private bool clickConnectBtn = false;//是否点击监听按钮
    /// <summary>
    /// 建立TCP通信连接
    /// </summary>
   private void ClickConnect()
   {
        try
        {
            int _port = Convert.ToInt32(inputPort);//获取端口号（32位，24字节）
            string _ip = inputIp;//获取IP地址
            Debug.Log("端口是" + _port);
            Debug.Log("ip地址是" + _ip);
            clickConnectBtn = true;  //点击了监听按钮，更改状态
            info = "ip地址：" + _ip + "端口号：" + _port;
            //点击开始监听按钮时，在服务器端创建一个负责监听IP和端口号的Socket
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(_ip);
            IPEndPoint point = new IPEndPoint(ip, _port);//创建对象端口
            socketWatch.Bind(point);// 绑定端口号
            Debug.Log("监听成功");
            info = "监听成功";
            socketWatch.Listen(10);//设置监听，最大同是连接10台

            Thread thread = new Thread(Listen);
            thread.IsBackground = true;
            thread.Start(socketWatch);

        }
        catch
        {

        }
   }
    /// <summary>
    /// 等待客户端的连接 并且创建与之通信的Socket
    /// </summary>
    /// <param name="o"></param>
    private void Listen(object o)
    {
        try
        {
            Socket socketWatch = o as Socket;
            while(true)
            {
                socketSend = socketWatch.Accept();// 等到接收客户端连接
                Debug.Log(socketSend.RemoteEndPoint.ToString() + ":连接成功");
                info = socketSend.RemoteEndPoint.ToString() + ":连接成功";


                Thread r_thread = new Thread(ReceivedMessage);      //开启一个新线程，执行接收消息方法
                r_thread.IsBackground = true;
                r_thread.Start(socketSend);

                Thread s_thread = new Thread(SendMessage);   //开启一个新线程，执行发送消息方法
                s_thread.IsBackground = true;
                s_thread.Start(socketSend);

            }
        }
        catch 
        {

           
        }
    }
    /// <summary>
    /// 服务器端不停的接收客户端发来的消息
    /// </summary>
    /// <param name="o"></param>
    void ReceivedMessage(object o)
    {
        try
        {
            Socket socketSend = o as Socket;
            while(true)
            {
                byte[] buffer = new byte[1024 * 6];  //客户端连接服务器成功后，服务器接收客户端发送的消息
                int len = socketSend.Receive(buffer);  //实际接收到的有效字节数
                if (len == 0)
                    break;
                string str = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log("接收的信息：" + socketSend.RemoteEndPoint + ":" + str);
                recMes = str;
                recTimes++;
                info = "接收到的信息：" + socketSend.RemoteEndPoint + ":" + str;
                Debug.Log("接收数据次数：" + recTimes);
            }
        }
        catch 
        {

        }
    }
    /// <summary>
    /// 服务器端不停的向客户端发送消息
    /// </summary>
    /// <param name="o"></param>
    void SendMessage(object o)
    {
        try
        {
            Socket socketSend = o as Socket;
            while (true)
            {
                if (isSendData)
                {
                    isSendData = false;
                    byte[] sendByte = Encoding.UTF8.GetBytes(inputMessage);
                    Debug.Log("发送的数据为 :" + inputMessage);
                    Debug.Log("发送的数据字节长度 :" + sendByte.Length);
                    socketSend.Send(sendByte);
                }
            }
        }
        catch
        {

        }
    }
    /// <summary>
    /// 关闭连接，释放资源
    /// </summary>
    private void OnDisable()
    {
        Debug.Log("begin OnDisable()");
        if(clickConnectBtn)
        {
            try
            {
                socketWatch.Shutdown(SocketShutdown.Both);//禁用Socket的发送和接收功能
                socketWatch.Close(); //关闭Socket连接并释放所有相关资源
                socketSend.Shutdown(SocketShutdown.Both);//禁用Socket的发送和接收功能
                socketSend.Close(); //关闭Socket连接并释放所有相关资源
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        Debug.Log("end OnDisable()");
    }
    //交互界面（代码创建）
    void OnGUI()
    {
        GUI.color = Color.black;	//字体颜色

        GUI.Label(new Rect(65, 10, 80, 20), "状态信息");

        GUI.Label(new Rect(155, 10, 80, 70), info);

        GUI.Label(new Rect(65, 80, 80, 20), "接收到消息：");

        GUI.Label(new Rect(155, 80, 80, 20), recMes);

        GUI.Label(new Rect(65, 120, 80, 20), "发送的消息：");

        inputMessage = GUI.TextField(new Rect(155, 120, 100, 20), inputMessage, 20);

        GUI.Label(new Rect(65, 160, 80, 20), "本机ip地址：");

        inputIp = GUI.TextField(new Rect(155, 160, 100, 20), inputIp, 20);

        GUI.Label(new Rect(65, 200, 80, 20), "本机端口号：");

        inputPort = GUI.TextField(new Rect(155, 200, 100, 20), inputPort, 20);

        if (GUI.Button(new Rect(65, 240, 60, 20), "开始监听"))
        {
            ClickConnect();	//点击开始
        }

        if (GUI.Button(new Rect(65, 280, 60, 20), "发送数据"))
        {
            isSendData = true;	//发送数据
        }
    }
}
