﻿public enum ClientPacketId
{
    None = 0,
    CreateAccount,
    DeleteAccount,
    Login,
    Logout,
    GameClose,
    CreateCharacter,
    DeleteCharacter,
    SelectCharacter,
    RequestCharacterStatus,
    RequestRoomList,
    SkillUp,
    EquipUpgrade,
    CreateRoom,
    EnterRoom,
    RoomUserData,
    ExitRoom,
    StartGame,
    RequestUDPConnection,
    UDPConnectComplete,
}

public enum ServerPacketId
{
    None = 0,
    CreateAccountResult,
    DeleteAccountResult,
    LoginResult,
    LogoutResult,
    CreateCharacterResult,
    DeleteChracterResult,
    SelectCharacterResult,
    RoomList,
    CharacterStatus,
    SkillUpResult,
    EquipUpgradeResult,
    CreateRoomResult,
    EnterRoomResult,
    RoomUserData,
    ExitRoomResult,
    StartGame,
    UDPConnection,
    StartDungeon,
}

public enum P2PPacketId
{
    None = 0,
    ConnectionCheckAnswer,
    RequestConnectionCheck,
    ConnectionAnswer,
    CreateUnit,
    CharacterPosition,
    CharacterAction,
}