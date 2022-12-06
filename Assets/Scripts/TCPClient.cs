using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class TCPClient : MonoBehaviour
{
    private string staInfo = "NULL";             //状态信息
    private string inputIp = "127.0.0.1";   //输入ip地址
    private string inputPort = "8080";           //输入端口号
    public string inputMes = "NULL";             //发送的消息
    private int recTimes = 0;                    //接收到信息的次数
    private string recMes = "NULL";              //接收到的消息
    private Socket socketSend;                   //客户端套接字，用来链接远端服务器
    private bool clickSend = false;              //是否点击发送按钮 

    /// <summary>
    /// 建立连接
    /// </summary>
    void ClickConnect()
    {
        try
        {
            int _port = Convert.ToInt32(inputPort);//获取端口号
            string _ip = inputIp;//获取ip地址
                                 //创建客户端Socket，获得远程ip和端口号
            socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(_ip);
            IPEndPoint point = new IPEndPoint(ip, _port);

            socketSend.Connect(point);
            Debug.Log("连接成功 , " + " ip = " + ip + " port = " + _port);
            staInfo = ip + ":" + _port + "  连接成功";

            Thread r_thread = new Thread(ReceivedMessage);//开启新的线程，不停的接收服务器发来的消息
            r_thread.IsBackground = true;
            r_thread.Start();


            Thread s_thread = new Thread(SendMessage);          //开启新的线程，不停的给服务器发送消息
            s_thread.IsBackground = true;
            s_thread.Start();
        }
        catch (Exception)
        {
            Debug.Log("IP或者端口号错误......");
            staInfo = "IP或者端口号错误......";
        }
    }

    /// <summary>
    /// 接收服务端返回的消息
    /// </summary>
    void ReceivedMessage()
    {
        while (true)
        {
            byte[] buffer = new byte[1024 * 6];
            int len = socketSend.Receive(buffer);
            if (len == 0)
            {
                break;
            }
            recMes = Encoding.UTF8.GetString(buffer, 0, len);
            Debug.Log("客户端接收到的数据 ： " + recMes);
            recTimes++;
            staInfo = "接收到一次数据，接收次数为 ：" + recTimes;
            Debug.Log("接收次数为：" + recTimes);
        }
    }
    /// <summary>
    /// 向服务器发送消息
    /// </summary>
    void SendMessage()
    {

        try
        {
            while (true)
            {
                clickSend = false;
                string msg = inputMes;
                byte[] buffer = new byte[1024 * 6];
                buffer = Encoding.UTF8.GetBytes(msg);
                socketSend.Send(buffer);
                Debug.Log("发送的数据为：" + msg);

            }
        }
        catch (Exception)
        {


        }
    }

    private void OnDisable()
    {
        Debug.Log("begin OnDisable()");

        if (socketSend.Connected)
        {
            try
            {
                socketSend.Shutdown(SocketShutdown.Both);    //禁用Socket的发送和接收功能
                socketSend.Close();                          //关闭Socket连接并释放所有相关资源
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }

        Debug.Log("end OnDisable()");
    }

    //用户界面
    void OnGUI()
    {
        GUI.color = Color.black;

        GUI.Label(new Rect(65, 10, 60, 20), "状态信息");

        GUI.Label(new Rect(135, 10, 80, 60), staInfo);

        GUI.Label(new Rect(65, 70, 50, 20), "服务器ip地址");

        inputIp = GUI.TextField(new Rect(125, 70, 100, 20), inputIp, 20);

        GUI.Label(new Rect(65, 110, 50, 20), "服务器端口");

        inputPort = GUI.TextField(new Rect(125, 110, 100, 20), inputPort, 20);

        GUI.Label(new Rect(65, 150, 80, 20), "接收到消息：");

        GUI.Label(new Rect(155, 150, 80, 20), recMes);

        GUI.Label(new Rect(65, 190, 80, 20), "发送的消息：");

        inputMes = GUI.TextField(new Rect(155, 190, 100, 20), inputMes, 20);

        if (GUI.Button(new Rect(65, 230, 60, 20), "开始连接"))
        {
            ClickConnect();
        }

        if (GUI.Button(new Rect(65, 270, 60, 20), "发送信息"))
        {
            clickSend = true;
        }
    }
}
