﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Collections.Generic;


//this class manage monsterStageLevel, sumon, player sumon, player death;
public class DungeonManager : MonoBehaviour
{
	//need revise code;

	public enum HostGuest
	{
		Host = 0,
		SubHost,
		Guest
	}
	;
	public HostGuest hostGuest;
    //MonsterController change-> DungeonManager;
    //DungeonScene change -> DungeonManager;
    [SerializeField]
    private GameObject[] players;
    public CharacterManager[] characters;
    public GameObject[] Players { get { return players; } }
    public SceneChangeObject nextSceneObject;
    public SceneChangeObject beforeScneObject;

    public int monsterCount;
    public BoomMonster[] boomMonster;
    public WarriorMonster[] warriorMonster;
	public ShockWaveMonster[] shockWaveMonster;
	public BossMonsterKYW bossMonster;

	public MonsterSpawnPoint spawnPoint;

    InputManager inputManager;
    UIManager uiManager;
    NetworkManager networkManager;
    GameObject m_camera;

    protected bool normalMode; //false  -> normalBattle, true -> Defence; 
	public bool NormalMode
    {
		get { return normalMode; }
		set { normalMode = value; }
    }

	public Vector3[] monsterTransForm;

    public Section[] section;

    [SerializeField]int mapNumber;



	void Start()
	{

		//test
		//players = GameObject.FindGameObjectsWithTag ("Player");


		DungeonConstruct();//mapNumber - > inspector define
		//        modeForm = false;
//		        ModeChange(n);//client get modeform and ingame play ;
		//section = transform.GetComponentsInChildren<Section> ();
		normalMode = true;

		//section = GameObject.FindGameObjectsWithTag ("DefenceMonsterSection");
		if(section.Length != 0){
			SectionSet ();
		}





	}

	void Update()
	{
		if (normalMode)
		{
			for (int i = 0; i < boomMonster.Length; i++) {
				if (hostGuest != HostGuest.Host) {
					boomMonster [i].GuestMonsterUpdate ();	
				}
				if (hostGuest == HostGuest.Host) {
					boomMonster [i].UpdateNormalMode ();
				}
			}
			for (int j = 0; j < warriorMonster.Length; j++) {
				if (hostGuest != HostGuest.Host) {
					warriorMonster [j].GuestMonsterUpdate ();
				}
				if (hostGuest == HostGuest.Host) {
					warriorMonster [j].UpdateNormalMode ();
				}
				
			}
			for (int k = 0; k < shockWaveMonster.Length; k++) {
				if (hostGuest != HostGuest.Host) {
					shockWaveMonster [k].GuestMonsterUpdate ();
						
				}
				if (hostGuest == HostGuest.Host) {
					shockWaveMonster [k].UpdateNormalMode ();
				}
			}
		}

		if (!normalMode)
		{
			for (int i = 0; i < section.Length; i++) {
				section [i].UpdateConduct ();
			}
		}

		if (bossMonster != null) {
			if(hostGuest == HostGuest.Host){
				bossMonster.BossMonsterUpdate ();
			}
			if(hostGuest != HostGuest.Host){
				//shockWaveMonster [k].GuestMonsterUpdate ();
				bossMonster.BossMonsterUpdate ();
			}
		}
	}

    //각종 매니저 초기화
    public void Initialize(int userNum)
    {
        Debug.Log("DungeonManager 초기화 - " + userNum);
        networkManager = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkManager>();
    }

    public void InitializePlayer(int playerNum)
    {
        players = new GameObject[playerNum];
        characters = new CharacterManager[playerNum];
    }

    //defence mode, normal mode
    public void ModeChange(bool modeForm)
    {
		if (normalMode)
        {
			modeForm = false;
            //player1,player2 ->  nextScene; 
            //respwanstart;
        }

        if (!modeForm)
        {
            //(close socket) send to otherclient socket;
        }
    }

    public void SceneChange()
    {
        if (mapNumber < 3)
        {
            SceneManager.LoadScene(mapNumber + 1);// loadScene;
        }
        //mapNumber == 3 -> 2 player SceneChange

        //        if (mapNumber < section.Length)
        //        {
        //            mapNumber += 1;
        //			//SceneManager.LoadScene ();
        //        }
        //
        //        if (mapNumber == section.Length)
        //        {
        //            ModeChange(modeForm);
        //        }
    }

