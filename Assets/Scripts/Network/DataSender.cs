﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class DataSender : MonoBehaviour
{
    private static DataSender instance;

    public static DataSender Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(DataSender)) as DataSender;
                if (!instance)
                {
                    GameObject container = new GameObject();
                    container.name = "DataSender";
                    instance = container.AddComponent(typeof(DataSender)) as DataSender;
                }
            }

            return instance;
        }
    }

    NetworkManager networkManager;
    CharacterManager characterManager;

    Socket tcpSock;
    Socket udpSock;

    Queue<DataPacket> sendMsgs;

    public int[] udpId;
    byte[] udpMsg;

    public Queue<DataPacket> SendMsgs { get { return sendMsgs; } }

    public void Initialize(Queue<DataPacket> newSendMsgs, Socket newTcpSock)
    {
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>();
        sendMsgs = newSendMsgs;
        InitializeTcpSend(newTcpSock);
        StartCoroutine(DataSend());
    }

    public void InitializeTcpSend(Socket newSocket)
    {
        tcpSock = newSocket;
    }
    
    public void InitializeUdpSend(Socket newSocket)
    {
        udpSock = newSocket;
    }

    //데이타를 전송하는 메소드. byte[] msg 를 newIPEndPoint로 전송한다.
    public IEnumerator DataSend()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.016f);

            int readCount = sendMsgs.Count;

            for (int i = 0; i < readCount; i++)
            {
                DataPacket packet;

                packet = sendMsgs.Dequeue();

                if (packet.endPoint != null)
                {
                    udpSock.SendTo(packet.msg, 0, packet.msg.Length, SocketFlags.None, packet.endPoint);
                }
                else if (packet.endPoint == null)
                {
                    tcpSock.Send(packet.msg, 0, packet.msg.Length, SocketFlags.None);
                }
            }
        }        
    }

    //비동기 콜백 메소드
    private void SendData(IAsyncResult ar)
    {
        udpSock.EndSendTo(ar);
    }

    //계정 생성 -> Server
    public void CreateAccount(string id, string pw)
    {
        Debug.Log("가입 요청");

        AccountData accountData = new AccountData(id, pw);
        AccountPacket accountDataPacket = new AccountPacket(accountData);
        accountDataPacket.SetPacketId((int)ClientPacketId.CreateAccount);

        DataPacket packet = new DataPacket(CreatePacket(accountDataPacket), null);
        sendMsgs.Enqueue(packet);
    }

    //계정 탈퇴 -> Server
    public void DeleteAccount(string id, string pw)
    {
        Debug.Log("탈퇴 요청");

        AccountData accountData = new AccountData(id, pw);
        AccountPacket accountDataPacket = new AccountPacket(accountData);
        accountDataPacket.SetPacketId((int)ClientPacketId.DeleteAccount);

        DataPacket packet = new DataPacket(CreatePacket(accountDataPacket), null);
        sendMsgs.Enqueue(packet);
    }

    //로그인 -> Server
    public void Login(string id, string pw)
    {
        Debug.Log("로그인");

        AccountData accountData = new AccountData(id, pw);
        AccountPacket accountDataPacket = new AccountPacket(accountData);
        accountDataPacket.SetPacketId((int)ClientPacketId.Login);

        DataPacket packet = new DataPacket(CreatePacket(accountDataPacket), null);
        sendMsgs.Enqueue(packet);
    }

    //로그아웃 -> Server
    public void Logout()
    {
        Debug.Log("로그아웃");

        ResultData resultData = new ResultData();
        ResultPacket resultPacket = new ResultPacket(resultData);
        resultPacket.SetPacketId((int)ClientPacketId.Logout);

        DataPacket packet = new DataPacket(CreatePacket(resultPacket), null);
        sendMsgs.Enqueue(packet);
    }

    //게임 종료 -> Server
    public void GameClose()
    {
        Debug.Log("게임 종료");

        ResultData resultData = new ResultData();
        ResultPacket resultDataPacket = new ResultPacket(resultData);
        resultDataPacket.SetPacketId((int)ClientPacketId.GameClose);

        byte[] msg = CreatePacket(resultDataPacket);

        try
        {
            tcpSock.Send(msg, 0, msg.Length, SocketFlags.None);
        }
        catch
        {
            Debug.Log("GameClose.Send 에러");
        }

        try
        {
            tcpSock.Close();
            udpSock.Close();
        }
        catch
        {
            Debug.Log("이미 소켓이 닫혀있습니다.");
        }
    }

    //캐릭터 생성 -> Server
    public void CreateCharacter(int gender, int hClass, string name)
    {
        Debug.Log("캐릭터 생성");

        CreateCharacterData createCharacterData = new CreateCharacterData((byte)gender, (byte)hClass, name);
        CreateCharacterPacket createCharacterPacket = new CreateCharacterPacket(createCharacterData);
        createCharacterPacket.SetPacketId((int)ClientPacketId.CreateCharacter);

        DataPacket packet = new DataPacket(CreatePacket(createCharacterPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //캐릭터 삭제 -> Server
    public void DeleteCharacter(int index)
    {
        Debug.Log("캐릭터 삭제");

        DeleteCharacterData deleteCharacterData = new DeleteCharacterData((byte)index);
        DeleteCharacterPacket deleteCharacterPacket = new DeleteCharacterPacket(deleteCharacterData);
        deleteCharacterPacket.SetPacketId((int)ClientPacketId.DeleteCharacter);

        DataPacket packet = new DataPacket(CreatePacket(deleteCharacterPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //캐릭터 선택 -> Server
    public void SelectCharacter(int index)
    {
        Debug.Log("캐릭터 선택");

        SelectCharacterData selectCharacterData = new SelectCharacterData((byte)index);
        SelectCharacterPacket selectCharacterPacket = new SelectCharacterPacket(selectCharacterData);
        selectCharacterPacket.SetPacketId((int)ClientPacketId.SelectCharacter);

        DataPacket packet = new DataPacket(CreatePacket(selectCharacterPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //캐릭터 정보 요청 -> Server
    public void RequestCharacterStatus()
    {
        Debug.Log("캐릭터 정보 요청");

        ResultData resultData = new ResultData();
        ResultPacket resultPacket = new ResultPacket(resultData);
        resultPacket.SetPacketId((int)ClientPacketId.RequestCharacterStatus);

        DataPacket packet = new DataPacket(CreatePacket(resultPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //방 목록 요청 -> Server
    public void RequestRoomList()
    {
        Debug.Log("방 목록 요청");

        ResultData resultData = new ResultData();
        ResultPacket resultPacket = new ResultPacket(resultData);
        resultPacket.SetPacketId((int)ClientPacketId.RequestRoomList);

        DataPacket packet = new DataPacket(CreatePacket(resultPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //스킬 투자 -> Server
    public void SkillUp(int index)
    {
        Debug.Log("스킬 투자");

        SkillUpData skillUpData = new SkillUpData(index);
        SkillUpPacket skillUpPacket = new SkillUpPacket(skillUpData);
        skillUpPacket.SetPacketId((int)ClientPacketId.RequestRoomList);

        DataPacket packet = new DataPacket(CreatePacket(skillUpPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //장비 강화 -> Server
    public void EquipUpgrade(int index)
    {
        Debug.Log("장비 강화");

        EquipUpgradeData equipUpgradeData = new EquipUpgradeData(index);
        EquipUpgradePacket equipUpgradePacket = new EquipUpgradePacket(equipUpgradeData);
        equipUpgradePacket.SetPacketId((int)ClientPacketId.EquipUpgrade);

        DataPacket packet = new DataPacket(CreatePacket(equipUpgradePacket), null);

        sendMsgs.Enqueue(packet);
    }

    //방 생성 -> Server
    public void CreateRoom(string roomName, int dungeonId, int dungeonLevel)
    {
        Debug.Log("방 생성");

        CreateRoomData createRoomData = new CreateRoomData(roomName, dungeonId, dungeonLevel);
        CreateRoomPacket createRoomPacket = new CreateRoomPacket(createRoomData);
        createRoomPacket.SetPacketId((int)ClientPacketId.CreateRoom);

        DataPacket packet = new DataPacket(CreatePacket(createRoomPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //방 입장 -> Server
    public void EnterRoom(int roomNum)
    {
        Debug.Log("방 입장 : " + roomNum);

        EnterRoomData enterRoomData = new EnterRoomData(roomNum);
        EnterRoomPacket enterRoomPacket = new EnterRoomPacket(enterRoomData);
        enterRoomPacket.SetPacketId((int)ClientPacketId.EnterRoom);

        DataPacket packet = new DataPacket(CreatePacket(enterRoomPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //방 퇴장 -> Server
    public void ExitRoom(int roomNum)
    {
        Debug.Log("방 퇴장");

        ExitRoomData exitRoomData = new ExitRoomData(roomNum);
        ExitRoomPacket exitRoomPacket = new ExitRoomPacket(exitRoomData);
        exitRoomPacket.SetPacketId((int)ClientPacketId.ExitRoom);

        DataPacket packet = new DataPacket(CreatePacket(exitRoomPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //게임 시작 -> Server
    public void StartGame()
    {
        Debug.Log("게임 시작");

        ResultData resultData = new ResultData();
        ResultPacket resultPacket = new ResultPacket(resultData);
        resultPacket.SetPacketId((int)ClientPacketId.StartGame);

        DataPacket packet = new DataPacket(CreatePacket(resultPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //UDP 연결 요청 -> Server
    public void RequestUdpConnection()
    {
        Debug.Log("Udp 연결 요청");

        ResultData resultData = new ResultData();
        ResultPacket resultPacket = new ResultPacket(resultData);
        resultPacket.SetPacketId((int)ClientPacketId.RequestUdpConnection);

        DataPacket packet = new DataPacket(CreatePacket(resultPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //UDP 연결 확인 요청 -> Client
    public void RequestConnectionCheck(EndPoint endPoint)
    {
        Debug.Log(endPoint.ToString() + " 연결 체크 요청");

        int index = networkManager.GetUserIndex(endPoint);

        ResultData resultData = new ResultData(new byte());
        ResultPacket resultDataPacket = new ResultPacket(resultData);
        resultDataPacket.SetPacketId((int)P2PPacketId.RequestConnectionCheck);

        DataPacket packet = new DataPacket(CreateUdpPacket(resultDataPacket, udpId[index]), endPoint);

        sendMsgs.Enqueue(packet);

        SendData sendData = new SendData(udpId[index], endPoint, packet.msg);
        networkManager.ReSendManager.AddReSendData(sendData, index);
        udpId[index]++;
    }

    //Udp 답신 -> Client
    public void UdpAnswer(EndPoint newEndPoint, int udpId)
    {
        Debug.Log(newEndPoint.ToString() + " Udp 답신 보냄 Id : " + udpId);

        ResultData resultData = new ResultData(new byte());
        ResultPacket resultDataPacket = new ResultPacket(resultData);
        resultDataPacket.SetPacketId((int)P2PPacketId.UdpAnswer);

        DataPacket packet = new DataPacket(CreateUdpPacket(resultDataPacket, udpId), newEndPoint);

        sendMsgs.Enqueue(packet);
    }

    //연결 완료 -> Server
    public void UdpConnectComplete()
    {
        Debug.Log("Udp 연결 완료");

        ResultData resultData = new ResultData();
        ResultPacket resultPacket = new ResultPacket(resultData);
        resultPacket.SetPacketId((int)ClientPacketId.UdpConnectComplete);

        DataPacket packet = new DataPacket(CreatePacket(resultPacket), null);

        sendMsgs.Enqueue(packet);
    }

    //캐릭터의 생성 -> Client
    public void CreateUnitSend(EndPoint endPoint, short characterId, float posX, float posY, float posZ)
    {
        Debug.Log(endPoint.ToString() + "캐릭터 생성 보냄");

        CreateUnitData createUnitData = new CreateUnitData(characterId, posX, posY, posZ);
        CreateUnitPacket createUnitDataPacket = new CreateUnitPacket(createUnitData);
        createUnitDataPacket.SetPacketId((int)P2PPacketId.CreateUnit);

        int index = networkManager.GetUserIndex(endPoint);

        DataPacket packet = new DataPacket(CreateUdpPacket(createUnitDataPacket, udpId[index]), endPoint);
        sendMsgs.Enqueue(packet);

        SendData sendData = new SendData(udpId[index], endPoint, packet.msg);
        networkManager.ReSendManager.AddReSendData(sendData, index);
        udpId[index]++;
    }

    //캐릭터 위치 -> Client
    public IEnumerator CharacterPositionSend()
    {
        if (characterManager == null)
        {
            characterManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterManager>();
        }
        Debug.Log("캐릭터 위치 보내기 시작");

        while (true)
        {
            yield return new WaitForSeconds(0.016f);

            bool dir = characterManager.charDir;
            float xPos = characterManager.transform.position.x;
            float yPos = characterManager.transform.position.y;
            float zPos = characterManager.transform.position.z;

            CharacterPositionData CharacterPosition = new CharacterPositionData(dir, (byte)characterManager.UserNum, xPos, yPos, zPos);
            CharacterPositionPacket characterPositionPacket = new CharacterPositionPacket(CharacterPosition);
            characterPositionPacket.SetPacketId((int)P2PPacketId.CharacterPosition);

            byte[] msg = CreatePacket(characterPositionPacket);

            for (int index = 0; index < networkManager.UserIndex.Count; index++)
            {
                int userIndex = networkManager.UserIndex[index].UserNum;

                if (networkManager.MyIndex != userIndex)
                {
                    DataPacket packet = new DataPacket(CreateUdpPacket(characterPositionPacket, udpId[userIndex]), networkManager.UserIndex[index].EndPoint);
                    sendMsgs.Enqueue(packet);
                }
            }
        }
    }

    //유닛 위치 -> Client
    public void UnitPositionSend()
    {

    }

    //캐릭터 움직임(공격, 점프, 스킬 등등) -> Client
    public void CharacterActionSend(int action)
    {
        if(characterManager == null)
        {
            characterManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterManager>();
        }

        CharacterActionData characterActionData = new CharacterActionData(action, (byte)characterManager.UserNum);
        CharacterActionPacket characterActionPacket = new CharacterActionPacket(characterActionData);
        characterActionPacket.SetPacketId((int)P2PPacketId.CharacterAction);

        byte[] msg = CreatePacket(characterActionPacket);

        Debug.Log("캐릭터 상태 보냄");
        
        for (int index = 0; index < networkManager.UserIndex.Count; index++)
        {
            int userIndex = networkManager.UserIndex[index].UserNum;

            if (networkManager.MyIndex != userIndex)
            {
                DataPacket packet = new DataPacket(CreateUdpPacket(characterActionPacket, udpId[userIndex]), networkManager.UserIndex[index].EndPoint);
                sendMsgs.Enqueue(packet);
            }
        }
    }

    //0.1초 마다 Udp메시지를 큐에 넣는다.
    //public IEnumerator EnqueueMessage()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForFixedUpdate();

    //        DataPacket packet = new DataPacket(udpMsg, null);

    //        foreach (EndPoint client in networkManager.UserIndex.Keys)
    //        {
    //            packet.endPoint = client;
    //            sendMsgs.Enqueue(packet);
    //        }

    //        udpMsg = new byte[0];
    //    }
    //}

    //패킷의 헤더 생성

    byte[] CreateHeader<T>(Packet<T> data)
    {
        byte[] msg = data.GetPacketData();

        HeaderData headerData = new HeaderData();
        HeaderSerializer headerSerializer = new HeaderSerializer();

        headerData.length = (short)msg.Length;
        headerData.source = (byte)NetworkManager.Source.ClientSource;
        headerData.id = (byte)data.GetPacketId();

        headerSerializer.Serialize(headerData);
        byte[] header = headerSerializer.GetSerializedData();

        return header;
    }

    //패킷 생성
    byte[] CreatePacket<T>(Packet<T> data)
    {
        byte[] msg = data.GetPacketData();
        byte[] header = CreateHeader(data);
        byte[] packet = CombineByte(header, msg);

        return packet;
    }

    //패킷의 헤더 생성
    byte[] CreateUdpHeader<T>(Packet<T> data, int udpId)
    {
        byte[] msg = data.GetPacketData();

        HeaderData headerData = new HeaderData();
        HeaderSerializer headerSerializer = new HeaderSerializer();

        headerData.length = (short)msg.Length;
        headerData.source = (byte)NetworkManager.Source.ClientSource;
        headerData.id = (byte)data.GetPacketId();
        headerData.udpId = udpId;

        headerSerializer.UdpSerialize(headerData);
        byte[] header = headerSerializer.GetSerializedData();

        return header;
    }

    //패킷 생성
    byte[] CreateUdpPacket<T>(Packet<T> data, int udpId)
    {
        byte[] msg = data.GetPacketData();
        byte[] header = CreateUdpHeader(data, udpId);
        byte[] packet = CombineByte(header, msg);

        return packet;
    }

    public static byte[] CombineByte(byte[] array1, byte[] array2)
    {
        byte[] array3 = new byte[array1.Length + array2.Length];
        Array.Copy(array1, 0, array3, 0, array1.Length);
        Array.Copy(array2, 0, array3, array1.Length, array2.Length);
        return array3;
    }

    public static byte[] CombineByte(byte[] array1, byte[] array2, byte[] array3)
    {
        byte[] array4 = CombineByte(CombineByte(array1, array2), array3);
        return array4;
    }
}