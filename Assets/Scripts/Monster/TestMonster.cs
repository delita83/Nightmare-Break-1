﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TestMonster : Monster
{
	public enum BigBearBossPatternName
	{
		BigBearBossIdle = 0,
		BigBearBossRun,
		BigBearBossAttack,
		BigBearBossOneHandAttack,
		BigBearJumpAttack,
		BigBearBossRoar,
		BigBearBossDeath};

	public float searchRange;
	public float moveSpeed;

	float AttackTime;
	int shootNumber;
	public const int bossPatternCount = 3;

	public UIManager uiManager;
	public BoxCollider[] MonsterWeapon;
	public BigBearBossPatternName BigBearBossState;
	AnimatorStateInfo stateInfo;
	bool monsterAttack;
	[SerializeField] Image skillInsertImage;
	public float imageSpeed = 1.0f;
	public float imageLerpTime;

	public GameObject handPos;
	public GameObject handSphere;
	public OneHandAttack handAttack;

	public GameObject[] bossplayer;

	public GameObject chasePlayer;
	public GameObject roar;

	public bool bossSkill;

	public enum insertImageState
	{
		Stop = 0,
		Left,
		Right
	}

	public insertImageState imageState = insertImageState.Stop;

	void Start ()
	{
		shootNumber = 6;
		RunRange = 30;
		attackRange = 3;
//		uiManager = GameObject.FindWithTag ("UIManager").GetComponent<UIManager> ();

		bossplayer = GameObject.FindGameObjectsWithTag ("Player");
		chasePlayer = null;
		animator = GetComponent<Animator> ();
		BoxCollider[] MonsterWeapon = new BoxCollider[2];
		skillInsertImage = GameObject.Find ("InGameUICanvas").transform.Find ("BossDeadlyPatternImage").Find("BossDeadlyPattern").GetComponent<Image>();
		//skillInsertImage = transform.Find("InGameUICanvas").gameObject;

		StartCoroutine (SetTargetPlayer());
		StartCoroutine (BossAI());
	}

	void Update ()
	{

		if (IsAlive)
		{
			if (!bossSkill)
			{
				stateInfo = this.animator.GetCurrentAnimatorStateInfo (0);
				searchRange = Vector3.Distance (chasePlayer.transform.position, transform.position);

				if (searchRange < attackRange)
				{
					BigBearBossPattern ((int)BigBearBossPatternName.BigBearBossAttack);
				}

				if (searchRange > RunRange)
				{
					BigBearBossPattern ((int)BigBearBossPatternName.BigBearBossIdle);
					if (stateInfo.IsName("BigBearBossIdle"))
					{
						changeDirection ();

					}

				}
				else if (searchRange <= RunRange && searchRange > attackRange)
				{
					BigBearBossPattern ((int)BigBearBossPatternName.BigBearBossRun);

					if (stateInfo.IsName ("BigBearBossRun"))
					{
						changeDirection ();
						transform.Translate ((chasePlayer.transform.position - transform.position) * moveSpeed * Time.deltaTime, 0);//반대로 걸어 가서 수정
					}
				}

				if (monsterAttack)
				{
					for (int i = 0; i < MonsterWeapon.Length; i++)
					{
						MonsterWeapon [i].enabled = true;
					}
				}
				else if (!monsterAttack)
				{
					for (int i = 0; i < MonsterWeapon.Length; i++)
					{
						MonsterWeapon [i].enabled = false;
					}
				}
			}
		}
		else
		{
			Destroy (this.gameObject, 5);
		}

	}

	void SetStateDefault ()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator> ();
		}

		animator.SetTrigger ("IdleState");
		//animator.SetBool ("Run", false);
	}

	public IEnumerator SetTargetPlayer()
	{
		while (IsAlive)
		{
			int chaseIndex = Random.Range (0, bossplayer.Length - 1);

			chasePlayer = bossplayer [chaseIndex];

			yield return new WaitForSeconds (15.0f);
		}
	}

	public IEnumerator BossAI()
	{
		while (IsAlive)
		{
			yield return new WaitForSeconds (5f);

			int pattern = Random.Range (0, bossPatternCount + 1);

			Debug.Log (pattern);

			bossSkill = true;
			animator.SetBool ("BossSkill", true);

			if (pattern == 0)
			{
				BigBearBossPattern ((int)BigBearBossPatternName.BigBearJumpAttack);			
			}
			else if (pattern == 1)
			{
				BigBearBossPattern ((int)BigBearBossPatternName.BigBearBossRoar);
			}
			else if (pattern == 2)
			{
				BigBearBossPattern ((int)BigBearBossPatternName.BigBearBossOneHandAttack);
			}
			else
			{
				SetStateDefault ();
				BigBearBossPattern ((int)BigBearBossPatternName.BigBearBossAttack);
			}

			yield return new WaitForSeconds (1.0f);

			bossSkill = false;
			animator.SetBool ("BossSkill", false);
		}
	}

	public void AnimatorReSet()
	{
	}

	public IEnumerator Shooting()
	{
		int shootNum = 0;

		while (shootNum < shootNumber)
		{
			yield return new WaitForSeconds (0.2f);

			int xPos = Random.Range (-2, 2);
			int zPos = Random.Range (-2, 2);

			Instantiate (Resources.Load<GameObject> ("Effect/OneHandSphere"), handPos.transform.position + (Vector3.right * xPos) + (Vector3.forward * zPos), Quaternion.Euler (0, 0, 0));

			shootNum++;
		}
	}

	public void RoarHit()
	{
		Instantiate (Resources.Load<GameObject> ("Effect/WarningEffect"), new Vector3(-3.55f, 0.15f , this.transform.position.z+10f) , Quaternion.Euler (-90, 0, 0));
	}


	public void changeDirection ()
	{//캐릭터 이동시 보스가 보는 방향을 정한다.
		Vector3 vecLookPos =chasePlayer.transform.position;
		vecLookPos.y = transform.position.y;
		vecLookPos.x = transform.position.x;

		transform.LookAt (vecLookPos);

	}


	public override void HitDamage (int _Damage, GameObject attacker)
	{
		stateInfo = this.animator.GetCurrentAnimatorStateInfo (0);

		if (IsAlive)
		{
			maxLife -= _Damage;

//			uiManager.bossHp.fillAmount = maxLife / currentLife;
			if (maxLife > 0)
			{
				//hitanimation
			}
			else if (maxLife <= 0)
			{
				if (!stateInfo.IsName ("BigBearBossDeath"))
				{
					BigBearBossPattern ((int)BigBearBossPatternName.BigBearBossDeath);
					IsAlive = false;
					return;
				}
			}
		}
	}

	public void BigBearBossPattern (int bossState)
	{
		monsterAttack = false;
		switch (bossState)
		{

		case 0:
			BigBearBossState = BigBearBossPatternName.BigBearBossIdle;
			animator.SetInteger ("state", 0);
			break;
		case 1:
			BigBearBossState = BigBearBossPatternName.BigBearBossRun;
			animator.SetInteger ("state", 1);
			break;
		case 2:
			BigBearBossState = BigBearBossPatternName.BigBearBossAttack;
			animator.SetInteger ("state", 2);
			monsterAttack = true;
			break;

		case 3:
			BigBearBossState = BigBearBossPatternName.BigBearBossOneHandAttack;
			animator.SetInteger ("state", 3);
			monsterAttack = true;
			break;

		case 4:
			BigBearBossState = BigBearBossPatternName.BigBearJumpAttack;
			animator.SetInteger ("state", 4);
			break;

		case 5:
			BigBearBossState = BigBearBossPatternName.BigBearBossRoar;
			animator.SetInteger ("state", 5);
			break;

		case 6:
			BigBearBossState = BigBearBossPatternName.BigBearBossDeath;
			animator.SetTrigger ("BigBearBossDeath");
			break;
		}
	}

	public void roarStart ()
	{//애니메이션 이벤트를 사용하여 포효시 붉은 이펙트를 켠다.
		//GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<RedRenderImage> ().enabled = true;
		imageState = insertImageState.Left; //이미지 상태 값 저장 왼쪽
		StartCoroutine (LMoveImage ()); //코루틴 실행

	}

	public void roarEnd ()
	{//애니메이션 이벤트를 사용하여 포효시 붉은 이펙트를 끈다.
		//GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<RedRenderImage> ().enabled = false;
		////이미지를 오른쪽이동하는 코루틴을 실행 시키는 함수 애니메이션 이벤트로 실행
//		imageState = insertImageState.Right;//이미지 상태 값 저장 오른쪽
	}
		
	IEnumerator LMoveImage ()
	{  //이미지를 왼쪽이동시키는 함수
		while (imageState == insertImageState.Left)
		{		

			yield return new WaitForSeconds (0.01f);	


			float x = skillInsertImage.GetComponent<RectTransform> ().localPosition.x;//현재 x값의 위치를 받아온다.
			if (x > 103)
			{
				skillInsertImage.GetComponent<RectTransform> ().Translate (15 * -imageSpeed, 0, 0);//10씩 왼쪽으로 이동

			}
			else
			{
				//	skillInsertImage.enabled = true;
				imageLerpTime += 0.01f;

				if (imageLerpTime >3)
				{
					float ImageAlpha = (4 - imageLerpTime);
					skillInsertImage.color = new Color (255,255,255, ImageAlpha);

						
					if (ImageAlpha < 0.3)
					{
						skillInsertImage.enabled = false;
						imageState = insertImageState.Stop;//x값이 0보다 작을 경우 멈춤
						ImageBackPos ();
					}

				}
			}
		}
	}

	public void ResetBossImage()
	{
		skillInsertImage.GetComponent<RectTransform> ().localPosition = new Vector3 (613, -152 , 0);
		skillInsertImage.color = new Color (255,255,255, 255);
		imageState = insertImageState.Stop;
	}

	public void ImageBackPos()
	{
		StopCoroutine (LMoveImage ());

		imageState = insertImageState.Right;

		ResetBossImage ();


		if (imageState == insertImageState.Stop)
		{
			skillInsertImage.enabled = true;
		}
	}





}