    void DungeonConstruct()
    {
		if (spawnPoint != null) {
			spawnPoint.SpawnMonsterGetting ();
		

			boomMonster = new BoomMonster[spawnPoint.boomMonsterCount];
			shockWaveMonster = new ShockWaveMonster[spawnPoint.shockWaveMonsterCount];
			warriorMonster = new WarriorMonster[spawnPoint.warriorMonsterCount];
			normalMode = true;
			MonsterSet ();
		}
        //if (mapNumber == 0) {
        //	nextSceneObject.SceneChangeObjectSet (mapNumber+1);
        //}
        //else if(mapNumber!= 0 || mapNumber!=4){
        //	nextSceneObject.SceneChangeObjectSet (mapNumber+1);
        //	beforeScneObject.SceneChangeObjectSet (mapNumber-1);
        //}
        //else if (mapNumber == 4) {
        //	beforeScneObject.SceneChangeObjec  tSet (mapNumber - 1);
        //}

    }

    public void MonsterSet()
    {
		monsterCount = 0;

		for (int i = 0; i < spawnPoint.boomMonsterCount; i++) {
			GameObject objBoomMonster = (GameObject)Instantiate (Resources.Load ("Monster/Frog"), spawnPoint.spawnVector[i] ,this.gameObject.transform.rotation);
			objBoomMonster.transform.SetParent (this.transform);
			boomMonster [i] = objBoomMonster.GetComponent<BoomMonster> ();
		}
		for (int i = 0; i < spawnPoint.shockWaveMonsterCount; i++) {
			GameObject objShockwaveMonster= (GameObject)Instantiate (Resources.Load ("Monster/Duck"), spawnPoint.spawnVector[i+spawnPoint.boomMonsterCount], this.transform.rotation);
			objShockwaveMonster.transform.SetParent (this.transform);
			shockWaveMonster [i] = objShockwaveMonster.GetComponent<ShockWaveMonster> (); 
		}
		for (int i = 0; i < spawnPoint.warriorMonsterCount; i++) {
			GameObject objWarriorMonster= (GameObject)Instantiate (Resources.Load ("Monster/Rabbit"), spawnPoint.spawnVector[i+spawnPoint.boomMonsterCount+spawnPoint.shockWaveMonsterCount], this.transform.rotation);
			objWarriorMonster.transform.SetParent (this.transform);
			warriorMonster[i]= objWarriorMonster.GetComponent<WarriorMonster> ();
		}

		if (bossMonster != null) {
			GameObject objBossMonster = (GameObject)Instantiate (Resources.Load ("Monster/Bear"), new Vector3 (0, 0, 0), this.transform.rotation);
			Debug.Log ("in boss");
			bossMonster = objBossMonster.GetComponent<BossMonsterKYW> ();
			monsterCount++;
		}

		monsterCount += spawnPoint.sumMonsterCount;

		for (int i = 0; i < boomMonster.Length; i++) {
			boomMonster [i].player = players;
			boomMonster [i].MonsterSet (900,2);
			boomMonster [i].MonsterMoveAI (normalMode);
			boomMonster [i].MonsterArrayNumber = i;

			if (hostGuest != HostGuest.Host) {
				boomMonster [i].GuestMonsterPatternChange ();
			} else if (hostGuest == HostGuest.Host) {
				boomMonster [i].MonSterPatternUpdateConduct (normalMode);
			}
				//boomMonster [i].MonSterPatternUpdateConduct (normalMode);
		}


		for (int j = 0; j < shockWaveMonster.Length; j++) {
			shockWaveMonster [j].player = players;
			shockWaveMonster [j].MonsterSet (900, 2);
			shockWaveMonster [j].MonsterMoveAI (normalMode);
			shockWaveMonster [j].MonsterArrayNumber = j;
			if (hostGuest == HostGuest.Host) {
				shockWaveMonster [j].MonSterPatternUpdateConduct (normalMode);
			} else if (hostGuest != HostGuest.Host) {
				shockWaveMonster [j].GuestMonsterPatternChange ();
			}
		}

		for (int k = 0; k < warriorMonster.Length; k++) {
			warriorMonster [k].player = players;
			warriorMonster [k].MonsterSet (900, 2);
			warriorMonster [k].MonsterMoveAI (normalMode);
			warriorMonster [k].MonsterArrayNumber = k;
			if (hostGuest == HostGuest.Host) {
				warriorMonster [k].MonSterPatternUpdateConduct (normalMode);
			} else if (hostGuest != HostGuest.Host) {
				warriorMonster [k].GuestMonsterPatternChange ();
			}
		}


		if (bossMonster != null) {
			bossMonster.player = players;
			bossMonster.BossMonsterSet (900, 2);
			if (hostGuest == HostGuest.Host) {
				bossMonster.BossMonsterPatternUpdateConduct ();
			}
			else if (hostGuest != HostGuest.Host) {
				//bossMonster.
				bossMonster.BossMonsterPatternUpdateConduct ();
			}

		}
	}

    

