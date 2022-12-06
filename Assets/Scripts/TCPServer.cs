using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
/// <summary>
/// ��ʵ�������˾��� ��IP �˿� Ȼ�����   ���տͻ��˵���Ϣ�͸��ͻ��˷�����Ϣ
/// </summary>
public class TCPServer : MonoBehaviour
{

    private string info="NULL";//״̬��Ϣ
    private string recMes = "NULL";// ���յ�����Ϣ
    private int recTimes = 0;//���յ�����Ϣ�Ĵ���

    private string inputIp = "127.0.0.1";//IP��ַ�����أ�
    private string inputPort = "8080";//�˿�ֵ
    private string inputMessage = "NULL";//���Է��͵���Ϣ

    private Socket socketWatch;//���Լ������׽���
    private Socket socketSend;//���ԺͿͻ���ͨ�ŵ��׽���

    private bool isSendData = false;//�Ƿ����������ݰ�ť
    private bool clickConnectBtn = false;//�Ƿ���������ť
    /// <summary>
    /// ����TCPͨ������
    /// </summary>
   private void ClickConnect()
   {
        try
        {
            int _port = Convert.ToInt32(inputPort);//��ȡ�˿ںţ�32λ��24�ֽڣ�
            string _ip = inputIp;//��ȡIP��ַ
            Debug.Log("�˿���" + _port);
            Debug.Log("ip��ַ��" + _ip);
            clickConnectBtn = true;  //����˼�����ť������״̬
            info = "ip��ַ��" + _ip + "�˿ںţ�" + _port;
            //�����ʼ������ťʱ���ڷ������˴���һ���������IP�Ͷ˿ںŵ�Socket
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(_ip);
            IPEndPoint point = new IPEndPoint(ip, _port);//��������˿�
            socketWatch.Bind(point);// �󶨶˿ں�
            Debug.Log("�����ɹ�");
            info = "�����ɹ�";
            socketWatch.Listen(10);//���ü��������ͬ������10̨

            Thread thread = new Thread(Listen);
            thread.IsBackground = true;
            thread.Start(socketWatch);

        }
        catch
        {

        }
   }
    /// <summary>
    /// �ȴ��ͻ��˵����� ���Ҵ�����֮ͨ�ŵ�Socket
    /// </summary>
    /// <param name="o"></param>
    private void Listen(object o)
    {
        try
        {
            Socket socketWatch = o as Socket;
            while(true)
            {
                socketSend = socketWatch.Accept();// �ȵ����տͻ�������
                Debug.Log(socketSend.RemoteEndPoint.ToString() + ":���ӳɹ�");
                info = socketSend.RemoteEndPoint.ToString() + ":���ӳɹ�";


                Thread r_thread = new Thread(ReceivedMessage);      //����һ�����̣߳�ִ�н�����Ϣ����
                r_thread.IsBackground = true;
                r_thread.Start(socketSend);

                Thread s_thread = new Thread(SendMessage);   //����һ�����̣߳�ִ�з�����Ϣ����
                s_thread.IsBackground = true;
                s_thread.Start(socketSend);

            }
        }
        catch 
        {

           
        }
    }
    /// <summary>
    /// �������˲�ͣ�Ľ��տͻ��˷�������Ϣ
    /// </summary>
    /// <param name="o"></param>
    void ReceivedMessage(object o)
    {
        try
        {
            Socket socketSend = o as Socket;
            while(true)
            {
                byte[] buffer = new byte[1024 * 6];  //�ͻ������ӷ������ɹ��󣬷��������տͻ��˷��͵���Ϣ
                int len = socketSend.Receive(buffer);  //ʵ�ʽ��յ�����Ч�ֽ���
                if (len == 0)
                    break;
                string str = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log("���յ���Ϣ��" + socketSend.RemoteEndPoint + ":" + str);
                recMes = str;
                recTimes++;
                info = "���յ�����Ϣ��" + socketSend.RemoteEndPoint + ":" + str;
                Debug.Log("�������ݴ�����" + recTimes);
            }
        }
        catch 
        {

        }
    }
    /// <summary>
    /// �������˲�ͣ����ͻ��˷�����Ϣ
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
                    Debug.Log("���͵�����Ϊ :" + inputMessage);
                    Debug.Log("���͵������ֽڳ��� :" + sendByte.Length);
                    socketSend.Send(sendByte);
                }
            }
        }
        catch
        {

        }
    }
    /// <summary>
    /// �ر����ӣ��ͷ���Դ
    /// </summary>
    private void OnDisable()
    {
        Debug.Log("begin OnDisable()");
        if(clickConnectBtn)
        {
            try
            {
                socketWatch.Shutdown(SocketShutdown.Both);//����Socket�ķ��ͺͽ��չ���
                socketWatch.Close(); //�ر�Socket���Ӳ��ͷ����������Դ
                socketSend.Shutdown(SocketShutdown.Both);//����Socket�ķ��ͺͽ��չ���
                socketSend.Close(); //�ر�Socket���Ӳ��ͷ����������Դ
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        Debug.Log("end OnDisable()");
    }
    //�������棨���봴����
    void OnGUI()
    {
        GUI.color = Color.black;	//������ɫ

        GUI.Label(new Rect(65, 10, 80, 20), "״̬��Ϣ");

        GUI.Label(new Rect(155, 10, 80, 70), info);

        GUI.Label(new Rect(65, 80, 80, 20), "���յ���Ϣ��");

        GUI.Label(new Rect(155, 80, 80, 20), recMes);

        GUI.Label(new Rect(65, 120, 80, 20), "���͵���Ϣ��");

        inputMessage = GUI.TextField(new Rect(155, 120, 100, 20), inputMessage, 20);

        GUI.Label(new Rect(65, 160, 80, 20), "����ip��ַ��");

        inputIp = GUI.TextField(new Rect(155, 160, 100, 20), inputIp, 20);

        GUI.Label(new Rect(65, 200, 80, 20), "�����˿ںţ�");

        inputPort = GUI.TextField(new Rect(155, 200, 100, 20), inputPort, 20);

        if (GUI.Button(new Rect(65, 240, 60, 20), "��ʼ����"))
        {
            ClickConnect();	//�����ʼ
        }

        if (GUI.Button(new Rect(65, 280, 60, 20), "��������"))
        {
            isSendData = true;	//��������
        }
    }
}