	public void SectionSet(){
		
		for (int i = 0; i < section.Length; i++) {
			section [i].MonsterSet ();
			section [i].GateNumber = i;
		}
	}

    public void RemoveMonsterArray()
    {
        monsterCount -= 1;
        if (monsterCount == 0)
        {
            SceneChange();
        }

    }

	//monsterspawnPoint getting
	public void GetMonsterTransForm(Vector3[] _monsterTransForm){
		monsterTransForm = _monsterTransForm;
	}


    public GameObject CreatePlayer(int characterId)
    {
        //여기서는 플레이어 캐릭터 딕셔너리 -> 각 직업에 따른 플레이어 스탯과 능력치, 스킬, 이름을 가지고 있음
        //딕셔너리를 사용하여 그에 맞는 캐릭터를 소환해야 하지만 Prototype 진행 시에는 고정된 플레이어를 소환하도록 함.

        GameObject player = Instantiate(Resources.Load("Warrior")) as GameObject;
        player.name = "Warrior";
        player.tag = "Player";
        player.transform.position = Vector3.zero;

        characters[networkManager.MyIndex] = player.GetComponent<CharacterManager>();
        characters[networkManager.MyIndex].enabled = true;
        characters[networkManager.MyIndex].SetUserNum(networkManager.MyIndex);

        players[networkManager.MyIndex] = player;

        Debug.Log("캐릭터 생성 번호 : " + networkManager.MyIndex);

        m_camera = GameObject.FindGameObjectWithTag("MainCamera");
        StartCoroutine(m_camera.GetComponent<CameraController>().CameraCtrl(player.transform));

        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        inputManager.InitializeManager();
        StartCoroutine(inputManager.GetKeyInput());

        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        //uiManager.SetBattleUIManager();

        player.GetComponent<CharacterManager>().UIManager = uiManager;
        player.GetComponent<CharacterManager>().SetCharacterStatus();
        player.GetComponent<CharacterManager>().SetCharacterType();

        for (int index = 0; index < networkManager.UserIndex.Count; index++)
        {
            if (networkManager.MyIndex != networkManager.UserIndex[index].UserNum)
            {
                DataSender.Instance.CreateUnitSend(networkManager.UserIndex[index].EndPoint, (short)characterId, player.transform.position.x, player.transform.position.y, player.transform.position.z);
            }
        }

        return player;
    }

    public GameObject CreateUnit(int unitId, int unitIndex, Vector3 newPosition)
    {
        //위와 같은 생성이지만 이곳에서는 다른 플레이어의 캐릭터를 생성한다.
        //DataHandler 에서 데이타를 받아서 실행된다.
        if (players[unitIndex] == null)
        {
            GameObject unit = Instantiate(Resources.Load("Warrior")) as GameObject;
            unit.transform.position = newPosition;
            unit.name = "Warrior";
            players[unitIndex] = unit;

            characters[unitIndex] = unit.GetComponent<CharacterManager>();
            characters[unitIndex].SetUserNum(unitIndex);

            return unit;
        }
        else
        {
            Debug.Log("이미 있는 캐릭터 인덱스");
            return null;
        }        
    }
}